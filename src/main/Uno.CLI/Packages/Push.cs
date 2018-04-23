using System;
using System.Collections.Generic;
using Mono.Options;
using Uno.Build.Packages;

namespace Uno.CLI.Packages
{
    class Push : Command
    {
        public override string Name => "push";
        public override string Description => "Upload Uno package(s) to remote server.";
        public override bool IsExperimental => true;

        public override void Help(IEnumerable<string> args)
        {
            WriteUsage("<options> <upk-file ...>");

            WriteHead("Upload options");
            WriteRow("-k, --key=STRING",          "Token to validate against a remote API");
            WriteRow("-s, --server=URL",          "URL to a remote Uno package server");
            WriteRow("-t, --timeout=NUMBER",      "Time out after [600] seconds", true);
            WriteRow("-o, --out=FILE",            "Save a list of packages to FILE", true);
            WriteRow("-U, --no-upload",           "Don't push anything to server");
        }

        public override void Execute(IEnumerable<string> args)
        {
            string server = null
                , key = null
                , file = null;
            var timeout = 600.0;
            var pusher = new UpkPusher(Disk);
            var upload = true;

            pusher.AddFiles(
                new OptionSet {
                        { "k=|key=",             value => key = value },
                        { "s=|source=|server=",  value => server = value },
                        { "o=|out=",             value => file = value },
                        { "t=|timeout=",         value => timeout = value.ParseDouble("timeout") },
                        { "U|no-upload",         value => upload = false }
                    }.Parse(args)
                    .Files("*.upk"));

            if (upload)
            {
                if (server == null)
                    throw new ArgumentException("No package source (--source)");

                pusher.Push(server, key, timeout);
            }

            if (file != null)
                pusher.SaveList(file);
        }
    }
}