using Uno.Math;

namespace Uno.Graphics.Utils.Text
{
    public sealed class TextRenderer
    {
        public bool HasBegun { get; private set; }

        int _maxCharCount;

        float _tracking;
        byte4 _color = byte4(0xff, 0xff, 0xff, 0xff);

        BitmapFont _font;
        TextShader _shader;
        TextTransform _transform;

        byte[] _buffer;

        VertexBuffer _vbo;
        IndexBuffer _ibo;
        TextShaderData _data;

        public TextRenderer(int maxCharCount, TextShader shader, TextTransform transform = null)
        {
            _maxCharCount = maxCharCount;

            var indexBuffer = new byte[maxCharCount * 6 * 2];

            for (int i = 0; i < maxCharCount; i++)
            {
                indexBuffer.Set(i * 12 + 00, (ushort)(i * 4 + 0));
                indexBuffer.Set(i * 12 + 02, (ushort)(i * 4 + 1));
                indexBuffer.Set(i * 12 + 04, (ushort)(i * 4 + 2));
                indexBuffer.Set(i * 12 + 06, (ushort)(i * 4 + 0));
                indexBuffer.Set(i * 12 + 08, (ushort)(i * 4 + 2));
                indexBuffer.Set(i * 12 + 10, (ushort)(i * 4 + 3));
            }

            _ibo = new IndexBuffer(indexBuffer, BufferUsage.Immutable);
            _vbo = new VertexBuffer(BufferUsage.Stream);

            _buffer = new byte[maxCharCount * 4 * 16];

            _data = new TextShaderData(_ibo, _vbo);

            _shader = shader;
            _transform = transform ?? new DefaultTextTransform();
        }

        public TextTransform Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }

        public float Scale
        {
            get { return _data.FontScale; }
            set { _data.FontScale = value; }
        }

        public float Tracking
        {
            get { return _tracking; }
            set { _tracking = value; }
        }

        public float4 Color
        {
            get { return float4((float)_color.X / 255.0f, (float)_color.Y / 255.0f, (float)_color.Z / 255.0f, (float)_color.W / 255.0f); }
            set { _color = byte4((byte)(Saturate(value.X) * 255.0f), (byte)(Saturate(value.Y) * 255.0f), (byte)(Saturate(value.Z) * 255.0f), (byte)(Saturate(value.W) * 255.0f)); }
        }

        public TextShader Shader
        {
            get { return _shader; }
            set { _shader = value; }
        }

        public void Begin(BitmapFont font)
        {
            if (font == null)
                throw new ArgumentNullException(nameof(font));

            _font = font;
            _data.CharCount = 0;

            HasBegun = true;
        }

        public float2 MeasureString(string text)
        {
            return MeasureString(text, 0, text.Length);
        }

        public float2 MeasureString(string text, int startIndex, int length)
        {
            if (_font == null || text == null)
                return float2(0);

            float caret = 0.0f;
            char last = '\0';

            for (int i = startIndex; i < startIndex+length; i++)
            {
                char c = text[i];

                // TODO: Find better solution to handle newline char/non-width space
                // It is drawn as a square if the font does not support this char
                if ((i == 0) && (c == '\u200B'))
                    continue;

                BitmapFont.GlyphInfo ci;
                if (_font.Glyphs.TryGetValue(c, out ci))
                {
                    float kerning;
                    if (_font.Kernings.TryGetValue(new BitmapFont.CharPair(last, c), out kerning))
                        caret += kerning * _data.FontScale;

                    caret += ci.Advance * _data.FontScale + _tracking;
                }
                else if (_font.FontFace != null && _font.FontFace.ContainsGlyph(_font.PixelSize, c))
                {
                    RenderedGlyph rg = _font.FontFace.RenderGlyph(_font.PixelSize, c);
                    float2 kerning;
                    if (_font.FontFace.TryGetKerning(_font.PixelSize, last, c, out kerning))
                    {
                        caret += kerning.X * _data.FontScale;
                    }

                    caret += Floor(rg.Advance.X + .5f) * _data.FontScale + _tracking;
                }

                last = c;
            }

            return float2(caret, _font.Ascent + _font.Descent);
        }

        public void WriteString(float2 caret, string text)
        {
            WriteString(caret, text, 0, text.Length);
        }

