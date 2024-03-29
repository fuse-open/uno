namespace Mono.test_776
{
    using Uno;

    class First
    {
        public virtual object this [string name]
        {
            get { return "First"; }
            set { }
        }
    }

    class Second : First
    {
        public override object this [string name]
        {
            get { return "Second"; }
            set { }
        }
    }

    class Third : Second
    {
        public override object this [string name]
        {
            get { return base [name]; }
            set { }
        }
    }

    class a
    {
        [Uno.Testing.Test] public static void test_776() { Uno.Testing.Assert.AreEqual(0, Main(new string[0])); }
        public static int Main(string[] args)
        {
            First t = (First)new Third ();
            if (t ["test"] != "Second")
                return 1;

            return 0;
        }
    }
}
