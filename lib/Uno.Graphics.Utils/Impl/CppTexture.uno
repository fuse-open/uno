using Uno.Compiler.ExportTargetInterop;
using Uno.IO;
using OpenGL;

namespace Uno.Graphics.Utils.Impl
{
    [extern(CPLUSPLUS) Require("Source.Include", "uBase/Memory.h")]
    [extern(CPLUSPLUS) Require("Source.Include", "XliPlatform/GL.h")]
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
    }
}
