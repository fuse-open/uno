namespace Mono.test_369
{
    class Test {
        static int count;
    
        static public bool operator == (Test x, Test y)
        {
            ++count;
            return false;
        }
    
        static public bool operator != (Test x, Test y)    { return true; }
        
        public override bool Equals (object o) { return false; }
    
        public override int GetHashCode () { return 0; }
    
        [Uno.Testing.Test] public static void test_369() { Main(); }
        public static void Main()
        {
            Test y = new Test ();
            if (y == null)
                throw new Uno.Exception ();
            if (count != 1)
                throw new Uno.Exception ("Operator == was not called");
        }
    }
}
