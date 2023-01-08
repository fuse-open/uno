using Uno.Compiler;

namespace Uno.Disasm.ILView.Packages
{
    public class PackageFolderItem : FolderBase
    {
        public PackageFolderItem(SourceBundle bundle, string name)
            : base(bundle.SourceDirectory, name)
        {
        }

        public PackageFolderItem(string outputDir, string name)
            : base(outputDir, name)
        {
        }

        public override ILIcon Icon => ILIcon.SolutionFolder;
    }
}