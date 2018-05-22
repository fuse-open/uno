using Uno;

namespace Uno.IO
{
    extern(!(MSVC || UNIX)) internal static class FileStatusHelpers
    {
        public static FileStatus GetFileStatus(string path)
        {
            throw new NotImplementedException();
        }
    }
}
