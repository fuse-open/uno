public struct Foo
{
    public int Width;
}

public class ListView
{
    Foo ms;

    public Foo Size
    {
        get { return ms; }
        set { ms = value; }
    }
}

class Main
{
    public Main()
    {
        var lvi = new ListView();
        lvi.Size.Width = 5; // $E2018

        Foo foo;
        foo.Width = 5;
        lvi.Size = foo;
    }
}