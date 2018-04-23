namespace Mono.test_712
{
    using Uno;
    
    interface IFoo
    {
        bool Equals (object o);
    }
    
    class Hello : IFoo
    {
        [Uno.Testing.Test] public static void test_712() { Main(); }
        public static void Main()
        {
            IFoo f = new Hello ();
            int i = f.GetHashCode ();
            bool b = f.Equals (f);
        }
    }
}
