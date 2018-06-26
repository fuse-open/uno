public class Main
{
    public Main()
    {
        var x = new [] { 1, "str" }; // $E2004

        char c = 'c';
        short s1 = 0;
        short s2 = -0;
        short s3 = 1;
        short s4 = -1;

        var array1 = new [] { s1, s2, s3, s4, c, '1' }; // $E2004
    }
}