using Uno;

class Main : Uno.Application
{
    public Main()
    {
        try {}
        catch() {}

        try {}
        catch {}

        try {}
        catch(Exception exception) {}

        try {}
        catch(Bar exception) {}

        try {}
        catch(Foo exception) {} // $E4045
    }
}

class Foo {}

class Bar : Exception {}