using Uno;
using Uno.Graphics;

namespace UnoTest.ShaderGenerator
{
    class SimpleDrawStatement
    {
        void Draw()
        {
            draw
            {
                float2[] xxx: new float2[] { float2(-1,-1), float2(1,-1) };
                float2 vdata: vertex_attrib(xxx);
                PrimitiveType: PrimitiveType.Lines;
                ClipPosition: float4(vdata,0,1);
                PixelColor: float4(.4f,.3f,.2f,1);
            };
        }

        public static void Run()
        {
            new SimpleDrawStatement().Draw();
        }
    }
}
