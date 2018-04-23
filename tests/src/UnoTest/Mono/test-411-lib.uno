namespace Mono.test_411
{
    // Compiler options: -t:library
    
    namespace Qt
    {
        public class QtSupport
        {
            protected static event MousePressEvent mousePressEvent;
    
            protected delegate void MousePressEvent ();
        }
    
    }
}
