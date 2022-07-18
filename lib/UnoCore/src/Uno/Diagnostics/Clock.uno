using Uno.Compiler.ExportTargetInterop;

namespace Uno.Diagnostics
{
    [extern(CPLUSPLUS) Require("Source.Include", "time.h")]
    [extern(CPLUSPLUS && UNIX) Require("Source.Include", "sys/time.h")]
    public static class Clock
    {
        extern(DOTNET)
        static System.Diagnostics.Stopwatch _watch = System.Diagnostics.Stopwatch.StartNew();

        [extern(CPLUSPLUS && APPLE) Require("Source.Include", "mach/mach.h")]
        [extern(CPLUSPLUS && APPLE) Require("Source.Include", "mach/mach_time.h")]
        [extern(CPLUSPLUS && WIN32) Require("Source.Include", "Uno/WinAPIHelper.h")]
        public static double GetSeconds()
        {
            if defined(CPLUSPLUS && WIN32)
            @{
                LARGE_INTEGER frequency;
                QueryPerformanceFrequency(&frequency);

                LARGE_INTEGER currentTime;
                QueryPerformanceCounter(&currentTime);

                return (double)currentTime.QuadPart / (double)frequency.QuadPart;
            @}
            else if defined(CPLUSPLUS && APPLE)
            @{
                //https://developer.apple.com/library/mac/qa/qa1398/_index.html
                static double multiplier = 0;
                if (multiplier == 0)
                {
                    mach_timebase_info_data_t timebase;
                    mach_timebase_info(&timebase);
                    multiplier = (double)timebase.numer / (double)timebase.denom / 1e9;
                }

                return (double)mach_absolute_time() * multiplier;
            @}
            else if defined(CPLUSPLUS)
            @{
                // this method is more accurate for android
                struct timespec now;
                clock_gettime(CLOCK_MONOTONIC, &now);
                return ((double)now.tv_sec) + (((double)now.tv_nsec) / 1000000000.0);
            @}
            else if defined(DOTNET)
                return _watch.Elapsed.TotalSeconds;
            else
                build_error;
        }

        [extern(CPLUSPLUS) Require("Source.Include", "chrono")]
        public static long GetTicks()
        {
            if defined(CPLUSPLUS)
            @{
                auto start = std::chrono::system_clock::now();
                return std::chrono::duration_cast<std::chrono::duration<int64_t, std::ratio<1, 10000000>>>(start.time_since_epoch()).count();
            @}
            else if defined(DOTNET)
                return DateTime.UtcNow.Ticks - 621355968000000000;
            else
                build_error;
        }

        public static int GetTimezoneOffset(int year, int month, int day)
        {
            if defined(CPLUSPLUS)
            @{
                struct tm stm;
                stm.tm_year = $0 - 1900;
                stm.tm_mon = $1 - 1;
                stm.tm_mday = $2;
                stm.tm_hour = 0;
                stm.tm_min = 0;
                stm.tm_sec = 0;
                stm.tm_wday = 0;
                stm.tm_yday = 0;
                stm.tm_isdst = 0;
                time_t current_time = mktime(&stm);
#ifdef WIN32
                gmtime_s(&stm, &current_time);
                time_t utc = mktime(&stm);

                localtime_s(&stm, &current_time);
                time_t local = mktime(&stm);
#else
                struct tm *info;
                info = gmtime(&current_time);
                time_t utc = mktime(info);

                info = localtime(&current_time);
                time_t local = mktime(info);
#endif
                return (int)(local - utc) / 60 + stm.tm_isdst * 60;
            @}
            else if defined(DOTNET)
                return (int) System.TimeZoneInfo.Local.GetUtcOffset(new DateTime(year, month, day)).TotalMinutes;
            else
                build_error;
        }
    }
}

namespace System.Diagnostics
{
    [DotNetType]
    extern(DOTNET) class Stopwatch
    {
        public static extern Stopwatch StartNew();

        public extern TimeSpan Elapsed { get; }
    }
}

namespace System
{
    using Uno;

    [DotNetType]
    extern(DOTNET) struct TimeSpan
    {
        public extern double TotalMinutes { get; }
        public extern double TotalSeconds { get; }
    }

    [DotNetType]
    extern(DOTNET) sealed class TimeZoneInfo
    {
        public static extern TimeZoneInfo Local { get; }

        public extern TimeSpan GetUtcOffset(DateTime dateTime);
    }
}
