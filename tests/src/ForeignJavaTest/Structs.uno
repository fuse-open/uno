using Uno;
using Uno.Testing;
using Uno.Graphics;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

public extern(android) class Structs
{
    [Test]
    public void Test()
    {
        var flt4 = float4(1,2,3,4);
        Assert.AreEqual(flt4, Float4RoundTrip(flt4));
        Assert.IsTrue(Int2Elements());
    }

    [Foreign(Language.Java)]
    static float4 Float4RoundTrip(float4 structArg)
    @{
        return @{StructToUno(float4):Call(structArg)};
    @}

    static float4 StructToUno(float4 x)
    {
        return x;
    }

    static bool Int2Elements()
    {
        var v = GetInt2();
        return v.X==10 && v.Y==20;
    }

    [Foreign(Language.Java)]
    static extern(Android) int2 GetInt2()
    @{
        int width = 10;
        int height = 20;
        return @{int2(int, int):New(width, (height))};
    @}
}
