using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.Syntax.Compilers;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public partial class NameResolver
    {
        public PartialExpression ResolveExpression(Namescope namescope, AstExpression e, int? typeParamCount)
        {
            switch (e.ExpressionType)
            {
                case AstExpressionType.Void:
                    return new PartialType(e.Source, DataType.Void);
                case AstExpressionType.Global:
                    return new PartialNamespace(e.Source, _compiler.Data.IL);

                case AstExpressionType.BuiltinType:
                    return new PartialType(e.Source, 
                        _compiler.Essentials.BuiltinTypes[(int) ((AstBuiltinType) e).BuiltinType]);

                case AstExpressionType.Identifier:
                {
                    var s = e as AstIdentifier;

                    var pi = TryResolveMemberRecursive(namescope, s, typeParamCount);
                    if (pi != null) return pi;

                    pi = TryResolveUsingNamespace(namescope, s, typeParamCount);
                    if (pi != null) return pi;

                    Log.Error(e.Source, ErrorCode.E3114, _compiler.GetUnresolvedIdentifierError(s, typeParamCount));
                    return PartialExpression.Invalid;
                }
                case AstExpressionType.Member:
                {
                    var s = e as AstMember;
                    var baseinfo = ResolveExpression(namescope, s.Base, null);

                    switch (baseinfo.ExpressionType)
                    {
                        case PartialExpressionType.Namespace:
                        {
                            var pn = baseinfo as PartialNamespace;
                            return TryResolveMember(pn.Namespace, s.Name, typeParamCount, s.Base) ??
                                    PartialError(s.Source, ErrorCode.E3111, _compiler.GetNamespaceMemberNotFoundError(s.Name, pn.Namespace));
                        }
                        case PartialExpressionType.Type:
                        {
                            var pdt = baseinfo as PartialType;
                            return TryResolveMember(pdt.Type, s.Name, typeParamCount, s.Base) ??
                                    PartialError(s.Source, ErrorCode.E3112, _compiler.GetTypeMemberNotFoundError(s.Name, pdt.Type));
                        }
                        case PartialExpressionType.Value:
                        {
                            var psym = baseinfo as PartialValue;
                            if (psym.Value.IsInvalid)
                                return psym;
                            break;
                        }
                    }

                    return PartialError(s.Source, ErrorCode.E3113, "<" + baseinfo.ExpressionType + "> does not contain type, block or namespace " + s.Name.GetParameterizedSymbol(typeParamCount).Quote());
                }
                case AstExpressionType.Parameterizer:
                {
                    var s = e as AstParameterizer;
                    var pi = ResolveExpression(namescope, s.Base, s.Arguments.Count);

                    var typeArgExp = s.Arguments;
                    var typeArgCount = typeArgExp.Count;

                    var typeArgs = new DataType[typeArgCount];
                    for (int i = 0; i < typeArgCount; i++)
                        typeArgs[i] = GetType(namescope, typeArgExp[i]);

                    if (pi is PartialType)
                    {
                        var dt = (pi as PartialType).Type;
                        if (dt.IsGenericDefinition && dt.GenericParameters.Length == typeArgs.Length)
                            return new PartialType(e.Source, _compiler.TypeBuilder.Parameterize(s.Source, dt, typeArgs));
                    }

                    return !pi.IsInvalid
                        ? PartialError(e.Source, ErrorCode.E3116, pi.Quote() + " is not a generic type definition")
                        : PartialExpression.Invalid;
                }
                case AstExpressionType.Generic:
                {
                    var s = e as AstGeneric;
                    return ResolveExpression(namescope, s.Base, s.ArgumentCount);
                }
                case AstExpressionType.Array:
                {
                    var s = e as AstUnary;
                    var elementType = GetType(namescope, s.Operand);
                    return new PartialType(s.Source, _compiler.TypeBuilder.GetArray(elementType));
                }
                case AstExpressionType.FixedArray:
                {
                    var s = e as AstFixedArray;
                    var size = s.OptionalSize != null
                        ? _compiler.CompileExpression(s.OptionalSize, namescope, _compiler.Essentials.Int)
                        : null;
                    var elementType = GetType(namescope, s.ElementType);
                    return new PartialType(s.Source, new FixedArrayType(s.Source, elementType, size, _compiler.Essentials.Int));
                }
            }

            return PartialError(e.Source, ErrorCode.E3135, "<" + e.ExpressionType + "> cannot be used as a type");
        }
    }
}
