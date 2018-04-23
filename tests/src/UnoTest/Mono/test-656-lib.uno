namespace Mono.test_656
{
    // Compiler options: -target:library
    
    public class Foo
    {
        public string this[params string[] types] { get { return ""; }}
    }
}
