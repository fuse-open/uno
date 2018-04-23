using System.Collections.Generic;
using Uno.Disasm.ILView.Packages;

namespace Uno.Disasm.ILView
{
    public class BuildItem : ILItem
    {
        public readonly List<PackageFolderItem> Folders = new List<PackageFolderItem>();
        public readonly List<PackageItem> Packages = new List<PackageItem>();

        public string TargetName { get; }
        public string BuildLog { get; set; }
        public override string DisplayName => TargetName ?? "(null)";
        public override ILIcon Icon => ILIcon.ApplicationDocument;
        public override Syntax Syntax => Syntax.None;

        public BuildItem(string targetName, string configuration = null)
        {
            TargetName = targetName;
            if (!string.IsNullOrEmpty(configuration))
                Suffix = "(" + configuration + ")";
        }

        protected override void Disassemble(Disassembler disasm)
        {
            disasm.Append(BuildLog);
        }
    }
}
