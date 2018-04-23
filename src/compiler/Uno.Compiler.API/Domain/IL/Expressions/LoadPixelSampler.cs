using System.Text;
using Uno.Compiler.API.Domain.Graphics;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class LoadPixelSampler : Expression
    {
        public DrawState State;
        public int Index;

        public override DataType ReturnType => State.PixelSamplers[Index].Type;

        public LoadPixelSampler(Source src, DrawState ds, int i)
            : base(src)
        {
            State = ds;
            Index = i;
        }

        public override ExpressionType ExpressionType => ExpressionType.LoadPixelSampler;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("pixel_sampler::" + State.PixelSamplers[Index].Name);
        }

        public override Expression CopyExpression(CopyState copy)
        {
            return new LoadPixelSampler(Source, State, Index);
        }
    }
}