using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.Compiler;
using Uno.IO;

namespace Uno.UX.Markup.CodeGeneration
{
    public class UXProcessor : DiskObject
    {
        // Increment this number when the UX compiler changes behavior,
        // for proper cache invalidation!
        public const int Version = 15;

        public static void Build(Disk disk, IReadOnlyList<SourcePackage> packages)
        {
            var p = new UXProcessor(disk);
            foreach (var upk in packages)
                if (upk.IsProject && upk.UXFiles.Count > 0)
                    p.Build(upk);
        }

        UXProcessor(Disk disk)
            : base(disk)
        {
        }

        void Build(SourcePackage upk)
        {
            // Avoid multiple visits
            upk.Flags &= ~SourcePackageFlags.Project;

            try
            {
                var cacheDir = Path.Combine(upk.CacheDirectory, "ux" + Version);
                var listFile = Path.Combine(cacheDir, upk.Name + ".g");

                if (!IsDirty(upk, listFile))
                {
                    foreach (var line in File.ReadAllText(listFile).Split('\n'))
                        if (line.Length > 0)
                            upk.SourceFiles.Add(line.Trim());
                    return;
                }

                var unoFiles = new List<string>();
                var generatedPath = Path.Combine(cacheDir, upk.Name + ".unoproj.g.uno");
                var compiler = CompilerReflection.ILCache.Create(Log, upk);
                var markupLog = new CompilerReflection.MarkupErrorLog(Log, upk);
                var sourceFilePath = generatedPath.ToRelativePath(upk.SourceDirectory).NativeToUnix();
                var uxSrc = upk.UXFiles.Select(x => new UXIL.Compiler.UXSource(Path.Combine(upk.SourceDirectory, x.NativePath)));
                var doc = UXIL.Compiler.Compile(new Reflection.CompilerDataTypeProvider(compiler), uxSrc, upk.SourceDirectory, upk.Name, generatedPath, markupLog);

                if (Log.HasErrors)
                    return;

                Disk.CreateDirectory(cacheDir);
                if (doc == null)
                {
                    File.WriteAllText(listFile, "");
                    return;
                }

                var garbage = new List<IDisposable>();
                using (var sw = Disk.CreateText(generatedPath))
                {
                    using (var cw = new TextFormatter(sw))
                    {
                        CodeGenerator.GenerateCode(doc, cw, markupLog, className => {
                            var gp = Path.Combine(cacheDir, className + ".g.uno");
                            var ret = new TextFormatter(Disk.CreateText(gp));
                            unoFiles.Add(gp.ToRelativePath(upk.SourceDirectory).NativeToUnix());
                            garbage.Add(ret);
                            return ret;
                        });
                    }
                }

                foreach (var g in garbage)
                    g.Dispose();

                unoFiles.Add(sourceFilePath);
                File.WriteAllText(listFile, string.Join("\n", unoFiles));

                foreach (var f in unoFiles)
                    upk.SourceFiles.Add(new FileItem(f));
            }
            catch (Exception e)
            {
                Log.Error(upk.Source, null, e.Message);
            }
        }

        bool IsDirty(SourcePackage upk, string listFile)
        {
            if (!File.Exists(listFile))
                return true;

            var listTime = File.GetLastWriteTime(listFile);

            foreach (var f in upk.UXFiles)
                if (listTime < File.GetLastWriteTime(
                        Path.Combine(upk.SourceDirectory, f.NativePath)))
                    return true;

            return false;
        }
    }
}
