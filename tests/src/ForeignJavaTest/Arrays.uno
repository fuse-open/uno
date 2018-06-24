using Uno;
using Uno.Testing;
using Uno.Graphics;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

[ForeignInclude(Language.Java, "java.lang.Runnable", "android.app.Activity")]
public extern(android) class Arrays
{
    //------------------------------------------------------------

    int[] intArray;
    bool[] boolArray;
    byte[] byteArray;
    string[] strArray;

    //------------------------------------------------------------

    [Test]
    public void Test()
    {
        byteArray = new byte[] {3, 4, 5};
        intArray = new int[] {3, 4, 5};
        boolArray = new bool[] {true, false, true};
        strArray = new string[] {"YES", "MORE", "STUFF!"};

        Assert.AreEqual(byteArray, Identity(byteArray));
        Assert.AreEqual(intArray, Identity(intArray));
        Assert.AreEqual(boolArray, Identity(boolArray));
        Assert.AreEqual(strArray, Identity(strArray));

        Assert.IsTrue(TestIntActionViaJava(intArray, boolArray));
        Assert.IsTrue(PassToJava0(intArray, strArray));

        Assert.IsTrue(Test1(strArray));
        Assert.IsTrue(Test2(intArray));
        Assert.IsTrue(Test3(intArray));
        Assert.IsTrue(Test4(strArray));
        Assert.IsTrue(Test5());
        Assert.IsTrue(Test6());
        Assert.IsTrue(Test7());
        Assert.IsTrue(Test8());

        Assert.AreEqual(37, Test9()[0]);
        Assert.AreEqual("Yo!", Test10()[0]);
        Assert.AreEqual(20, Test11()[1]);
    }

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public static bool[] Identity(bool[] arr)
    @{
        return arr;
    @}

    [Foreign(Language.Java)]
    public static byte[] Identity(byte[] arr)
    @{
        return arr;
    @}

    [Foreign(Language.Java)]
    public static int[] Identity(int[] arr)
    @{
        return arr;
    @}

    [Foreign(Language.Java)]
    public static string[] Identity(string[] arr)
    @{
        return arr;
    @}

    //------------------------------------------------------------

    public bool TestIntActionViaJava(int[] intArr, bool[] boolArr)
    {
        var foo = new TestClass1();
        return PassArrayToAction(TestIntArray, intArr, boolArr);
    }

    void TestIntArray(int[] x) {}

    [Foreign(Language.Java)]
    public bool PassArrayToAction (Action<int[]> y, int[] z, bool[] a)
    @{
        y.run(z);
        return true;
    @}

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public bool PassToJava0(int[] intArr, string[] strArr)
    @{
        return @{PassToJava1(int[],string[]):Call(intArr, strArr)};
    @}

    public static bool PassToJava1(int[] intArr, string[] strArr)
    {
        return intArr[0] == 3 && strArr[0] == "YES";
    }

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public bool Test1(string[] strArr)
    @{
        return strArr.get(0).equals("YES");
    @}

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public bool Test2(int[] intArr)
    @{
        if (intArr.get(0) != 3) return false;
        intArr.set(0, 10);
        return (intArr.get(0) == 10);
    @}

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public bool Test3(int[] intArr)
    @{
        int[] intCopy = intArr.copyArray();
        for (int i=0; i < intCopy.length; i++)
            if (intCopy[i] != intArr.get(i))
                return false;
        return true;
    @}

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public bool Test4(string[] strArr)
    @{
        String[] strCopy = strArr.copyArray();
        for (int i=0; i < strCopy.length; i++)
            if (!strCopy[i].equals(strArr.get(i)))
                return false;
        return true;
    @}

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public bool Test5()
    @{
        ShortArray freshShortArr = new ShortArray(20);
        StringArray freshStringArr = new StringArray(100);
        ByteArray freshByteArr = new ByteArray(10,false);
        ByteArray freshUByteArr = new ByteArray(10,true);
        return @{CheckArraysFromJava0(short[], string[], sbyte[], byte[]):Call(freshShortArr, freshStringArr, freshByteArr, freshUByteArr)};
    @}

    static bool CheckArraysFromJava0(short[] a, string[] b, sbyte[] c, byte[] d)
    {
        return (a != null) && (b != null) && (c != null) && (d != null);
    }

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public bool Test6()
    @{
        StringArray freshStringArr = new StringArray(100);
        freshStringArr.set(34, "hi there");
        return freshStringArr.get(34).equals("hi there");
    @}

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public bool Test7()
    @{
        ByteArray freshByteArr = new ByteArray(10,false);
        freshByteArr.set(3, (byte)100);
        return freshByteArr.get(3) == 100;
    @}

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public bool Test8()
    @{
        ByteArray freshUByteArr = new ByteArray(10,true);
        freshUByteArr.set(3, (byte)100);
        return freshUByteArr.get(3) == 100;
    @}

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public int[] Test9()
    @{
        IntArray x = new IntArray(3);
        x.set(0, 37);
        return x;
    @}

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public string[] Test10()
    @{
        StringArray x = new StringArray(3);
        x.set(0, "Yo!");
        return x;
    @}

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public int[] Test11()
    @{
        return new IntArray(new int[]{10,20,30});
    @}

    //------------------------------------------------------------

    [Test]
    public void Test12()
    {
        char[] h = Test12Inner();
        Assert.AreEqual('a', h[0]);
        Assert.AreEqual('!', h[1]);
        Assert.AreEqual('c', h[2]);
    }

    [Foreign(Language.Java)]
    public char[] Test12Inner()
    @{
        return new CharArray(new char[]{'a','!','c'});
    @}

    //------------------------------------------------------------

    [Test]
    public void Test13()
    {
        var x = new TestClass1();
        var y = new TestClass1();
        object[] h = Test13Inner(x, y);
        Assert.AreEqual(x, h[0]);
        Assert.AreEqual(y, h[1]);
    }

    [Foreign(Language.Java)]
    public object[] Test13Inner(object x, object y)
    @{
        return new ObjectArray(new UnoObject[]{x, y});
    @}

    //------------------------------------------------------------

    [Test]
    public void Test14()
    {
        var x = "TestA";
        var y = "TestB";
        object[] h = Test14Inner(x, y);
        Assert.AreEqual(x, h[0]);
        Assert.AreEqual(y, h[1]);
    }

    [Foreign(Language.Java)]
    public object[] Test14Inner(object x, object y)
    @{
        return new ObjectArray(new UnoObject[]{x, y});
    @}

    //------------------------------------------------------------

    [Test]
    public void Test15()
    {
        int[] arr = new int[] {1, 2, 3, 4};
        for (var i = 0; i<700; i++)
        {
            Test15Inner(arr);
        }
    }

    [Foreign(Language.Java)]
    public void Test15Inner(int[] arr)
    @{
        int[] arrCopy = arr.copyArray();
    @}
}

public extern(android) class TestClass1
{
    public TestClass1() {}
    public TestClass1(int x) {}

}
