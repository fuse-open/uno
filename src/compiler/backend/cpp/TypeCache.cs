using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Backends.CPlusPlus
{
    public struct TypeCache
    {
        public readonly Dictionary<DataType, int> Global;
        public readonly Dictionary<DataType, int> Scope;
        public readonly Dictionary<DataType, int> Precalc;

        public TypeCache(
            Dictionary<DataType, int> global,
            Dictionary<DataType, int> scope = null,
            Dictionary<DataType, int> precalc = null)
        {
            Global = global;
            Scope = scope;
            Precalc = precalc;
        }
    }
}
