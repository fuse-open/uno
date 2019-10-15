using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Uno.IO;
using Uno.Logging;
using Timer = System.Timers.Timer;

namespace Uno.Diagnostics
{
    public class Shell : LogObject
    {
        public static readonly Shell Default = new Shell(Log.Default);
        static readonly HashSet<Process> Processes = new HashSet<Process>();
        static readonly object Lock = new object();
        static Timer _timer;
        static bool _timerInited;

        public Shell(Log log)
            : base(log)
        {
        }

        public void Open(string filename, string args = "")
        {
            var command = GetCommand(filename, args, null);

            if (PlatformDetection.IsMac)
                Run("open", command);
            else if (PlatformDetection.IsWindows && string.IsNullOrEmpty(args))
                Run("explorer", command);
            else
            {
                Log.VeryVerbose("open " + command, ConsoleColor.Cyan);

                Process.Start(new ProcessStartInfo
                {
                    FileName = filename,
                    Arguments = args,
                    CreateNoWindow = true,
                    UseShellExecute = true
                });
            }
        }

        public int Run(string filename, string args = "", RunFlags flags = 0, string input = null)
        {
            return Run(filename, args, null, flags, input);
        }

        public int Run(string filename, string args, string workingDir, RunFlags flags = 0, string input = null)
        {
            try
            {
                return Start(filename, args, workingDir, flags, input).Result;
            }
            catch (Exception e)
            {
                Log.Error("Exception while running " + filename.Quote() + ": " + e.Message);
                Log.Trace(e);
                return 0xFF;
            }
        }

        public Task<int> Start(string filename, string args = "", RunFlags flags = 0, string input = null, CancellationToken ct = default(CancellationToken))
        {
            return Start(filename, args, null, flags, input, ct);
        }

        public async Task<int> Start(string filename, string args, string workingDir, RunFlags flags = 0, string input = null, CancellationToken ct = default(CancellationToken))
        {
            var level = flags.HasFlag(RunFlags.Compact)
                ? LogLevel.Verbose
                : LogLevel.Compact;

            var output = new StringBuilder();
            var exitCode = await Start(filename, args, workingDir,
                (s, e) => {
                    if (e.Data == null)
                        return;

                    if (level == LogLevel.Verbose && !Log.IsVerbose)
                        output.AppendLine(e.Data);

                    if (!flags.HasFlag(RunFlags.Colors))
                        Log.WriteLine(level, e.Data);
                    else if (e.Data.Length > 2 && e.Data[0] == '#' && e.Data[1] == '#')
                        Log.WriteLine(e.Data.Substring(2).TrimStart(), ConsoleColor.DarkGreen);
                    else if (e.Data.IndexOf("error:", StringComparison.InvariantCultureIgnoreCase) != -1 ||
                             e.Data.IndexOf(": error", StringComparison.InvariantCultureIgnoreCase) != -1 ||
                             e.Data.IndexOf(": fatal error", StringComparison.InvariantCultureIgnoreCase) != -1 ||
                             e.Data.StartsWith("npm ERR!"))
                        Log.WriteErrorLine(e.Data.Trim(), ConsoleColor.Red);
                    else if (e.Data.IndexOf("warning:", StringComparison.InvariantCultureIgnoreCase) != -1 ||
                             e.Data.IndexOf(": warning", StringComparison.InvariantCultureIgnoreCase) != -1 ||
                             e.Data.StartsWith("npm WARN"))
                        Log.WriteErrorLine(e.Data.Trim(), ConsoleColor.Yellow);
                    else if (e.Data.StartsWith("Downloading ", StringComparison.InvariantCultureIgnoreCase) ||
                             e.Data.StartsWith("Download ", StringComparison.InvariantCultureIgnoreCase))
                        Log.WriteLine(e.Data, ConsoleColor.Blue);
                    else
                        Log.WriteLine(level, e.Data);
                },
                (s, e) => {
                    if (e.Data == null)
                        return;

                    if (!flags.HasFlag(RunFlags.Colors))
                        Log.WriteErrorLine(e.Data);
                    else if (e.Data.IndexOf("error:", StringComparison.InvariantCultureIgnoreCase) != -1)
                        Log.WriteErrorLine(e.Data, ConsoleColor.Red);
                    else
                        Log.WriteErrorLine(e.Data, ConsoleColor.Yellow);
                },
                input, ct).ConfigureAwait(false);

            if (exitCode != 0 && output.Length > 0)
                Log.WriteLine(output.ToString().Trim());

            return exitCode;
        }

        public Task<int> Start(string filename, string args, DataReceivedEventHandler outputReceived, DataReceivedEventHandler errorReceived, string input = null, CancellationToken ct = default(CancellationToken))
        {
            return Start(filename, args, null, outputReceived, errorReceived, input, ct);
        }

