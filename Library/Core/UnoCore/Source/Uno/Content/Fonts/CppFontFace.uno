using Uno.Compiler.ExportTargetInterop;
using Uno.IO;

namespace Uno.Content.Fonts
{
    [TargetSpecificType]
    [Set("TypeName", "uImage::FontFace*")]
    [Set("ForwardDeclaration", "namespace uImage { class FontFace; }")]
    [Require("Header.Include", "uImage/FontFace.h")]
    extern(CPLUSPLUS)
    struct CppFontFaceHandle
    {
    }

    [Require("Source.Include", "uBase/BufferStream.h")]
    [Require("Source.Include", "uBase/Bundle.h")]
    [Require("Source.Include", "Uno/Support.h")]
    extern(CPLUSPLUS)
    class CppFontFace : FontFace
    {
        static CppFontFaceHandle LoadFontFaceHandle(string filename)
        @{
            uBase::Auto<uBase::Stream> f = uBase::Bundle->OpenFile(uStringToXliString($0));
            return uImage::FontFace::Load(f);
        @}

        static CppFontFaceHandle LoadFontFaceHandle(byte []data, int offset, int length)
        @{
            uBase::BufferStream stream(new uBase::BufferPtr((char*)$0->Ptr() + $1, $2, false), true, false);
            return uImage::FontFace::Load(&stream);
        @}

        CppFontFaceHandle _handle;

        public CppFontFace(BundleFile file)
        {
            _handle = LoadFontFaceHandle(file.BundlePath);
        }

        public CppFontFace(byte []data, int offset, int length)
        {
            _handle = LoadFontFaceHandle(data, offset, length);
        }

        public override void Dispose()
        @{
            @{$$._handle}->Release();
            @{$$._handle} = 0;
        @}

        public override string FamilyName
        {
            get @{ return uStringFromXliString(@{$$._handle}->GetFamilyName()); @}
        }

        public override string StyleName
        {
            get @{ return uStringFromXliString(@{$$._handle}->GetStyleName()); @}
        }

        public override float GetAscender(float size)
        @{
            return @{$$._handle}->GetAscender($0);
        @}

        public override float GetDescender(float size)
        @{
            return @{$$._handle}->GetDescender($0);
        @}

        public override float GetLineHeight(float size)
        @{
            return @{$$._handle}->GetLineHeight($0);
        @}

        public override bool ContainsGlyph(float size, char glyph)
        @{
            return @{$$._handle}->ContainsGlyph($0, $1);
        @}

        [Require("Source.Include", "@{int2:Include}")]
        [Require("Source.Include", "@{float2:Include}")]
        [Require("Source.Include", "@{Uno.Content.Images.Bitmap:Include}")]
        [Require("Source.Include", "@{Uno.Content.Fonts.RenderedGlyph:Include}")]
        [Require("Source.Include", "uImage/Bitmap.h")]
        public override RenderedGlyph RenderGlyph(float size, char glyph)
        @{
            uBase::Vector2 advance, bearing;
            uBase::Auto<uImage::Bitmap> bitmap = @{$$._handle}->RenderGlyph($0, $1, uImage::FontRenderModeNormal, &advance, &bearing);

            @{Uno.Buffer} resultBuffer = uBufferFromXliDataAccessor(bitmap);
            @{Uno.Content.Images.Bitmap} resultBitmap = @{Uno.Content.Images.Bitmap(int2,Uno.Graphics.Format,Uno.Buffer):New(@{int2(int,int):New(bitmap->GetWidth(), bitmap->GetHeight())}, @{Uno.Graphics.Format.L8}, resultBuffer)};

            return @{Uno.Content.Fonts.RenderedGlyph(float2,float2,Uno.Content.Images.Bitmap):New(@{float2(float,float):New(advance.X, advance.Y)}, @{float2(float,float):New(bearing.X, bearing.Y)}, resultBitmap)};
        @}

        [Require("Source.Include", "@{float2:Include}")]
        public override bool TryGetKerning(float size, char left, char right, out float2 result)
        @{
            uBase::Vector2 kerning;
            if (@{$$._handle}->TryGetKerning($0, $1, $2, &kerning))
            {
                $3->X = kerning.X;
                $3->Y = kerning.Y;
                return true;
            }

            $3->X = $3->Y = 0;
            return false;
        @}
    }
}