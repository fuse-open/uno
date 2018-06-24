class Main
{
    public static void Main()
    {
        bool result = true;
        if (result > 0) // $E2016
        {
            // Do something.
        }

        int i = 1;
        // You cannot compare an integer and a boolean value.
        if (i == true) // $E2016
        {
            //Do something...
        }

        // The following use of == causes no error. It is the comparison of
        // an integer and a boolean value that causes the error in the
        // previous if statement.
        if (result == true)
        {
            //Do something...
        }

        string s = "Just try to subtract me.";
        float f = 100 - s; // $E2016
    }
}
