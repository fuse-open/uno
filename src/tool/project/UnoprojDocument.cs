using System.Collections.Generic;
using Uno.Collections;

namespace Uno.ProjectFormat
{
    public class UnoprojDocument
    {
        public readonly LowerCamelDictionary<SourceValue> Properties = new LowerCamelDictionary<SourceValue>();
        public readonly List<IncludeItem> Includes = new List<IncludeItem>();
        public List<ProjectReference> OptionalProjectReferences;
        public List<LibraryReference> OptionalLibraryReferences;
        public List<SourceValue> OptionalInternalsVisibleTo;
        public List<SourceValue> OptionalExcludes;
    }
}
