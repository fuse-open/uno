namespace foo
{
    public class App
    {
    }

    public class Locals
    {
        void NeverUsed()
        {
            int i; // $W0000 The variable 'i' is declared but never used
        }

        void OnlyStored()
        {
            int i; // $W The variable 'i' is assigned but its value is never used
            i = 42;
            int j = 42; // $W The variable 'j' is assigned but its value is never used
        }

        void OnlyLoaded()
        {
            string s;
            debug_log s; // $E4511
        }
    }

    public class Fields
    {
        int i;
        int j = 10;
        int k = 20;
        int l;

        int  f()
        {
            int a = l; // $W The variable 'a' is assigned but its value is never used
            return k;
        }
    }
}
