class ReadonlyStructs
{
    struct Struct1
    {
        public readonly int Integer;
    }

    void Test1()
    {
        var s = new Struct1();
        s.Integer = 123; // $E4087
    }

    struct Struct2
    {
        public readonly object Obj;
    }

    void Test2()
    {
        var s = new Struct2();
        s.Obj = new object(); // $E4087
    }

    struct Struct3
    {
        public readonly Struct1 Struct;
    }

    void Test3()
    {
        var s = new Struct3();
        s.Struct = new Struct1(); // $E4087
    }

    void Test4()
    {
        var s = new Struct3();
        s.Struct.Integer = 123; // $E4087
    }

    struct Struct4
    {
        public int Integer;
    }

    struct Struct5
    {
        public readonly Struct4 Struct;
    }

    void Test5()
    {
        var s = new Struct5();
        s.Struct.Integer = 123;
    }

    static Struct4 StaticStruct4 = new Struct4();

    void Test6()
    {
        StaticStruct4.Integer = 123;
    }

    static Struct5 StaticStruct5 = new Struct5();

    void Test7()
    {
        StaticStruct5.Struct.Integer = 123;
    }

    static Struct3 StaticStruct3 = new Struct3();

    void Test8()
    {
        StaticStruct3.Struct = new Struct1(); // $E4087
    }

    void Test9()
    {
        StaticStruct3.Struct.Integer = 13; // $E4087
    }
}

class ReadonlyClasses
{
    class Class1
    {
        public readonly int Integer;
    }

    void Test1()
    {
        var s = new Class1();
        s.Integer = 123; // $E4087
    }

    class Class2
    {
        public readonly object Obj;
    }

    void Test2()
    {
        var s = new Class2();
        s.Obj = new object(); // $E4087
    }

    struct Class3
    {
        public readonly Class1 Class;
    }

    void Test3()
    {
        var s = new Class3();
        s.Class = new Class1(); // $E4087
    }

    void Test4()
    {
        var s = new Class3();
        s.Class.Integer = 123; // $E4087
    }

    class Class4
    {
        public int Integer;
    }

    class Class5
    {
        public readonly Class4 Class;
    }

    void Test5()
    {
        var s = new Class5();
        s.Class.Integer = 123;
    }

    static Class4 StaticClass4 = new Class4();

    void Test6()
    {
        StaticClass4.Integer = 123;
    }

    static Class5 StaticClass5 = new Class5();

    void Test7()
    {
        StaticClass5.Class.Integer = 123;
    }

    static Class3 StaticClass3 = new Class3();

    void Test8()
    {
        StaticClass3.Class = new Class1(); // $E4087
    }

    void Test9()
    {
        StaticClass3.Class.Integer = 13; // $E4087
    }
}