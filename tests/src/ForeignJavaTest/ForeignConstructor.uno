using Uno;
using Uno.Testing;
using Uno.Graphics;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

public extern(android) class ForeignConstructor
{
    [Test]
    public static void Test()
    {
        var fc = new ClassWithForeignConstructor();
        Assert.IsTrue(fc.Test());
    }
}

// A base class for some tests
// Used to prove that the initialization chain works
public extern(android) class FCBase
{
    protected int Proof1 = 20;
    protected string Proof2;
    public FCBase()
    {
        Proof2 = "PROOF";
    }
}

// Uno class with foreign constructor
public extern(android) class ClassWithForeignConstructor : FCBase
{
    [Foreign(Language.Java)]
    public ClassWithForeignConstructor()
    @{
        // Inside the foreign constructor
    @}

    public bool Test()
    {
        return (Proof1==20) && (Proof2=="PROOF");
    }
}
