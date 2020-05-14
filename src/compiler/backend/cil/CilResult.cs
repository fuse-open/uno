using System.Collections.Generic;
using System.Linq;
using IKVM.Reflection;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Backends.CIL
{
    public class CilResult : BackendResult
    {
        readonly SortedSet<Location> _locations;
        readonly Dictionary<string, string> _unoNameToCilType = new Dictionary<string, string>();

        public string AssemblyName { get; }
        public IEnumerable<string> AllUnoTypeNames => _unoNameToCilType.Keys;

        internal CilResult(Assembly asm, IReadOnlyDictionary<DataType, Type> types, SortedSet<Location> loc)
        {
            AssemblyName = asm.GetName().Name;
            _locations = loc;

            foreach (var t in types)
                _unoNameToCilType[t.Key.ToString()] = t.Value.FullName;
        }

        public string GetCilTypeFromUnoName(string name)
        {
            return _unoNameToCilType[name];
        }

        public Location GetLocation(int ilOffset)
        {
            return _locations.TakeWhile(l => l.ILOffset < ilOffset).FirstOrDefault();
        }
    }
}
