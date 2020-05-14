using Uno.Compiler.API.Domain.IL;

namespace Uno.Disasm.ILView.Namespaces
{
    public class NamespaceItem : ILItem
    {
        public readonly Namespace Namespace;
        public override string DisplayName => Namespace.FullName;
        public override ILIcon Icon => ILIcon.Namespace;

        public NamespaceItem(Namespace @namespace)
        {
            Namespace = @namespace;
        }

        protected override void Disassemble(Disassembler disasm, bool publicOnly)
        {
            disasm.AppendHeader(Namespace);
            disasm.BeginNamespace(Namespace);

            foreach (var child in Children)
            {
                var typeItem = child as TypeItem;
                if (typeItem != null && (
                        !publicOnly || 
                        typeItem.Type.IsPublic && !typeItem.Type.IsGenerated
                    ))
                    disasm.AppendType(typeItem.Type, true);
            }


            disasm.EndNamespace();
        }
    }
}
