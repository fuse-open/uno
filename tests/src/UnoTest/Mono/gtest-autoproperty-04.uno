namespace Mono.gtest_autoproperty_04
{
    using Uno;
    
    namespace MonoTests
    {
        public abstract class MainClass
        {
            protected virtual string[] foo { get; set; }
            public abstract string[] bar { get; set; }
    
            [Uno.Testing.Test] public static void gtest_autoproperty_04() { Main(new string[0]); }
        public static void Main(string[] args)
            {
                Console.WriteLine ("Hello World!");
            }
        }
        public class ChildClass : MainClass
        {
            protected override string[] foo { get; set; }
            public override string[] bar { get; set; }
        }
    }
}
