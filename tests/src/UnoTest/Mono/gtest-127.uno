namespace Mono.gtest_127
{
    public class A<T>
    {
        public delegate void Changed (A<T> a);
    
        protected event Changed _changed;
    
        public void Register (Changed changed)
        {
            _changed += changed;
            _changed (this);
        }
    }
    
    public class Test
    {
        [Uno.Testing.Test] public static void gtest_127() { Main(); }
        public static void Main()
        {
            A<int> a = new A<int> ();
            a.Register (new A<int>.Changed (Del));
        }
    
        public static void Del (A<int> a)
        {
            Console.WriteLine ("Solved");
        }
    }
}
