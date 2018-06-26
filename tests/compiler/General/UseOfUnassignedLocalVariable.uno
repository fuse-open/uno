using Uno;

public class App
{
    public App()
    {
        Foo(4);
    }

    void Foo(int option)
    {
        int i;

        switch (option)
        {
            case 0: i = 0; break;
            case 1: i = 1; break;
            case 2: i = 2; break;
        }

        var j = i; // $E4511 $W The variable 'j' is assigned but its value is never used
    }
}
