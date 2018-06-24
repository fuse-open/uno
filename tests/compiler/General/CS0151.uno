class Main
{
    public static implicit operator int (Main aa)
    {
        return 0;
    }

    public static implicit operator long (Main aa)
    {
        return 0;
    }

    static void M()
    {
    }

    public static void Main()
    {
        var a = new Main();

        // Compiler cannot choose between int and long
        switch (a)
        {
            case 1:
            break;
        }

        switch (M()) // $E3415
        {
            default:
                break;
        }
    }
}
