namespace foo
{
    public class Base
    {
        //Fields
        int i;
        int j;

        //Literals
        const int SomeLiteral = 42;

        //Properties
        int Prop1 {get; set;}
        int Prop2 {get; set;}
        virtual int Prop3 {get; set;}
        public int this[int index] { get { return 0; } }

        //Methods
        void m1() {}
        virtual void m2() {}
        void m3(float f){}
        void m4<T>(T t) {}
        void m5<T, U>(T t, U u) {}
    }

    public class Derived : Base
    {
        //Fields
        int j; // $W [Ignore] hiding member

        //Literals
        float SomeLiteral = 13.37f; // $W [Ignore] hiding a literal

        //Properties
        int Prop1 {get; set;} // $W [Ignore] property hiding a property
        float Prop2; // $W [Ignore] non-property hiding a property
        override int Prop3 {get; set;}
        public int this[int index] { get { return 0; } } // $W 'foo.Derived[int]' hides inherited member 'foo.Base[int]' -- use the 'new' modifier if hiding is intentional
        public int this[string index] { get { return 0; } }

        //New
        new int i; // $W foo.Derived.i specifies 'new', but does not hide any inherited members.
        new int k; // $W foo.Derived.k specifies 'new', but does not hide any inherited members.

        //Methods
        int m1; // $W [Ignore] non-method hiding a method
        override void m2() {} // Overriding is ok
        void m3(int i){} // Different parameter list is ok
        void m4<U>(U u) {} // $W [Ignore] same number of generic parameters
        void m5<U>(U u) {} // Different number of generic parameters is ok
    }

    abstract class FooBase
    {
        abstract protected object Bar<T>(object property);
    }

    class Foo : FooBase
    {
        protected override object Bar<T>(object property)
        {
            return null;
        }
    }
}
