namespace Mono.test_583
{
    class Program
    {
        [Uno.Testing.Test] public static void test_583() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            IExtContainer e = null;
            ObjectContainerBase b = null;
            return (e == b ? 0 : 1);
        }
    }
    
    public interface IContainer
    {
    }
    
    public interface IExtContainer : IContainer
    {
    }
    
    public abstract class ObjectContainerBase : IContainer
    {
    }
}
