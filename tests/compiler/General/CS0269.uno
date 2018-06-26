using Uno;

class Main
{
    public static void F(out int i)
    {
        int k = i;  // $E4511 $W The variable 'k' is assigned but its value is never used
        i = 1;
    }

    public static void E(out int i)
    {
        try
        {
            // Assignment occurs, but compiler can't verify it
            i = 1;
        } catch(Exception e) {}

        int k = i;  // $E4511 $W The variable 'k' is assigned but its value is never used
        i = 1;
    }

    public static void Method()
    {
        int myInt = 1;
        F(out myInt);

        int myInt2;
        E(out myInt2);
    }
}
