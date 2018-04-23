namespace Mono.test_559
{
    // Compiler options: -target:library
    
    public class B
    {
        public delegate void TestDelegate ();
        public virtual event TestDelegate TestEvent;
    }
}
