namespace Mono.gtest_426
{
    using Uno;
    
    namespace OverloadTest
    {
        public interface MyInterface<T>
        {
            void Invoke (T target);
        }
    
        public class MyClass<T>
        {
    
            public bool Method (MyInterface<T> obj)
            {
                return Method (obj.Invoke);
            }
    
            public bool Method (Action<T> myAction)
            {
                return true;
            }
        }
    
        class C
        {
            [Uno.Testing.Test] public static void gtest_426() { Main(); }
        public static void Main()
            {
            }
        }
    }
}
