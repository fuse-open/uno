using Uno.Content.Fonts;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Content.Fonts
{
    [Obsolete]
    extern(JAVASCRIPT) class CanvasFontFace : FontFace
    {
        readonly string _handle;

        public CanvasFontFace(string name)
        {
            _handle = name;
        }

        public override void Dispose()
        {
        }

        public override string FamilyName { get { return ""; } }
        public override string StyleName { get { return _handle; } }

        public override float GetAscender(float size)
        {
            return 0.0f;
        }

        public override float GetDescender(float size)
        {
            return 0.0f;
        }

        public override float GetLineHeight(float size)
        {
            return 0.0f;
        }

        public override bool ContainsGlyph(float size, char glyph)
        {
            return false;
        }

        public override RenderedGlyph RenderGlyph(float size, char glyph)
        {
            return new RenderedGlyph(float2(0), float2(0), null);
        }

        public override bool TryGetKerning(float size, char left, char right, out float2 result)
        {
            result = float2(0);
            return false;
        }
    }
}
