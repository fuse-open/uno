class Main
{
    // Non-static field
    public int field;

    // Non-static method
    public void Method(){}

    // Non-static property
    int Prop
    {
        get { return 1; }
    }

    public static void Main()
    {
        field = 10;   // $E3118
        Method();   // $E3124
        int property = Prop;   // $E3122
        var main = new Main();
        main.field = 10;
        main.Method();
        int property2 = main.Prop;
    }
}