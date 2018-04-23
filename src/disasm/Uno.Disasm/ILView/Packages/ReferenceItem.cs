using System.IO;
using Uno.Compiler;

namespace Uno.Disasm.ILView.Packages
{
    public class ReferenceItem : ILItem
    {
        public readonly SourcePackage Package;

        public override object Object => Package;
        public override string DisplayName => Package.Name;
        public override ILIcon Icon => Path.GetExtension(Package.Source.FullPath).ToUpperInvariant() == ".UNOPROJ"
            ? ILIcon.ProjectProjectReference
            : ILIcon.ProjectPackageReference;

        public ReferenceItem(SourcePackage upk)
        {
            Package = upk;

            if (Package.Version != null)
                Suffix = "(" + Package.Version + ")";

            foreach (var reference in upk.References)
                AddChild(new ReferenceItem(reference));
        }

        protected override void Disassemble(Disassembler disasm)
        {
            disasm.AppendHeader(Package);

            foreach (var c in Children)
                disasm.AppendLine(c.ToString());
        }
    }
}