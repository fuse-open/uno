using System.Collections.Generic;

namespace Uno.ProjectFormat
{
    public class UnoprojDocument
    {
        public readonly Dictionary<string, SourceValue> Properties = new Dictionary<string,SourceValue>();
        public readonly List<IncludeItem> Includes = new List<IncludeItem>();
        public List<ProjectReference> OptionalProjectReferences;
        public List<LibraryReference> OptionalLibraryReferences;
        public List<SourceValue> OptionalInternalsVisibleTo;
        public List<SourceValue> OptionalExcludes;
    }
}
