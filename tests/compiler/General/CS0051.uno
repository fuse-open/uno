public class Main : Uno.Application
{
    private class B {}

    public static void F(B b) {} // $E4128
}