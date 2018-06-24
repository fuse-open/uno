class TestClass
{
    public static implicit operator int(TestClass o)
    {
        return 1;
    }

    public static implicit operator TestClass(int v)
    {
        return new TestClass();
    }

    public static implicit operator bool(TestClass o)
    {
        return 0; //$E2047
    }
}

class Main
{
    public void Main()
    {
        var o1 = new TestClass();
        var o2 = new TestClass();
        var o3 = o1 & o2;
    }
}
