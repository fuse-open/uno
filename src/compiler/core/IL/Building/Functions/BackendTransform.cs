using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.Core.IL.Utilities;

namespace Uno.Compiler.Core.IL.Building.Functions
{
    class BackendTransform : CompilerPass
    {
        Statement _current;
        Statement _parent;

        public BackendTransform(CompilerPass parent)
            : base(parent)
        {
        }

        public override bool Condition => !Backend.IsDefault;

        public override void Begin(ref Statement s)
        {
            _current = s;
            _parent = s is Expression ? null : s;
        }

        public override void Next(Statement s)
        {
            _parent = s is Expression ? null : s;
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            if (e.IsInvalid &&
                Backend.AllowInvalidCode &&
                Function.HasAttribute(Essentials.UxGeneratedAttribute))
                _current.Tag = Expression.Invalid;

            switch (e.ExpressionType)
            {
                case ExpressionType.NullOp:
                {
                    if (Backend.Has(FunctionOptions.DecodeNullOps))
                    {
                        var s = e as NullOp;
                        e = s.TransformNullOpToConditionalOp(Essentials, Type);
                        Begin(ref e, u);
                    }
                    break;
                }
                case ExpressionType.Swizzle:
                {
                    if (Backend.Has(FunctionOptions.DecodeSwizzles))
                    {
                        var s = e as Swizzle;
                        e = s.TransformSwizzleToNewObject(Type);
                        Begin(ref e, u);
                    }
                    break;
                }
                case ExpressionType.SetProperty:
                {
                    var s = e as SetProperty;
                    if (Backend.Has(FunctionOptions.DecodeSetChains) &&
                            s.TryTransformSetPropertyChainToSequence(Type, _parent, ref e))
                        Begin(ref e, u);
                    break;
                }
                case ExpressionType.CallMethod:
                {
                    var s = e as CallMethod;
                    if (Backend.Has(FunctionOptions.DecodeEnumOps) &&
                            s.TryTransformEnumHasFlagToIntOps(Log, ref e))
                        Begin(ref e, u);
                    break;
                }
                case ExpressionType.CallUnOp:
                {
                    var s = e as CallUnOp;
                    if (Backend.Has(FunctionOptions.DecodeEnumOps) &&
                            s.TryTransformEnumUnOpToIntUnOp(Log, ref e))
                        Begin(ref e, u);
                    break;
                }
                case ExpressionType.CallBinOp:
                {
                    var s = e as CallBinOp;
                    if (Backend.Has(FunctionOptions.DecodeEnumOps) &&
                            s.TryTransformEnumBinOpToIntBinOp(Log, ref e) ||
                        Backend.Has(FunctionOptions.DecodeDelegateOps) &&
                            s.TryTransformDelegateBinOp(ILFactory, ref e))
                        Begin(ref e, u);
                    break;
                }
            }

            _parent = e;
        }

        public override void End(ref Statement e)
        {
            if (e.Tag is InvalidExpression)
                e = new NoOp(e.Source);

            switch (e.StatementType)
            {
                case StatementType.DrawDispose:
                {
                    if (!Backend.CanExportDontExports)
                        e = ILFactory.CallMethod(e.Source, new This(e.Source, Type).Address, "free_DrawCalls");
                    break;
                }
            }
        }
    }
}
