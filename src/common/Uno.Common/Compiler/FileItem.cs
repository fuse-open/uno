using System;

namespace Uno.Compiler
{
    public struct FileItem
    {
        public string UnixPath;
        public string Condition;

        public string NativePath => UnixPath.UnixToNative();

        public FileItem(string path, string cond = null)
        {
            UnixPath = path;
            Condition = cond;
        }

        public static implicit operator FileItem(string path)
        {
            return new FileItem(path);
        }

        public override string ToString()
        {
            return UnixPath + (
                    !string.IsNullOrEmpty(Condition)
                        ? ":" + Condition
                        : null
                );
        }

        public static FileItem FromString(string str)
        {
            var parts = str.Split(':');
            if (parts.Length > 2)
                throw new ArgumentException("Invalid arguments provided for 'FILENAME[:CONDITION]'");
            return new FileItem(
                    parts[0],
                    parts.Length > 1
                        ? parts[1]
                        : null);
        }
    }
}
