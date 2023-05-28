namespace Mono.gtest_036
{
    //
    // This is another "important" test: it checks whether we set
    // TypeContainer.CurrentType correctly when compiling the
    // declaration of `Stack'.
    //

    class Stack<T>
    {
        //
        // This must be encoded as a TypeSpec (Stack<!0>) and
        // not as a TypeDef.
        //
        // PEVerify the resulting executable on Windows to make sure !
        //
        void Hello (Stack<T> stack)
        {
        }

        void Test ()
        {
            Hello (this);
        }
    }

    class X
    {
        [Uno.Testing.Test] public static void gtest_036() { Main(); }
        public static void Main()
        { }
    }
}
