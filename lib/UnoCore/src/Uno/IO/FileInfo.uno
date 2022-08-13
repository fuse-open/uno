using Uno;

namespace Uno.IO
{
    public class FileInfo : FileSystemInfo
    {
        long _length;


        public FileInfo(string originalPath) : base(originalPath)
        {
        }


        extern(DOTNET) internal override System.IO.FileSystemInfo LoadStatus()
        {
            return new System.IO.FileInfo(_fullPath);
        }


        extern(!DOTNET) internal override FileStatus LoadStatus()
        {
            var status = base.LoadStatus();
            // If we're stat'ing a directory, ignore and return "not exist" status
            if ((status.Attributes & FileAttributes.Directory) != 0)
                return new FileStatus();

            return status;
        }


        public long Length
        {
            get
            {
                if defined(DOTNET)
                    return ((System.IO.FileInfo)Status).Length;
                else
                    return Status.Length;
            }
        }
    }
}
