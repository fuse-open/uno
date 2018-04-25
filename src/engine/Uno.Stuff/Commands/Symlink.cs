using System;
using System.Collections.Generic;
using System.Linq;
using Stuff.Core;

namespace Stuff.Commands
{
    class Symlink : Command
    {
        public override string Name        => "symlink";
        public override string Description => "Finds duplicate files in a directory tree and replaces them with symlinks";

        public override void Help(IEnumerable<string> args)
        {
            WriteUsage("<directory>");
        }

        public override void Execute(IEnumerable<string> args)
        {
            var dir = args.FirstOrDefault();
            if (string.IsNullOrEmpty(dir))
                throw new ArgumentException("No directory was given");

            Packer.ReplaceDuplicateFilesWithSymlinks(dir);
        }
    }
}