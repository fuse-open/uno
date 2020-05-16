using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.Core.Syntax.Binding;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        public PartialExpression ResolveExpression(AstExpression e, int? typeParamCount)
        {
            switch (e.ExpressionType)
            {
                default:
                    return new PartialValue(CompileExpression(e));
                case AstExpressionType.Global:
                    return new PartialNamespace(e.Source, Compiler.Data.IL);
                case AstExpressionType.Void:
                    return new PartialType(e.Source, DataType.Void);
                case AstExpressionType.This:
                    return new PartialThis(e.Source);

                case AstExpressionType.Identifier:
                    return ResolveIdentifier(e as AstIdentifier, typeParamCount);
                case AstExpressionType.Member:
                    return ResolveMember(e as AstMember, typeParamCount);
                case AstExpressionType.LookUp:
                    return ResolveLookUp(e as AstCall);
                case AstExpressionType.BuiltinType:
                    return new PartialType(e.Source, 
                        Essentials.BuiltinTypes[(int) ((AstBuiltinType) e).BuiltinType]);

                case AstExpressionType.Parameterizer:
                {
                    var s = e as AstParameterizer;

                    var args = new DataType[s.Arguments.Count];
                    for (int i = 0; i < args.Length; i++)
                        args[i] = NameResolver.GetType(Namescope, s.Arguments[i]);

                    var b = ResolveExpression(s.Base, s.Arguments.Count);

                    var mg = b as PartialMethodGroup;
                    if (mg != null)
                    {
                        var methods = new Method[mg.Methods.Count];

                        for (int i = 0, l = mg.Methods.Count; i < l; i++)
                            methods[i] = TypeBuilder.Parameterize(s.Source, mg.Methods[i], args);

                        return new PartialMethodGroup(s.Source, mg.Object, mg.IsQualified, methods);
                    }

                    var eg = b as PartialExtensionGroup;
                    if (eg != null)
                    {
                        var methods = new Method[eg.Methods.Count];

                        for (int i = 0, l = eg.Methods.Count; i < l; i++)
                            methods[i] = TypeBuilder.Parameterize(s.Source, eg.Methods[i], args);

                        return new PartialExtensionGroup(s.Source, eg.Object, methods);
                    }

                    var dt = b as PartialType;
                    return dt != null && dt.Type.IsGenericDefinition
                            ? new PartialType(s.Source, TypeBuilder.Parameterize(s.Source, dt.Type, args)) :
                        !b.IsInvalid
                            ? PartialError(s.Source, ErrorCode.E2041, b.Quote() + " is not a generic method or type definition")
                            : PartialExpression.Invalid;
                }
            }
        }
    }
}
