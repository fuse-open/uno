using Uno;
using Uno.Collections;
using Uno.UX;

namespace UXHelpers
{
    [UXTestBootstrapperFor("UXHelpers.ParentObject")]
    public class UXParentObjectBootstrapper
    {
        [UXContent]
        public ParentObject Child { get; set; }

        public bool TestWasRun = false;
        public void RunTest() 
        {
            TestWasRun = true;
        }
    }

    public class ParentObject : PropertyObject
    {
        readonly List<object> _children = new List<object>();

        [UXPrimary]
        public IList<object> Children { get { return _children; } }

        [UXName]
        public string Name { get; set; }
    }
}
