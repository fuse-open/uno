using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.Bytecode
{
    public class Label
    {
        public TryCatchFinally TryCatchBlock;
        public int Offset = -1;

        public Label(TryCatchFinally tcBlock)
        {
            TryCatchBlock = tcBlock;
        }

        public override string ToString()
        {
            return "[" + Offset.ToString("0000") + "]";
        }
    }
}
