namespace UnoTest
{
    class NestedBlock
    {
        static float4 GetColor()
        {
            return float4(1, 1, 0, 1);
        }

        class Class
        {
            public void Draw()
            {
                draw Block;
            }

            block Block
            {
                float2[] VertexData:
                    new[] {
                        float2(0, 0), float2(1, 0), float2(1, 1),
                        float2(0, 0), float2(1, 1), float2(0, 1)
                    };
                ClipPosition:
                    float4(vertex_attrib(VertexData), 0, 0);
                PixelColor:
                    GetColor();
            }
        }
    }
}
