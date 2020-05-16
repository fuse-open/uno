using System.IO;
using Uno.Disasm.ILView.Commands;

namespace Uno.Disasm.ILView.Packages
{
    public class FileItem : FileBase, IFile
    {
        public readonly string SourceDirectory;
        public readonly string UnixName;

        public FileItem(string sourceDir, string unixName)
        {
            SourceDirectory = sourceDir;
            UnixName = unixName;
        }

        public override string FullName => Path.Combine(SourceDirectory, UnixName.UnixToNative());

        public override string DisplayName => UnixName.UnixBaseName();

        protected override void Disassemble(Disassembler disasm)
        {
            disasm.BeginHeader(UnixName);
            disasm.AppendLine("/// FullName:     " + FullName);
            disasm.AppendLine("/// LastChanged:  " + File.GetLastWriteTime(FullName));
            disasm.AppendLine("/// Length:       " + Contents.Length.ToString("N0"));
            disasm.EndHeader();
            disasm.Append(Contents);
        }
    }
}