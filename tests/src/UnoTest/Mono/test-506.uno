namespace Mono.test_506
{
    using Uno;
    
    namespace ProtectedSetter
    {
        public abstract class BaseClass
        {
            public abstract string Name { get; internal set;}
        }
    
        public class DerivedClass : BaseClass
        {
            
            public override String Name
            {
                get {
                    return null;
                }
                internal set {
                }
            }
            
            [Uno.Testing.Test] public static void test_506() { Main(); }
        public static void Main() {}
        }
    }
}
