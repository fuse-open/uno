using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.Collections;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.IL.Utilities;
using Uno.IO;
using Uno.Logging;

namespace Uno.Compiler.Core.Syntax.Builders
{
    public class BundleBuilder : LogObject, IBundle
    {
        readonly Dictionary<SourceBundle, ClassType> _bundles = new Dictionary<SourceBundle, ClassType>();
        readonly ListDictionary<SourceBundle, BundleFile> _files = new ListDictionary<SourceBundle, BundleFile>();
        readonly Dictionary<string, Expression> _cache = new Dictionary<string, Expression>();
        readonly Dictionary<Field, Expression> _fields = new Dictionary<Field, Expression>();
        readonly HashSet<Field> _emittedFields = new HashSet<Field>();

        readonly Backend _backend;
        readonly BuildEnvironment _env;
        readonly ILFactory _ilf;
        readonly Compiler _compiler;

        public BundleBuilder(
            Backend backend,
            BuildEnvironment env,
            ILFactory ilf,
            Compiler compiler)
            : base(compiler)
        {
            _backend = backend;
            _env = env;
            _ilf = ilf;
            _compiler = compiler;
        }

        public void Build()
        {
            if (_backend.IsDefault)
                return;

            foreach (var bundle in _compiler.Input.Bundles)
            {
                foreach (var file in bundle.BundleFiles)
                {
                    var filename = file.UnixPath;
                    if (_compiler.Disk.GetFullPath(bundle.Source, bundle.SourceDirectory, ref filename))
                        CreateFile(bundle.Source, filename);
                }
            }

            // Emit fields and statements in sorted order for deterministic build result
            foreach (var e in _bundles.Values)
            {
                e.Fields.Sort();

                foreach (var f in e.Fields)
                    Emit(f);
            }

            foreach (var e in _files)
            {
                e.Value.Sort((a, b) => a.BundleName.CompareTo(b.BundleName));

                var index = new List<string>();
                foreach (var f in e.Value)
                    index.Add(f.BundleName + ":" + f.TargetName);

                var bundleFile = Path.Combine(_env.CacheDirectory, e.Key.Name + ".bundle");
                Directory.CreateDirectory(Path.GetDirectoryName(bundleFile));
                File.WriteAllText(bundleFile, string.Join("\n", index));
                _compiler.Data.Extensions.BundleFiles.Add(new BundleFile(e.Key, e.Key.Name + ".bundle", bundleFile));
            }

            var bundleList = new List<string>();
            var bundles = _compiler.Input.Bundles.ToArray();
            Array.Sort(bundles, (a, b) => a.Name.CompareTo(b.Name));

            foreach (var e in bundles)
            {
                var bundleFile = Path.Combine(_env.CacheDirectory, e.Name + ".bundle");
                var files = _files.GetList(e);
                files.Sort((a, b) => a.BundleName.CompareTo(b.BundleName));

                if (File.Exists(bundleFile))
                    bundleList.Add(e.Name);
            }

            var bundlesFile = Path.Combine(_env.CacheDirectory, "bundles");
            using (var f = _compiler.Disk.CreateBufferedText(bundlesFile))
                f.Write(string.Join("\n", bundleList));
            _compiler.Data.Extensions.BundleFiles.Add(new BundleFile(_compiler.Input.Bundle, "bundles", bundlesFile));

            // Deprecated: The 'bundle' file is no longer used, but may be used directly by 3rdparty code.
            {
                var index = new List<string>();

                foreach (var e in bundles)
                {
                    var line = new List<string> {e.Name};
                    var files = _files.GetList(e);
                    files.Sort((a, b) => a.BundleName.CompareTo(b.BundleName));

                    foreach (var f in files)
                    {
                        line.Add(f.BundleName);
                        line.Add(f.TargetName);
                    }

                    index.Add(string.Join(":", line));
                }

                var bundleFile = Path.Combine(_env.CacheDirectory, "bundle");
                using (var f = _compiler.Disk.CreateBufferedText(bundleFile))
                    f.Write(string.Join("\n", index));
                _compiler.Data.Extensions.BundleFiles.Add(new BundleFile(_compiler.Input.Bundle, "bundle", bundleFile));
            }
        }

