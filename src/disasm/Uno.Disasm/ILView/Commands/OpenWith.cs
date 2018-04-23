using System.Diagnostics;
using System.IO;
using Uno.Diagnostics;
using Uno.Disasm.ILView.Packages;

namespace Uno.Disasm.ILView.Commands
{
    public class OpenWith : ILCommand
    {
        public override string Header => "Open with...";

        public override bool IsDefault(ILItem item)
        {
            return item.GetVisibleChildCount() == 0;
        }

        public override ILIcon GetIcon(ILItem item)
        {
            return item.Icon;
        }

        public override bool CanShow(ILItem item)
        {
            return PlatformDetection.IsWindows && item is IFile && !(item is FolderBase);
        }

        public override bool CanExecute(ILItem item)
        {
            var file = (IFile)item;
            return File.Exists(file.FullName);
        }

        public override void Execute(ILItem item)
        {
            var file = (IFile)item;
            Process.Start("rundll32.exe", "shell32.dll, OpenAs_RunDLL " + file.FullName.QuoteSpace());
        }
    }
}