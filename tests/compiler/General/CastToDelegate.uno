class Main
{
    delegate string FooDelegate(string someValue);

    public void UseDelegateMethod(FooDelegate fooDelegate, string someValue)
    {
        var ret = fooDelegate(someValue);
    }

    public string Log(string value)
    {
        return value;
    }

    public Main()
    {
        UseDelegateMethod(FooDelegate, "stringvalue"); // $E2061
        UseDelegateMethod(new [] { "" }, "stringvalue"); // $E3128
        UseDelegateMethod(0, "stringvalue"); // $E3128
        UseDelegateMethod(this, "stringvalue"); // $E3128
        UseDelegateMethod(Log, "stringvalue");
        UseDelegateMethod(null, "stringvalue"); // Should maybe give a warning??

        var foo = (FooDelegate)Log;
        var result = foo("sdf");
        var foo2 = (FooDelegate)UseDelegateMethod; // $E2029
        var foo3 = (FooDelegate)null;
        var foo4 = (FooDelegate)0; // $E2029
    }
}