using System;
using System.IO;
using Uno.IO;

namespace Uno.ProjectFormat
{
    public class ProjectReference : IComparable<ProjectReference>
    {
        public Source Source { get; set; }
        public string ProjectPath { get; set; }
        public string ProjectName => Path.GetFileNameWithoutExtension(ProjectPath);

        public ProjectReference(Source src, string path)
        {
            Source = src;
            ProjectPath = path;
        }

        public static ProjectReference FromString(string str)
        {
            return FromString(Source.Unknown, str);
        }

        public static ProjectReference FromString(Source src, string str)
        {
            var parts = str.PathSplit();

            if (parts.Length > 1)
                throw new ArgumentException("Invalid arguments provided for 'FILENAME'");

            return new ProjectReference(src, parts[0].Trim());
        }

        public override string ToString()
        {
            return ProjectPath.NativeToUnix();
        }

        public int CompareTo(ProjectReference other)
        {
            return string.Compare(ProjectPath, other.ProjectPath, StringComparison.InvariantCultureIgnoreCase);
        }

        public string GetFullPath(string parentDir)
        {
            return ProjectPath.ToFullPath(parentDir);
        }
    }
}
