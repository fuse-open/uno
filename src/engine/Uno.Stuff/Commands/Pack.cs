using System;
using System.Collections.Generic;
using Stuff.Core;
using Stuff.Options;

namespace Stuff.Commands
{
    class Pack : Command
    {
        public override string Name        => "pack";
        public override string Description => "Builds .ZIP and .STUFF-UPLOAD file(s)";

        public override void Help(IEnumerable<string> args)
        {
            WriteUsage("[options] [input-directory]");
            WriteLine("By default this will pack the current directory.");

            WriteHead("Available options");
            WriteRow("-n, --name=STRING",       "Specify .STUFF-UPLOAD name");
            WriteRow("-c, --condition=STRING",  "Install stuff only if this STRING", true);
            WriteRow("-s, --suffix=STRING",     "Append suffix to .ZIP file(s)", true);
            WriteRow("-o, --out-dir=PATH",      "Specify output directory", true);
            WriteRow("-i, --install-dir=PATH",  "Specify installation directory", true);
            WriteRow("-m, --modular",           "Modular mode: Build multiple packages using .STUFF-PACK files");
        }

        public override void Execute(IEnumerable<string> args)
        {
            var packer = new Packer();
            var input = new OptionSet
                {
                    { "n=|name=", x => packer.Name = x },
                    { "c=|condition=", x => packer.Condition = x },
                    { "s=|suffix=", x => packer.Suffix = x },
                    { "o=|out-dir=|output-dir=", x => packer.OutputDirectory = x },
                    { "i=|install-dir=", x => packer.InstallDirectory = x },
                    { "m|modular", x => packer.Modular = x != null }
                }
                .Parse(args)
                .GetSingleArgument(".");

            if (packer.Name == null)
                throw new ArgumentException("--name was not specified");

            packer.Pack(input);
        }
    }
}
