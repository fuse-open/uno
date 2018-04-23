namespace Mono.test_575
{
    //
    // This comes from bug 82064, sadly, Mono does not currently abort
    // as it should on the extra value on the stack
    //
    using Uno;
    using Uno.IO;
    
    class Program
    {
        [Uno.Testing.Test] public static void test_575() { Main(new string[0]); }
        public static void Main(string[] args)
        {
            using (StringWriter stringWriter = new StringWriter ()) {
            }
        }
    }
}
