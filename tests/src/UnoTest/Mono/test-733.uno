namespace Mono.test_733
{
    using Uno;
    
    public class Test
    {
        [Uno.Testing.Test] public static void test_733() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            float a = 1f / 3;
            float b = (float) Math.Acos ((float) (a * 3));
            Console.WriteLine (b);
            if (b != 0 && !float.IsNaN (b)) {
                throw new ApplicationException (b.ToString () + b.GetType ().ToString ());
            }
    
            return 0;
        }
    }
}
