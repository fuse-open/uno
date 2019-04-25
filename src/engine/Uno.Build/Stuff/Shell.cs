using System;
using System.Diagnostics;
using System.Text;
using Uno;
using Uno.Logging;

namespace Uno.Build.Stuff
{
    public static class Shell
    {
        public static void Chmod(Log log, string mode, string directory)
        {
            Run(log, "chmod", new[] {"-R", mode, directory});
        }

        public static void Symlink(Log log, string dst, string src)
        {
            Run(log, "ln", new[] {"-sf", dst, src});
        }

        public static bool Readlink(Log log, string path)
        {
            return Run(log, "readlink", new[] {path}, acceptCode: 1) == 0;
        }

        public static void Untar(Log log, string file, string directory)
        {
            Run(log, "tar", new[] {"-xzf", file, "-C", directory});
        }

        public static void Unzip(Log log, string file, string directory)
        {
            // Exit code=1 means a warning was given, no problem
            Run(log, "unzip", new[] {file, "-d", directory}, acceptCode: 1);
        }

        public static void Zip(Log log, string directory, string file)
        {
            Run(log, "zip", new[] {"-r", "--symlinks", file, "."}, workingDir: directory);
        }

        static int Run(Log log, string command, string[] arguments, int acceptCode = 0, string workingDir = "")
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
                log.WriteLine(sb);
                throw new InvalidOperationException(command.Quote() + " timed out after 10 minutes");
            }

            if (proc.ExitCode != 0 && proc.ExitCode != acceptCode)
            {
                // Print log on error
                log.WriteLine(sb);
                throw new InvalidOperationException(command.Quote() + " failed with exit code " + proc.ExitCode);
            }

            return proc.ExitCode;
        }
    }
}
