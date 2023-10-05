using Uno;
using Uno.Testing;
using Uno.Graphics;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

public extern(android) class Properties
{
    [Test]
    public void Test()
    {
        Assert.AreEqual(1, Foo);
        Foo = 2;
        Assert.AreEqual(2, Foo);
    }

    int val = 1;

    [Foreign(Language.Java)]
    public int Foo
    {
        get
        @{
            return @{Properties:of(_this).val};
        @}
        set
        @{
            @{Properties:of(_this).val:set(value)};
        @}
    }
}
