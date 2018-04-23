namespace Mono.gtest_autoproperty_07
{
    struct Foo
    {
        public Foo (object newValue)
            : this ()
        {
            this.NewValue = newValue;
        }
    
        public object NewValue
        {
            get;
            private set;
        }
    }
    
    class C
    {
        [Uno.Testing.Test] public static void gtest_autoproperty_07() { Main(); }
        public static void Main()
        {
        }
    }
}
