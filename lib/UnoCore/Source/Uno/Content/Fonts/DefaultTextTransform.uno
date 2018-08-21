using Uno.Matrix;

namespace Uno.Content.Fonts
{
    [Obsolete]
    public class DefaultTextTransform : TextTransform
    {
        float4x4 _matrix = float4x4.Identity;

        public override float4x4 Matrix
        {
            get { return _matrix; }
            set { _matrix = value; }
        }

        public extern(!MOBILE) override float4x4 ResolveClipSpaceMatrix()
        {
            var csScale = float2(2, -2) / (float2)Application.Current.GraphicsController.Viewport.Size;
            var csOffset = float2(-1, 1);
            var csMatrix = Mul(Scaling(float3(csScale, 1)), Translation(float3(csOffset, 0)));
            return Mul(Matrix, csMatrix);
        }

        public extern(MOBILE) override float4x4 ResolveClipSpaceMatrix()
        {
            debug_log "DefaultTextTransform: Not implemented on mobile";
            return _matrix;
        }
    }
}
