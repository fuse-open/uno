namespace Mono.gtest_215
{
    public class R {}
    public class A<T> where T : R {}
    public partial class D : A<R> {}
    class MainClass { [Uno.Testing.Test] public static void gtest_215() { Main(); }
        public static void Main() {} }
}
