using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Uno.TestRunner.Loggers
{
    [Synchronization]
    public class FileHelpers
    {
        private static readonly object FileLock = new object();
        public static string ReadAllTextFromSharedFile(string fileName, int maxLength = 10000)
        {
            lock (FileLock)
            {
                using (Stream s = new FileStream(fileName,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite))
                {
                    var bytes = new byte[maxLength];
                    s.Read(bytes, 0, maxLength);
                    return Encoding.UTF8.GetString(bytes);
                }
            }
        }

        public static void WriteAllTextToSharedFile(string fileName, string line)
        {
            lock (FileLock)
            {
                EnsureFileExists(fileName);
                using (Stream s = new FileStream(fileName,
                    FileMode.Truncate,
                    FileAccess.Write,
                    FileShare.ReadWrite))
                {
                    byte[] bytes = new UTF8Encoding(true).GetBytes(line);
                    s.Write(bytes, 0, bytes.Length);
                }
            }
        }

        public static void AppendAllTextToSharedFile(string fileName, string line)
        {
            lock (FileLock)
            {
                EnsureFileExists(fileName);
                using (Stream s = new FileStream(fileName,
                    FileMode.Append,
                    FileAccess.Write,
                    FileShare.ReadWrite))
                {
                    byte[] bytes = new UTF8Encoding(true).GetBytes(line);
                    s.Write(bytes, 0, bytes.Length);
                }
            }
        }

        private static void EnsureFileExists(string fileName)
        {
            File.AppendAllText(fileName, "");
        }

        public static Match WaitForFileToExistAndMatch(string filename, string regex, int timeoutMillis)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            while (true)
            {
                if (stopWatch.ElapsedMilliseconds > timeoutMillis)
                {
                    break;
                }
                if (File.Exists(filename))
                {
                    try
                    {
                        var content = ReadAllTextFromSharedFile(filename);
                        var match = Regex.Match(content, regex);
                        if (match.Success)
                        {
                            return match;
                        }
                    }
                    catch (IOException)
                    {
                    }
                }
                Thread.Sleep(10);
            }
            throw new Exception("Timed out waiting for " + filename + " to exist and match " + regex);
        }
    }
}
