namespace Uno.Compiler.Core.IL.Validation.ControlFlow
{
    public struct Instruction
    {
        public Source Source;
        public Opcodes Opcode;
        public object Argument;
        public Instruction(Source src, Opcodes opcode, object arg = null) { this.Source = src; this.Opcode = opcode; this.Argument = arg; }

        public override string ToString()
        {
            return Opcode.ToString() + (Argument != null ? " " + Argument : "");
        }
    }
}
