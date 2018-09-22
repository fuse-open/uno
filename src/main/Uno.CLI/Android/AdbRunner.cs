using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.Configuration;
using Uno.Diagnostics;

namespace Uno.CLI.Android
{
    class AdbRunner
    {
        static string SdkDirectory => UnoConfig.Current.GetFullPath("Android.SDK.Directory", "AndroidSdkDirectory");

        readonly Shell _shell;
        readonly string _adb = Path.Combine(SdkDirectory,
            "platform-tools", PlatformDetection.IsWindows ? "adb.exe" : "adb");

        public AdbRunner(Shell shell)
        {
            _shell = shell;

            if (!File.Exists(_adb))
                throw new FileNotFoundException("Android 'adb' was not found at " + _adb.Quote());
        }

        public int Run(string args)
        {
            return _shell.Run(_adb, args);
        }

        public List<AdbDevice> GetDevices()
        {
            var devices = new List<AdbDevice>();
            var output = _shell.GetOutput(_adb, "devices");

            foreach (var line in output.Split('\n').Skip(1))
            {
                var parts = line.Cut();
                if (parts.Count == 2 && parts[1] == "device")
                    devices.Add(new AdbDevice(_shell, _adb, parts[0]));
            }

            return devices;
        }
    }
}
