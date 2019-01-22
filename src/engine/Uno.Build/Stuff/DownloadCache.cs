using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace Uno.Build.Stuff
{
    public static class DownloadCache
    {
        // Use '%TEMP%/.stuff' for download cache
        public static readonly string StuffDirectory = Environment.GetEnvironmentVariable("STUFF_CACHE").Or(Path.Combine(Path.GetTempPath(), ".stuff"));
        static bool _hasCollected;

        public static void AutoCollect()
        {
            if (_hasCollected)
                return;

            _hasCollected = true;
            var timestampFile = Path.Combine(StuffDirectory, ".gc");

            if (File.Exists(timestampFile))
            {
                // Check only once per week to save I/O performance
                var timeWrite = File.GetLastWriteTimeUtc(timestampFile);
                var timeNow = DateTime.UtcNow;
                if ((timeNow - timeWrite).TotalDays < 7.0)
                    return;

                CollectGarbage();
            }

            if (Directory.Exists(StuffDirectory))
                File.WriteAllBytes(timestampFile, new byte[0]);
        }

        public static void CollectGarbage(double days = 14.0)
        {
            CollectGarbage(StuffDirectory, days);
        }

        static void CollectGarbage(string dir, double days, bool hasLog = false)
        {
            if (!Directory.Exists(dir))
                return;

            foreach (var f in Directory.EnumerateFiles(dir))
            {
                using (new FileLock(f))
                {
                    var timeWrite = File.GetLastWriteTimeUtc(f);
                    var timeNow = DateTime.UtcNow;
                    if ((timeNow - timeWrite).TotalDays < days)
                        continue;

                    if (!hasLog)
                    {
                        Log.WriteLine(ConsoleColor.Blue, "stuff: Collecting garbage");
                        hasLog = true;
                    }

                    Disk.DeleteFile(f);
                }
            }
        }

        public static string GetFile(string url)
        {
            var dst = GetFileName(url);

            // 1) Early out if the file exists locally
            if (File.Exists(dst))
            {
                Disk.TouchFile(dst);
                return dst;
            }

            Disk.CreateDirectory(StuffDirectory);

            try
            {
                // Use a set to avoid repeating errors
                var errors = new HashSet<string>();

                for (int tries = 0;; tries++)
                {
                    // 2) Download the file from URL
                    Log.WriteLine(ConsoleColor.Blue, "Downloading " + url + (
                            tries > 0
                                ? " (" + tries + "...)"
                                : null
                        ));

                    try
                    {
                        using (var client = new StuffWebClient())
                            client.DownloadFile(url, dst);
                        return dst;
                    }
                    catch (Exception e)
                    {
                        // Fail the 10th time, or unless WebException
                        if (tries >= 10 || !(e is WebException))
                        {
                            // Print previous errors, before rethrowing
                            foreach (var s in errors)
                                Log.Error(s);
                            throw;
                        }

                        errors.Add(e.Message);
                        Thread.Sleep(500);
                    }
                }
            }
            catch
            {
                Disk.DeleteFile(dst);
                throw;
            }
        }

        public static void UpdateTimestamp(string url)
        {
            Disk.TouchFile(GetFileName(url));
        }

        public static string GetFileName(string url)
        {
            return Path.Combine(StuffDirectory, url.UrlToFile());
        }

        public static string Or(this string str, string other)
        {
            return string.IsNullOrEmpty(str) ? other : str;
        }

        public static string UrlToFile(this string url, int max = 75)
        {
            var hash = url.GetHashCode();
            var ext = url.ToUpper().EndsWith(".TAR.GZ")
                    ? ".tar.gz"
                    : ".zip";
            return new string(url
                        .Skip(Math.Max(0, url.Length - max))
                        .ToArray())
                    .GenerateSlug(max)
                        + hash
                        + ext;
        }

        // http://stackoverflow.com/questions/2920744/url-slugify-algorithm-in-c
        public static string GenerateSlug(this string phrase, int max)
        {
            var str = phrase.ToLowerInvariant();
            // invalid chars
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim
            str = str.Substring(Math.Max(0, str.Length - max), Math.Min(str.Length, max)).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens
            return str;
        }
    }
}
