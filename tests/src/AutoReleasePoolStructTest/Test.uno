using Uno;
using Uno.Reflection;
using Uno.Testing;

namespace Tests
{
    public struct MyStruct
    {
        // Needs to be a struct with a reference field, otherwise it won't break :)
        string SomeClassField;
    }

    public class MyType
    {
        // Changing this property's type to `object` gets around the bug.
        //  This is because then the struct will get wrapped in this function before it gets returned.
        public MyStruct Value { get { return new MyStruct(); } }
    }

    public class Tests
    {
        [Test]
        public void Test()
        {
            if defined(CPLUSPLUS && REFLECTION) TestImpl();

            Assert.IsTrue(true); // If the test breaks, we will never get here
        }

        extern(CPLUSPLUS && REFLECTION)
        void TestImpl()
        {
            extern "uAutoReleasePool ____pool";

            var obj = new MyType();

            // Need to refer to the property explicitly, else it gets stripped
            debug_log "Got value without reflection: " + obj.Value;

            var functions = CppReflection.GetFunctions(obj.GetType());
            foreach (var f in functions)
            {
                if (f.Name == "get_Value")
                {
                    var getter = f;

                    // The `Invoke` impl should trigger some bad objects to be put into the AutoReleasePool, causing issues when the function returns.
                    //  Note that it may not be caught without DEBUG_ARC defined!!
                    //  See the original fix description here: https://github.com/fusetools/uno/pull/1526
                    var value = getter.Invoke(obj);
                    // Do something with the value
                    debug_log "Got value with reflection: " + value;
                }
            }
        }
    }
}
