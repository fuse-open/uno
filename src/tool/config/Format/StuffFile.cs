using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.Diagnostics;
using Uno.IO;

namespace Uno.Configuration.Format
{
    public class StuffFile : List<StuffItem>
    {
        public static IEnumerable<string> DefaultDefines =>
            DefaultConstants.Where(x => x.Value).Select(x => x.Key);

        public static readonly Dictionary<string, bool> DefaultConstants = new Dictionary<string, bool>
        {
            {"WINDOWS", PlatformDetection.IsWindows},
            {"WIN32", PlatformDetection.IsWindows},
            {"OSX", PlatformDetection.IsMac},
            {"DARWIN", PlatformDetection.IsMac},
            {"MAC", PlatformDetection.IsMac},
            {"LINUX", PlatformDetection.IsLinux},
            {"UNIX", !PlatformDetection.IsWindows},
            {"ARM", PlatformDetection.IsArm},
            {"X86", !PlatformDetection.IsArm && IntPtr.Size == 4},
            {"X64", !PlatformDetection.IsArm && IntPtr.Size == 8}
        };

        public readonly string Filename;
        public readonly string ParentDirectory;
        public readonly HashSet<string> Defines;

        public StuffFile(string filename, HashSet<string> defines)
        {
            Filename = filename;
            ParentDirectory = Path.GetDirectoryName(filename);
            Defines = defines;
        }

        public void Parse(string stuff, Action<string> printTokens = null)
        {
            var parser = new Parser(this, stuff);

            if (printTokens != null)
            {
                if (Defines != null)
                    printTokens(string.Join(" ", Defines));

                foreach (var token in parser)
                    printTokens(
                        Filename.ToRelativePath() +
                        "(" + token.LineNumber + "." + token.LinePosition +
                        ") <" + token.Type + "> " + token);
            }

            Clear();
            parser.Parse();
        }

        public StuffMap Flatten(Func<string, string> requireResolver = null)
        {
            var map = new StuffMap();
            map.AddFile(this, requireResolver);
            return map;
        }

        public void Add(string key, string value, int lineNumber, StuffItemType type)
        {
            if (value == null)
                value = string.Empty;
            else if (value.StartsWith("~/") || value == "~")
                value = PlatformDetection.HomeDirectory + value.Substring(1);

            Add(new StuffItem(this, type, lineNumber, key, Environment.ExpandEnvironmentVariables(value)));
        }
    }
}
