namespace Uno.Compiler.API.Domain.Bytecode
{
    public struct Instruction
    {
        public Opcodes Opcode;
        public object Argument;

        public Instruction(Opcodes opcode, object arg = null)
        {
            Opcode = opcode;
            Argument = arg;
        }

        public override string ToString()
        {
            return Opcode + (Argument != null ? " " + Argument : "");
        }
    }
}
