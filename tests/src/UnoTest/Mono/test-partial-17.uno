namespace Mono.test_partial_17
{
    public class Test
    {
        public static bool Check (string name, string[] names)
        {
            foreach (string partial in names) {
                if (name.StartsWith (partial))
                    return true;
            }
    
            return false;
        }
    
        [Uno.Testing.Test] public static void test_partial_17() { Main(); }
        public static void Main()
        {
        }
    }
}
