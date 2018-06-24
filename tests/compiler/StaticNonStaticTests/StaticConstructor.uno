static class Foo
{
    public static Foo() {} // $E [Ignore] A static constructor does not take access modifiers
}

static class Bar
{
    static Bar(bool isNotGoodHere) {} // $E4027 [Ignore]
}

static class Foo1
{
    static Foo1() {}
}

public class Main
{
    public Main()
    {
        Foo1.Foo1(); // $E3107
    }
}
