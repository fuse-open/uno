using System.Text;
using Uno.Compiler.API.Domain.Graphics;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class LoadVarying : Expression
    {
        public DrawState State;
        public int Index;
        public int[] Fields = new int[0];

        public override DataType ReturnType => State.Varyings[Index].Type;

        public LoadVarying(Source src, DrawState state, int index)
            : base(src)
        {
            State = state;
            Index = index;
        }

        public override ExpressionType ExpressionType => ExpressionType.LoadVarying;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("interpolate::" + State.Varyings[Index].Name);

            if (Fields.Length > 0)
            {
                sb.Append(".");
                var fields = new[] { 'X', 'Y', 'Z', 'W' };

                for (int i = 0; i < Fields.Length; i++)
                    sb.Append(fields[i]);
            }
        }

        public override Expression CopyExpression(CopyState copy)
        {
            return new LoadVarying(Source, State, Index);
        }
    }
}