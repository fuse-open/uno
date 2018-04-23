using System.Text;
using Uno.Compiler.API.Domain.Graphics;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class CallShader : Expression
    {
        public ShaderFunction Function;
        public Expression[] Arguments;

        public override DataType ReturnType => Function.ReturnType;

        public CallShader(Source src, ShaderFunction func, params Expression[] args)
            : base(src)
        {
            Function = func;
            Arguments = args;
        }

        public override ExpressionType ExpressionType => ExpressionType.CallShader;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append(Function.Name + "(");

            for (int i = 0; i < Arguments.Length; i++)
            {
                sb.CommaWhen(i > 0);
                Arguments[i].Disassemble(sb);
            }

            sb.Append(")");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            for (int i = 0; i < Arguments.Length; i++)
            {
                p.Begin(ref Arguments[i]);
                Arguments[i].Visit(p);
                p.End(ref Arguments[i]);
            }
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new CallShader(Source, state.GetMember(Function), Arguments.Copy(state));
        }
    }
}