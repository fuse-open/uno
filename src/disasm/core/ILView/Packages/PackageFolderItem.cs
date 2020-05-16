using Uno.Compiler;

namespace Uno.Disasm.ILView.Packages
{
    public class PackageFolderItem : FolderBase
    {
        public PackageFolderItem(SourcePackage upk, string name)
            : base(upk.SourceDirectory, name)
        {
        }

        public PackageFolderItem(string outputDir, string name)
            : base(outputDir, name)
        {
        }

        public override ILIcon Icon => ILIcon.SolutionFolder;
    }
}