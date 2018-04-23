public class Foo
{
    void PrivateMethod() {}
}

public class Main : Uno.Application
{
    public static void Main()   // $E 'Main': Member names cannot be the same as their enclosing types
    {
        var foo = new Foo();
        foo.PrivateMethod();   // $E4040
   }
}
