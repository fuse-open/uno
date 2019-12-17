using Uno.IO;

namespace Uno.Graphics.Utils
{
    using Impl;

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
            if defined(CPLUSPLUS)
                return CppTexture.Load2D(filename, bytes);
            else if defined(DOTNET)
                return DotNetTexture.Load2D(filename, bytes);
            else
                throw new NotImplementedException();
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

        public static texture2D JpegByteArrayToTexture2D(byte[] arr)
        {
            try
            {
                if defined(CPLUSPLUS)
                    return CppTexture.JpegByteArrayToTexture2D(arr);
                else if defined(DOTNET)
                    return DotNetTexture.JpegByteArrayToTexture2D(arr);
                else
                    throw new NotImplementedException();
            }
            catch (Exception jpegException)
            {
                try
                {
                    if defined(CPLUSPLUS)
                        return CppTexture.PngByteArrayToTexture2D(arr);
                    else if defined(DOTNET)
                        return DotNetTexture.PngByteArrayToTexture2D(arr);
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

        public static texture2D PngByteArrayToTexture2D(byte[] arr)
        {
            try
            {
                if defined(CPLUSPLUS)
                    return CppTexture.PngByteArrayToTexture2D(arr);
                else if defined(DOTNET)
                    return DotNetTexture.PngByteArrayToTexture2D(arr);
                else
                    throw new NotImplementedException();
            }
            catch (Exception pngException)
            {
                try
                {
                    if defined(CPLUSPLUS)
                        return CppTexture.JpegByteArrayToTexture2D(arr);
                    else if defined(DOTNET)
                        return DotNetTexture.JpegByteArrayToTexture2D(arr);
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

        public static texture2D ByteArrayToTexture2DFilename(byte[] arr, string filename)
        {
            filename = filename.ToLower();
            if (filename.EndsWith(".png"))
                return PngByteArrayToTexture2D(arr);
            else if (filename.EndsWith(".jpg") || filename.EndsWith(".jpeg"))
                return JpegByteArrayToTexture2D(arr);
            else
                throw new InvalidContentTypeException(filename);
        }

        public static texture2D ByteArrayToTexture2DContentType(byte[] arr, string contentType)
        {
            if (contentType.IndexOf("image/jpeg") != -1 || contentType.IndexOf("image/jpg") != -1)
                return JpegByteArrayToTexture2D(arr);
            else if (contentType.IndexOf("image/png") != -1)
                return PngByteArrayToTexture2D(arr);
            else if (contentType.IndexOf("application/octet-stream") != -1)
                return JpegByteArrayToTexture2D(arr);
            else if (contentType.IndexOf("binary/octet-stream") != -1)
                return JpegByteArrayToTexture2D(arr);
            else
                throw new InvalidContentTypeException(contentType);
        }
    }

    public class InvalidContentTypeException : Exception
    {
        public InvalidContentTypeException(string reason) : base(reason) { }
    }
}
