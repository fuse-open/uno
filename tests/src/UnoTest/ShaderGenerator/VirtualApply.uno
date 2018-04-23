using Uno;

namespace UnoTest.ShaderGenerator
{
    class VirtualApply
    {
        abstract class Base
        {
        }

        class SubA : Base
        {
            public float4 SomeColor = float4(1,0,0,0);
        }

        class SubAA : SubA
        {
        }

        class SubAB : SubA
        {
        }

        class SubBA : SubB
        {
        }

        class SubBB : SubB
        {
        }

        class SubBBA : SubBB
        {
        }

        class SubB : Base
        {
            public float4 SomeColor
            {
                get;
                set;
            }

            meta block Foobar
            {
                apply Cube, DefaultShading;
                WorldPosition: prev + float3(0,0,10);
            }
        }

        void Draw(Base b, Base c)
        {
            draw DefaultShading, Cube, virtual b, virtual c
            {
                VertexCount: 0;
                VertexPosition: float3();
                PixelColor: SomeColor;
            };
        }

        void Draw()
        {
            var lol = new SubB();
            lol.SomeColor = float4(1,1,0,1);
            Draw(lol, lol);
        }

        public static void Run()
        {
            new VirtualApply().Draw();
        }
    }
}
