class A
{
}

class B: A
{
}

class C: A
{
}

class Main
{
    static void Main() // $E0000 'Main': Member names cannot be the same as their enclosing types
    {
        var a = new C();
        var b = new B();
        C c; // $W The variable 'c' is assigned but its value is never used

        // This is valid; there is a built-in reference
        // conversion from A to C.
        c = a as C;

        //The following generates CS0039; there is no
        // built-in reference conversion from B to C.
        c = b as C;  // $E4118
    }
}
