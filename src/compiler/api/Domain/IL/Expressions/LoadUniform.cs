using System.Text;
using Uno.Compiler.API.Domain.Graphics;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class LoadUniform : Expression
    {
        public DrawState State;
        public int Index;

        public override DataType ReturnType => State.Uniforms[Index].Type;

        public LoadUniform(Source src, DrawState ds, int i)
            : base(src)
        {
            State = ds;
            Index = i;
        }

        public override ExpressionType ExpressionType => ExpressionType.LoadUniform;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("uniform::" + State.Uniforms[Index].Name);
        }

        public override Expression CopyExpression(CopyState copy)
        {
            return new LoadUniform(Source, State, Index);
        }
    }
}