class Main
{
    class Class
    {
    }

    void Value(int x)
    {
    }

    void Out(out int x)
    {
        x = 123;
    }

    void Ref(ref int x)
    {
    }

    void Const(const int x)
    {
    }

    void Value(Class x)
    {
    }

    void Out(out Class x)
    {
        x = new Class();
    }

    void Ref(ref Class x)
    {
    }

    void Const(const Class x)
    {
    }

    public Main()
    {
        int x = 123;
        Value(out x); // $E3129
        Value(ref x); // $E3129
        Value(const x); // $E0107
        Value(x);
        Out(out x);
        Out(ref x); // $E3129
        Out(const x); // $E0107 $E3129
        Out(x); // $E3129
        Ref(out x); // $E3129
        Ref(ref x);
        Ref(const x); // $E0107 $E3129
        Ref(x); // $E3129
        Const(out x); // $E3129
        Const(ref x); // $E3129
        Const(const x); // $E0107 $E3129
        Const(x); // $E3129

        Class c = new Class();
        Value(out c); // $E3129
        Value(ref c); // $E3129
        Value(const c); // $E0107
        Value(c);
        Out(out c);
        Out(ref c); // $E3129
        Out(const c); // $E0107 $E3129
        Out(c); // $E3129
        Ref(out c); // $E3129
        Ref(ref c);
        Ref(const c); // $E0107 $E3129
        Ref(c); // $E3129
        Const(out c); // $E3129
        Const(ref c); // $E3129
        Const(const c); // $E0107 $E3129
        Const(c); // $E3129
    }
}