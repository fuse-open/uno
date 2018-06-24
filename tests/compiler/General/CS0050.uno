class Foo {} // Check that defaults to internal is valid (c# has private as default)

public class Main
{
    public static Foo MyMethod() // $E4128
    {
        return new Foo();
    }

    public static void Main() {} // $E0000 'Main': Member names cannot be the same as their enclosing types
}
