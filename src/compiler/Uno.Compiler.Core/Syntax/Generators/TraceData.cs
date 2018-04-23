using System.Collections.Generic;
using Uno.Compiler.API.Domain.Graphics;

namespace Uno.Compiler.Core.Syntax.Generators
{
    class TraceData
    {
        public readonly List<MetaLocation> Stack = new List<MetaLocation>();
        public readonly List<TraceError> Errors = new List<TraceError>();
        public readonly HashSet<MetaLocation> VisitedLocations = new HashSet<MetaLocation>();
    }
}