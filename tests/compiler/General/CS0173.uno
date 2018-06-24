public class C {}

public class A {}

public class Main
{
    public static void F(bool b)
    {
        var a = new A();
        var c = new C();

        object o = b ? a : c; // $E2047
    }

   public static void Main()
   {
        F(true);

        int X = 1;
        object o = (X == 0) ? null : null; // $E2086
   }
}