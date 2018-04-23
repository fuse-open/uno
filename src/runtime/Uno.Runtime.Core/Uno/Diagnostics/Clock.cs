// This file was generated based on Library/Core/UnoCore/Source/Uno/Diagnostics/Clock.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Diagnostics
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public static class Clock
    {
        public static global::System.Diagnostics.Stopwatch _watch;

        static Clock()
        {
            Clock._watch = global::System.Diagnostics.Stopwatch.StartNew();
        }

        public static double GetSeconds()
        {
            return Clock._watch.Elapsed.TotalSeconds;
        }

        public static long GetTicks()
        {
            return global::System.DateTime.UtcNow.Ticks - 621355968000000000;
        }

        public static int GetTimezoneOffset(int year, int month, int day)
        {
            return (int)global::System.TimeZoneInfo.Local.GetUtcOffset(new global::System.DateTime(year, month, day)).TotalMinutes;
        }
    }
}
