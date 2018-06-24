using Uno;
using Uno.Testing;
using Uno.Graphics;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

public extern(android) class TestClass0
{
}

[ForeignInclude(Language.Java, "java.lang.Runnable", "android.app.Activity")]
public extern(android) class Args
{
    [Test]
    public void Test()
    {
        var testObj = new TestClass0();
        Assert.IsTrue(BasicBoxing0("test"));
        Assert.IsTrue(BasicBoxing1());
        Assert.IsTrue(BasicBoxing2(testObj));
        Assert.IsTrue(CallInstanceMethod0());
        Assert.IsTrue(CallUnoWithRunable0());
        Assert.IsTrue(JavaToUnoCall0());
        Assert.IsTrue(JavaToUnoCall1());
        Assert.IsTrue(JavaToUnoCall2());
        Assert.IsTrue(JavaToJavaCall0());
        Assert.IsTrue(ChainedCall2("test"));
    }

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public static bool BasicBoxing0(string arg)
    @{
        return (arg instanceof String);
    @}

    [Foreign(Language.Java)]
    public bool BasicBoxing1()
    @{
        return (_this instanceof UnoObject);
    @}

    [Foreign(Language.Java)]
    public static bool BasicBoxing2(object arg)
    @{
        return (arg instanceof UnoObject);
    @}

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public bool CallInstanceMethod0()
    @{
        @{Args:Of(_this).ExampleInstanceMtd():Call()};
        return true;
    @}

    public void ExampleInstanceMtd()
    {
    }

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public bool CallUnoWithRunable0()
    @{
        java.lang.Runnable rrr = new Runnable() {
            @Override
            public void run() {
                android.util.Log.d("Native", "weeee");
            }
        };

        return (rrr == @{Args.StaticIdentity(Java.Object):Call(rrr)});
    @}

    public static Java.Object StaticIdentity(Java.Object r)
    {
        return r;
    }

    public static Java.Object InstanceIdentity(Java.Object r)
    {
        return r;
    }

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public bool JavaToUnoCall0()
    @{
        @{Args:Of(_this).TestCallable(int, string):Call(1234, "state.name()")};
        return true;
    @}

    [Foreign(Language.Java)]
    public bool JavaToUnoCall1()
    @{
        @{Args:Of(_this).TestCallable(int, long):Call(1234, 123456789)};
        return true;
    @}

    [Foreign(Language.Java)]
    public bool JavaToUnoCall2()
    @{
        @{Args.StaticTestCallable(int):Call(1234)};
        return true;
    @}

    public void TestCallable(int id, long percents)
    {
    }

    public void TestCallable(int id, string state)
    {
    }

    public static void StaticTestCallable(int val)
    {
    }

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public bool JavaToJavaCall0()
    @{
        return @{ChainedCall0(int):Call(1)};
    @}

    [Foreign(Language.Java)]
    public static extern(android) bool ChainedCall0(int a)
    @{
        return @{ChainedCall1(TestNestedClass0):Call(@{TestNestedClass0(int,int):New(1, 2)})};
    @}

    public static bool ChainedCall1(TestNestedClass0 x)
    {
        return x.A == 1 && x.B == 2;
    }

    public class TestNestedClass0
    {
        public int A;
        public int B;

        public TestNestedClass0(int a, int b)
        {
            A = a;
            B = b;
        }
    }

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public static extern(android) bool ChainedCall2(object bar)
    @{
        return @{ChainedCall3(object):Call(bar)};
    @}

    public static bool ChainedCall3(object x)
    {
        String foo = ((string)x);
        return foo != null;
    }
}
