using Uno;
using Uno.Testing;
using Uno.Graphics;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

public extern(android) class ExternalFiles
{
    [Test]
    public void Test()
    {
        Assert.IsTrue(CallExternalJavaFile());
    }

    [Foreign(Language.Java)]
    bool CallExternalJavaFile()
    @{
        return com.fuse.Testing.TestJava.TrueIfThirty(30);
    @}
}
