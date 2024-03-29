namespace Mono.gtest_482
{
    using Uno;

    namespace TestBug
    {
        public class BugClass<T>
        {
            public event EventHandler Evt { add { } remove { } }

            public void Bug ()
            {
                Evt += Handler;
            }

            public static void Handler (object sender, EventArgs e)
            {
            }
        }

        class MainClass
        {
            [Uno.Testing.Test] public static void gtest_482() { Main(); }
        public static void Main()
            {
                BugClass<int> bc = new BugClass<int> ();
                bc.Evt += BugClass<int>.Handler;
            }
        }
    }
}
