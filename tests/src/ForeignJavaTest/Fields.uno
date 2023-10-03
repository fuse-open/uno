using Uno;
using Uno.Testing;
using Uno.Graphics;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

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
        return @{_staticInt} == 0 && @{_staticInt:get()} == 0;
    @}

    [Foreign(Language.Java)]
    public extern(android) bool Test1()
    @{
        return @{_staticJavaObj} == null && @{_staticJavaObj:get()} == null;
    @}

    [Foreign(Language.Java)]
    public extern(android) bool Test2()
    @{
        return (@{Fields:of(_this)._instanceInt} == 1) &&
               (@{Fields:of(_this)._instanceInt:get()} == 1);
    @}

    [Foreign(Language.Java)]
    public extern(android) bool Test3()
    @{
        return (@{Fields:of(_this)._instanceJavaObj} == null) &&
               (@{Fields:of(_this)._instanceJavaObj:get()} == null);
    @}

    [Foreign(Language.Java)]
    public extern(android) bool Test4()
    @{
        return @{Fields:of(_this)._instanceJavaObjProperty:get()} == null;
    @}

    [Foreign(Language.Java)]
    public extern(android) bool Test5()
    @{
        @{Fields:of(_this)._instanceInt:set(10)};
        return (@{Fields:of(_this)._instanceInt} == 10);
    @}

    [Foreign(Language.Java)]
    public extern(android) bool Test6()
    @{
        @{_staticInt:set(20)};
        return @{_staticInt} == 20;
    @}

    [Foreign(Language.Java)]
    [ForeignInclude(Language.Java, "java.util.ArrayList")]
    public extern(android) bool Test7()
    @{
        @{_staticJavaObj:set(new ArrayList<String>())};
        return @{_staticJavaObj} != null;
    @}
}
