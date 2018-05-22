using Uno;

namespace Uno.IO
{
    public class DirectoryInfo : FileSystemInfo
    {
        public DirectoryInfo(string originalPath) : base(originalPath)
        {
        }


        extern(DOTNET) internal override System.IO.FileSystemInfo LoadStatus()
        {
            return new System.IO.DirectoryInfo(_fullPath);
        }


        extern(!DOTNET) internal override FileStatus LoadStatus()
        {
            var status = base.LoadStatus();
            // If we're stat'ing a file, ignore and return "not exist" status
            if ((status.Attributes & FileAttributes.Directory) == 0)
                return new FileStatus();

            return status;
        }
    }
}
