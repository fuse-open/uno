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
    }
}
