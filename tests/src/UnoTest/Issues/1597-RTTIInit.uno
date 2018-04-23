using Uno.Testing;

namespace RTTIInit
{
    class Outer1<TSelf, TResult>
    {
        Inner.FailingDelegateType _myField;

        public Outer1()
        {
            new Inner();
            _myField = null;
        }

        public class Inner
        {
            public delegate Uno.Collections.IEnumerable<TResult> FailingDelegateType(Outer1<int, int> o);
        }
    }

    class Outer2<TSelf, TResult>
    {
        Inner.FailingDelegateType _myField;

        public Outer2()
        {
            new Inner();
            _myField = null;
        }

        public class Inner
        {
            public delegate Outer2<int, int> FailingDelegateType(Uno.Collections.List<TResult> o);
        }
    }

    public class Test
    {
        [Test]
        public void Test1()
        {
            new Outer1<bool, bool>();
        }

        [Test]
        public void Test2()
        {
            new Outer2<bool, bool>();
        }
    }
}