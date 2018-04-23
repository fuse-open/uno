namespace Mono.gtest_465
{
    // Compiler options: -r:gtest-465-lib.dll
    
    public class DerivedClass : InterfaceWithGenericMethod
    {
        public void GenericMethod_1<T> () where T : struct, II
        {
        }
    
        public void GenericMethod_2<T> () where T : class, II
        {
        }
    
        public void GenericMethod_3<T> () where T : II, new ()
        {
        }
    }
    
    class Program
    {
        [Uno.Testing.Test] public static void gtest_465() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            new DerivedClass ();
            return 0;
        }
    }
}
