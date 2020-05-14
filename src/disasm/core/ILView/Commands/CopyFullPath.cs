using System.Windows.Forms;
using Uno.Diagnostics;

namespace Uno.Disasm.ILView.Commands
{
    public class CopyFullPath : ILCommand
    {
        public override string Header => "Copy Full Path";

        public override bool CanShow(ILItem item)
        {
            return PlatformDetection.IsWindows && item is IFile;
        }

        public override void Execute(ILItem item)
        {
            var file = (IFile)item;
            Clipboard.SetText(file.FullName);
        }
    }
}