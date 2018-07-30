using Uno.Compiler.ExportTargetInterop;
using Uno.Content.Images;
using Uno.Graphics;

namespace Uno.Content.Fonts
{
    public struct RenderedGlyph
    {
        public float2 Advance;
        public float2 Bearing;
        public int2 Size;
        public Format Format;
        public byte[] Data;

        public RenderedGlyph(float2 advance, float2 bearing, int2 size, Format format, byte[] data)
        {
            Advance = advance;
            Bearing = bearing;
            Size = size;
            Format = format;
            Data = data;
        }

        [Obsolete]
        public RenderedGlyph(float2 advance, float2 bearing, Bitmap bitmap)
        {
            Advance = advance;
            Bearing = bearing;
            Size = bitmap.Size;
            Format = bitmap.Format;
            Data = bitmap.Buffer.GetBytes();
        }

        [Obsolete]
        public Bitmap Bitmap
        {
            get { return new Bitmap(Size, Format, new Buffer(Data)); }
        }
    }
}
