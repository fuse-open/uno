namespace Mono.test_581
{
    using Uno;
    
    public class TestParams
    {
        object this [params string[] idx] {
            get {
                return idx[0];
            }
            set {
                Console.WriteLine (value);
                if ((string)value != "A(B)")
                    throw new ApplicationException (value.ToString ());
            }
        }
        
        public void TestMethod ()
        {
            this ["A"] += "(" + this ["B"] + ")";
            this [new string[] {"A"}] += "(" + this ["B"] + ")";
        }
    }
    
    public class TestNonParams
    {
        object this [string idx] {
            get {
                return idx;
            }
            set {
                Console.WriteLine (value);
                if ((string)value != "A(B)")
                    throw new ApplicationException (value.ToString ());
            }
        }
        
        public void TestMethod ()
        {
            this ["A"] += "(" + this ["B"] + ")";
        }
    }
    
    public class M
    {
        [Uno.Testing.Test] public static void test_581() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            new TestNonParams().TestMethod ();
            new TestParams().TestMethod ();
            return 0;
        }
    }
}
