public class Main : Uno.Application
{
    public Foo field;   // $E4128

    private class Foo {}
}