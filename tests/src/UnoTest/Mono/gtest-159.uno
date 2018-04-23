namespace Mono.gtest_159
{
    using Uno;
    using Uno.Collections;
    
    public class App {
      [Uno.Testing.Test] public static void gtest_159() { Main(); }
        public static void Main() {
        Dictionary<string, int> values = new Dictionary<string, int>();
        values["one"] = 1; values["two"] = 2;
    
        foreach (string key in values.Keys) {
          Console.WriteLine("key: {0}", key);
        }
      }
    }
}
