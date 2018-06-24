using Uno;

public class Main
{
    public void Main() // $E0000 'Main': Member names cannot be the same as their enclosing types
    {
        int i = 0;
        switch (i)
        {
            case 1:
                i++;

            case 2:
                i++;
                return;

            case 4:
            case 5:
                i = 3;
                break;

            default:
                i = 0;

        }

        int myInt = 1;
        F(out myInt);

        int myInt2;
        E(out myInt2);
    }

    public static void F(out int i)
    {
        int k = i; // $W The variable 'k' is assigned but its value is never used
        i = 1;
    }

    public static void E(out int i)
    {
        try
        {
            // Assignment occurs, but compiler can't verify it
            i = 1;
        } catch(Exception e) {}

        int k = i; // $W The variable 'k' is assigned but its value is never used
        i = 1;
    }


}
