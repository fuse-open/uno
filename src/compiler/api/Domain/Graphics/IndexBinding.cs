using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Domain.Graphics
{
    public class IndexBinding
    {
        public Expression IndexType;
        public Expression Buffer;

        public IndexBinding(Expression type, Expression buffer)
        {
            IndexType = type;
            Buffer = buffer;
        }
    }
}