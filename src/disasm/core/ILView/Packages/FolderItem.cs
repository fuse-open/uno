using System.IO;
using Uno.Disasm.ILView.Commands;

namespace Uno.Disasm.ILView.Packages
{
    public class FolderItem : FolderBase, IFile
    {
        public FolderItem(string sourceDir, string name)
            : base(sourceDir, name)
        {
        }

        public string FullName => Path.Combine(SourceDirectory, UnixName.UnixToNative());
        public override ILIcon Icon => IsExpanded ? ILIcon.FolderOpen : ILIcon.FolderClosed;
    }
}
