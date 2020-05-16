using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Backends.CPlusPlus
{
    public class CppFunction
    {
        public readonly HashSet<DataType> Dependencies = new HashSet<DataType>();
        public readonly HashSet<DataType> PrecalcedTypes = new HashSet<DataType>();
        public readonly List<Variable> Constrained = new List<Variable>();
        public HashSet<IEntity> CachedDependencies;
    }
}
