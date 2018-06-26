public class Main
{
    public static Main operator - (Main c)
    {
        return null;
    }

    public static int prop
    {
        get
        {
            return 1;
        }
        set
        {
        }
    }

    public static void Main()
    {
        op_Negate(null); // $E3102
        // use the increment operator as follows
        // var x = new Main();
        // x++;

        set_prop(1); // $E3102
        // try the following line instead
        // prop = 1;
    }
}