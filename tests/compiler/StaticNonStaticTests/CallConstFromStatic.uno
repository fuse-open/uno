public class Main
{
    public Main()
    {
        var foo = Multipart.ContentType; // $W0000 The variable 'foo' is assigned but its value is never used
    }
}

static class Multipart
{
    private const string Boundary = "c0pp4Jt08gc0j34q0M2Y";
    public static string ContentType = "multipart/form-data; boundary=" + Boundary;
}
