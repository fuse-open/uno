namespace Uno.Build.Targets.Generators
{
    class XcodeFile
    {
        public readonly string BuildFileUUID;
        public readonly string FileReferenceUUID;
        public readonly string Name;
        public readonly string Path;
        public readonly string Category;
        public readonly string SourceTree;
        public readonly string Settings;

        public XcodeFile(string filename, string category, string sourceTree, string settings = null)
        {
            BuildFileUUID = CreateUUID();
            FileReferenceUUID = CreateUUID();
            Name = System.IO.Path.GetFileName(filename);
            Path = filename;
            Category = category;
            SourceTree = sourceTree;
            Settings = settings;
        }

        static ulong _uuid = 0;

        public static string CreateUUID()
        {
            var result = "F0057001" + _uuid.ToString("X16");
            ++_uuid;
            return result;
        }
    }
}