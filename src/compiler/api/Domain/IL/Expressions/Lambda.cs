using System.Text;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class Lambda : Expression
    {
        public readonly Parameter[] Parameters;
        Statement _body;
        public Statement Body => _body;
        public readonly DelegateType DelegateType;

        public Lambda(
            Source src,
            Parameter[] parameters,
            DelegateType delegateType,
            Statement body)
            : base(src)
        {
            Parameters = parameters;
            _body = body;
            DelegateType = delegateType;
        }

        public void SetBody(Statement body)
        {
            _body = body;
        }

        public override ExpressionType ExpressionType => ExpressionType.Lambda;
        public override DataType ReturnType => DelegateType;

        public override Expression CopyExpression(CopyState state)
        {
            var result = new Lambda(Source, Parameters.Copy(state), state.GetType(DelegateType), null);
            state.AddLambda(this, result);
            var body = Body.CopyStatement(state);
            result.SetBody(body);
            return result;
        }

        public override void Disassemble(StringBuilder sb, ExpressionUsage u = ExpressionUsage.Argument)
        {
            sb.Append(Parameters.BuildString(includeParamNames: true) + " => <lambda body>");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.Begin(ref _body);
            Body.Visit(p);
            p.End(ref _body);
        }
    }
}
