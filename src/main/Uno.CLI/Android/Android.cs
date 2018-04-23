using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Uno.CLI.System;
using Uno.Configuration;
using Uno.Diagnostics;

namespace Uno.CLI.Android
{
    class Android : Command
    {
        public static string NdkDirectory => UnoConfig.Current.GetFullPath("Android.NDK.Directory", "AndroidNdkDirectory");
        public static string SdkDirectory => UnoConfig.Current.GetFullPath("Android.SDK.Directory", "AndroidSdkDirectory");

        public override string Name => "android";
        public override string Description => "Open Deprecated Android SDK Manager.";
        public override bool IsExperimental => true;

        public override void Help()
        {
            WriteUsage("");
        }

        public override void Execute(IEnumerable<string> args)
        {
            WriteLine("This command is deprecated as Android is moving to using the 'tools/bin/sdkmanager' script. Please update your scripts to use 'uno sdkmanager'");
            if (PlatformDetection.IsWindows)
            {
                var filename = Path.Combine(SdkDirectory, "SDK Manager.exe");

                if (!File.Exists(filename))
                    throw new FileNotFoundException("Android SDK Manager was not found at " + filename.Quote());

                Shell.Open(filename, args.JoinArguments());

                for (int i = 0; i < 50; i++)
                {
                    Thread.Sleep(200);

                    foreach (var p in Process.GetProcesses())
                    {
                        if (p.MainWindowHandle != IntPtr.Zero && p.MainWindowTitle == "Android SDK Manager")
                        {
                            Open.ForceForegroundWindow(p.MainWindowHandle);
                            return;
                        }
                    }
                }
            }
            else
            {
                var filename = Path.Combine(SdkDirectory, "tools", "android");

                if (!File.Exists(filename))
                    throw new FileNotFoundException("Android command was not found at " + filename.Quote());

                Shell.Run(filename, args.JoinArguments());
            }
        }
    }
}