        public async Task<int> Start(string filename, string args, string workingDir, DataReceivedEventHandler outputReceived, DataReceivedEventHandler errorReceived, string input = null, CancellationToken ct = default(CancellationToken))
        {
            var proc = CreateProcess(filename, args, workingDir);
            Log.VeryVerbose("Starting " + GetCommand(proc.StartInfo.FileName, proc.StartInfo.Arguments, workingDir), ConsoleColor.Blue);

            proc.OutputDataReceived += outputReceived;
            proc.ErrorDataReceived += errorReceived;

            lock (Processes)
                Processes.Add(proc);

            using (new ProcessRunner(proc, input, ct))
            {
                await Task.Run((Action) proc.WaitForExit).ConfigureAwait(false);

                lock (Processes)
                    Processes.Remove(proc);

                Log.VeryVerbose("> (exit code: " + proc.ExitCode + ")", ConsoleColor.DarkBlue);
                return proc.ExitCode;
            }
        }

        public string GetOutput(string filename, string args = "", RunFlags flags = 0, string input = null)
        {
            int exitCode;
            return GetOutput(filename, args, null, flags, input, out exitCode);
        }

        public string GetOutput(string filename, string args, string workingDir, RunFlags flags = 0, string input = null)
        {
            int exitCode;
            return GetOutput(filename, args, workingDir, flags, input, out exitCode);
        }

        public string GetOutput(string filename, string args, string workingDir, RunFlags flags, string input, out int exitCode)
        {
            var proc = CreateProcess(filename, args, workingDir);
            var list = new List<string>();
            Log.VeryVerbose("Running " + GetCommand(proc.StartInfo.FileName, proc.StartInfo.Arguments, workingDir), ConsoleColor.Blue);

            proc.OutputDataReceived += (s, e) =>
            {
                if (e.Data != null)
                    list.Add(e.Data);
            };

            proc.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null)
                    list.Add(e.Data);
            };

            lock (Processes)
                Processes.Add(proc);

            using (new ProcessRunner(proc, input))
            {
                proc.WaitForExit();

                lock (Processes)
                    Processes.Remove(proc);

                var output = string.Join("\n", list);

                if (proc.ExitCode == 0 || flags.HasFlag(RunFlags.NoThrow))
                {
                    if (!flags.HasFlag(RunFlags.NoOutput))
                        Log.Verbose(output);

                    Log.VeryVerbose("> (exit code: " + proc.ExitCode + ")", ConsoleColor.DarkBlue);
                    exitCode = proc.ExitCode;
                    return output;
                }

                Log.OutWriter.Write(output);
                throw new InvalidOperationException("Process " + proc.ProcessName.Quote() + " exited with code " + proc.ExitCode);
            }
        }

        static string GetCommand(string filename, string args, string workingDir)
        {
            var command = filename.ToRelativePath(workingDir).QuoteSpace();

            if (!string.IsNullOrEmpty(args))
                command += " " + args;
            if (!string.IsNullOrEmpty(workingDir))
                command += " (in " + workingDir.ToRelativePath() + ")";

            return command;
        }

        static Process CreateProcess(string filename, string args, string workingDir)
        {
            InitTimer();
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = filename,
                    Arguments = args,
                    WorkingDirectory = workingDir,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    // make.exe crashes without a valid STDIN handle
                    RedirectStandardInput = true,
                }
            };
        }

        static void InitTimer()
        {
            // If we're on Windows running inside cmd.exe we need to monitor parent processes
            // in order to properly terminate all subprocesses when CTRL+C was pressed.

            // In Uno we can get cases where cmd.exe launches uno.exe, which launches a new cmd.exe which launches a new uno.exe,
            // which again can launch something else. This is what happens when running `uno build android --run`.

            // When pressing CTRL+C at this point, the signal is handled by the first cmd.exe which doesn't kill the entire
            // process tree, leaving dangling processes around unless we detect that the parent process has exited.

            if (PlatformDetection.IsWindows && !_timerInited)
            {
                lock (Lock)
                {
                    if (_timerInited)
                        return;

                    _timerInited = true;
                    var proc = Process.GetCurrentProcess();
                    var parents = proc.GetParents();

                    foreach (var parent in parents)
                    {
                        if (parent.ProcessName == "cmd")
                        {
                            _timer = new Timer(1000);
                            _timer.Elapsed += (s, e) =>
                            {
                                foreach (var p in parents)
                                {
                                    if (p.HasExited)
                                    {
                                        _timer.Stop();
                                        proc.KillTree();
                                        return;
                                    }
                                }
                            };

                            _timer.Start();
                            return;
                        }
                    }
                }
            }
        }

        public static void CleanUp()
        {
            _timer?.Stop();
            KillAll();
        }

        public static void KillAll()
        {
            lock (Processes)
            {
                foreach (var proc in Processes)
                {
                    try
                    {
                        if (!proc.HasExited)
                        {
                            Log.Default.Verbose("Killing process " + proc.MainModule.FileName.Quote() + " (" + proc.Id + ")");
                            proc.KillTree();
                        }
                    }
                    catch
                    {
                        // Swallow exceptions
                    }
                }
            }
        }
    }
}
