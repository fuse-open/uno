using Uno.IO;

namespace Uno.Graphics.Utils
{
    using Cpp;
    using DotNet;

    public static class TextureLoader
    {
        public static Texture2D Load2D(BundleFile file)
        {
            return Load2D(file.Name, file.ReadAllBytes());
        }

        public static Texture2D Load2D(string filename)
        {
            return Load2D(filename, File.ReadAllBytes(filename));
        }

        public static Texture2D Load2D(string filename, byte[] bytes)
        {
            filename = filename.ToLower();
            if (filename.EndsWith(".png"))
                return Load2DPng(bytes);
            else
                return Load2DJpeg(bytes);
        }

        public static TextureCube LoadCube(BundleFile file)
        {
            return LoadCube(file.Name, file.ReadAllBytes());
        }

        public static TextureCube LoadCube(string filename)
        {
            return LoadCube(filename, File.ReadAllBytes(filename));
        }

        public static TextureCube LoadCube(string filename, byte[] bytes)
        {
            if defined(CPLUSPLUS)
                return CppTexture.LoadCube(filename, bytes);
            else if defined(DOTNET)
                return DotNetTexture.LoadCube(filename, bytes);
            else
                throw new NotImplementedException();
        }

        internal static texture2D Load2DJpeg(byte[] bytes)
        {
            try
            {
                if defined(CPLUSPLUS)
                    return CppTexture.Load2DJpeg(bytes);
                else if defined(DOTNET)
                    return DotNetTexture.Load2D("image.jpg", bytes);
                else
                    throw new NotImplementedException();
            }
            catch (Exception jpegException)
            {
                try
                {
                    if defined(CPLUSPLUS)
                        return CppTexture.Load2DPng(bytes);
                    else if defined(DOTNET)
                        return DotNetTexture.Load2D("image.png", bytes);
                    else
                        throw new NotImplementedException();
                } 
                catch (Exception pngException)
                {
                    // both threw, but since the user asked for JPEG, answer with the JPEG-error
                    throw jpegException;
                }
            }
        }

        internal static texture2D Load2DPng(byte[] bytes)
        {
            try
            {
                if defined(CPLUSPLUS)
                    return CppTexture.Load2DPng(bytes);
                else if defined(DOTNET)
                    return DotNetTexture.Load2D("image.png", bytes);
                else
                    throw new NotImplementedException();
            }
            catch (Exception pngException)
            {
                try
                {
                    if defined(CPLUSPLUS)
                        return CppTexture.Load2DJpeg(bytes);
                    else if defined(DOTNET)
                        return DotNetTexture.Load2D("image.jpg", bytes);
                    else
                        throw new NotImplementedException();
                }
                catch (Exception jpegException)
                {
                    // both threw, but since the user asked for PNG, answer with the PNG-error
                    throw pngException;
                }
            }
        }
    }
}
