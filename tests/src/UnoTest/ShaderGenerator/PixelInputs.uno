using Uno;
using Uno.Graphics;
using Uno.Diagnostics;

namespace UnoTest.ShaderGenerator
{
    public class PixelInputs
    {
        public void Draw()
        {
            draw DefaultShading, Cube
            {
                VertexCount: 0;
                VertexPosition: float3();
                PixelColor: PixelCoord;
            };
        }
    }
}
