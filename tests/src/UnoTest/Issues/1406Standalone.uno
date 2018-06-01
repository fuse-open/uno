using Uno;
using Uno.Testing;

class MyClass
{
    public int X = 123;
}

class ClassWithStaticThing
{
    public static MyClass StaticThing = new MyClass();
}

struct UseStaticThing
{
    public MyClass GetStaticThing()
    {
        return ClassWithStaticThing.StaticThing;
    }
}

public class App
{
    public App()
    {
        var tmp = new UseStaticThing().GetStaticThing();

        Assert.AreEqual(123, tmp.X);
    }
}
