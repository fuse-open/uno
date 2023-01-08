using System;
using System.Collections.Generic;
using System.IO;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.Backends.CPlusPlus;
using Uno.Compiler.Foreign.ObjC;
using Uno.IO;

namespace Uno.Compiler.Foreign
{
    public class ForeignFilePass : Pass
    {
        readonly SourceBundle Bundle;
        readonly ExtensionRoot Extensions;
        readonly CppBackend Backend;

        public ForeignFilePass(
            ExtensionRoot extensions,
            SourceBundle bundle,
            CppBackend backend)
            : base(backend)
        {
            Extensions = extensions;
            Bundle = bundle;
            Backend = backend;
        }

        public override void Run()
        {
            var seen = new List<SourceBundle>();
            var toProcess = new List<SourceBundle> {Bundle};

            while (toProcess.Count > 0)
            {
                var bundle = toProcess[0];
                toProcess.RemoveAt(0);
                seen.Add(bundle);

                foreach (var f in bundle.ForeignSourceFiles)
                {
                    if (f.SourceKind != ForeignItem.Kind.Unknown &&
                        Environment.Test(bundle.Source, f.Condition))
                    {
                        var originalSource = new Source(bundle, bundle.SourceDirectory.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);
                        var copyFileType = f.GetCopyFileType();
                        Extensions.CopyFiles.Add(new CopyFile(
                            new SourceValue(originalSource, f.UnixPath),
                            CopyFileFlags.ProcessFile,
                            GetRequiredDestination(f, originalSource),
                            new SourceValue(originalSource, f.Condition),
                            new SourceValue(originalSource, copyFileType),
                            x => Preprocess(f, x)));
                        if (!string.IsNullOrEmpty(copyFileType))
                           Environment.Require(copyFileType, new SourceValue(originalSource, f.UnixPath));
                    }
                }

                foreach (var p in bundle.References)
                    if (!seen.Contains(p))
                        toProcess.Add(p);
            }
        }

        string Preprocess(ForeignItem f, string code)
        {
            switch (f.SourceKind)
            {
                case ForeignItem.Kind.ObjCHeader:
                case ForeignItem.Kind.ObjCSource:
                    return new ForeignObjCPass(Backend).Preprocess(new Source(f.UnixPath), code);
                case ForeignItem.Kind.Java:
                default: return code;
            }
        }

		SourceValue? GetRequiredDestination(ForeignItem fsource, Source projectSource)
		{
			if (fsource.SourceKind != ForeignItem.Kind.Java)
				return null;

            var fullJavaSourcePath = fsource.UnixPath.UnixToNative()
                    .ToFullPath(projectSource.Bundle.SourceDirectory);
            var packageName = GetJavaPackageName(fullJavaSourcePath);
            return new SourceValue(projectSource, Environment.GetString("Java.SourceDirectory") + "/" + packageName.Replace('.', '/') + Path.GetFileName(fsource.UnixPath));
        }

        string GetJavaPackageName(string path)
        {
            // this is super dumb, will make more robust soon
            using (var sr = new StreamReader(path))
            {
                var package = "";
                while(!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Trim();
                    if (line.Length > 8 && line.Substring(0,8) == "package ")
                    {
                        if (line[line.Length - 1] != ';')
                            throw new Exception("Foreign Code Error: Malformed java package line. Missing semicolon");
                        package = line.Substring(8, line.Length-9) + ".";
                        break;
                    }
                }

                return package;
            }
        }
    }
}
