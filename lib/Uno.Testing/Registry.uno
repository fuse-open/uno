using Uno.Collections;

namespace Uno.Testing
{
    public class Registry
    {
        private List<NamedTestMethod> _tests = new List<NamedTestMethod>();

        public void Add(Action method, string name, bool ignore, string ignoreReason)
        {
            _tests.Add(new NamedTestMethod(method, name, ignore, ignoreReason));
        }

        internal int Count { get { return _tests.Count; } }
        internal NamedTestMethod this[int index] { get { return _tests[index]; } }
    }
}
