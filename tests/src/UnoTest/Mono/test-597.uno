namespace Mono.test_597
{
    namespace TestNS
    {
        public interface IHoge {}

        public class Foo {}

        public class XElement : Element
        {
            public new Bar Document { get { return null; } }

            public object CrashHere {
                get { return (Document.Root == this) ? null : ""; }
            }
        }

        public class Element
        {
            public Foo Document { get { return null; } }
        }

        public class Bar
        {
            public IHoge Root { get { return null; } }
        }

        public class C
        {
            [Uno.Testing.Test] public static void test_597() { Main(); }
        public static void Main()
            {
            }
        }
    }
}
