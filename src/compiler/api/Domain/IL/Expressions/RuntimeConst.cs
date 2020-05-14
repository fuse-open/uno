using System.Text;
using Uno.Compiler.API.Domain.Graphics;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class RuntimeConst : Expression
    {
        public DrawState State;
        public int Index;

        public override DataType ReturnType => State.RuntimeConstants[Index].Type;

        public RuntimeConst(Source src, DrawState ds, int i)
            : base(src)
        {
            State = ds;
            Index = i;
        }

        public override ExpressionType ExpressionType => ExpressionType.RuntimeConst;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("const::" + State.RuntimeConstants[Index].Name);
        }

        public override Expression CopyExpression(CopyState copy)
        {
            return new RuntimeConst(Source, State, Index);
        }
    }
}