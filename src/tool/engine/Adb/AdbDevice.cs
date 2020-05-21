using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Uno.CLI;
using Uno.Diagnostics;

namespace Uno.Build.Adb
{
    public class AdbDevice
    {
        readonly Shell _shell;
        readonly string _adb;
        readonly string _id;

        internal AdbDevice(Shell shell, string adb, string id)
        {
            _shell = shell;
            _adb = adb;
            _id = id;
        }

        public void Install(string package, string filename)
        {
            string output;
            if (TryInstall(package, filename, out output))
                return;

            // Print failure information, i.e. - Failure [INSTALL_FAILED_OLDER_SDK]
            foreach (var line in output.Grep("Failure"))
                _shell.Log.WriteLine(line);

            throw new InvalidOperationException("Failed to install APK on device " + _id);
        }

        bool TryInstall(string package, string filename, out string output)
        {
            output = _shell.GetOutput(_adb, "-s " + _id + " install -r " + filename.QuoteSpace());
            if (!output.Contains("\nFailure"))
                return true;

            var isInstalled = !string.IsNullOrWhiteSpace(_shell.GetOutput(_adb, "-s " + _id + " shell pm path " + package));
            if (!isInstalled)
                return false;

            // ADB might refuse to replace app when signed using a different key, so uninstall and try again
            output = _shell.GetOutput(_adb, "-s " + _id + " uninstall " + package);
            if (output.Contains("\nFailure"))
                return false;

            output = _shell.GetOutput(_adb, "-s " + _id + " install -r " + filename.QuoteSpace());
            return !output.Contains("\nFailure");
        }

        public void Start(string package, string activity)
        {
            _shell.GetOutput(_adb, "-s " + _id + " shell am start " +
                                   "-a android.intent.action.MAIN " +
                                   "-c [android.intent.category.LAUNCHER] " +
                                   "-n " + package + "/" + package + "." + activity);
        }

        public class PsEntry
        {
            // Assume PID is the second entry and process name is the last
            private static readonly Regex PsRegex =
                new Regex(@"^(?<USER>[^\s]+)\s+(?<PID>\d+).+?\s+(?<NAME>[^\s]+)\r?$",
                    RegexOptions.Multiline | RegexOptions.Compiled);

            public static IEnumerable<PsEntry> Parse(string lines)
            {
                foreach (var match in PsRegex.Matches(lines).Cast<Match>())
                {
                    yield return new PsEntry(int.Parse(match.Groups["PID"].Value), match.Groups["NAME"].Value);
                }
            }

            private PsEntry(int pid, string name)
            {
                Pid = pid;
                Name = name;
            }

            public int Pid { get; }
            public string Name { get; }
        }

        public IEnumerable<PsEntry> Ps(string package)
        {
            return PsEntry.Parse(_shell.GetOutput(_adb, "-s " + _id + " shell ps", RunFlags.NoOutput)).Where(x => x.Name == package);
        }

        public bool IsRunning(string package)
        {
            return Ps(package).Any();
        }

        public int GetProcessId(string package)
        {
            foreach (var psEntry in Ps(package))
                return psEntry.Pid;

            throw new ArgumentException("Process " + package + " was not found");
        }

        public void ClearLog()
        {
            _shell.GetOutput(_adb, "-s " + _id + " logcat -c");
        }

        public string Logcat(string package, string args = null)
        {
            var log = new List<string>();
            var pid = GetProcessId(package).ToString(CultureInfo.InvariantCulture);
            _shell.Log.Verbose("PID: " + pid);
            _shell.Start(_adb, "-s " + _id + " logcat " + args,
                (s, e) => { if (e.Data != null && e.Data.Contains(pid)) _shell.Log.WriteLine(e.Data); log.Add(e.Data); },
                (s, e) => _shell.Log.WriteErrorLine(e.Data ?? string.Format ("Unknown Logcat Error: raised from 'adb -s {0} logcat {1}'", _id, args))).Wait(); 

            return string.Join("\n", log);
        }

        public override string ToString()
        {
            return _id;
        }
    }
}
