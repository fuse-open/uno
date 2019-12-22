using Uno.Collections;

namespace Uno.Graphics.Utils.Text
{
    public sealed class BitmapFont : IDisposable
    {
        public struct GlyphInfo
        {
            public float Advance;
            public float2 Bearing;
            public float2 Size;
            public ushort2 UpperLeft;
            public ushort2 LowerRight;
        }

        public struct CharPair
        {
            public readonly char Left, Right;

            public CharPair(char left, char right)
            {
                Left = left;
                Right = right;
            }

            public override int GetHashCode()
            {
                int hash = 27;
                hash = hash * 13 + (int)Left;
                hash = hash * 13 + (int)Right;
                return hash;
            }
        }

        public readonly Dictionary<char, GlyphInfo> Glyphs = new Dictionary<char, GlyphInfo>();
        public readonly Dictionary<char, float> Advances = new Dictionary<char, float>();
        public readonly Dictionary<CharPair, float> Kernings = new Dictionary<CharPair, float>();

        public texture2D Texture;

        public float Ascent;
        public float Descent;
        public float LineHeight;

        public float PixelSize;
        public float PixelSpread;

        public string FamilyName;
        public string StyleName;
        public FontFace FontFace;

        public void Dispose()
        {
            if (Texture != null)
                Texture.Dispose();
        }
    }
}
