class Main
{
    static void Main(string[] args)
    {
        var foo = new Foo();
        foo.Method();
        foo.Method(10);

        // Foo does not contain an method that takes two arguments.
        foo.Method(10, 20); // $E3129
    }
}

class Foo
{
    public void Method() {}
    public void Method(int i) {}
}