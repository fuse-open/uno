namespace Mono.gtest_244
{
    public class B
    {
        public virtual T Get<T> ()
        {
            return default (T);
        }
    }
    
    public class A : B
    {
        public override T Get<T>()
        {
            T resp = base.Get<T> ();
            Console.WriteLine("T: " + resp);
            return resp;
        }
    
        [Uno.Testing.Test] public static void gtest_244() { Main(); }
        public static void Main()
        {
            new A().Get<int> ();
        }
    }
}
