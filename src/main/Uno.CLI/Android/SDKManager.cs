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
    class SDKManager : Command
    {
        public static string NdkDirectory => UnoConfig.Current.GetFullPath("Android.NDK.Directory", "AndroidNdkDirectory");
        public static string SdkDirectory => UnoConfig.Current.GetFullPath("Android.SDK.Directory", "AndroidSdkDirectory");

        public override string Name => "sdkmanager";
        public override string Description => "Interact with Android's CLI SDK Manager.";
        public override bool IsExperimental => true;

        public override void Help()
        {
            WriteUsage("[arguments ...]");
        }

        public override void Execute(IEnumerable<string> args)
        {
            var sdkManager = Path.Combine(SdkDirectory, "tools", "bin", PlatformDetection.IsWindows ? "sdkmanager.bat" : "sdkmanager");

            if (!File.Exists(sdkManager))
                throw new FileNotFoundException("Android SDK Manager was not found at " + sdkManager.Quote());

            Shell.Run(sdkManager, args.JoinArguments());
        }
    }
}
