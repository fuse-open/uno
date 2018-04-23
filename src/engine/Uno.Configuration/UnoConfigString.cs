using Uno.IO;

namespace Uno.Configuration
{
    public struct UnoConfigString
    {
        public string ParentDirectory;
        public string Value;

        public override string ToString()
        {
            if (Value.IsFullPath() || Value.Contains("://"))
                return Value;

            var relative = ParentDirectory.ToRelativePath();
            return !string.IsNullOrEmpty(relative)
                ? "(" + relative + ") " + Value
                : Value;
        }
    }
}