namespace Mono.gtest_122
{
    class Test
        {
            [Uno.Testing.Test] public static void gtest_122() { Main(new string[0]); }
        public static void Main(string[] args)
            {
                A<int> a = new A<int>(new A<int>.B(D), 3);
                a.Run();
            }
            public static void D(int y)
            {
                Console.WriteLine("Hello " + 3);
            }
        }
        class A<T>
        {
            public delegate void B(T t);
    
            protected B _b;
            protected T _value;
    
            public A(B b, T value)
            {
              _b = b;
              _value = value;
          }
            public void Run()
            {
                _b(_value);
            }
        }
}