        void Emit(Field field)
        {
            var dt = field.DeclaringType;
            var value = _fields[field];
            _emittedFields.Add(field);

            foreach (var d in _compiler.Utilities.FindDependencies(value))
            {
                var f = d as Field;
                if (f != null && f.DeclaringType == dt &&
                    !_emittedFields.Contains(f))
                    Emit(f);
            }
            if (dt.Initializer.HasBody)
                dt.Initializer.Body.Statements.Add(new StoreField(value.Source, null, field, value));
        }

        public Expression AddProgram(DrawBlock block, Expression program)
        {
            return AddCached("Shader", () => program, block.Method.DeclaringType.UnoName, block);
        }

        public Expression AddFile(Source src, string filename)
        {
            return AddCached("File",
                () =>
                {
                    var file = CreateFile(src, filename).BundleName;
                    return GetFile(src, file);
                },
                filename);
        }

        Expression GetFile(Source src, string file)
        {
            return _ilf.CallMethod(
                src,
                _ilf.CallMethod(src,
                    _ilf.Essentials.Bundle,
                    "Get",
                    new Constant(src, _ilf.Essentials.String, src.Bundle.Name)),
                "GetFile",
                new Constant(src, _ilf.Essentials.String, file));
        }

        Expression AddCached(string type, Func<Expression> factory, params object[] descriptor)
        {
            if (descriptor.Length == 0)
                throw new ArgumentException("Expected at least one descriptor");

            var hash = type.GetHashCode();
            foreach (var e in descriptor)
                hash = 13 * hash + (e ?? "<null>").ToString().GetHashCode();

            var path = (descriptor[0] ?? "").ToString();
            if (path.IsValidPath())
                type = Path.GetFileNameWithoutExtension(path);

            Expression result;
            var key = (type + hash.ToString("x8")).ToIdentifier();
            if (!_cache.TryGetValue(key, out result))
            {
                var value = factory();
                var bundle = GetClass(value.Source.Bundle);
                var field = new Field(value.Source, bundle, key,
                    null, Modifiers.Public | Modifiers.Static,
                    FieldModifiers.ReadOnly, value.ReturnType);
                result = new LoadField(value.Source, null, field);
                bundle.Fields.Add(field);
                _fields[field] = value;
                _cache[key] = result;
            }

            return result;
        }

        ClassType GetClass(SourceBundle bundle)
        {
            if (bundle.IsUnknown)
                bundle = _compiler.Input.Bundle;

            ClassType result;
            if (!_bundles.TryGetValue(bundle, out result))
            {
                result = new ClassType(bundle.Source, _compiler.Data.IL, null, Modifiers.Generated | Modifiers.Public | Modifiers.Static, bundle.Name.ToIdentifier() + "_bundle");
                result.Initializer = new Constructor(bundle.Source, result, null, Modifiers.Static, ParameterList.Empty, new Scope(bundle.Source));
                _compiler.Data.IL.Types.Add(result);
                _bundles.Add(bundle, result);
            }

            return result;
        }

        BundleFile CreateFile(Source src, string filename, string targetName = null)
        {
            var bundleName = filename.ToRelativePath(src.Bundle.SourceDirectory).NativeToUnix();

            // Bundle transpiled FuseJS files as their original name.
            // This makes them easier to require() in the resulting app.
            if (bundleName.StartsWith(FuseJSPrefix))
                bundleName = bundleName.Substring(FuseJSPrefix.Length);

            var result = new BundleFile(src.Bundle, bundleName, targetName ?? bundleName.GetNormalizedFilename(), filename);
            _files.Add(src.Bundle, result);
            _compiler.Data.Extensions.BundleFiles.Add(result);
            return result;
        }

        // Files in this directory are written by BundleCache.cs
        const string FuseJSPrefix = ".uno/fusejs/";
    }
}
