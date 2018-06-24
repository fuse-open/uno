struct Bar
{
    public string field;

    public Bar(out string value)
    {
        Method(out field);
        value = field;
    }

    public static void Method(out string param)
    {
        param = null;
    }
}

struct Foo
{
    public string field;

    public Foo(out int x) // $E4512 [Ignore]
    {
        Method2(out x); // $E4511 [Ignore]
    }

    public void Method2(out int param)
    {
        param = 0;
    }
}
