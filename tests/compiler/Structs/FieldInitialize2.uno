public struct Foo
{
    public Bar bar;
}

public struct Bar
{
    public int x, y;
}

class Main
{
    public void Method1(out Foo foo)
    {
        foo = Foo(); // $E2061
    }

    public void Method2(out Foo foo)
    {
        foo.bar = Bar(); //$E2061
    }

    public void Method3(out Foo foo)
    {
        foo.bar.x = int();
        foo.bar.y = int();
    }

    public void Method4(out Foo foo) // $E4513 [Ignore]
    {
        foo.bar.x = 1;
    }
}
