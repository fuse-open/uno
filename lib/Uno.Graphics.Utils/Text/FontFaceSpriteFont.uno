using Uno.Collections;
using Uno.Math;

namespace Uno.Graphics.Utils.Text
{
    public static class CharacterSets
    {
        public const string Ascii = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZÆØÅ[\\]^_`abcdefghijklmnopqrstuvwxyzæøå¨{|}~°\u2022";
    }

    public static class FontFaceSpriteFont
    {
        public static BitmapFont RenderSpriteFont(this FontFace font, float size, string charset)
        {
            var glyphs = new Dictionary<char, RenderedGlyph>();
            var charsetInclWs = charset + " \0";
            var maxSize = 0;

            for (int i = 0; i < charsetInclWs.Length; i++)
            {
                var c = charsetInclWs[i];

                if (font.ContainsGlyph(size, c))
                {
                    if (glyphs.ContainsKey(c))
                        continue;

                    var g = font.RenderGlyph(size, c);
                    glyphs.Add(c, g);

                    if (g.Data != null)
                    {
                        maxSize = Max(maxSize, g.Size.X);
                        maxSize = Max(maxSize, g.Size.Y);
                    }
                }
            }

            var sideCount = (int)Ceil(Sqrt((float)glyphs.Count));
            var sideSize = NextPow2(sideCount * maxSize);

            var dst = new Bitmap(int2(sideSize, sideSize), Format.L8);
            var tex = new Texture2D(dst.Size, dst.Format, true);

            var result = new BitmapFont()
            {
                Texture = tex,

                Ascent = Floor(font.GetAscender(size) + .5f),
                Descent = Floor(font.GetDescender(size) + .5f),
                LineHeight = Floor(font.GetLineHeight(size) + .5f),

                PixelSize = size,
                PixelSpread = 0,

                FontFace = font,
                FamilyName = font.FamilyName,
                StyleName = font.StyleName,
            };

            int gi = 0;
            foreach (var e in glyphs)
            {
                int dstX = (gi % sideCount) * (sideSize / sideCount);
                int dstY = ((gi / sideCount) % sideCount) * (sideSize / sideCount);
                gi++;

                var src = e.Value;
                var srcSize = float2(0, 0);

                if (src.Data != null)
                {
                    srcSize = src.Size;
                    var bpp = FormatHelpers.GetStrideInBytes(src.Format);

                    for (int srcY = 0; srcY < src.Size.Y; srcY++)
                        for (int srcX = 0; srcX < src.Size.X; srcX++)
                            dst.Data[(dstY + srcY) * dst.Size.X + dstX + srcX] =
                                src.Data[((srcY * src.Size.X) + srcX) * bpp];
                }

                BitmapFont.GlyphInfo g;
                g.Advance = Floor(e.Value.Advance.X + .5f);
                g.Bearing = Floor(e.Value.Bearing + .5f);
                g.Size = (float2)srcSize;
                g.UpperLeft.X = (ushort)((double)ushort.MaxValue * (double)(dstX) / (double)dst.Size.X);
                g.UpperLeft.Y = (ushort)((double)ushort.MaxValue * (double)(dstY) / (double)dst.Size.Y);
                g.LowerRight.X = (ushort)((double)ushort.MaxValue * (double)(dstX + srcSize.X) / (double)dst.Size.X);
                g.LowerRight.Y = (ushort)((double)ushort.MaxValue * (double)(dstY + srcSize.Y) / (double)dst.Size.Y);

                result.Glyphs.Add(e.Key, g);
                result.Advances.Add(e.Key, g.Advance);
            }

            tex.Update(dst.Data);

            if (tex.IsMipmap)
                tex.GenerateMipmap();

            for (int i = 0; i < charsetInclWs.Length; i++)
            {
                var left = charsetInclWs[i];

                for (int j = 0; j < charsetInclWs.Length; j++)
                {
                    var right = charsetInclWs[j];

                    float2 kerning;
                    if (font.TryGetKerning(size, left, right, out kerning) && Abs(kerning.X) > 0)
                        result.Kernings[new BitmapFont.CharPair(left, right)] = kerning.X;
                }
            }

            return result;
        }
    }
}
