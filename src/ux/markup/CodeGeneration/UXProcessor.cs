using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.Compiler;
using Uno.Compiler.API;
using Uno.IO;
using Uno.UX.Markup.Reflection;
using Uno.UX.Markup.UXIL;

namespace Uno.UX.Markup.CodeGeneration
{
    public class UXProcessor : DiskObject
    {
        // Increment this number when the UX compiler changes behavior,
        // for proper cache invalidation!
        public const int Version = 15;

        public static void Build(Disk disk, IReadOnlyList<SourceBundle> bundles, ITranspiler transpiler)
        {
            var p = new UXProcessor(disk);
            foreach (var bundle in bundles)
                if (bundle.IsProject && bundle.UXFiles.Count > 0)
                    p.Build(bundle, transpiler);
        }

        UXProcessor(Disk disk)
            : base(disk)
        {
        }

        void Build(SourceBundle bundle, ITranspiler transpiler)
        {
            // Avoid multiple visits
            bundle.Flags &= ~SourceBundleFlags.Project;

            try
            {
                var cacheDir = Path.Combine(bundle.CacheDirectory, "ux" + Version);
                var listFile = Path.Combine(cacheDir, bundle.Name + ".g");

                if (!IsDirty(bundle, listFile))
                {
                    foreach (var line in File.ReadAllText(listFile).Split('\n'))
                        if (line.Length > 0)
                            bundle.SourceFiles.Add(line.Trim());
                    return;
                }

                var unoFiles = new List<string>();
                var generatedPath = Path.Combine(cacheDir, bundle.Name + ".unoproj.g.uno");
                var compiler = CompilerReflection.ILCache.Create(Log, bundle);
                var markupLog = new CompilerReflection.MarkupErrorLog(Log, bundle);
                var sourceFilePath = generatedPath.ToRelativePath(bundle.SourceDirectory).NativeToUnix();
                var uxSrc = bundle.UXFiles.Select(x => new UXIL.Compiler.UXSource(Path.Combine(bundle.SourceDirectory, x.NativePath)));
                var doc = UXIL.Compiler.Compile(new CompilerDataTypeProvider(compiler), uxSrc, bundle.SourceDirectory, bundle.Name, generatedPath, markupLog);
                TranspileScripts(doc, transpiler);

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
                            unoFiles.Add(gp.ToRelativePath(bundle.SourceDirectory).NativeToUnix());
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
                    bundle.SourceFiles.Add(new FileItem(f));
            }
            catch (Exception e)
            {
                Log.Error(bundle.Source, null, e.Message);
            }
        }

        static bool IsDirty(SourceBundle bundle, string listFile)
        {
            if (!File.Exists(listFile))
                return true;

            var listTime = File.GetLastWriteTime(listFile);

            foreach (var f in bundle.UXFiles)
                if (listTime < File.GetLastWriteTime(
                        Path.Combine(bundle.SourceDirectory, f.NativePath)))
                    return true;

            return false;
        }

        void TranspileScripts(Project project, ITranspiler transpiler)
        {
            foreach (var doc in project.Documents.Values)
            {
                foreach (var node in doc.NodesInDocumentOrder)
                {
                    if (node is not NewObjectNode tag)
                        continue;

                    var isJavaScript = tag.DataType.QualifiedName == "Fuse.Reactive.JavaScript";
                    var isTypeScript = tag.DataType.QualifiedName == "Fuse.Reactive.TypeScript";

                    if (!isJavaScript && !isTypeScript)
                        continue;

                    var transpile = isTypeScript || GetPropertyBool(tag, "Transpile") == true;

                    if (!transpile)
                        continue;

                    var codeProp = GetProperty(tag, "Code");

                    if (codeProp?.Value is not String code)
                        continue;

                    var filename = code.Source.FileName.ToRelativePath() +
                        "@" + code.Source.LineNumber + (
                            isTypeScript
                                ? ".ts"
                                : ".js"
                        );

                    Log.Verbose("Transpiling " + filename);

                    if (transpiler.TryTranspile(filename, code.Value, out string output))
                        codeProp.Value = new String(output, code.Source);
                }
            }
        }

        static bool? GetPropertyBool(Node node, string name)
        {
            var prop = GetProperty(node, name);

            if (prop?.Value is Bool value)
                return value.Value;

            return null;
        }

        static AtomicProperty GetProperty(Node node, string name)
        {
            foreach (var prop in node.AtomicProperties)
                if (prop.Facet.Name == name)
                    return prop;

            return null;
        }
    }
}
