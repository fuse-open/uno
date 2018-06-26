class ii
{
    int i
    {
        get { return 0; }
    }
}

public class a
{
    public int i;
    public static a operator ii(a aa) // $E Expected overloadable unary operator -- found 'ii' (Identifier)

    // Use the following line instead:
    //public static a operator ++(a aa)
    {
        aa.i++;
        return aa;
    }
}
