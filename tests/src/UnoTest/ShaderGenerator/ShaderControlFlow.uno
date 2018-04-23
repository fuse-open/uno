using Uno;
using Uno.Graphics;
using Uno.Diagnostics;

namespace UnoTest.ShaderGenerator {

    class Colors {
        const int max_count = 2;

        public int[] On = new int[max_count];
        public float3[] Value = new float3[max_count];

        PixelColor: {
            float3 sum = float3(0,0,0);
            for( int i=0; i < max_count; ++i ) {
                //doesn't work
                if( On[i] == 0 ) {
                    continue;
                }
                sum += Value[i];

                //works (but obviously not what I want)
                //sum += Value[i] * (float)On[i];
            }
            return prev * float4( sum, 1 );
        };
    }

    public class ShaderControlFlow {

        Colors colors = new Colors();

        public ShaderControlFlow() {
            colors.Value[0] = float3(1,0,0);
            colors.On[0] = 0;
            colors.Value[1] = float3(0,1,0);
            colors.On[1] = 0;
        }

        public void Draw() {
            draw DefaultShading, Sphere {
                Radius: 20;
                Translation: float3(0,0,50);
                VertexCount: 0;
                VertexPosition: float3();
            };

            var frameTime = Clock.GetSeconds();

            colors.On[0] = Math.Sin( frameTime * 3) > 0 ? 1 :0;
            colors.On[1] = Math.Sin( frameTime * 2 ) > 0 ? 1 : 0;
            draw DefaultShading, colors, Sphere {
                Radius: 20;
                Translation: float3(0,0,00);
                VertexCount: 0;
                VertexPosition: float3();
            };


            colors.On[0] = Math.Cos( frameTime * 3) > 0 ? 1 :0;
            colors.On[1] = Math.Cos( frameTime * 2 ) > 0 ? 1 : 0;
            draw DefaultShading, colors, Sphere {
                Radius: 20;
                Translation: float3(0,0,-50);
                VertexCount: 0;
                VertexPosition: float3();
            };
        }
    }
}
