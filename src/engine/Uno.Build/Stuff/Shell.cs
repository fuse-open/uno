using System;
using System.Diagnostics;
using System.Text;
using Uno;

namespace Uno.Build.Stuff
{
    public static class Shell
    {
        public static void Chmod(string mode, string directory)
        {
            Run("chmod", new[] {"-R", mode, directory});
        }

        public static void Symlink(string dst, string src)
        {
            Run("ln", new[] {"-sf", dst, src});
        }

        public static bool Readlink(string path)
        {
            return Run("readlink", new[] {path}, acceptCode: 1) == 0;
        }

        public static void Untar(string file, string directory)
        {
            Run("tar", new[] {"-xzf", file, "-C", directory});
        }

        public static void Unzip(string file, string directory)
        {
            // Exit code=1 means a warning was given, no problem
            Run("unzip", new[] {file, "-d", directory}, acceptCode: 1);
        }

        public static void Zip(string directory, string file)
        {
            Run("zip", new[] {"-r", "--symlinks", file, "."}, workingDir: directory);
        }

        public static int Run(string command, string[] arguments, int acceptCode = 0, string workingDir = "")
        {
            for (int i = 0; i < arguments.Length; i++)
                if (arguments[i].IndexOf(' ') != -1)
                    arguments[i] = "\"" + arguments[i] + "\"";

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = string.Join(" ", arguments),
                    WorkingDirectory = workingDir,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            var sb = new StringBuilder();
            var mutex = new Object();
            proc.OutputDataReceived += (sender, args) => { lock (mutex) { sb.AppendLine(args.Data); } };
            proc.ErrorDataReceived += (sender, args) => { lock (mutex) { sb.AppendLine(args.Data); } };
            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            // Timeout after 10 minutes
            if (!proc.WaitForExit(1000 * 60 * 10))
            {
                proc.Kill();
                Log.WriteLine("{0}", sb);
                throw new InvalidOperationException(command.Quote() + " timed out after 10 minutes");
            }

            if (proc.ExitCode != 0 && proc.ExitCode != acceptCode)
            {
                // Print log on error
                Log.WriteLine("{0}", sb);
                throw new InvalidOperationException(command.Quote() + " failed with exit code " + proc.ExitCode);
            }

            return proc.ExitCode;
        }
    }
}
