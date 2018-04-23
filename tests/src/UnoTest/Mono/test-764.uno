namespace Mono.test_764
{
    using Uno;
    
    class Item
    {
        internal static object Field = "Test";
    }
    
    class Caller
    {
        public string this[string x]
        {
            get { return x; }
        }
    
        public int this[int x]
        {
            get { return x; }
        }
    
        public void Foo ()
        {
            var v = Item.Field.ToString ();
        }
    
        [Uno.Testing.Test] public static void test_764() { Main(); }
        public static void Main()
        {
        }
    }
}
