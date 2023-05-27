using Uno;
using Uno.Testing;
using Uno.Graphics;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

public extern(android) class Macros
{
    [Test]
    public void Test()
    {
        Assert.IsTrue(RectStripped());
        Assert.IsTrue(TypeExpansion());
    }

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public static bool RectStripped()
    @{
        #if @{Uno.Rect:IsStripped}
        return false
        #else
        return true;
        #endif
    @}

    [Foreign(Language.Java)]
    public bool TypeExpansion()
    @{
        @{string} a;
        @{sbyte} b;
        @{Rect} d;
        @{object} e;
        @{int[]} f;
        @{Action} g;
        @{Action<int>} h;
        return true;
    @}
}
