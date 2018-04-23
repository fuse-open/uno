using System;
using System.IO;
using Uno.Compiler;
using Uno.Configuration;
using Uno.Diagnostics;
using Uno.IO;
using Uno.Logging;

namespace Uno.Build.JavaScript
{
    class NPM : DiskObject
    {
        static readonly string _npm = UnoConfig.Current.GetFullPath("Tools.NPM", false) ?? "npm";
        readonly Shell _shell;

        public NPM(Log log)
            : base(log)
        {
            _shell = new Shell(log);
        }

        public static bool NeedsInstall(SourcePackage upk)
        {
            var packageFile = Path.Combine(upk.SourceDirectory, "package.json");
            if (!File.Exists(packageFile))
                return false;

            var timestampFile = Path.Combine(upk.CacheDirectory, "npm-install");
            if (File.Exists(timestampFile) &&
                File.GetLastWriteTime(timestampFile) >= File.GetLastWriteTime(packageFile))
                return false;

            return true;
        }

        public void Install(SourcePackage upk)
        {
            Log.WriteLine("Installing NPM packages for " + upk.Name, ConsoleColor.Blue);
            Run("install", upk.SourceDirectory);
            Disk.CreateFile(Path.Combine(upk.CacheDirectory, "npm-install")).Dispose();
        }

        void Run(string command, string workingDirectory)
        {
            var filename = _npm;
            var args = command;

            // NPM doesn't work properly unless run via cmd.exe on Windows
            if (PlatformDetection.IsWindows)
            {
                filename = "cmd.exe";
                args = "/C " + _npm.QuoteSpace() + " " + command;
            }

            // Don't print warnings unless --verbose
            if (!Log.IsVerbose)
                args += " --loglevel error";

            var result = _shell.Run(filename, args, workingDirectory, RunFlags.Colors);

            if (result != 0)
                throw new InvalidOperationException("'npm " + command + "' returned with error (" + result + ")");
        }
    }
}
