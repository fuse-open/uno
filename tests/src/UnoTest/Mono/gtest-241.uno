namespace Mono.gtest_241
{
    abstract public class a
      {
        public abstract void func<T>(ref T arg);
      }
      public class b : a
      {
         public override void func<T>(ref T arg)
         {
         }
      }
    class main {
        [Uno.Testing.Test] public static void gtest_241() { Main(); }
        public static void Main() {}
    }
}
