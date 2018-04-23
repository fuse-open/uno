namespace Mono.test_711
{
    using Uno;
    
    namespace N
    {
        enum FieldType
        {
            Foo
        }
    }
    
    namespace N
    {
        class Test
        {
            public FieldType FieldType = FieldType.Foo;
    
            public Test ()
            {
            }
    
            public Test (int i)
            {
            }
    
            [Uno.Testing.Test] public static void test_711() { Main(); }
        public static void Main()
            {
            }
        }
    }
}
