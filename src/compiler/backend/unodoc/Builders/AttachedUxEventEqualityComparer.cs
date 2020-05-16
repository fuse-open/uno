using System.Collections.Generic;
using Uno.Compiler.Backends.UnoDoc.Builders.Extensions;

namespace Uno.Compiler.Backends.UnoDoc.Builders
{
    public class AttachedUxEventEqualityComparer : IEqualityComparer<AttachedUxEvent>
    {
        public bool Equals(AttachedUxEvent x, AttachedUxEvent y)
        {
            return x.UnderlyingMethod.GetUri() == y.UnderlyingMethod.GetUri();
        }

        public int GetHashCode(AttachedUxEvent obj)
        {
            return obj.UnderlyingMethod.GetUri().GetHashCode();
        }
    }
}