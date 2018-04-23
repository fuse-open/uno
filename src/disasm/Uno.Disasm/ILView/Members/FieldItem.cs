using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Disasm.ILView.Members
{
    public class FieldItem : ILItem
    {
        public readonly Field Field;
        public override string DisplayName => Field.Name;
        public override object Object => Field;
        public override ILIcon Icon => Field.IsStatic ?
            Field.IsPublic ? ILIcon.FieldStatic : ILIcon.FieldStaticNonPublic :
            Field.IsPublic ? ILIcon.Field : ILIcon.FieldNonPublic;

        public FieldItem(Field field)
        {
            Field = field;
            Suffix = field.ReturnType.ToString();
        }

        protected override void Disassemble(Disassembler disasm)
        {
            disasm.AppendHeader(Field);
            disasm.AppendField(Field);
        }
    }
}
