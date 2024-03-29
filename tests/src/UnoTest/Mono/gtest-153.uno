namespace Mono.gtest_153
{
    public interface IBase
    {
        void DoSomeThing();
    }

    public interface IExtended : IBase
    {
        void DoSomeThingElse();
    }

    public class MyClass<T> where T: IExtended, new()
    {
        public MyClass()
        {
            T instance = new T();
            instance.DoSomeThing();
        }
    }

    class X
    {
        [Uno.Testing.Test] public static void gtest_153() { Main(); }
        public static void Main()
        { }
    }
}
