using Uno;
using Uno.Testing;
using Uno.Graphics;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

public extern(android) class ForeignOverloading
{
    [Test]
    public static void Test()
    {
        var fo = new ClassWithOverloading();
        Assert.IsTrue(fo.Foo(1));
        Assert.IsTrue(fo.Foo(1.1));
    }
}

// Uno class with foreign constructor
public extern(android) class ClassWithOverloading : FCBase
{
    [Foreign(Language.Java)]
    public bool Foo(int code)
    @{
        return @{ClassWithOverloading:Of(_this).Foo(string):Call("" + code)};
    @}

    [Foreign(Language.Java)]
    public bool Foo(double code)
    @{
        int i = (int)code;
        return @{ClassWithOverloading:Of(_this).Foo(string):Call("" + i)};
    @}

    public bool Foo(string numStr)
    {
        return numStr == "1";
    }
}
