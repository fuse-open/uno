using Uno.Compiler.API.Domain.Extensions;

namespace Uno.Disasm.ILView.Packages
{
    public class BundleFileItem : FileBase
    {
        public readonly BundleFile BundleFile;

        public BundleFileItem(BundleFile file)
        {
            BundleFile = file;
        }

        public override object Object => BundleFile;
        public override string FullName => BundleFile.SourcePath;
        public override string DisplayName => BundleFile.TargetName;

        protected override void Disassemble(Disassembler disasm)
        {
            disasm.BeginHeader(BundleFile.TargetName);
            disasm.AppendLine("/// SourcePath:   " + BundleFile.SourcePath);
            disasm.EndHeader();
            disasm.Append(Contents);
        }
    }
}