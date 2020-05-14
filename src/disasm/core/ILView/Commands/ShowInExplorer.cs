using System.Diagnostics;
using System.IO;
using Uno.Diagnostics;

namespace Uno.Disasm.ILView.Commands
{
    public class ShowInExplorer : ILCommand
    {
        public override string Header => "Show in Explorer";

        public override ILIcon GetIcon(ILItem item)
        {
            return ILIcon.FolderOpen;
        }

        public override bool CanShow(ILItem item)
        {
            return PlatformDetection.IsWindows && item is IFile;
        }

        public override bool CanExecute(ILItem item)
        {
            var file = (IFile)item;
            return Directory.Exists(file.FullName) || File.Exists(file.FullName);
        }

        public override void Execute(ILItem item)
        {
            var file = (IFile)item;
            Process.Start("explorer.exe", (Directory.Exists(file.FullName) ? null : "/select,") + file.FullName.QuoteSpace());
        }
    }
}
