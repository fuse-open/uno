public class Main
{
    public void Method()
    {
        int i = 0;
        switch (i)
        {
            case 1: // $E4515
                i++;

            case 2:
                i++;
                return;

            // goto not supported in Uno
            /*
            case 3:
                i = 2;
                goto case 2;
            */

            case 4:
            case 5:
                i = 3;
                break;

            default: // $E4515
                i = 0;

        }
    }
}
