using Uno;
using Uno.Testing;
using Uno.Graphics;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

[ForeignInclude(Language.Java, "java.util.ArrayList")]
public extern(android) class Fields
{
    [Test]
    public void Test()
    {
        Assert.IsTrue(Test0());
        Assert.IsTrue(Test1());
        Assert.IsTrue(Test2());
        Assert.IsTrue(Test3());
        Assert.IsTrue(Test4());
    }

    //------------------------------------------------------------
    // Test data
    int _instanceInt = 1;
    static int _staticInt = 0;

    Java.Object _instanceJavaObj = null;
    static Java.Object _staticJavaObj = null;

    Java.Object _instanceJavaObjProperty { get; set; }

    //------------------------------------------------------------

    [Foreign(Language.Java)]
    public extern(android) bool Test0()
    @{
        return @{_staticInt} == 0 && @{_staticInt:Get()} == 0;
    @}

    [Foreign(Language.Java)]
    public extern(android) bool Test1()
    @{
        return @{_staticJavaObj} == null && @{_staticJavaObj:Get()} == null;
    @}

    [Foreign(Language.Java)]
    public extern(android) bool Test2()
    @{
        return (@{Fields:Of(_this)._instanceInt} == 1) &&
               (@{Fields:Of(_this)._instanceInt:Get()} == 1);
    @}

    [Foreign(Language.Java)]
    public extern(android) bool Test3()
    @{
        return (@{Fields:Of(_this)._instanceJavaObj} == null) &&
               (@{Fields:Of(_this)._instanceJavaObj:Get()} == null);
    @}

    [Foreign(Language.Java)]
    public extern(android) bool Test4()
    @{
        return @{Fields:Of(_this)._instanceJavaObjProperty:Get()} == null;
    @}

    [Foreign(Language.Java)]
    public extern(android) bool Test5()
    @{
        @{Fields:Of(_this)._instanceInt:Set(10)};
        return (@{Fields:Of(_this)._instanceInt} == 10);
    @}

    [Foreign(Language.Java)]
    public extern(android) bool Test6()
    @{
        @{_staticInt:Set(20)};
        return @{_staticInt} == 20;
    @}

    [Foreign(Language.Java)]
    public extern(android) bool Test7()
    @{
        @{_staticJavaObj:Set(new ArrayList<String>())};
        return @{_staticJavaObj} != null;
    @}
}
