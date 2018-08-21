using Uno.Graphics;
using Uno.Vector;
using Uno.Math;

namespace Uno.Content.Fonts
{
    [Obsolete]
    block SpriteFontShaderBlock : TextShader
    {
        float AlphaThreshold: 0.15f;

        PixelColor: Color * float4(1, 1, 1, FontMask);

        BlendSrcRgb: BlendOperand.SrcAlpha;
        BlendDstRgb: BlendOperand.OneMinusSrcAlpha;

        BlendSrcAlpha: BlendOperand.One;
        BlendDstAlpha: BlendOperand.OneMinusSrcAlpha;
    }

    [Obsolete]
    public class SpriteFontShader : TextShader
    {
        public override void Draw(TextShaderData data)
        {
            draw this, data, SpriteFontShaderBlock;
        }
    }
}
