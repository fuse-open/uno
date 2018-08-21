using Uno.Compiler.ExportTargetInterop;
using Uno.Content.Images;
using Uno.Graphics;
using Uno.IO;
using Uno.Native.Fonts;

namespace Uno.Content.Fonts
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
            var g = _ff.RenderGlyph(size, glyph, FontRenderMode.Normal);

            // TODO: Format is hardcoded

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

namespace Uno.Native.Fonts
{
    [extern(CIL) DotNetType]
    [extern(CIL) Require("Template", "Uno.Native")]
    extern(DOTNET)
    public class FontFace : NativeObject
    {
        public extern string FamilyName { get; private set; }
        public extern string StyleName { get; private set; }

        public extern FontFace(string filename);
        public extern FontFace(byte[] bytes);
        public extern FontFace(Stream stream);
        public extern float GetAscender(float pixelSize);
        public extern float GetDescender(float pixelSize);
        public extern float GetLineHeight(float pixelSize);
        public extern bool ContainsGlyph(float pixelSize, char glyph);
        public extern RenderedGlyph RenderGlyph(float pixelSize, char glyph, FontRenderMode mode);
        public extern bool TryGetKerning(float pixelSize, char left, char right, out float kerningX, out float kerningY);
    }

    [extern(CIL) DotNetType]
    [extern(CIL) Require("Template", "Uno.Native")]
    extern(DOTNET)
    public struct RenderedGlyph
    {
        public float AdvanceX, AdvanceY;
        public float BearingX, BearingY;
        public Textures.PixelFormat PixelFormat;
        public int Width, Height;
        public byte[] Bitmap;
    }

    [extern(CIL) DotNetType]
    [extern(CIL) Require("Template", "Uno.Native")]
    extern(DOTNET)
    public enum FontRenderMode
    {
        None,
        Normal,
        Monochrome,
    }
}

namespace Uno.Native
{
    [extern(CIL) DotNetType]
    [extern(CIL) Require("Template", "Uno.Native")]
    extern(DOTNET)
    public class NativeObject : IDisposable
    {
        public extern void Dispose();
    }
}
