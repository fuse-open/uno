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

        public static string GetPath()
        {
            try
            {
                if (PlatformDetection.IsMac)
                {
                    var sb = new StringBuilder(4096);
                    var len = (uint)sb.Capacity;
                    var retval = _NSGetExecutablePath(sb, ref len);
                    if (retval != 0)
                        throw new InvalidOperationException("returned " + retval + ", len " + len);
                    return sb.ToString();
                }
            }
            catch (Exception e)
            {
                Log.Default.Warning("MonoInfo.GetPath() failed: " + e.Message);
                Log.Default.Trace(e);
            }

            return "mono";
        }

        [DllImport("/usr/lib/libSystem.dylib")]
        static extern int _NSGetExecutablePath(StringBuilder buf, ref uint bufsize);
    }
}
