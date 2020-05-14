using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Disasm.ILView.Members
{
    public class PropertyItem : ILItem
    {
        public readonly Property Property;

        public override string DisplayName => Property.NameWithParameterList;
        public override object Object => Property;

        public override ILIcon Icon
        {
            get
            {
                if (Property.Parameters.Length > 0)
                    return ILIcon.Indexer;

                return Property.IsStatic ?
                    Property.IsPublic ? ILIcon.PropertyStatic : ILIcon.PropertyStaticNonPublic :
                    Property.IsPublic ? ILIcon.Property : ILIcon.PropertyNonPublic;
            }
        }

        public PropertyItem(Property p)
        {
            Property = p;
            Suffix = p.ReturnType.ToString();

            if (p.GetMethod != null)
                AddChild(new FunctionItem(p.GetMethod));
            if (p.SetMethod != null)
                AddChild(new FunctionItem(p.SetMethod));
            if (p.ImplicitField != null)
                AddChild(new FieldItem(p.ImplicitField));
        }

        protected override void Disassemble(Disassembler disasm)
        {
            disasm.AppendHeader(Property);
            disasm.AppendProperty(Property);
        }
    }
}
