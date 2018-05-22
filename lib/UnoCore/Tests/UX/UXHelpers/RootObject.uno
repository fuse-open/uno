using Uno;
using Uno.Collections;
using Uno.UX;

namespace UXHelpers
{
    public class RootObject : PropertyObject
    {
        readonly List<object> _children = new List<object>();

        [UXPrimary]
        public IList<object> Children { get { return _children; } }

        [UXName]
        public string Name { get; set; }

        public virtual void Root()
        {
            foreach (var child in Children)
            {
                var po = child as RootObject;
                if (po != null)
                    po.Root();
            }
        }
    }
}
