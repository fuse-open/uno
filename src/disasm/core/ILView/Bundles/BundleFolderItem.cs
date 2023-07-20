using Uno.Compiler;

namespace Uno.Disasm.ILView.Bundles
{
    public class BundleFolderItem : FolderBase
    {
        public BundleFolderItem(SourceBundle bundle, string name)
            : base(bundle.SourceDirectory, name)
        {
        }

        public BundleFolderItem(string outputDir, string name)
            : base(outputDir, name)
        {
        }

        public override ILIcon Icon => ILIcon.SolutionFolder;
    }
}