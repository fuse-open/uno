using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Disasm.ILView.Members
{
    public class LiteralItem : ILItem
    {
        public readonly Literal Literal;
        public override string DisplayName => Literal.Name;
        public override ILIcon Icon => ILIcon.Constant;
        public override object Object => Literal;

        public LiteralItem(Literal literal)
        {
            Literal = literal;
            Suffix = literal.ReturnType.ToString();
        }

        protected override void Disassemble(Disassembler disasm)
        {
            disasm.AppendHeader(Literal);
            disasm.AppendLiteral(Literal);
        }
    }
}
