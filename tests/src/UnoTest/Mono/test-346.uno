namespace Mono.test_346
{
    using Uno;
    
    namespace TestMethods
    {
        class Class1
        {
            [Uno.Testing.Ignore, Uno.Testing.Test] public static void test_346() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
                int test_int = 1;
                TestClass testClass = new TestClass();
                test_int *= testClass.AddItem (new TestParam());
                test_int *= testClass.AddItem (new ParamClass());
    
                int base_int = 1;
                BaseClass baseClass = testClass as BaseClass;
                base_int *= baseClass.AddItem (new TestParam());
                base_int *= baseClass.AddItem (new ParamClass());
    
                return (test_int == 4 && base_int == 9) ? 0 : 1;
            }
        }
        
        public class ParamClass {}
        
        public class TestParam : ParamClass {}
        
        public abstract class BaseClass
        {
            public abstract int AddItem (ParamClass val);
        }
        
        public class TestClass : BaseClass
        {
            public int AddItem (object val)
            {
                return 2;
            }
            
            public override int AddItem (ParamClass val)
            {
                return 3;
            }
        }
    }
}
