class Main
{
    public Main()
    {
        new Foo();
        new Bar(); // $E2090
        new Enum(); // $E2012
        new Struct(); // $E2012 [Ignore]

        // TODO: uno parser doesn't handle this very gracefully...
        //new 0;
        //new new;
        //new null;

        new Main();
        new int(); // $E2012 [Ignore] $W Redundant 'new' operator on builtin struct initialization
    }
}

enum Enum {}
struct Struct {}
class Foo {}
static class Bar {}
