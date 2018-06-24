class Foo
{
    public int i;
}

class MainHest
{
    public static void Main(string[] args)
    {
        // i and j are not initialized.
        int i, j; // $W The variable 'j' is assigned but its value is never used

        // You can provide a value for args[0] in the 'Command line arguments'
        // text box on the Debug tab of the project Properties window.
        if (args[0] == "test")
        {
            i = 0;
        }

        // Because i might not have been initialized, the following yields an error
        j = i; // $E4511

        // The following example causes error because foo is
        // declared but not instantiated.
        Foo foo;
        // The following line causes the error.
        foo.i = 0;  // $E4511
    }
}
