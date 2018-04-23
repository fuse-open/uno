namespace Mono.test_partial_31
{
    using Uno;
    
    namespace TestPartialOverride.BaseNamespace
    {
        public abstract class Base
        {
            protected virtual void OverrideMe ()
            {
                Console.Out.WriteLine ("OverrideMe");
            }
        }
    }
    
    namespace TestPartialOverride.Outer.Nested.Namespace
    {
        internal partial class Inherits
        {
            protected override void OverrideMe ()
            {
                Console.Out.WriteLine ("Overridden");
            }
        }
    }
    
    namespace TestPartialOverride.Outer
    {
        namespace Nested.Namespace
        {
            internal partial class Inherits : TestPartialOverride.BaseNamespace.Base
            {
                public void DoesSomethignElse ()
                {
                    OverrideMe ();
                }
            }
        }
    
        public class C
        {
            [Uno.Testing.Ignore, Uno.Testing.Test] public static void test_partial_31() { Main(); }
        public static void Main()
            {
                new TestPartialOverride.Outer.Nested.Namespace.Inherits ().DoesSomethignElse ();
            }
        }
    }
}
