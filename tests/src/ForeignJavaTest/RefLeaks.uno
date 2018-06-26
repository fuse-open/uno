using Uno;
using Uno.Testing;
using Uno.Graphics;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

public extern(android) class Leaks
{
    [Test]
    public void Test()
    {
        Assert.IsTrue(Test0());
        Assert.IsTrue(Test1());
        Assert.IsTrue(Test2());
        Assert.IsTrue(Test3());
        Assert.IsTrue(Test4());
        Assert.IsTrue(Test5());
        Assert.IsTrue(TestJavaLeaks0());
        Assert.IsTrue(TestJavaLeaks1());
        Assert.IsTrue(TestJavaLeaks2());
    }

    public bool Test0()
    {
        // pass string to java 800 times
        for (int i = 0; i < 800; i++) {
            LeakA("sup");
        }
        return true;
    }

    public bool Test1()
    {
        // pass object to java 800 times
        for (int i = 0; i < 800; i++) {
            LeakB(this);
        }
        return true;
    }

    public bool Test2()
    {
        // pass Java.Object to java 800 times
        var testObj = ObjectToLeak();
        for (int i = 0; i < 800; i++) {
            LeakC(testObj);
        }
        return true;
    }

    public bool Test3()
    {
        // return java object from java 800 times
        for (int i = 0; i < 800; i++) {
            LeakJavaObjectNew();
        }
        return true;
    }
    public bool Test4()
    {
        // return struct from java 800 times
        for (int i = 0; i < 800; i++) {
            LeakStructNew();
        }
        return true;
    }

    public bool Test5()
    {
        // return uno object from java 800 times
        for (int i = 0; i < 800; i++) {
            LeakObjectNew();
        }
        return true;
    }

    [Foreign(Language.Java)]
    Java.Object ObjectToLeak()
    @{
        return (Object)1;
    @}

    [Foreign(Language.Java)]
    void LeakA(string x)
    @{
    @}

    [Foreign(Language.Java)]
    void LeakB(object x)
    @{
    @}

    [Foreign(Language.Java)]
    void LeakC(Java.Object x)
    @{
    @}

    [Foreign(Language.Java)]
    bool TestJavaLeaks0()
    @{
        // pass string to uno 800 times"
        for (int i=0; i < 800; i++)
            @{Leaks:Of(_this).LeakD(string):Call("test")};
        return true;
    @}

    [Foreign(Language.Java)]
    bool TestJavaLeaks1()
    @{
        // pass Object to uno 800 times"
        for (int i=0; i < 800; i++)
            @{Leaks:Of(_this).LeakE(object):Call(_this)};
        return true;
    @}

    [Foreign(Language.Java)]
    bool TestJavaLeaks2()
    @{
        // pass Java.Object to uno 800 times"
        Object testObj = (Object)1;
        for (int i=0; i < 800; i++)
            @{Leaks:Of(_this).LeakF(Java.Object):Call(testObj)};
        return true;
    @}

    void LeakD(string x)
    {
    }

    void LeakE(object x)
    {
    }

    void LeakF(Java.Object x)
    {
    }

    class Leaker {}

    [Foreign(Language.Java)]
    private extern(Android) Java.Object LeakJavaObjectNew()
    @{
        return new Object();
    @}

    [Foreign(Language.Java)]
    private extern(Android) Leaker LeakObjectNew()
    @{
        return @{Leaker():New()};
    @}

    [Foreign(Language.Java)]
    private extern(Android) int2 LeakStructNew()
    @{
        return @{int2(int,int):New(1,2)};
    @}
}
