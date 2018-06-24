abstract public class Bar
{
    virtual protected void F() {}
}

public class Foo : Bar
{
    public override void F() {} // $E4022
    public static void Main() {}
}