using Uno;
using Uno.Testing;
using Uno.Graphics;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

public extern(android) class Enums
{
    [Test]
    public void Test()
    {
        Assert.AreEqual(MyEnum.Value2, UseEnum(MyEnum.Value1));
        Assert.AreEqual(MyEnum.Value1, UseEnum(MyEnum.Value2));
        Assert.AreEqual(MyEnum2.Value2, UseEnum2(MyEnum2.Value1));
        Assert.AreEqual(MyEnum2.Value1, UseEnum2(MyEnum2.Value2));
    }

    enum MyEnum
    {
        Value1,
        Value2,
    }

    enum MyEnum2 : sbyte
    {
        Value1 = 1,
        Value2 = 2,
    }

    [Foreign(Language.Java)]
    static MyEnum UseEnum(MyEnum e)
    @{
        if (e == @{MyEnum.Value1})
            return @{MyEnum.Value2};
        else
            return @{MyEnum.Value1};
    @}

    [Foreign(Language.Java)]
    static MyEnum2 UseEnum2(MyEnum2 e)
    @{
        if (e == @{MyEnum2.Value1})
            return @{MyEnum2.Value2};
        else
            return @{MyEnum2.Value1};
    @}
}
