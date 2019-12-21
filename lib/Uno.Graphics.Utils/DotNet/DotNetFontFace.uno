using Uno.Compiler.ExportTargetInterop;
using Uno.Graphics.Utils.Text;
using Uno.IO;

namespace Uno.Graphics.Utils.DotNet
{
    extern(DOTNET)
    class DotNetFontFace : FontFace
    {
        readonly Native.Fonts.FontFace _ff;

        public DotNetFontFace(Stream stream)
        {
            _ff = new Native.Fonts.FontFace(stream);
        }

        public override void Dispose()
        {
            _ff.Dispose();
        }

        public override float GetAscender(float size)
        {
            return _ff.GetAscender(size);
        }

        public override float GetDescender(float size)
        {
            return _ff.GetDescender(size);
        }

        public override float GetLineHeight(float size)
        {
            return _ff.GetLineHeight(size);
        }

        public override bool ContainsGlyph(float size, char glyph)
        {
            return _ff.ContainsGlyph(size, glyph);
        }

        public override RenderedGlyph RenderGlyph(float size, char glyph)
        {
            var g = _ff.RenderGlyph(size, glyph, Uno.Native.Fonts.FontRenderMode.Normal);
            return new RenderedGlyph
            {
                Advance = new Float2(g.AdvanceX, g.AdvanceY),
                Bearing = new Float2(g.BearingX, g.BearingY),
                Size = new Int2(g.Width, g.Height),
                Format = Format.L8,
                Data = g.Bitmap
            };
        }

        public override bool TryGetKerning(float size, char left, char right, out Float2 result)
        {
            return _ff.TryGetKerning(size, left, right, out result.X, out result.Y);
        }

        public override string FamilyName
        {
            get { return _ff.FamilyName; }
        }

        public override string StyleName
        {
            get { return _ff.StyleName; }
        }
    }
}
