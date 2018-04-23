namespace Mono.test_573
{
    public class C
    {
        private static float current_factor_width = 1f;
    
        [Uno.Testing.Test] public static void test_573() { Main(); }
        public static void Main()
        {
            int width = 5;
            width += -(int)(((current_factor_width) - 1f) * -4.0f);
        }
    }
}
