namespace Uno.Graphics.Utils.Text
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
    }
}
