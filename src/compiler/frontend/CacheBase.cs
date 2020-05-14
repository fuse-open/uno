using System;
using System.Collections.Generic;
using System.IO;
using Uno.IO;
using Uno.Logging;

namespace Uno.Compiler.Frontend
{
    public abstract class CacheBase<T> : LogObject
        where T : class
    {
        protected readonly Disk Disk;
        public readonly string MagicString;
        readonly HashSet<string> _processedFiles;

        protected CacheBase(Log log, uint magic, HashSet<string> processedFiles)
            : base(log)
        {
            Disk = new Disk(log);
            MagicString = magic.MagicString();
            _processedFiles = processedFiles;
        }

        public abstract bool Parse(SourcePackage upk, string filename, List<T> result);
        public abstract void Deserialize(SourcePackage upk, string filename, List<T> resultAsync);
        public abstract void Serialize(SourcePackage upk, string filename, IEnumerable<T> value);

        public void Load(SourcePackage upk, string relative, List<T> resultAsync)
        {
            var sourceFilename = relative;
            var cacheFilename = GetCacheFilename(upk, relative);

            if (!VerifyFilename(upk.Source, upk.SourceDirectory, ref sourceFilename))
                return;

            if (!Disk.IsNewer(sourceFilename, cacheFilename))
            {
                try
                {
                    Deserialize(upk, cacheFilename, resultAsync);
                    return;
                }
                catch (Exception e)
                {
                    Log.Warning(upk.Source, ErrorCode.W0000, "Unable to load cache: " + e.Message);
                }
            }

            Log.Event(IOEvent.Read, sourceFilename);

            var result = new List<T>();
            if (Parse(upk, sourceFilename, result))
            {
                lock (resultAsync)
                    resultAsync.AddRange(result);

                try
                {
                    Disk.CreateDirectory(Path.GetDirectoryName(cacheFilename));
                    Serialize(upk, cacheFilename, result);
                }
                catch (Exception e)
                {
                    Log.Warning(upk.Source, ErrorCode.W0000, "Unable to save cache: " + e.Message);
                }
            }
            else
                Disk.DeleteFile(cacheFilename);
        }

        public string GetCacheFilename(SourcePackage upk, string relative)
        {
            return Path.Combine(upk.CacheDirectory, MagicString, relative.GetNormalizedBasename());
        }

        public bool VerifyFilename(Source src, string dir, ref string filename)
        {
            Disk.GetFullPath(src, dir, ref filename);

            lock (_processedFiles)
            {
                if (_processedFiles.Contains(filename))
                {
                    Log.Warning(src, ErrorCode.W0000, filename.Quote() + " is already added to compilation");
                    return false;
                }

                _processedFiles.Add(filename);
                return true;
            }
        }
    }
}
