namespace Mono.gtest_591
{
    // Compiler options: -target:library
    
    using Uno;
    
    namespace A
    {
        public class B<T>
        {
            public abstract class C : Uno.IEquatable<C>
            {
                public abstract bool Equals (C other);
            }
        }
    }
}
