using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Uno.Logging;

namespace Uno.Diagnostics
{
    public static class MonoInfo
    {
        public static bool IsRunningMono => Type.GetType("Mono.Runtime") != null;

        public static string GetVersion()
        {
            var mono = Type.GetType("Mono.Runtime");
            var displayName = mono?.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
            return displayName?.Invoke(null, null)?.ToString();
        }
    }
}
