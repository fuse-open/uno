using System.Linq;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Statements;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        private Expression CompileLambda(AstLambda l)
        {
            var ps = l.ParameterList.Parameters;

            // The parser currently doesn't parse default values so this shouldn't happen,
            // so this check is for if the parser is changed in the future
            if (ps.Any(p => p.OptionalValue != null))
                Error(l.Source, ErrorCode.E0000, "Lambda parameters cannot have default values");

            if (ps.Count > 0)
            {
                var hasType = ps[0].OptionalType != null;
                if (ps.Any(p => (p.OptionalType != null) != hasType))
                    Error(l.Source, ErrorCode.E0748,
                        "Inconsistent lambda parameter usage; all parameter types must either be explicit or implicit");
            }

            // We defer compiling the lambda until it is implicitly cast to a delegate type
            return new UncompiledLambda(l.Source, l);
        }

        private Expression TryCompileImplicitLambdaCast(UncompiledLambda uncompiledLambda, DelegateType dt)
        {
            var parameters = TypedLambdaParameterList(uncompiledLambda.AstLambda.ParameterList, dt);
            return CompileLambdaBody(uncompiledLambda.Source, parameters, uncompiledLambda.AstLambda.Body, dt);
        }

        private Lambda CompileLambdaBody(Source src, Parameter[] parameters, AstStatement body, DelegateType dt)
        {
            var result = new Lambda(src, parameters, dt, null);

            Lambdas.Push(result);

            if (body is AstExpression)
            {
                var compiledExpr = CompileExpression((AstExpression)body);

                var compiledBody = MakeLambdaStatementBody(
                    dt.ReturnType.IsVoid
                        ? compiledExpr
                        : CompileImplicitCast(body.Source, dt.ReturnType, compiledExpr),
                    dt.ReturnType.IsVoid);
                result.SetBody(compiledBody);
            }
            else
            {
                result.SetBody(CompileStatement(body));
            }

            Lambdas.Pop();

            return result;
        }

        private static Statement MakeLambdaStatementBody(Expression lambdaBody, bool returnVoid)
        {
            // Turn expression lambda to statement
            if (returnVoid)
                return new Scope(lambdaBody.Source, lambdaBody, new Return());
            else
                return new Return(lambdaBody.Source, lambdaBody);
        }

        private Parameter[] TypedLambdaParameterList(AstParameterList parameterList, DelegateType dt)
        {
            var parameters = dt.Parameters;

            if (parameterList.Parameters.Count != parameters.Length)
            {
                Error(parameterList.Source, ErrorCode.E1593,
                    "Delegate " + dt.Quote() + " does not take " + parameterList.Parameters.Count + " arguments");
                return new Parameter[0];
            }

            Parameter[] result = new Parameter[parameters.Length];

            for (var i = 0; i < parameters.Length; ++i)
            {
                var param = parameters[i];
                var astParam = parameterList.Parameters[i];

                var astParamType = astParam.OptionalType != null
                    ? Compiler.NameResolver.GetType(Function.DeclaringType, astParam.OptionalType)
                    : param.Type;

                if (!astParamType.Equals(param.Type)
                    || param.Modifier != astParam.Modifier
                    || astParam.OptionalValue != null)
                    Error(parameterList.Source, ErrorCode.E1661,
                        "Cannot convert anonymous method block to delegate type " + dt.Quote() +
                        " because the specified block's parameter types do not match the delegate parameter types");

                var attributes = Compiler.CompileAttributes(Function.DeclaringType, astParam.Attributes);

                result[i] = new Parameter(
                    astParam.Name.Source,
                    attributes,
                    param.Modifier,
                    param.Type,
                    astParam.Name.Symbol, null);
            }

            return result;
        }
    }
}