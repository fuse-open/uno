namespace Uno.Graphics.Utils.Text
{
    public abstract class FontFace : IDisposable
    {
        public abstract void Dispose();

        public abstract string FamilyName { get; }
        public abstract string StyleName { get; }

        public abstract float GetAscender(float size);
        public abstract float GetDescender(float size);
        public abstract float GetLineHeight(float size);

        public abstract bool ContainsGlyph(float size, char glyph);
        public abstract RenderedGlyph RenderGlyph(float size, char glyph);

        public abstract bool TryGetKerning(float size, char left, char right, out float2 result);
    }
}
