using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Mono.Options;
using Uno.Build.Adb;
using Uno.Configuration;
using Uno.Diagnostics;
using Timer = System.Timers.Timer;

namespace Uno.CLI.Android
{
    class LaunchApk : Command
    {
        static string NdkDirectory => UnoConfig.Current.GetFullPath("Android.NDK", "Android.NDK.Directory");

        const int Timeout = 7;

        public override string Name => "launch-apk";
        public override string Description => "Deploy and start APK on a connected device";
        public override bool IsExperimental => true;

        public override void Help()
        {
            WriteUsage("[options] <filename>");

            WriteHead("Available options");
            WriteRow("-a, --activity=NAME",  "Android activity name");
            WriteRow("-p, --package=NAME",   "Java package name");
            WriteRow("-s, --sym-dir=PATH",   "Symbol directory, for stack traces", true);
            WriteRow("-i, --install",        "Install only, then exit");
            WriteRow("-C, --no-clear",       "Don't clear logcat logs before launch");
            WriteRow("-L, --no-log",         "Don't run logcat, just launch");
        }

        public override void Execute(IEnumerable<string> args)
        {
            string activity = null
                , package = null
                , symbolDir = null;
            bool installOnly = false
                , clearLog = true
                , runLogcat = true;
            var filename = new OptionSet {
                    {"a=|activity=", value => activity = value},
                    {"p=|package=", value => package = value},
                    {"s=|sym-dir=", value => symbolDir = value},
                    {"i|install", value => installOnly = true},
                    {"C|no-clear", value => clearLog = false},
                    {"L|no-log", value => runLogcat = false},
                }.Parse(args)
                .Path();

            if (string.IsNullOrEmpty(activity))
                throw new ArgumentException("--activity was not given");
            if (string.IsNullOrEmpty(package))
                throw new ArgumentException("--package was not given");
            if (!File.Exists(filename))
                throw new FileNotFoundException("An APK was not found at " + filename.Quote());

            var adb = new AdbRunner(Shell);
            var devices = adb.GetDevices();

            for (int i = Timeout; devices.Count == 0; i--)
            {
                if (i == 0)
                    throw new NotSupportedException("No Android device connected. Please make sure you have developer mode enabled, and any required drivers installed.");

                Log.Message("Waiting for device (" + i + "..)");
                Thread.Sleep(1000);

                devices = adb.GetDevices();
            }

            Log.Message("Installing APK on " + devices.Count + " device".Plural(devices));

            foreach (var dev in devices)
                dev.Install(package, filename);

            if (installOnly)
                return;

            var masterDevice = devices[0];
            Log.Message("Launching activity " + activity.Quote());

            if (runLogcat && clearLog)
                masterDevice.ClearLog();

            foreach (var dev in devices)
                dev.Start(package, activity);

            if (!runLogcat)
                return;

            // Wait for process to start
            using (var timer = new Timer(1000))
            {
                timer.Elapsed += (s, e) => {
                    timer.Stop();
                    Shell.KillAll();
                    
                    try
                    {
                        // -d: Dump log and exit
                        NdkStack(masterDevice.Logcat(package, "-d"), symbolDir);
                    }
                    catch (Exception ex)
                    {
                        Log.Trace(ex);
                    }

                    Log.Error("Process " + package + " did not start.");
                    Environment.Exit(1);
                };

                timer.Start();
                while (!masterDevice.IsRunning(package))
                    Thread.Yield();
                timer.Stop();
            }

            using (var timer = new Timer(1000))
            {
                timer.Elapsed += (s, e) => {
                    try
                    {
                        if (!masterDevice.IsRunning(package))
                        {
                            timer.Stop();
                            Shell.KillAll();
                            Log.Message("Process " + package + " terminated.");
                            Environment.Exit(0);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Trace(ex);
                        Log.Error("Unhandled exception: " + ex.Message + " (pass --trace for stack trace)");
                        Environment.Exit(1);
                    }
                };

                timer.Start();
                Log.Message("Running logcat on " + masterDevice.Quote());
                // Filter logcat through ndk-stack for stack traces, including C++ line number info
                NdkStack(masterDevice.Logcat(package), symbolDir);
            }
        }

        static void NdkStack(string input, string symbolDir)
        {
            var filename = Path.Combine(NdkDirectory, "ndk-stack");

            if (PlatformDetection.IsWindows)
                filename = File.Exists(filename + ".exe")
                    ? filename + ".exe"
                    : filename + ".cmd";

            if (!File.Exists(filename))
                throw new FileNotFoundException("ndk-stack was not found at " + filename.Quote());

            Shell.Run(filename, "-sym " + (symbolDir ?? ".").QuoteSpace(), 0, input);
        }
    }
}
