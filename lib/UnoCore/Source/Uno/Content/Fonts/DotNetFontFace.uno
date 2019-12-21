using Uno.Compiler.ExportTargetInterop;

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
