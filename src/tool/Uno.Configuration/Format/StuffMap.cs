using System;
using System.Collections.Generic;
using System.IO;
using Uno.IO;
using Uno.Logging;

namespace Uno.Configuration.Format
{
    public class StuffMap : Dictionary<string, StuffItem>
    {
        readonly HashSet<string> _filesUpper = new HashSet<string>();

        public void AddFile(string filename)
        {
            _filesUpper.Add(filename.ToUpperInvariant());
        }

        public bool ContainsFile(string filename)
        {
            return _filesUpper.Contains(filename.ToUpperInvariant());
        }

        public StuffObject ToObject()
        {
            return new StuffObject(this);
        }

        public void AddFile(StuffFile file, Func<string, string> requireResolver)
        {
            AddFile(file.Filename);

            foreach (var item in file)
            {
                try
                {
                    switch (item.Type)
                    {
                        case StuffItemType.Require:
                        {
                            var filename = item.Value.Replace('/', Path.DirectorySeparatorChar);

                            if (filename.IsValidPath())
                            {
                                filename = Path.GetFullPath(Path.Combine(file.ParentDirectory, filename));

                                if (File.Exists(filename))
                                {
                                    Require(filename, file.Defines, requireResolver);
                                    break;
                                }
                            }

                            if (filename.IndexOf('*') == -1)
                                Log.Default.VeryVerbose(file.Filename.ToRelativePath() + "(" + item.LineNumber + "): File not found");
                            else
                                foreach (var foundFile in Directory.EnumerateFiles(file.ParentDirectory, filename))
                                    if (!ContainsFile(foundFile))
                                        Require(foundFile, file.Defines, requireResolver);
                            break;
                        }
                        case StuffItemType.Append:
                        {
                            StuffItem value;
                            if (TryGetValue(item.Key, out value))
                                value.Next = item;
                            else
                                this[item.Key] = item;
                            break;
                        }
                        default:
                            this[item.Key] = item;
                            break;
                    }
                }
                catch (Exception e)
                {
                    throw new FormatException(file.Filename.ToRelativePath() + "(" + item.LineNumber + "): " + e.Message, e);
                }
            }
        }

        public void Require(string filename, HashSet<string> defines, Func<string, string> requireResolver = null)
        {
            // Early out on circular include
            if (ContainsFile(filename))
                return;

            if (requireResolver == null)
                throw new NotSupportedException("'require' is not supported because a 'requireResolver' was not provided");

            var includeFile = new StuffFile(filename, defines);
            includeFile.Parse(requireResolver(filename));
            AddFile(includeFile, requireResolver);
        }
    }
}