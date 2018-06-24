public class Foo
{
    public Foo(char i) {}

    public static void Main()
    {
        var foo = new Foo(2222);   // $E2009
    }
}