        private void WriteGlyph(BitmapFont.GlyphInfo ci, float2 caret)
        {
            if (ci.UpperLeft.X != ci.LowerRight.X &&
                ci.UpperLeft.Y != ci.LowerRight.Y)
            {
                var p = caret + (ci.Bearing * _data.FontScale);
                var o = _data.CharCount * 64;

                var s = ci.Size * _data.FontScale;

                // LL
                _buffer.Set(o + 00, p.X);
                _buffer.Set(o + 04, p.Y + s.Y);
                _buffer.Set(o + 08, ci.UpperLeft.X);
                _buffer.Set(o + 10, ci.LowerRight.Y);
                _buffer.Set(o + 12, _color);

                // LR
                _buffer.Set(o + 16, p.X + s.X);
                _buffer.Set(o + 20, p.Y + s.Y);
                _buffer.Set(o + 24, ci.LowerRight.X);
                _buffer.Set(o + 26, ci.LowerRight.Y);
                _buffer.Set(o + 28, _color);

                // UR
                _buffer.Set(o + 32, p.X + s.X);
                _buffer.Set(o + 36, p.Y);
                _buffer.Set(o + 40, ci.LowerRight.X);
                _buffer.Set(o + 42, ci.UpperLeft.Y);
                _buffer.Set(o + 44, _color);

                // UL
                _buffer.Set(o + 48, p.X);
                _buffer.Set(o + 52, p.Y);
                _buffer.Set(o + 56, ci.UpperLeft.X);
                _buffer.Set(o + 58, ci.UpperLeft.Y);
                _buffer.Set(o + 60, _color);

                _data.CharCount++;
            }
        }

        public void WriteString(float2 caret, string text, int startIndex, int length)
        {
            char last = '\0';

            // Offset caret because SDF fonts are indented
            if (_font.PixelSpread > 0)
                caret -= float2(_font.PixelSpread * _data.FontScale) * float2(1, 2);

            for (int i = startIndex; i < startIndex+length; i++)
            {
                char c = text[i];

                // TODO: Find better solution to handle newline char/non-width space
                // It is drawn as a square if the font does not support this char
                if ((i == 0) && (c == '\u200B'))
                    continue;

                BitmapFont.GlyphInfo ci;
                if (_font.Glyphs.TryGetValue(c, out ci))
                {
                    float kerning;
                    if (_font.Kernings.TryGetValue(new BitmapFont.CharPair(last, c), out kerning))
                    {
                        caret.X += kerning * _data.FontScale;
                    }

                    if (ci.UpperLeft.X != ci.LowerRight.X &&
                        ci.UpperLeft.Y != ci.LowerRight.Y)
                    {
                        WriteGlyph(ci, caret);

                        if (_data.CharCount == _maxCharCount)
                        {
                            var temp = _font;
                            End();
                            Begin(temp);
                        }
                    }

                    caret.X += ci.Advance * _data.FontScale + _tracking;
                }
                else if (_font.FontFace != null && _font.FontFace.ContainsGlyph(_font.PixelSize, c))
                {
                    var oldFont = _font;
                    using (var tempFont = oldFont.FontFace.RenderSpriteFont(oldFont.PixelSize, c.ToString()))
                    {
                        if (tempFont.Glyphs.TryGetValue(c, out ci))
                        {
                            float kerning;
                            if (tempFont.Kernings.TryGetValue(new BitmapFont.CharPair(last, c), out kerning))
                            {
                                caret.X += kerning * _data.FontScale;
                            }

                            if (ci.UpperLeft.X != ci.LowerRight.X &&
                                ci.UpperLeft.Y != ci.LowerRight.Y)
                            {
                                End();
                                Begin(tempFont);
                                WriteGlyph(ci, caret);
                                End();
                                Begin(oldFont);
                            }

                            caret.X += ci.Advance * _data.FontScale + _tracking;
                        }
                    }
                }

                last = c;
            }
        }

        public void End()
        {
            if (_data.CharCount > 0)
            {
                _vbo.Update(_buffer);
                _data.FontTexture = _font.Texture;
                _data.FontSpread = _font.PixelSpread;
                _data.ClipSpaceMatrix = _transform.ResolveClipSpaceMatrix();
                _data.DataCullFace = _transform.CullFace;
                _shader.Draw(_data);
                _data.CharCount = 0;
            }

            _font = null;

            HasBegun = false;
        }

        public void DrawString(float2 caret, string text, BitmapFont font)
        {
            Begin(font);
            WriteString(caret, text);
            End();
        }
    }
}
