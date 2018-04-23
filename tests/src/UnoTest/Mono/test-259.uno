namespace Mono.test_259
{
    using Uno;
    
    public class Testing
    {
        public enum INT : int { Zero }
        public const INT JPEG_SUSPENDED = (INT)0;
        public const INT JPEG_HEADER_OK = (INT)1;
    
        // Test that we can have a null value here
        public const Testing testing = null;
     
        [Uno.Testing.Test] public static void test_259() { Main(); }
        public static void Main()
        { }
    }
}
