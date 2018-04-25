using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Stuff.Format;
using Stuff.Options;

namespace Stuff.Commands
{
    class Push : Command
    {
        public override string Name        => "push";
        public override string Description => "Uploads .STUFF-UPLOAD file(s) to a remote server";

        public override void Help(IEnumerable<string> args)
        {
            WriteUsage("[options] [directory|file|glob ...]");

            WriteHead("Available options");
            WriteRow("-u, --url=URL",          "Remote server for .ZIP files");
            WriteRow("-t, --api-token=STRING", "Sets X-API-Token header", true);
            WriteRow("-o, --out-dir=PATH",     "Output directory for .STUFF file(s)", true);
        }

        public override void Execute(IEnumerable<string> args)
        {
            string url = null;
            string apiToken = null;
            string outDir = null;
            var files = new OptionSet
                {
                    { "u=|url=", x => url = x },
                    { "t=|a=|api-token=", x => apiToken = x },
                    { "o=|out-dir=|output-dir=", x => outDir = x },
                }
                .Parse(args)
                .GetFiles("*.stuff-upload");

            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("--url was not specified");

            if (outDir != null)
                Disk.CreateDirectory(outDir);

            foreach (var f in files)
            {
                var result = new Dictionary<string, string>();
                var sizes = new Dictionary<string, long>();
                var stuffDir = outDir ?? Path.GetDirectoryName(f);

                foreach (var item in StuffObject.Load(f, StuffFlags.AcceptAll))
                {
                    if (item.Value == null)
                        continue;

                    using (var web = new WebClient())
                    {
                        var uploadFile = Path.Combine(Path.GetDirectoryName(f), item.Value.ToString());
                        Log.WriteLine(ConsoleColor.Blue, "stuff: POST " + uploadFile.Relative());

                        try
                        {
                            if (apiToken != null)
                                web.Headers.Add("X-API-Token: " + apiToken);

                            var bytes = web.UploadFile(url, uploadFile);
                            var logFile = Path.Combine(stuffDir, item.Value + ".log");
                            Log.Event(IOEvent.Write, logFile);
                            File.WriteAllBytes(logFile, bytes);
                            result.Add(item.Key, GetUrl(logFile, bytes));
                            sizes.Add(item.Key, new FileInfo(uploadFile).Length);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(uploadFile.Relative() + ": " + e.Message, e);
                        }
                    }
                }

                var stuffFile = Path.Combine(stuffDir, Path.GetFileNameWithoutExtension(f) + ".stuff");
                Log.Event(IOEvent.Write, stuffFile);

                // This is not the optimal solution, but it works for now.
                // In reality there could be multiple conditions in the file, and so on, 
                // but we don't want to write a parser now.
                var condition = GetFirstCondition(f);

                using (var w = new StreamWriter(stuffFile) {NewLine = "\n"})
                {
                    foreach (var item in result)
                    {
                        var line = "/* " + ConvertBytesToMegabytes(sizes[item.Key])
                                       .ToString("0.00", CultureInfo.InvariantCulture) +
                                   "MB */ " + item.Key.Literal() + ": " + item.Value.Literal();
                        Console.WriteLine(line + (
                            !string.IsNullOrEmpty(condition) 
                                ? " (" + condition + ")" 
                                : null));
                        w.WriteConditional(condition, line);
                    }
                }
            }
        }

        static string GetFirstCondition(string file)
        {
            var stuff = File.ReadAllText(file).Split('\n');
            return stuff[0].StartsWith("if ")
                ? stuff[0].Substring(3).Trim().Trim(' ', '{')
                : null;
        }

        string GetUrl(string logFile, byte[] bytes)
        {
            var links = GetLinks(Encoding.UTF8.GetString(bytes));

            switch (links.Count)
            {
                case 1:
                    return links[0];
                case 0:
                    throw new Exception(logFile.Relative() + ": Server did not return a valid URL, please see log file");
                default:
                    Log.Warning(logFile + ": Server returned more than one URL, please see log file");
                    return links[0];
            }
        }

        // http://stackoverflow.com/questions/9125016/get-url-from-a-text
        List<string> GetLinks(string message)
        {
            var list = new List<string>();
            var matches = new Regex(
                    @"((https?|ftp|file)\://|www.)[A-Za-z0-9\.\-]+(/[A-Za-z0-9\?\&\=;\+!'\(\)\*\-\._~%]*)*",
                    RegexOptions.IgnoreCase)
                .Matches(message);

            foreach (Match match in matches)
                if (match.Value.Contains("://"))
                    list.Add(match.Value);

            return list;
        }

        static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }
}
