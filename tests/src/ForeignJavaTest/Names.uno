using Uno;
using Uno.Testing;
using Uno.Graphics;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

public extern(android) class Names
{
    [Test]
    public void Test()
    {
        Assert.IsTrue(ArgNames0(123));
        Assert.IsTrue(ArgNames1(null, 1f));
    }

    //------------------------------------------------------------

    int aName;

    [Foreign(Language.Java)]
    bool ArgNames0(int aName)
    @{
        return true;
    @}

    [Foreign(Language.Java)]
    static bool ArgNames1 ( Java.Object _x, float y)
    @{
        // had an bug with underscores in arg names.
        return true;
    @}
}
