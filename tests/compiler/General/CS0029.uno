class Main
{
    private int x = 0;

    static void Main()
    {
        var main = new Main();
        int i = main; // $E2047
    }
}