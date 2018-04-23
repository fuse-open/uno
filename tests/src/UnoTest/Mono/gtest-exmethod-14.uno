namespace Mono.gtest_exmethod_14
{
    using Uno;
    
    public interface IA
    {
        void Foo (IA self);
    }
    
    public static class C
    {
        public static void Foo (this IA self)
        {
            self.Foo<int> ();
        }
        
        public static void Bar<U> (this IA self)
        {
            self.Foo<U> ();
        }
        
        public static void Foo<T> (this IA self)
        {
        }
        
        [Uno.Testing.Test] public static void gtest_exmethod_14() { Main(); }
        public static void Main()
        {
        }
    }
}
