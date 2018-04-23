namespace Mono.gtest_optional_38
{
    class C
    {
    }
    
    class Foo 
    {
            public int SetValue (string name, string value, string defaultValue = null, bool preserveExistingCase = false)
            {
                return 1;
            }
    
            public int SetValue (string name, C value, C defaultValue = default(C), bool relativeToProject = true, C relativeToPath = default(C), bool mergeToMainGroup = false, string condition = null)
            {
                return 2;
            }
    
            public int SetValue (string name, object value, C defaultValue = null)
            {
                return 3;
            }
    }
    
    class Test 
    {
        [Uno.Testing.Test] public static void gtest_optional_38() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main() 
        {
            var f = new Foo ();
            C b = null;
            C c = null;
    
            if (f.SetValue ("a", b, c) != 2)
                return 1;
    
            return 0;
        }
    }
}
