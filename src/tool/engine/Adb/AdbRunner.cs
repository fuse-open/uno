using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.CLI;
using Uno.Configuration;
using Uno.Diagnostics;

namespace Uno.Build.Adb
{
    public class AdbRunner
    {
        static string SdkDirectory => UnoConfig.Current.GetFullPath("Android.SDK", "Android.SDK.Directory");
        static bool IgnoreNetworkDevices => UnoConfig.Current.GetBool("Android.IgnoreNetworkDevices");

        readonly Shell _shell;
        public readonly string Location = Path.Combine(SdkDirectory, "platform-tools",
                                                       PlatformDetection.IsWindows ? "adb.exe" : "adb");

        public AdbRunner(Shell shell)
        {
            _shell = shell;

            if (!File.Exists(Location))
                throw new FileNotFoundException("Android 'adb' was not found at " + Location.Quote());
        }

        public int Run(string args)
        {
            return _shell.Run(Location, args);
        }

        public List<AdbDevice> GetDevices()
        {
            var devices = new List<AdbDevice>();
            var output = _shell.GetOutput(Location, "devices");

            foreach (var line in output.Split('\n').Skip(1))
            {
                var parts = line.Cut();
                if (parts.Count == 2 && parts[1] == "device" &&
                        (!IgnoreNetworkDevices || !IsIpAndPort(parts[0])))
                    devices.Add(new AdbDevice(_shell, Location, parts[0]));
            }

            return devices;
        }

        // Ignore devices that look like <IP>:<port> (when Android.IgnoreNetworkDevices is enabled).
        static bool IsIpAndPort(string arg)
        {
            if (arg.IndexOf(':') == -1)
                return false;

            int port;
            var parts = arg.Split(':');
            return parts.Length == 2 && int.TryParse(parts[1], out port);
        }
    }
}
