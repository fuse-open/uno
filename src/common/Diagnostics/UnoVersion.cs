using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Uno.Diagnostics
{
    public static class UnoVersion
    {
        static readonly Assembly _assembly = typeof(UnoVersion).Assembly;

        public static string LongHeader => "Uno version " + VersionString;
        public static string ShortHeader => "Uno " + VersionString;

        static string _versionString;
        public static string VersionString
        {
            get
            {
                if (_versionString != null)
                    return _versionString;

                var buildNumber = FileVersion.FilePrivatePart;
                var buildString = buildNumber <= 0
                    ? "dev-build"
                    : "build " + buildNumber;
                return _versionString =
                    $"{InformationalVersion} ({buildString}) {PlatformDetection.SystemString} {CommitSha.Truncate(7)}"
                        .TrimEnd();
            }
        }

        static bool? _isDevBuild;
        public static bool IsDevBuild => (_isDevBuild ?? (_isDevBuild = VersionString.Contains("(dev-build)"))).Value;

        public static string CommitSha
        {
            get
            {
                var config = GetAttribute<AssemblyConfigurationAttribute>()?.Configuration;
                return !string.IsNullOrEmpty(config)
                    ? config
                    : "N/A";
            }
        }

        public static string Copyright
        {
            get
            {
                var copyright = GetAttribute<AssemblyCopyrightAttribute>()?.Copyright;
                return string.IsNullOrEmpty(copyright)
                        ? "(no copyright information)" :
                    PlatformDetection.IsWindows // The Windows shell can't print the © character
                        ? copyright.Replace("©", "(C)")
                        : copyright;
            }
        }

        public static string InformationalVersion
        {
            get
            {
                var version = GetAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
                return !string.IsNullOrEmpty(version)
                    ? version
                    : FileVersion.FileVersion;
            }
        }

        public static FileVersionInfo FileVersion => FileVersionInfo.GetVersionInfo(_assembly.Location);

        static T GetAttribute<T>()
        {
            return _assembly.GetCustomAttributes(true).OfType<T>()
                .FirstOrDefault();
        }
    }
}
