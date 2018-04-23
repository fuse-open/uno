namespace Mono.gtest_220
{
    public class A<T1>
    {
        public T1 a;
    
        public class B<T2> : A<T2>
        {
            public T1 b;
    
            public class C<T3> : B<T3>
            {
                public T1 c;
            }
        }
    }
    
    class PopQuiz
    {
        [Uno.Testing.Ignore, Uno.Testing.Test] public static void gtest_220() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            A<int>.B<char>.C<bool> o = new A<int>.B<char>.C<bool>();
            string s = o.a.GetType().FullName;
            Console.WriteLine(s);
            if (s != "Uno.Boolean")
                return 1;
    
            s = o.b.GetType().FullName;
            Console.WriteLine(s);
            if (s != "Uno.Char")
                return 2;
            
            s = o.c.GetType().FullName;
            Console.WriteLine();
            if (s != "Uno.Int")
                return 3;
            
            return 0;
        }
    }
}
