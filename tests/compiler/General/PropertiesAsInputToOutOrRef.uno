class Main
{
    public int Property { get; set; }

    public int field;

    public void Method(ref int value)
    {
        var foo = value; // $W The variable 'foo' is assigned but its value is never used
    }

    public void Method2(out int value)
    {
        value = 0;
    }

    public void Method3(out int value) // $E4513 [Ignore]
    {
        var foo = value; // $E4511 [Ignore] $W0000 The variable 'foo' is assigned but its value is never used
    }

    public void MainMethod()
    {
        Method(ref field);
        Method2(out field);

        int test;
        Method(ref test); // $E4511 [Ignore]
        Method2(out test);

        Method(ref Property); // $E4126
        Method2(out Property); // $E4126
    }
}
