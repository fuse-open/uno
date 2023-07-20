using System.IO;
using Uno.Compiler;

namespace Uno.Disasm.ILView.Bundles
{
    public class ReferenceItem : ILItem
    {
        public readonly SourceBundle Bundle;

        public override object Object => Bundle;
        public override string DisplayName => Bundle.Name;
        public override ILIcon Icon => Path.GetExtension(Bundle.Source.FullPath).ToUpperInvariant() == ".UNOPROJ"
            ? ILIcon.ProjectProjectReference
            : ILIcon.ProjectLibraryReference;

        public ReferenceItem(SourceBundle bundle)
        {
            Bundle = bundle;

            if (Bundle.Version != null)
                Suffix = "(" + Bundle.Version + ")";

            foreach (var reference in bundle.References)
                AddChild(new ReferenceItem(reference));
        }

        protected override void Disassemble(Disassembler disasm)
        {
            disasm.AppendHeader(Bundle);

            foreach (var c in Children)
                disasm.AppendLine(c.ToString());
        }
    }
}