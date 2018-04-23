using System;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public struct NamescopeKey : IEquatable<NamescopeKey>
    {
        public readonly Namescope Namescope;
        public readonly string Path;
        public readonly int Number;
        public readonly int Hash;

        public NamescopeKey(Namescope ns, string path, int number = 0)
        {
            Namescope = ns;
            Path = path;
            Number = number;
            Hash = 27;
            Hash = 13 * Hash + path.GetHashCode();
            Hash = 13 * Hash + ns.GetHashCode();
            Hash = 13 * Hash + number;
        }

        public override int GetHashCode()
        {
            return Hash;
        }

        public override bool Equals(object obj)
        {
            return obj is NamescopeKey && Equals((NamescopeKey) obj);
        }

        public bool Equals(NamescopeKey ns)
        {
            return ns.Namescope == Namescope && ns.Path == Path && ns.Number == Number;
        }

        public override string ToString()
        {
            return Namescope + ":" + Path + "`" + Number;
        }
    }
}
