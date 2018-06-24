public class Main
{
    public Main()
    {
        Foo.Bar(); // $E3124
        Foo.Bar2();
        Foo.Bar3();
    }
}

static class Foo
{
    public void Bar() {} // $E [Ignore] Static class can not have non-static members
    public static void Bar2() {}
    static void Bar3() {} // $E [Ignore]
}
