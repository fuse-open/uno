namespace Mono.test_553
{
    class A
    {
        public virtual void Add (object o)
        {
        }
    }
    
    class B : A
    {
        public virtual bool Add (object o)
        {
            return false;
        }
        
        [Uno.Testing.Test] public static void test_553() { Main(); }
        public static void Main() {}
    }
}
