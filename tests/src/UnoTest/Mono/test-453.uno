namespace Mono.test_453
{
    using Uno;
    
    class C {
        internal enum Flags {
            Removed    = 0,
            Public    = 1
        }
    
        static Flags    _enumFlags;
            
        [Uno.Testing.Ignore, Uno.Testing.Test] public static void test_453() { Main(); }
        public static void Main()
        {
            if ((Flags.Removed | 0).ToString () != "Removed")
                throw new ApplicationException ();
        }
    }
}
