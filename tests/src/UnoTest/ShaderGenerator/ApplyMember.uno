using Uno;

namespace UnoTest.ShaderGenerator
{
    public class ApplyMember
    {
        public Batch Batch;

        public void Draw()
        {
            draw DefaultShading, Cube
            {
                apply this.Batch;
            };
        }
    }

    public class ApplyMember2
    {
        public Batch Batch { get; set; }

        public void Draw()
        {
            int foo = 0;

            draw DefaultShading, Cube
            {
                apply Batch;
                PixelColor: float4(foo);
            };
        }
    }

    public class ApplyMember3
    {
        public Batch Batch { get; set; }

        public void Draw()
        {
            draw virtual this.Batch, DefaultShading, Cube;
        }
    }
}
