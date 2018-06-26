
class Main
{
    apply A;

    public override void Draw()
    {
        draw
        {
            PixelColor: float4(0);
            ClipPosition: float4(0);
            VertexCount: 0;
        };
    }
}

block A
{
    apply B; // $E0105 [Ignore]
}

block B
{
    apply A;
}


class C: D
{
    public void Draw()
    {
        draw
        {
            PixelColor: float4(0);
            ClipPosition: float4(0);
            VertexCount: 0;
        };
    }
}

class D: C // $E3048
{

}
