using Uno.Compiler.ExportTargetInterop;
using Uno.Content.Images;

namespace Uno.Content.Fonts
{
    public struct RenderedGlyph
    {
        public float2 Advance;
        public float2 Bearing;
        public Bitmap Bitmap;

        public RenderedGlyph(float2 advance, float2 bearing, Bitmap bitmap)
        {
            Advance = advance;
            Bearing = bearing;
            Bitmap = bitmap;
        }
    }
}
