using Uno;
using Uno.IO;

namespace Mono
{
    static class Console
    {
        public static TextWriter Out;

        public static void Write(params object[] p)
        {
        }

        public static void WriteLine(params object[] p)
        {
        }
    }

    class ApplicationException : Exception
    {
        public ApplicationException()
        {
        }

        public ApplicationException(string msg)
            : base(msg)
        {
        }
    }
}
