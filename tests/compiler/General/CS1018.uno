class C {}

class a : C
{
    public a(int i) {}

    public a () :
    {} // $E Expected 'base' or 'this' following ':' -- found '{' (LeftCurlyBrace)
}
