using Uno.Compiler.API.Domain.Graphics;

namespace Uno.Disasm.ILView.Members
{
    public class MetaPropertyItem : ILItem
    {
        public readonly MetaProperty MetaProperty;

        public override string DisplayName => MetaProperty.Name;
        public override ILIcon Icon => ILIcon.MetaProperty;
        public override object Object => MetaProperty;

        public MetaPropertyItem(MetaProperty metaProperty)
        {
            MetaProperty = metaProperty;
            Suffix = metaProperty.ReturnType.ToString();
        }

        protected override void Disassemble(Disassembler disasm)
        {
            disasm.AppendHeader(MetaProperty);
            disasm.AppendMetaProperty(MetaProperty);
        }
    }
}
