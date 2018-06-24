using Uno;
using Uno.Graphics;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

public extern(android) class Nulls
{
    [Test]
    public void Test()
    {
        Assert.AreEqual(null, NullStrings(null));
        Assert.AreEqual(null, NullObjects(null));
        Assert.AreEqual(null, NullUnoObjects(null));
        Assert.AreEqual(null, NullDelegates(null));
        Assert.IsTrue(null == NullArrays(null));
        Assert.AreEqual(null, NullStrings2(null));
        Assert.AreEqual(null, NullObjects2(null));
        Assert.AreEqual(null, NullUnoObjects2(null));
        Assert.AreEqual(null, NullDelegates2(null));
        Assert.IsTrue(null == NullArrays2(null));
    }

    [Foreign(Language.Java)]
    static string NullStrings(string x)
    @{
        return null;
    @}

    [Foreign(Language.Java)]
    static Java.Object NullObjects(Java.Object x)
    @{
        return null;
    @}

    [Foreign(Language.Java)]
    static object NullUnoObjects(object x)
    @{
        return null;
    @}

    [Foreign(Language.Java)]
    static Func<int, string> NullDelegates(Func<int, string> x)
    @{
        return null;
    @}

    [Foreign(Language.Java)]
    static string[] NullArrays(string[] x)
    @{
        return null;
    @}

    [Foreign(Language.Java)]
    static string NullStrings2(string x)
    @{
        return x;
    @}

    [Foreign(Language.Java)]
    static Java.Object NullObjects2(Java.Object x)
    @{
        return x;
    @}

    [Foreign(Language.Java)]
    static object NullUnoObjects2(object x)
    @{
        return x;
    @}

    [Foreign(Language.Java)]
    static Func<int, string> NullDelegates2(Func<int, string> x)
    @{
        return x;
    @}

    [Foreign(Language.Java)]
    static string[] NullArrays2(string[] x)
    @{
        return x;
    @}
}
