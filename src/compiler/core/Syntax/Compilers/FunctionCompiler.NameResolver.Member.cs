using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.Core.IL.Utilities;
using Uno.Compiler.Core.Syntax.Binding;
using Uno.Logging;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        public PartialExpression ResolveMember(AstMember qualifier, int? typeParamCount)
        {
            var baseinfo = ResolveExpression(qualifier.Base, null);
            if (baseinfo.IsInvalid)
                return baseinfo;

            switch (baseinfo.ExpressionType)
            {
                case PartialExpressionType.Value:
                case PartialExpressionType.Namespace:
                case PartialExpressionType.Type:
                case PartialExpressionType.Block:
                    break;

                default:
                    baseinfo = new PartialValue(CompilePartial(baseinfo));
                    if (baseinfo.IsInvalid)
                        return baseinfo;
                    break;
            }

            switch (baseinfo.ExpressionType)
            {
                case PartialExpressionType.Value:
                {
                    var ps = baseinfo as PartialValue;
                    var obj = ps.Value.Address;
                    var dt = obj.ReturnType;
                    var pinfo = TryResolveTypeMember(dt, qualifier.Name, typeParamCount, qualifier.Base, obj);
                    if (pinfo != null)
                        return pinfo;

                    dt.PopulateMembers();

                    // Check if it is a swizzle
                    if (typeParamCount == null &&
                        dt.Swizzlers.Count > 0)
                    {
                        var swizzle = qualifier.Name.Symbol;

                        // Match identfier with field name patterns
                        int sp = 0;
                        bool match = false;

                        List<Field> fields = null;
                        List<Expression> args = null;

                        while (true)
                        {
                            bool found = false;

                            foreach (var field in dt.Fields)
                            {
                                if (field.IsStatic || swizzle.IndexOf(field.UnoName, sp) != sp)
                                    continue;

                                sp += field.UnoName.Length;

                                if (fields == null)
                                {
                                    fields = new List<Field>();
                                    args = new List<Expression>();
                                }

                                fields.Add(field);
                                args.Add(new LoadField(qualifier.Source, obj, field));
                                found = true;
                                break;
                            }

                            if (found)
                            {
                                if (sp != swizzle.Length)
                                    continue;

                                match = true;
                            }

                            break;
                        }

                        if (match)
                        {
                            var candidates = new List<Constructor>();

                            foreach (var swt in dt.Swizzlers)
                            {
                                swt.PopulateMembers();
                                candidates.AddRange(swt.Constructors);
                            }

                            var ctor = TryResolveConstructorOverload(qualifier.Name.Source, candidates, args.ToArray());
                            if (ctor == null)
                            {
                                Log.Error(qualifier.Name.Source, ErrorCode.E3103, "No matching method overload");
                                return PartialExpression.Invalid;
                            }

                            Transforms.TryCreateReadonlyValueFieldIndirection(Namescope, ref obj);
                            return new PartialValue(new Swizzle(qualifier.Source, ctor, obj, fields.ToArray()));
                        }
                    }

                    // Check if it is a member in an interface implemented by a generic parameter
                    if (dt.IsGenericParameter)
                    {
                        foreach (var it in dt.Interfaces)
                        {
                            pinfo = TryResolveTypeMember(it, qualifier.Name, typeParamCount, qualifier.Base, obj);
                            if (pinfo != null)
                                return pinfo;
                        }
                    }

                    // Check if it is a static member in a type named <member.Base>
                    if (qualifier.Base is AstIdentifier)
                    {
                        var id = qualifier.Base as AstIdentifier;
                        pinfo = NameResolver.TryResolveMember(Namescope, id, null, null);

                        if (pinfo is PartialType)
                        {
                            pinfo = TryResolveTypeMember((pinfo as PartialType).Type, qualifier.Name, typeParamCount, id, null);
                            if (pinfo != null)
                                return pinfo;
                        }
                    }

                    pinfo = TryResolveTypeExtension(obj, qualifier.Name, typeParamCount);
                    if (pinfo != null)
                        return pinfo;

                    return Compiler.Backend.AllowInvalidCode && Function.HasAttribute(Essentials.UxGeneratedAttribute)
                        ? PartialExpression.Invalid
                        : PartialError(qualifier.Source, ErrorCode.E3104, Compiler.GetTypeMemberNotFoundError(qualifier.Name, dt));
                }
                case PartialExpressionType.Namespace:
                {
                    var pn = baseinfo as PartialNamespace;
                    return NameResolver.TryResolveMember(pn.Namespace, qualifier.Name, typeParamCount, qualifier.Base) ??
                            PartialError(qualifier.Source, ErrorCode.E3105, Compiler.GetNamespaceMemberNotFoundError(qualifier.Name, pn.Namespace));
                }
                case PartialExpressionType.Block:
                {
                    var pdt = baseinfo as PartialBlock;
                    return NameResolver.TryResolveMember(pdt.Block, qualifier.Name, typeParamCount, qualifier.Base) ??
                            PartialError(qualifier.Source, ErrorCode.E3106, pdt.Block.Quote() + " does not contain a member named " + qualifier.Name.GetParameterizedSymbol(typeParamCount).Quote());
                }
                case PartialExpressionType.Type:
                {
                    var pdt = baseinfo as PartialType;
                    return TryResolveTypeMember(pdt.Type, qualifier.Name, typeParamCount, qualifier.Base, null) ??
                            PartialError(qualifier.Source, ErrorCode.E3107, Compiler.GetTypeMemberNotFoundError(qualifier.Name, pdt.Type));
                }
            }

            throw new FatalException(qualifier.Source, ErrorCode.I0076, "Unhandled expression form: " + baseinfo.ExpressionType.ToString());
        }
    }
}
