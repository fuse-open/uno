using Uno.Testing;

namespace MultipleVariableDeclarations
{
    public class Generic<T>
    {
        public T X;
        public T Y;

        public void Run()
        {
            T x = default(T), y = default(T);
            X = x;
            Y = y;
        }
    }

    public class Test1504
    {
        [Test]
        public void Run()
        {
            var g = new Generic<int>();

            g.X = 42;
            g.Y = 43;

            g.Run();

            Assert.AreEqual(0, g.X);
            Assert.AreEqual(0, g.Y);
        }
    }
}
