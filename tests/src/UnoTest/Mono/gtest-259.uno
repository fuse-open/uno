namespace Mono.gtest_259
{
    using Uno;
    
    public class Class1<T>
        where T : MyType
    {
        public void MethodOfClass1 (T a, MyType b)
        {
            a.MethodOfMyBaseType ();
        }
    }
    
    public class MyType : MyBaseType
    {
        public override void MethodOfMyBaseType ()
        {
        }
    }
    
    public abstract class MyBaseType
    {
        public abstract void MethodOfMyBaseType ();
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_259() { Main(); }
        public static void Main()
        { }
    }
}
