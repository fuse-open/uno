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
    }// $E Expected 'catch' or 'finally' following '}' -- found '}' (RightCurlyBrace)
}

class Foo {}

class Bar : Exception {}
