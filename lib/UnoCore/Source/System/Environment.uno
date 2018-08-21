using Uno.Compiler.ExportTargetInterop;
using Uno.IO;
using Uno;

namespace System
{
    [extern(DOTNET) DotNetType]
    public static extern(DOTNET) class Environment
    {
        public enum SpecialFolder
        {
            MyDocuments = 5,
            MyMusic = 13,
            DesktopDirectory = 16,
            Templates = 21,
            ApplicationData = 26,
            LocalApplicationData = 28,
            MyPictures = 39,
        }

        public static extern string GetFolderPath(SpecialFolder folder);
    }
}
