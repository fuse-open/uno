using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.Syntax.Binding;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        public Expression CompileCall(AstCall e)
        {
            // Builtin struct initialization
            if (e.Base is AstBuiltinType)
            {
                var dt = NameResolver.GetType(Namescope, e.Base);

                if (dt.IsStruct)
                {
                    // Default ctor
                    if (e.Arguments.Count == 0)
                        return new Default(e.Source, dt);

                    dt.PopulateMembers();

                    Constructor ctor;
                    Expression[] args;
                    return TryResolveConstructorOverload(e.Source, dt.Constructors, e.Arguments, out ctor, out args)
                        ? new NewObject(e.Source, ctor, args)
                        : Error(e.Source, ErrorCode.E3125, dt.Quote() + " has no constructors matching the argument list " +
                            PrintableArgumentList(e.Arguments) +
                            NameResolver.SuggestCandidates(dt.Constructors));
                }

                return Error(e.Source, ErrorCode.E3126, dt.Quote() + " must be instantiated using 'new' because it is not a struct");
            }

            var pe = ResolveExpression(e.Base, null);

            switch (pe.ExpressionType)
            {
                case PartialExpressionType.Block:
                case PartialExpressionType.Type:
                case PartialExpressionType.Namespace:
                    if (e.Base is AstIdentifier)
                    {
                        var p2 = NameResolver.TryResolveUsingType(Namescope, e.Base as AstIdentifier, null);
                        if (p2 != null)
                            pe = p2;
                    }
                    break;
            }

            var sym = CompilePartial(pe);
            if (sym.IsInvalid)
                return Expression.Invalid;

            // Delegate call
            if (sym.ReturnType.IsDelegate)
            {
                var dt = (DelegateType)sym.ReturnType;
                dt.AssignBaseType();
                var args = TryApplyDefaultValuesOnArgumentList(sym.Source, dt.Parameters, CompileArgumentList(e.Arguments));
                return TryApplyImplicitCastsOnArgumentList(dt.Parameters, args)
                    ? new CallDelegate(e.Source, sym, args)
                    : Error(e.Source, ErrorCode.E3127, "Call to delegate of type " + dt.Quote() + " has some invalid arguments " +
                        PrintableArgumentList(e.Arguments));
            }

            // Using static fall back
            if (!(sym is MethodGroup) && e.Base is AstIdentifier)
            {
                var p2 = NameResolver.TryResolveUsingType(Namescope, e.Base as AstIdentifier, null);
                if (p2 != null)
                {
                    sym = CompilePartial(p2);
                    if (sym.IsInvalid)
                        return Expression.Invalid;
                }
            }

            // Method call
            if (sym is MethodGroup)
            {
                var g = sym as MethodGroup;

                Method method;
                Expression[] args;
                return TryResolveMethodOverload(e.Source, g.Candidates, e.Arguments, out method, out args)
                    ? (g.IsQualified && g.Object != null && method.IsStatic
                            ? Error(sym.Source, ErrorCode.E3123, method.Quote() + " is static -- qualify with the type name") :
                        g.Object == null && !method.IsStatic
                            ? Error(sym.Source, ErrorCode.E3124, method.Quote() + " is non-static and cannot be accessed from a static context")
                            : new CallMethod(e.Source, method.IsStatic ? null : g.Object, method, args))
                    : g.Candidates.Count == 1
                        ? Error(e.Source, ErrorCode.E3128, "Call to " + g.Candidates[0].Quote() + " has some invalid arguments " +
                            PrintableArgumentList(e.Arguments))
                        : Error(e.Source, ErrorCode.E3129, "No overloads of " + g.CandidatesBaseName.Quote() + " matches the argument list " +
                            PrintableArgumentList(e.Arguments) +
                            NameResolver.SuggestCandidates(g.Candidates));
            }

            // Extension method call
            if (sym is ExtensionGroup)
            {
                var g = sym as ExtensionGroup;

                Method method;
                Expression[] args;
                var astArgs = new AstArgument[e.Arguments.Count + 1];
                astArgs[0] = new AstIL(g.Object);
                for (int i = 0; i < e.Arguments.Count; i++)
                    astArgs[i + 1] = e.Arguments[i];

                return TryResolveMethodOverload(e.Source, g.Candidates, astArgs, out method, out args)
                    ? new CallMethod(e.Source, null, method, args)
                    : g.Candidates.Count == 1
                        ? Error(e.Source, ErrorCode.E3128, "Call to " + g.Candidates[0].Quote() + " has some invalid arguments " +
                            PrintableArgumentList(astArgs))
                        : Error(e.Source, ErrorCode.E3129, "No overloads of " + g.CandidatesBaseName.Quote() + " matches the argument list " +
                            PrintableArgumentList(e.Arguments) +
                            NameResolver.SuggestCandidates(g.Candidates));
            }

            return Error(e.Source, ErrorCode.E3130, "Instances of type " + sym.ReturnType.Quote() + " cannot be called as a function");
        }
    }
}
