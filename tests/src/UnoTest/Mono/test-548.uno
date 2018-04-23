namespace Mono.test_548
{
    using Uno;
    
    namespace Bugs
    {
        class Bug0
        {
            struct MyBoolean
            {
                private bool value;
                public MyBoolean(bool value)
                {
                    this.value = value;
                }
                public static implicit operator MyBoolean(bool value)
                {
                    return new MyBoolean(value);
                }
                public static implicit operator bool(MyBoolean b)
                {
                    return b.value;
                }
            }
    
            [Uno.Testing.Test] public static void test_548() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
                MyBoolean b = true;
                if (true && b)
                {
                    return 0;
                }
                else
                {
                    return 100;
                }
            }
        }
    }
}
