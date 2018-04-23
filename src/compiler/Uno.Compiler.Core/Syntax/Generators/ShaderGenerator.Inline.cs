using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.Core.Syntax.Generators
{
    partial class ShaderGenerator
    {
        bool InlineOnStage(MetaStage stage, Expression a, Expression b)
        {
            return InlineOnStage(stage, a) && InlineOnStage(stage, b);
        }

        bool InlineOnStage(MetaStage stage, Expression a, Expression b, Expression c)
        {
            return InlineOnStage(stage, a) && InlineOnStage(stage, b) && InlineOnStage(stage, c);
        }

        bool InlineOnStage(MetaStage stage, params Expression[] args)
        {
            if (args != null)
                foreach (var a in args)
                    if (!InlineOnStage(stage, a))
                        return false;

            return true;
        }

        bool InlineOnStage(MetaStage stage, Expression a, params Expression[] args)
        {
            return InlineOnStage(stage, a) && InlineOnStage(stage, args);
        }

        bool InlineOnStage(MetaStage stage, Expression a, Expression b, params Expression[] args)
        {
            return InlineOnStage(stage, a) && InlineOnStage(stage, b) && InlineOnStage(stage, args);
        }

        bool InlineOnStage(MetaStage stage, Expression e)
        {
            if (e == null)
                return stage <= MetaStage.Volatile;

            if (stage > MetaStage.Volatile && e.ReturnType.IsReferenceType)
                return false;

            switch (e.ExpressionType)
            {
                case ExpressionType.This:
                case ExpressionType.Base:
                case ExpressionType.TypeOf:
                    return stage <= MetaStage.Volatile;

                case ExpressionType.CapturedArgument:
                case ExpressionType.CapturedLocal:
                    return stage == MetaStage.Volatile;

                case ExpressionType.Invalid:
                case ExpressionType.Constant:
                case ExpressionType.Default:
                case ExpressionType.ExternOp:
                case ExpressionType.ExternString:
                case ExpressionType.LoadUniform:
                case ExpressionType.LoadVarying:
                case ExpressionType.LoadPixelSampler:
                case ExpressionType.MethodGroup:
                case ExpressionType.NewDelegate:
                case ExpressionType.LoadLocal:
                case ExpressionType.LoadArgument:
                case ExpressionType.LoadVertexAttrib:
                case ExpressionType.PlaceholderValue:
                case ExpressionType.PlaceholderReference:
                case ExpressionType.RuntimeConst:
                    return true;

                case ExpressionType.AddressOf:
                    return InlineOnStage(stage, (e as AddressOf).Operand);

                case ExpressionType.Swizzle:
                    return InlineOnStage(stage, (e as Swizzle).Object);
                case ExpressionType.LoadField:
                    return InlineOnStage(stage, (e as LoadField).Object);

                case ExpressionType.IsOp:
                    return stage <= MetaStage.Volatile && InlineOnStage(stage, (e as IsOp).Operand);
                case ExpressionType.AsOp:
                    return stage <= MetaStage.Volatile && InlineOnStage(stage, (e as AsOp).Operand);

                case ExpressionType.CallUnOp:
                    return stage <= MetaStage.Volatile && InlineOnStage(stage, (e as CallUnOp).Operand);
                case ExpressionType.CastOp:
                    return stage <= MetaStage.Volatile && InlineOnStage(stage, (e as CastOp).Operand);

                case ExpressionType.CallCast:
                    return InlineOnStage(stage, (e as CallCast).Operand);

                case ExpressionType.PlaceholderArray:
                {
                    var s = e as PlaceholderArray;
                    return InlineOnStage(stage, s.OptionalInitializer);
                }
                case ExpressionType.BranchOp:
                {
                    var s = e as BranchOp;
                    return InlineOnStage(stage, s.Left, s.Right);
                }
                case ExpressionType.ReferenceOp:
                {
                    var s = e as ReferenceOp;
                    return InlineOnStage(stage, s.Left, s.Right);
                }
                case ExpressionType.SequenceOp:
                {
                    var s = e as SequenceOp;
                    return InlineOnStage(stage, s.Left, s.Right);
                }
                case ExpressionType.ConditionalOp:
                {
                    var s = e as ConditionalOp;
                    return InlineOnStage(stage, s.Condition, s.True, s.False);
                }
                case ExpressionType.NullOp:
                {
                    var s = e as NullOp;
                    return stage <= MetaStage.Volatile && InlineOnStage(stage, s.Left, s.Right);
                }
                case ExpressionType.LoadElement:
                {
                    var s = e as LoadElement;
                    return InlineOnStage(stage, s.Array, s.Index);
                }
                case ExpressionType.CallBinOp:
                {
                    var s = e as CallBinOp;
                    return stage <= MetaStage.Volatile && InlineOnStage(stage, s.Left, s.Right);
                }
                case ExpressionType.GetProperty:
                {
                    var s = e as GetProperty;
                    return InlineOnStage(stage, s.Object, s.Arguments);
                }
                case ExpressionType.NewObject:
                {
                    var s = e as NewObject;
                    return s.ReturnType.IsValueType && InlineOnStage(stage, s.Arguments);
                }
                case ExpressionType.CallMethod:
                {
                    var s = e as CallMethod;
                    return
                        stage <= MetaStage.Volatile &&
                        s.Method.HasAttribute(Essentials.StageInlineAttribute) &&
                        InlineOnStage(stage, s.Object, s.Arguments);
                }
            }

            return false;
        }
    }
}
