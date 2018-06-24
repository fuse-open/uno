public class Main
{
    public Main()
    {
        new Foo(); // $E2090 [Ignore]
    }
}

static class Foo
{
    static Foo() {}
    private static Foo() {}  // $E3002
    public Foo() {}  // $E [Ignore] Static class cannot contain instance constructors or Static class cannot have non-static constructor
}
