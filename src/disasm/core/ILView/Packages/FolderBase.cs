using System.Collections.Generic;
using System.Linq;

namespace Uno.Disasm.ILView.Packages
{
    public abstract class FolderBase : ILItem
    {
        public string SourceDirectory;
        public string UnixName;
        readonly Dictionary<string, FolderItem> _subFolders = new Dictionary<string, FolderItem>();
        protected string _displayName;

        protected FolderBase(string sourceDir, string unixName)
        {
            SourceDirectory = sourceDir;
            UnixName = unixName;
        }

        public override Syntax Syntax => Syntax.Stuff;

        public override string DisplayName => _displayName ?? (_displayName = UnixName.UnixBaseName());

        public void AddFile(string sourceDir, string unixFile)
        {
            GetFolder(unixFile.UnixDirectoryName()).AddChild(new FileItem(sourceDir, unixFile));
        }

        public void Collapse()
        {
            foreach (var c in Children)
                if (c is FolderBase)
                    (c as FolderBase).Collapse();

            foreach (var c in Children.ToArray())
            {
                var folder = c as FolderBase;
                if (folder == null)
                    continue;

                var folderChildCount = 0;
                foreach (var folderChild in folder.Children)
                    if (folderChild is FolderBase)
                        folderChildCount++;

                if (folderChildCount == folder.Children.Count)
                {
                    foreach (var folderChild in folder.Children)
                    {
                        var childFolder = (FolderBase) folderChild;
                        childFolder._displayName = folder.DisplayName + "/" + childFolder.DisplayName;
                        Children.Add(childFolder);
                    }

                    folder.Children.Clear();
                }
            }
        }

        FolderBase GetFolder(string unixDir)
        {
            var parts = unixDir.Split('/');

            if (parts.Length > 1)
            {
                var f = this;
                for (int i = 0; i < parts.Length; i++)
                    f = f.GetFolder(parts[i]);
                return f;
            }

            if (string.IsNullOrEmpty(unixDir))
                return this;

            FolderItem folder;
            if (!_subFolders.TryGetValue(unixDir, out folder))
            {
                folder = new FolderItem(SourceDirectory, this is PackageFolderItem ? unixDir : UnixName + "/" + unixDir);
                _subFolders.Add(unixDir, folder);
                AddChild(folder);
            }

            return folder;
        }

        void FindFiles(List<FileItem> files)
        {
            foreach (var c in Children)
                if (c is FileItem)
                    files.Add(c as FileItem);
                else if (c is FolderBase)
                    (c as FolderItem).FindFiles(files);
        }

        protected override void Disassemble(Disassembler disasm)
        {
            disasm.AppendHeader(UnixName);

            var files = new List<FileItem>();
            FindFiles(files);

            foreach (var f in files)
                disasm.AppendLine(f.UnixName);
        }
    }
}