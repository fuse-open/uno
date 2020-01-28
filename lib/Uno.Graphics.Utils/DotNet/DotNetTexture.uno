using OpenGL;
using Uno.Compiler.ExportTargetInterop;
using Uno.IO;
using Uno.Native.Textures;
using Uno.Runtime.InteropServices;

namespace Uno.Graphics.Utils.DotNet
{
    extern(DOTNET)
    static class DotNetTexture
    {
        public static texture2D Load2D(string filename, byte[] data)
        {
            using (var bitmap = new Texture(filename, data))
            {
                if (bitmap.TextureType != TextureType.Texture2D)
                    throw new Exception("Input is not a 2D image");

                GLPixelFormat internalFormat, pixelFormat;
                GLPixelType pixelType;
                Uno.Graphics.Format format;

                switch (bitmap.PixelFormat)
                {
                    case PixelFormat.RGBA_8_8_8_8_UInt_Normalize:
                        internalFormat = GLPixelFormat.Rgba;
                        pixelFormat = GLPixelFormat.Rgba;
                        pixelType = GLPixelType.UnsignedByte;
                        format = Uno.Graphics.Format.RGBA8888;
                        break;

                    case PixelFormat.RGB_8_8_8_UInt_Normalize:
                        internalFormat = GLPixelFormat.Rgb;
                        pixelFormat = GLPixelFormat.Rgb;
                        pixelType = GLPixelType.UnsignedByte;
                        format = Uno.Graphics.Format.Unknown;
                        break;

                    case PixelFormat.LA_8_8_UInt_Normalize:
                        internalFormat = GLPixelFormat.LuminanceAlpha;
                        pixelFormat = GLPixelFormat.LuminanceAlpha;
                        pixelType = GLPixelType.UnsignedByte;
                        format = Uno.Graphics.Format.LA88;
                        break;

                    case PixelFormat.L_8_UInt_Normalize:
                        internalFormat = GLPixelFormat.Luminance;
                        pixelFormat = GLPixelFormat.Luminance;
                        pixelType = GLPixelType.UnsignedByte;
                        format = Uno.Graphics.Format.L8;
                        break;

                    default:
                        throw new Exception("Unhandled PixelFormat: " + bitmap.PixelFormat);
                }

                var textureHandle = GL.CreateTexture();
                GL.BindTexture(GLTextureTarget.Texture2D, textureHandle);
                GL.PixelStore(GLPixelStoreParameter.PackAlignment, 1);
                GL.PixelStore(GLPixelStoreParameter.UnpackAlignment, 1);
                var dataPin = GCHandle.Alloc(bitmap.ReadData(), GCHandleType.Pinned);
                GL.TexImage2D(GLTextureTarget.Texture2D, 0, internalFormat, bitmap.Width, bitmap.Height, 0, pixelFormat, pixelType, dataPin.AddrOfPinnedObject());
                dataPin.Free();
                return new Uno.Graphics.Texture2D(textureHandle,
                                                  int2(bitmap.Width, bitmap.Height),
                                                  1,
                                                  format);
            }
        }

        public static textureCube LoadCube(string filename, byte[] data)
        {
            using (var bitmap = new Texture(filename, data))
            {
                if (bitmap.TextureType == TextureType.TextureCube)
                    bitmap.Convert2DToCube();

                if (bitmap.TextureType != TextureType.TextureCube)
                    throw new Exception("Input is not a Cube image");

                GLPixelFormat internalFormat, pixelFormat;
                GLPixelType pixelType;
                Uno.Graphics.Format format;

                switch (bitmap.PixelFormat)
                {
                    case PixelFormat.RGBA_8_8_8_8_UInt_Normalize:
                        internalFormat = GLPixelFormat.Rgba;
                        pixelFormat = GLPixelFormat.Rgba;
                        pixelType = GLPixelType.UnsignedByte;
                        format = Uno.Graphics.Format.RGBA8888;
                        break;

                    case PixelFormat.RGB_8_8_8_UInt_Normalize:
                        internalFormat = GLPixelFormat.Rgb;
                        pixelFormat = GLPixelFormat.Rgb;
                        pixelType = GLPixelType.UnsignedByte;
                        format = Uno.Graphics.Format.Unknown;
                        break;

                    case PixelFormat.LA_8_8_UInt_Normalize:
                        internalFormat = GLPixelFormat.LuminanceAlpha;
                        pixelFormat = GLPixelFormat.LuminanceAlpha;
                        pixelType = GLPixelType.UnsignedByte;
                        format = Uno.Graphics.Format.LA88;
                        break;

                    case PixelFormat.L_8_UInt_Normalize:
                        internalFormat = GLPixelFormat.Luminance;
                        pixelFormat = GLPixelFormat.Luminance;
                        pixelType = GLPixelType.UnsignedByte;
                        format = Uno.Graphics.Format.L8;
                        break;

                    default:
                        throw new Exception("Unhandled PixelFormat: " + bitmap.PixelFormat);
                }

                var textureHandle = GL.CreateTexture();
                GL.BindTexture(GLTextureTarget.TextureCubeMap, textureHandle);
                GL.PixelStore(GLPixelStoreParameter.PackAlignment, 1);
                GL.PixelStore(GLPixelStoreParameter.UnpackAlignment, 1);

                for (int i = 0; i < 6; i++)
                {
                    var dataPin = GCHandle.Alloc(bitmap.ReadData(i), GCHandleType.Pinned);
                    GL.TexImage2D((GLTextureTarget) (GLTextureTarget.TextureCubeMapPositiveX + i), 0, internalFormat,
                        bitmap.Width, bitmap.Height, 0, pixelFormat, pixelType,
                        dataPin.AddrOfPinnedObject());
                    dataPin.Free();
                }

                return new Uno.Graphics.TextureCube(textureHandle,
                                                    bitmap.Width,
                                                    1,
                                                    format);
            }
        }
    }
}
