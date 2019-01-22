using System;
using System.Collections.Generic;
using NuGet;
using Uno.Build.Packages.Feeds;
using Uno.Collections;
using Uno.Configuration.Format;
using Uno.IO;

namespace Uno.Build.Packages
{
    public class UpkPusher : DiskObject
    {
        const string UserAgent = "uno";

        readonly List<UpkFile> _packages = new List<UpkFile>();

        public UpkPusher(Disk disk)
            : base(disk)
        {
        }

        public void AddFile(string upkFile)
        {
            _packages.Add(new UpkFile(upkFile));
        }

        public void AddFiles(IEnumerable<string> upkFiles)
        {
            foreach (var f in upkFiles)
                AddFile(f);
        }

        public void Push(string serverUrl, string apiKey = null, double timeout = 600.0)
        {
            if (_packages.Count == 0)
                throw new InvalidOperationException("No input files");

            var server = new PackageServer(serverUrl, UserAgent);
            var ms = (int)(timeout * 1000.0);

            foreach (var upk in _packages)
                using (Log.StartAnimation("Uploading " + upk, ConsoleColor.Blue))
                    server.PushPackage(apiKey, upk.ZipPackage, upk.ZipPackageSize, ms, false);
        }

        public void SaveList(string stuffFile)
        {
            var list = new ListDictionary<string, string>();

            foreach (var upk in _packages)
                list.Add(upk.Name, upk.Version);
            
            var stuff = list.StringifyStuff(true);
            Disk.WriteAllText(stuffFile, stuff);
            Log.Verbose(stuff);
        }
    }
}