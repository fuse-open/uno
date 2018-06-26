class A
{
    // allows for the conversion of A object to int
    public static implicit operator int (A s)
    {
        return 0;
    }

    public static implicit operator string (A i)
    {
        return null;
    }
}

class B
{
    public static implicit operator int (B s)
    // one way to resolve this CS0034 is to make one conversion explicit
    // public static explicit operator int (B s)
    {
        return 0;
    }

    public static implicit operator string (B i)
    {
        return null;
    }

    public static implicit operator B (string i)
    {
        return null;
    }

    public static implicit operator B (int i)
    {
        return null;
    }
}

class Main
{
    public static void Main ()
    {
        var a = new A();
        var b = new B();
        b = b + a;   // $E0109
        b = b + (int)a;
    }
}