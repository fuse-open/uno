using Uno.Compiler.ExportTargetInterop;
using Uno.IO;
using Uno;
using OpenGL;

namespace Uno.Graphics.Utils.Cpp
{
    [Require("Source.Include", "@{texture2D:Include}")]
    [Require("Source.Include", "@{Exception:Include}")]
    [Require("Source.Include", "Uno/GLHelper.h")]
    [Require("Source.Include", "uBase/Buffer.h")]
    [Require("Source.Include", "uBase/BufferStream.h")]
    [Require("Source.Include", "uBase/Memory.h")]
    [Require("Source.Include", "uImage/Jpeg.h")]
    [Require("Source.Include", "uImage/Png.h")]
    [Require("Source.Include", "uImage/Texture.h")]
    [Require("Template", "uImage")]
    extern(CPLUSPLUS) static class CppTexture
    {
        public static textureCube LoadCube(string filename, byte[] data)
        @{
            uBase::Auto<uImage::Texture> tex = @{LoadTextureData(string, byte[]):Call($0, $1)};

            if (tex->Faces.Length() == 1)
            {
                U_LOG("Converting 2D texture to cube texture");
                tex = tex->Convert2DToCube();
            }

            @{GLTextureInfo} info;
            GLuint handle = @{CreateGLTexture(TexturePtr, bool, ref GLTextureInfo):Call(tex, true, &info)};

            if (info.GLTarget != GL_TEXTURE_CUBE_MAP)
                U_THROW_IOE("Invalid cube map");

            return @{textureCube(GLTextureHandle,int,int,Format):New(handle, info.Width, info.MipCount, @{Format.Unknown})};
        @}

        public static texture2D Load2DJpeg(byte[] bytes)
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

                @{GLTextureInfo} info;
                GLuint handle = @{CreateGLTexture(TexturePtr, bool, ref GLTextureInfo):Call(tex, false, &info)};

                return @{texture2D(GLTextureHandle,int2,int,Format):New(handle, @{int2(int,int):New(originalWidth, originalHeight)}, info.MipCount, @{Format.Unknown})};
            }
            catch (const uBase::Exception &e)
            {
                const auto temp = e.GetMessage();
                U_THROW(@{Exception(string):New(uString::Utf8(temp.Ptr(), temp.Length()))});
            }
        @}

        public static texture2D Load2DPng(byte[] bytes)
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

                @{GLTextureInfo} info;
                GLuint handle = @{CreateGLTexture(TexturePtr, bool, ref GLTextureInfo):Call(tex, false, &info)};

                return @{texture2D(GLTextureHandle,int2,int,Format):New(handle, @{int2(int,int):New(originalWidth, originalHeight)}, info.MipCount, @{Format.Unknown})};
            }
            catch (const uBase::Exception &e)
            {
                const auto temp = e.GetMessage();
                U_THROW(@{Exception(string):New(uString::Utf8(temp.Ptr(), temp.Length()))});
            }
        @}

        [TargetSpecificType]
        [Set("TypeName", "::uImage::Texture*")]
        [Set("Include", "uImage/Texture.h")]
        struct TexturePtr
        {
        }

        [TargetSpecificType]
        [Set("TypeName", "::uImage::Format")]
        [Set("Include", "uImage/Format.h")]
        struct NativeFormat
        {
        }

        struct GLTextureInfo
        {
            uint GLTarget;
            int Width;
            int Height;
            int Depth;
            int MipCount;
        }

        [Require("Source.Include", "uBase/Path.h")]
        static TexturePtr LoadTextureData(string filename, byte[] data)
        @{
            uCString temp($0);
            uBase::String fnUpper = uBase::String(temp.Ptr, (int)temp.Length).ToUpper();
            uBase::BufferPtr buffer($1->Ptr(), $1->Length(), false);
            uBase::BufferStream stream(&buffer, true, false);
            uBase::Auto<uImage::ImageReader> ir;

            if (fnUpper.EndsWith(".PNG"))
                ir = uImage::Png::CreateReader(&stream);
            else if (fnUpper.EndsWith(".JPG") || fnUpper.EndsWith(".JPEG"))
                ir = uImage::Jpeg::CreateReader(&stream);
            else
                U_THROW_IOE("Unsupported texture extension");

            uBase::Auto<uImage::Bitmap> bmp = ir->ReadBitmap();
            return uImage::Texture::Create(bmp);
        @}

        static uint CreateGLTexture(TexturePtr texData, bool generateMips, ref GLTextureInfo outInfo)
        @{
            GLuint texHandle;
            glGenTextures(1, &texHandle);

            int width = texData->Faces[0].MipLevels[0]->GetWidth();
            int height = texData->Faces[0].MipLevels[0]->GetHeight();
            int mipCount = texData->Faces[0].MipLevels.Length();

            GLenum texTarget =
                texData->Type == uImage::TextureTypeCube ?
                    GL_TEXTURE_CUBE_MAP :
                    GL_TEXTURE_2D;

            glBindTexture(texTarget, texHandle);
            glPixelStorei(GL_UNPACK_ALIGNMENT, 1);
            glPixelStorei(GL_PACK_ALIGNMENT, 1);

            for (int i = 0; i < texData->Faces.Length(); i++)
            {
                GLenum texFace =
                    texTarget == GL_TEXTURE_CUBE_MAP ?
                        GL_TEXTURE_CUBE_MAP_POSITIVE_X + i :
                        GL_TEXTURE_2D;

                for (int j = 0; j < texData->Faces[i].MipLevels.Length(); j++)
                {
                    uImage::Image* mip = texData->Faces[i].MipLevels[j];
                    uBase::Auto<uImage::Bitmap> bmp = mip->ToBitmap();

                    GLenum glFormat, glType;
                    if (!@{TryGetGLFormat(NativeFormat, ref uint, ref uint):Call(bmp->GetFormat(), &glFormat, &glType)})
                        U_THROW_IOE("Unsupported texture format");

                    glTexImage2D(texFace, j, glFormat, bmp->GetWidth(), bmp->GetHeight(), 0, glFormat, glType, bmp->GetPtr());
                }
            }

            if (generateMips)
            {
                glGenerateMipmap(texTarget);
                GLenum err = glGetError();

                if (err == GL_NO_ERROR)
                {
                    int w = width, h = height;

                    while (w > 1 || h > 1)
                    {
                        w /= 2;
                        h /= 2;
                        mipCount++;
                    }
                }
            }

            if (outInfo)
            {
                outInfo->GLTarget = texTarget;
                outInfo->Width = width;
                outInfo->Height = height;
                outInfo->Depth = 1;
                outInfo->MipCount = mipCount;
            }

            return texHandle;
        @}

        static bool TryGetGLFormat(NativeFormat format, ref uint glFormat, ref uint glType)
        @{
            switch (format)
            {
            case uImage::FormatRGBA_8_8_8_8_UInt_Normalize:
                *glFormat = GL_RGBA;
                *glType = GL_UNSIGNED_BYTE;
                return true;

            case uImage::FormatRGB_8_8_8_UInt_Normalize:
                *glFormat = GL_RGB;
                *glType = GL_UNSIGNED_BYTE;
                return true;

            case uImage::FormatR_8_UInt_Normalize:
            case uImage::FormatL_8_UInt_Normalize:
                *glFormat = GL_LUMINANCE;
                *glType = GL_UNSIGNED_BYTE;
                return true;

            case uImage::FormatRG_8_8_UInt_Normalize:
            case uImage::FormatLA_8_8_UInt_Normalize:
                *glFormat = GL_LUMINANCE_ALPHA;
                *glType = GL_UNSIGNED_BYTE;
                return true;

            default:
                return false;
            }
        @}
    }
}
