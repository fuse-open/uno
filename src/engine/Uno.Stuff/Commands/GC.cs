using System.Collections.Generic;
using System.Linq;
using Stuff.Core;
using Stuff.Options;

namespace Stuff.Commands
{
    public class GC : Command
    {
        public override string Name        => "gc";
        public override string Description => "Deletes files older than specified number of days from the download cache";

        public override void Help(IEnumerable<string> args)
        {
            WriteUsage("[options] [number-of-days]");
            WriteLine("By default this deletes files that aren't used in the last 14 days.");
            WriteEnvironment();
        }

        public override void Execute(IEnumerable<string> args)
        {
            var days = double.Parse(args.FirstOrDefault() ?? "14");
            DownloadCache.CollectGarbage(days);
        }
    }
}
