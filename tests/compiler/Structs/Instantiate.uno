class Main
{
    public Main()
    {
        var foo = Foo(); // $E2061
        var foo2 = new Foo();
    }
}

struct Foo {}
