using Uno.IO;

namespace Uno.Graphics.Utils
{
    using Cpp;
    using DotNet;
    using Text;

    public static class FontLoader
    {
        public static FontFace LoadFace(BundleFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            if defined(CPLUSPLUS)
                return new CppFontFace(file);
            else if defined(DOTNET)
                using (var s = file.OpenRead())
                    return new DotNetFontFace(s);
            else
                throw new NotImplementedException();
        }

        public static FontFace LoadFace(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if defined(CPLUSPLUS)
                return new CppFontFace(data, 0, data.Length);
            else if defined(DOTNET)
                using (var s = new MemoryStream(data))
                    return new DotNetFontFace(s);
            else
                throw new NotImplementedException();
        }
    }
}
