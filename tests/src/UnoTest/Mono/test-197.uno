namespace Mono.test_197
{
    using Uno;
    
    public interface A
    {
        void Foo ();
    }
    
    public interface B : A
    { }
    
    public abstract class X : A
    {
        public abstract void Foo ();
    }
    
    public abstract class Y : X, B
    { }
    
    public class Z : Y
    {
        public override void Foo ()
        {
            Console.WriteLine ("Hello World!");
        }
    }
    
    class Test
    {
        [Uno.Testing.Test] public static void test_197() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Z z = new Z ();
            A a = z;
            a.Foo ();
            return 0;
        }
    }
}
