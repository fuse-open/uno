using System.Collections.Generic;
using Uno.Compiler.Backends.UnoDoc.Builders.Extensions;

namespace Uno.Compiler.Backends.UnoDoc.Builders
{
    public class AttachedUxPropertyEqualityComparer : IEqualityComparer<AttachedUxProperty>
    {
        public bool Equals(AttachedUxProperty x, AttachedUxProperty y)
        {
            return x.UnderlyingMethod.GetUri() == y.UnderlyingMethod.GetUri();
        }

        public int GetHashCode(AttachedUxProperty obj)
        {
            return obj.UnderlyingMethod.GetUri().GetHashCode();
        }
    }
}