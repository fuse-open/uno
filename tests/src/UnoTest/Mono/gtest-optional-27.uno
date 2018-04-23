namespace Mono.gtest_optional_27
{
    using Uno;
    
    class EnumWrapperCtor<T>
    {
        public enum Test
        {
            Wrong,
            MyDefault
        }
    
        readonly Test myVal;
    
        public EnumWrapperCtor (Test value = Test.MyDefault)
        {
            myVal = value;
        }
    
        public Test getValue ()
        {
            return myVal;
        }
    }
    
    public class C
    {
        [Uno.Testing.Test] public static void gtest_optional_27() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            var ew = new EnumWrapperCtor<int> ();
            if ((int) ew.getValue () != 1)
                return 1;
    
            return 0;
        }
    }
}
