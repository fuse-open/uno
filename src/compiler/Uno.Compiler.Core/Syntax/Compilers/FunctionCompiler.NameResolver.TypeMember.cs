using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.Syntax.Binding;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        DataType TryResolveDataTypeIdentifier(AstIdentifier identifier)
        {
            var pi = NameResolver.TryResolveMemberRecursive(Namescope, identifier, null) ??
                     NameResolver.TryResolveUsingNamespace(Namescope, identifier, null);
            return (pi as PartialType)?.Type;
        }

        bool AllowStaticContext(AstExpression qualifier, Expression instance)
        {
            // Allow static context when
            // - instance == null (proper static context)
            // - expression is unqualified (both static/non-static context)
            // - qualifier also resolves to instance.DataType (member/type ambigouity -> both static/non-static context)
            return instance == null ||
                   qualifier == null ||
                   qualifier is AstIdentifier && TryResolveDataTypeIdentifier(qualifier as AstIdentifier) == instance.ReturnType;
        }

        public PartialExpression TryResolveTypeMember(DataType dt, AstIdentifier id, int? typeParamCount, AstExpression qualifier, Expression instance)
        {
            var result = NameResolver.TryGetTypeMemberCached(dt, id.Symbol, typeParamCount);

            if (result == null || 
                dt.Methods.Count == 1 && dt.Methods[0].GenericType == dt)
                return null;

            if (dt.IsFlattenedDefinition)
                return PartialError(id.Source, ErrorCode.I0000, "Invalid member-access on non-parameterized type");

            var methods = result as IReadOnlyList<Method>;
            if (methods != null)
                return new PartialMethodGroup(id.Source, instance, !AllowStaticContext(qualifier, instance), methods);

            var literal = result as Literal;
            if (literal != null)
            {
                if (AllowStaticContext(qualifier, instance))
                {
                    ILVerifier.VerifyConstUsage(id.Source, literal, Function);
                    return new PartialValue(new Constant(id.Source, literal.ReturnType, literal.Value is AstExpression
                            ? Compiler.CompileConstant((AstExpression) literal.Value, literal.DeclaringType, literal.ReturnType).Value
                            : literal.Value));
                }

                return PartialError(id.Source, ErrorCode.E0000, literal.Quote() + " is a constant -- qualify with the type name");
            }

            var field = result as Field;
            if (field != null)
            {
                if (field.IsStatic)
                {
                    if (instance != null)
                        return AllowStaticContext(qualifier, instance)
                            ? new PartialField(id.Source, field, null)
                            : PartialError(id.Source, ErrorCode.E3117, field.Quote() + " is static -- qualify with the type name");
                }
                else
                {
                    if (instance == null)
                        return qualifier == null && TryResolveDataTypeIdentifier(id) == field.ReturnType
                            ? new PartialType(id.Source, field.ReturnType)
                            : PartialError(id.Source, ErrorCode.E3118, field.Quote() + " is non-static and cannot be accessed from a static context");
                }

                return new PartialField(id.Source, field, instance);
            }

            var ev = result as Event;
            if (ev != null)
            {
                if (ev.IsStatic)
                {
                    if (instance != null)
                        return AllowStaticContext(qualifier, instance)
                            ? new PartialEvent(id.Source, ev, null)
                            : PartialError(id.Source, ErrorCode.E3119, ev.Quote() + " is static -- qualify with the type name");
                }
                else
                {
                    if (instance == null)
                        return qualifier == null && TryResolveDataTypeIdentifier(id) == ev.ReturnType
                            ? new PartialType(id.Source, ev.ReturnType)
                            : PartialError(id.Source, ErrorCode.E3120, ev.Quote() + " is non-static and cannot be accessed from a static context");
                }

                return new PartialEvent(id.Source, ev, instance);
            }

            var prop = result as Property;
            if (prop != null)
            {
                if (prop.IsStatic)
                {
                    if (instance != null)
                        return AllowStaticContext(qualifier, instance)
                            ? new PartialProperty(id.Source, prop, null)
                            : PartialError(id.Source, ErrorCode.E3121, prop.Quote() + " is static -- qualify with the type name");
                }
                else
                {
                    if (instance == null)
                        return qualifier == null && TryResolveDataTypeIdentifier(id) == prop.ReturnType
                            ? new PartialType(id.Source, prop.ReturnType)
                            : PartialError(id.Source, ErrorCode.E3122, prop.Quote() + " is non-static and cannot be accessed from a static context");
                }

                return new PartialProperty(id.Source, prop, instance);
            }

            var genericParam = result as GenericParameterType;
            if (genericParam != null)
                return qualifier == null ?
                    new PartialType(id.Source, genericParam) :
                    null;

            var innerType = result as DataType;
            var innerBlock = result as Block;
            if (innerType != null || innerBlock != null)
                return AllowStaticContext(qualifier, instance)
                    ? (innerType != null 
                        ? (PartialExpression) new PartialType(id.Source, innerType)
                        :                     new PartialBlock(id.Source, innerBlock))
                    : PartialError(id.Source, ErrorCode.E0000, "Cannot reference block or type " + result.Quote() + " through an expression -- qualify with the type name");

            return result is List<object>
                ? PartialError(id.Source, ErrorCode.E0000, (dt + "." + id.GetParameterizedSymbol(typeParamCount)).Quote() + " is ambiguous. This can be resolved using an explicit cast to a more specific interface type")
                : result as PartialExpression ??
                    PartialError(id.Source, ErrorCode.I0000, "Unknown type member: " + result);
        }
    }
}
