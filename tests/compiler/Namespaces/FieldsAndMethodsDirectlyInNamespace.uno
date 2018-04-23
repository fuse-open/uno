namespace x
{
    void Method() // $E Expected namespace, type or block following '{' -- found 'void' (Void)
    {
    }

    string Property { get; set; }

    class Foo
    {
        void Method2()
        {
        }
    }
}
