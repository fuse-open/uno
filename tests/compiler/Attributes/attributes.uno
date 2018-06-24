[AttributeUsage(AttributeTargets.Struct)]
class StructAttribute : Attribute
{
}

[Struct]
struct Test
{
    [Struct] // $E0000 Attribute [Struct] can only be used on 'struct'
    public bool Field;

    [Struct] // $E0000 Attribute [Struct] can only be used on 'struct'
    public void Method()
    {

    }
}

[Struct] // $E0000 Attribute [Struct] can only be used on 'struct'
class Test2
{
}

[Uno.AttributeUsage(Uno.AttributeTargets.Class)]
public sealed class ClassOnlyAttribute : Uno.Attribute { }

[ClassOnly]
class Test3
{
	[ClassOnly] // $E0000 Attribute [ClassOnly] can only be used on 'class'
	int foo;
}

class Test4
{
    [Uno.Obsolete]
    bool foo;

    [Uno.Obsolete("Neque porro quisquam est qui dolorem ipsum quia dolor sit amet")]
    bool foo2;

    [Uno.Obsolete("Neque porro quisquam est qui dolorem ipsum quia dolor sit amet", true)]
    bool foo3;

    bool loadObsoleteFields()
    {
        var a = foo; // $W4139 Test4.foo is obsolete
        var b = foo2; // $W4139 Test4.foo2 is obsolete: 'Neque porro quisquam est qui dolorem ipsum quia dolor sit amet'
        var c = foo3; // $E4140 Test4.foo3 is obsolete: 'Neque porro quisquam est qui dolorem ipsum quia dolor sit amet'
	return a && b && c;
    }

    void storeObsoleteFields()
    {
        foo = true; // $W4139 Test4.foo is obsolete
        foo2 = true; // $W4139 Test4.foo2 is obsolete: 'Neque porro quisquam est qui dolorem ipsum quia dolor sit amet'
        foo3 = true; // $E4140 Test4.foo3 is obsolete: 'Neque porro quisquam est qui dolorem ipsum quia dolor sit amet'
    }

    void indirectStore(out bool output)
    {
        output = true;
    }

    void indirectStoreObsoleteFields()
    {
        indirectStore(out foo); // $W4139 Test4.foo is obsolete
        indirectStore(out foo2); // $W4139 Test4.foo2 is obsolete: 'Neque porro quisquam est qui dolorem ipsum quia dolor sit amet'
        indirectStore(out foo3); // $E4140 Test4.foo3 is obsolete: 'Neque porro quisquam est qui dolorem ipsum quia dolor sit amet'
    }

    enum PartiallyObsoleteEnum
    {
        NonObsoleteValue,
        [Uno.Obsolete] ObsoleteValue,
    }

    bool assignObsoleteEnumValue()
    {
        var a = PartiallyObsoleteEnum.NonObsoleteValue;
        var b = PartiallyObsoleteEnum.ObsoleteValue; // $W4139 Test4.PartiallyObsoleteEnum.ObsoleteValue is obsolete
        return a != b;
    }

    const string nonObsoleteString = "foo";
    [Uno.Obsolete]
    const string obsoleteString = "bar";

    bool loadObsoleteConstant()
    {
        var a = nonObsoleteString;
        var b = obsoleteString; // $W4139 Test4.obsoleteString is obsolete
        return a != b;
    }

    [Uno.Obsolete]
    class InnerObsolete
    {
        bool loadObsoleteField(Test4 input)
        {
            return input.foo;
        }
    }

    [Uno.Obsolete]
    class ObsoleteClass { public int foo; }

    ObsoleteClass newObsoleteClass()
    {
        return new ObsoleteClass(); // $W4139 Test4.ObsoleteClass is obsolete
    }

    class ObsoleteConstructor
    {
        [Uno.Obsolete]
        public ObsoleteConstructor(int foo)
        {
            Foo = foo;
        }

        public readonly int Foo;
    }

    ObsoleteConstructor newObsoleteConstructor()
    {
        return new ObsoleteConstructor(0); // $W4139 'Test4.ObsoleteConstructor..ctor(int)' is obsolete
    }
}
