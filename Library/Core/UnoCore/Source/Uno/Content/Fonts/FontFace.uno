using Uno.Compiler.ExportTargetInterop;
using Uno.Collections;
using Uno.Graphics;
using Uno.IO;

namespace Uno.Content.Fonts
{
    public abstract class FontFace : IDisposable
    {
        public static FontFace Load(BundleFile file)
        {
            if defined(CPLUSPLUS)
                return new CppFontFace(file);
            else if defined(JAVASCRIPT)
                return new CanvasFontFace(file.Name);
            else if defined(DOTNET)
                using (var s = file.OpenRead())
                    return new DotNetFontFace(s);
            else
                build_error;
        }

        [Obsolete]
        public static FontFace Load(string name, byte[] data, int offset, int length)
        {
            if defined(CPLUSPLUS)
                return new CppFontFace(data, offset, length);
            else if defined(JAVASCRIPT)
                return new CanvasFontFace(name);
            else if defined(DOTNET)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));
                if (offset != 0)
                    throw new ArgumentOutOfRangeException(nameof(offset));
                if (length != data.Length)
                    throw new ArgumentOutOfRangeException(nameof(length));

                using (var s = new MemoryStream(data))
                    return new DotNetFontFace(s);
            }
            else
                build_error;
        }

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
