using Uno;
using Uno.Testing;
using Uno.Graphics;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

// The Delegate definitions
public extern(android) delegate void BigOlDelegate(int resultCode, Java.Object intent, object info);
public extern(android) delegate int BigOlDelegateReturn(int resultCode, Java.Object intent, object info);
public extern(android) delegate object BigOlDelegateReturnObject(int resultCode, Java.Object intent, object info);
public extern(android) delegate Java.Object BigOlDelegateReturnJavaObject(int resultCode, Java.Object intent, object info);

public extern(android) class Delegates
{
    [Test]
    public void Test()
    {
        Assert.IsTrue(StartForVoid(BigOlStatic, "dummyData"));
        Assert.IsTrue(StaticForVoid(BigOlStatic, "dummyData"));
        Assert.IsTrue(StartForVoid(BigOlInstance, "dummyData"));
        Assert.IsTrue(StaticForVoid(BigOlInstance, "dummyData"));

        Assert.IsTrue(StartForResult(BigOlStaticReturn, "dummyData"));
        Assert.IsTrue(StaticForResult(BigOlStaticReturn, "dummyData"));
        Assert.IsTrue(StartForResult(BigOlInstanceReturn, "dummyData"));
        Assert.IsTrue(StaticForResult(BigOlInstanceReturn, "dummyData"));

        Assert.IsTrue(StartForResultObject(BigOlStaticReturnObject, "dummyData"));
        Assert.IsTrue(StaticForResultObject(BigOlStaticReturnObject, "dummyData"));
        Assert.IsTrue(StartForResultObject(BigOlInstanceReturnObject, "dummyData"));
        Assert.IsTrue(StaticForResultObject(BigOlInstanceReturnObject, "dummyData"));

        Assert.IsTrue(StartForResultJavaObject(BigOlStaticReturnJavaObject, "dummyData"));
        Assert.IsTrue(StaticForResultJavaObject(BigOlStaticReturnJavaObject, "dummyData"));
        Assert.IsTrue(StartForResultJavaObject(BigOlInstanceReturnJavaObject, "dummyData"));
        Assert.IsTrue(StaticForResultJavaObject(BigOlInstanceReturnJavaObject, "dummyData"));

        Assert.IsTrue(Wrap10000Delegates());
    }

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public bool StartForVoid(BigOlDelegate callback, object obj)
    @{
        callback.run(10, null, null);
        callback.run(20, "ok", obj);
        callback.run(30, new android.view.SurfaceView(com.fuse.Activity.getRootActivity()), obj);
        return true;
    @}

    [Foreign(Language.Java)]
    static public bool StaticForVoid(BigOlDelegate callback, object obj)
    @{
        callback.run(10, null, null);
        callback.run(20, "ok", obj);
        callback.run(30, new android.view.SurfaceView(com.fuse.Activity.getRootActivity()), obj);
        return true;
    @}

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public bool StartForResult(BigOlDelegateReturn callback, object obj)
    @{
        callback.run(10, null, null);
        callback.run(20, "ok", obj);
        callback.run(30, new android.view.SurfaceView(com.fuse.Activity.getRootActivity()), obj);
        return true;
    @}

    [Foreign(Language.Java)]
    static public bool StaticForResult(BigOlDelegateReturn callback, object obj)
    @{
        callback.run(10, null, null);
        callback.run(20, "ok", obj);
        callback.run(30, new android.view.SurfaceView(com.fuse.Activity.getRootActivity()), obj);
        return true;
    @}

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public bool StartForResultObject(BigOlDelegateReturnObject callback, object obj)
    @{
        callback.run(10, null, null);
        callback.run(20, "ok", obj);
        callback.run(30, new android.view.SurfaceView(com.fuse.Activity.getRootActivity()), obj);
        return true;
    @}

    [Foreign(Language.Java)]
    static public bool StaticForResultObject(BigOlDelegateReturnObject callback, object obj)
    @{
        callback.run(10, null, null);
        callback.run(20, "ok", obj);
        callback.run(30, new android.view.SurfaceView(com.fuse.Activity.getRootActivity()), obj);
        return true;
    @}

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public bool StartForResultJavaObject(BigOlDelegateReturnJavaObject callback, object obj)
    @{
        callback.run(10, null, null);
        callback.run(20, "ok", obj);
        callback.run(30, new android.view.SurfaceView(com.fuse.Activity.getRootActivity()), obj);
        return true;
    @}

    [Foreign(Language.Java)]
    static public bool StaticForResultJavaObject(BigOlDelegateReturnJavaObject callback, object obj)
    @{
        callback.run(10, null, null);
        callback.run(20, "ok", obj);
        callback.run(30, new android.view.SurfaceView(com.fuse.Activity.getRootActivity()), obj);
        return true;
    @}

    //------------------------------------------------------------

    public bool Wrap10000Delegates()
    {
        for (int i = 0; i < 10000; i++)
        {
            WrapADelegate(BigOlStatic);
        }
        return true;
    }

    [Foreign(Language.Java)]
    private void WrapADelegate(BigOlDelegate callback)
    @{
    @}

    //------------------------------------------------------------
    // The Test Methods

    static void BigOlStatic(int resultCode, Java.Object intent, object info)
    {
    }

    void BigOlInstance(int resultCode, Java.Object intent, object info)
    {
    }

    static int BigOlStaticReturn(int resultCode, Java.Object intent, object info)
    {
        return 1;
    }

    int BigOlInstanceReturn(int resultCode, Java.Object intent, object info)
    {
        return 2;
    }

    static object BigOlStaticReturnObject(int resultCode, Java.Object intent, object info)
    {
        return info;
    }

    object BigOlInstanceReturnObject(int resultCode, Java.Object intent, object info)
    {
        return info;
    }

    static Java.Object BigOlStaticReturnJavaObject(int resultCode, Java.Object intent, object info)
    {
        return intent;
    }

    Java.Object BigOlInstanceReturnJavaObject(int resultCode, Java.Object intent, object info)
    {
        return intent;
    }

    //------------------------------------------------------------
}
