namespace Mono.gtest_optional_25
{
    using Uno;
    
    public class Usage
    {
        [Uno.Testing.Test] public static void gtest_optional_25() { Main(); }
        public static void Main()
        {
            var bug = new Bug ();
            string[] tags = bug.MethodWithOptionalParameter<string> (0);
        }
    }
    
    public class Bug
    {
        public TValue[] MethodWithOptionalParameter<TValue> (int index, TValue[] defaultValue = null)
        {
            return null;
        }
    }
}
