using Uno.Compiler.ExportTargetInterop;
using Uno.IO;
using OpenGL;

namespace Uno.Graphics.Utils.Impl
{
    [extern(CPLUSPLUS) Require("Source.Include", "@{Uno.Graphics.Texture2D:Include}")]
    [extern(CPLUSPLUS) Require("Source.Include", "@{Uno.Exception:Include}")]
    [extern(CPLUSPLUS) Require("Source.Include", "uBase/Buffer.h")]
    [extern(CPLUSPLUS) Require("Source.Include", "uBase/BufferStream.h")]
    [extern(CPLUSPLUS) Require("Source.Include", "uBase/Memory.h")]
    [extern(CPLUSPLUS) Require("Source.Include", "XliPlatform/GL.h")]
    [extern(CPLUSPLUS) Require("Source.Include", "uImage/Jpeg.h")]
    [extern(CPLUSPLUS) Require("Source.Include", "uImage/Png.h")]
    [extern(CPLUSPLUS) Require("Source.Include", "uImage/Texture.h")]
    [extern(CPLUSPLUS) Require("Source.Include", "Uno/Support.h")]
    extern(CPLUSPLUS) static class CppTexture
    {
        public static texture2D Load2D(string filename, byte[] data)
        @{
            uBase::Auto<uImage::Texture> tex = uLoadXliTexture(uStringToXliString($0), $1);

            uGLTextureInfo info;
            GLuint handle = uCreateGLTexture(tex, false, &info);

            if (info.GLTarget != GL_TEXTURE_2D)
                throw uBase::Exception("Invalid 2D texture");

            return @{texture2D(GLTextureHandle,int2,int,Format):New(handle, @{int2(int,int):New(info.Width, info.Height)}, info.MipCount, @{Format.Unknown})};
        @}

        public static textureCube LoadCube(string filename, byte[] data)
        @{
            uBase::Auto<uImage::Texture> tex = uLoadXliTexture(uStringToXliString($0), $1);

            if (tex->Faces.Length() == 1)
            {
                U_LOG("Converting 2D texture to cube texture");
                tex = tex->Convert2DToCube();
            }

            uGLTextureInfo info;
            GLuint handle = uCreateGLTexture(tex, true, &info);

            if (info.GLTarget != GL_TEXTURE_CUBE_MAP)
                throw uBase::Exception("Invalid cube map");

            return @{textureCube(GLTextureHandle,int,int,Format):New(handle, info.Width, info.MipCount, @{Format.Unknown})};
        @}

        public static texture2D JpegByteArrayToTexture2D(byte[] arr)
        @{
            try
            {
                uBase::Auto<uBase::BufferPtr> bp = new uBase::BufferPtr($0->Ptr(), $0->Length(), false);
                uBase::Auto<uBase::BufferStream> bs = new uBase::BufferStream(bp, true, false);
                uBase::Auto<uImage::ImageReader> ir = uImage::Jpeg::CreateReader(bs);
                uBase::Auto<uImage::Bitmap> bmp = ir->ReadBitmap();
                int originalWidth = bmp->GetWidth(), originalHeight = bmp->GetHeight();

                int maxTextureSize;
                glGetIntegerv(GL_MAX_TEXTURE_SIZE, &maxTextureSize);
                while (bmp->GetWidth() > maxTextureSize ||
                        bmp->GetHeight() > maxTextureSize)
                {
                    bmp = bmp->DownSample2x2();
                }

                uBase::Auto<uImage::Texture> tex = uImage::Texture::Create(bmp);
                uGLTextureInfo info;

                GLuint handle = uCreateGLTexture(tex, false, &info);

                return @{Uno.Graphics.Texture2D(OpenGL.GLTextureHandle,int2,int,Uno.Graphics.Format):New(handle, @{int2(int,int):New(originalWidth, originalHeight)}, info.MipCount, @{Uno.Graphics.Format.Unknown})};
            }
            catch (const uBase::Exception &e)
            {
                U_THROW(@{Uno.Exception(string):New(uStringFromXliString(e.GetMessage()))});
            }
        @}

        public static texture2D PngByteArrayToTexture2D(byte[] arr)
        @{
            try
            {
                uBase::Auto<uBase::BufferPtr> bp = new uBase::BufferPtr($0->Ptr(), $0->Length(), false);
                uBase::Auto<uBase::BufferStream> bs = new uBase::BufferStream(bp, true, false);
                uBase::Auto<uImage::ImageReader> ir = uImage::Png::CreateReader(bs);
                uBase::Auto<uImage::Bitmap> bmp = ir->ReadBitmap();
                int originalWidth = bmp->GetWidth(), originalHeight = bmp->GetHeight();

                int maxTextureSize;
                glGetIntegerv(GL_MAX_TEXTURE_SIZE, &maxTextureSize);
                while (bmp->GetWidth() > maxTextureSize ||
                        bmp->GetHeight() > maxTextureSize)
                {
                    bmp = bmp->DownSample2x2();
                }

                uBase::Auto<uImage::Texture> tex = uImage::Texture::Create(bmp);
                uGLTextureInfo info;
                GLuint handle = uCreateGLTexture(tex, false, &info);

                return @{Uno.Graphics.Texture2D(OpenGL.GLTextureHandle,int2,int,Uno.Graphics.Format):New(handle, @{int2(int,int):New(originalWidth, originalHeight)}, info.MipCount, @{Uno.Graphics.Format.Unknown})};
            }
            catch (const uBase::Exception &e)
            {
                U_THROW(@{Uno.Exception(string):New(uStringFromXliString(e.GetMessage()))});
            }
        @}
    }
}
