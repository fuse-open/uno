using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.Build.FuseJS;
using Uno.Build.Libraries;
using Uno.Build.Stuff;
using Uno.Compiler;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.Frontend;
using Uno.Configuration;
using Uno.Configuration.Format;
using Uno.Diagnostics;
using Uno.IO;
using Uno.Logging;
using Uno.ProjectFormat;
using Uno.UX.Markup.CodeGeneration;

namespace Uno.Build
{
    using Compiler.Core;

    class BuildDriver : LogObject, IDisposable
    {
        readonly BuildTarget _target;
        readonly BuildOptions _options;
        readonly CompilerOptions _compilerOptions;
        readonly Compiler _compiler;
        readonly BuildEnvironment _env;
        readonly Backend _backend;
        readonly SourceReader _input;
        readonly Project _project;
        readonly UnoConfig _config;
        readonly BuildFile _file;
        readonly BundleCache _cache;
        readonly LazyTranspiler _transpiler;
        IDisposable _anim;

        public bool IsUpToDate => !_options.Force && _env.HasUpToDateOptions &&
                                  !_input.HasAnythingChangedSince(_file.Timestamp);
        public bool CanBuildNative => _options.NativeBuild && _target.CanBuild(_file) && (
                                      _options.Force ||
                                      !_file.IsProductUpToDate);

        public string ProductPath => !string.IsNullOrEmpty(_file.Product)
                                    ? Path.Combine(_file.RootDirectory, _file.Product).UnixToNative()
                                        .Replace(Path.DirectorySeparatorChar + "." + Path.DirectorySeparatorChar,
                                                Path.DirectorySeparatorChar.ToString())
                                    : null;

        public void StopAnim()
        {
            _anim?.Dispose();
            _anim = null;
        }

        public BuildDriver(Log log, BuildTarget target, BuildOptions options, Project project, UnoConfig config)
            : base(log)
        {
            _target = target;
            _options = options;
            _project = project;
            _config = config;
            Log.MaxErrorCount = options.MaxErrorCount;
            Log.WarningLevel = options.WarningLevel;

            Log.Reset();
            if (target.IsObsolete)
                Log.Warning("The build target " + target.Quote() + " is obsolete and might not work any more.");

            _anim = Log.StartAnimation("Configuring");
            PrintRow("Project file", project.FullPath);

            _compilerOptions = new CompilerOptions {
                BuildTarget = _target.Identifier,
                Configuration = _options.Configuration.ToString(),
                OutputDirectory = !string.IsNullOrEmpty(_options.OutputDirectory)
                    ? Path.GetFullPath(_options.OutputDirectory)
                    : project.OutputDirectory,
                MainClass = _options.MainClass,
                Debug = _options.Configuration != BuildConfiguration.Release,
                Parallel = _options.Parallel,
                Strip = _options.Strip ?? _target.DefaultStrip,
                CanCacheIL = _options.BundleCache != null
            };

            if (_options.Defines.Contains("LAZY"))
            {
                if (!target.SupportsLazy)
                    Log.Warning("The build target " + target.Quote() + " does not support lazy compilation.");
                else
                    _compilerOptions.Lazy = true;
            }

            if (_options.Test)
            {
                _options.Defines.Add("TEST");
                _compilerOptions.TestOptions = new TestOptions {
                    Filter = _options.TestFilter
                };
            }
            else if (project.OutputType == OutputType.Test || project.OutputType == OutputType.AppTest)
            {
                Log.Warning(project.Source, ErrorCode.W0000, "Test projects should be built & run using \"uno test\"");
            }

            _transpiler = new LazyTranspiler(Log, _config);
            _cache = _options.BundleCache ?? new BundleCache(Log, _config, _transpiler);
            PrintRow("Search paths", _cache.SearchPaths);

            _compiler = new Compiler(
                Log,
                _target.CreateBackend(config),
                GetBundle(),
                _compilerOptions);
            _env = _compiler.Environment;
            _backend = _compiler.Backend;
            _input = _compiler.Input;

            if (_options.Clean)
                _compiler.Disk.DeleteDirectory(_env.OutputDirectory);

            _file = new BuildFile(_env.OutputDirectory);
            _env.HasUpToDateOptions = _file.Exists && _file.Load() == GetHashCode();
        }

        SourceBundle GetBundle()
        {
            using (Log.StartProfiler(_cache))
                return _cache.GetBundle(_project);
        }

        public void Dispose()
        {
            _transpiler.Dispose();
        }

        public void Clean()
        {
            _compiler.Disk.DeleteDirectory(_env.CacheDirectory);
            _compiler.Disk.DeleteDirectory(_env.OutputDirectory);
        }

        public BackendResult Build()
        {
            if (Log.HasErrors)
                return null;

            PrintRow("References", _input.Bundles);
            PrintRow("Output dir", _env.OutputDirectory);

            _file.Delete();
            _env.Define(_backend.Name, _backend.ShaderBackend.Name);

            if (_compilerOptions.Debug)
                _env.Define("DEBUG");
            if (_options.Configuration != BuildConfiguration.Debug)
                _env.Define(_options.Configuration.ToString());
            if (_options.Configuration == BuildConfiguration.Preview)
                _env.Define("REFLECTION", "SIMULATOR", "STACKTRACE");
            if (_project.OutputType != OutputType.Undefined)
                _env.Define(_project.OutputType.ToString());

            foreach (var def in StuffFile.DefaultDefines)
                _env.Define("HOST_" + def);
            foreach (var def in _options.Defines)
                _env.Define(def);
            foreach (var def in (_project.GetString(_target.ProjectGroup + ".defines") ?? "").Split('\n'))
                if (!string.IsNullOrEmpty(def))
                    _env.Define(def);

            _target.Initialize(_env);

            foreach (var def in _options.Undefines)
                _env.Undefine(def);

            if (!_env.Test(_project.Source, _project.BuildCondition))
            {
                if (Log.IsVerbose)
                    Log.Skip();
                Log.WriteLine("\"buildCondition\" in project file is not satisfied -- stopping build");
                Log.DisableSkip();
                StopAnim();
                return null;
            }

            foreach (var p in _project.GetProperties(Log))
                _env.Set("project." + p.Key, p.Value);
            foreach (var p in _config.Flatten())
                _env.Set("config." + p.Key, GetConfigValue(p.Key, p.Value));
            foreach (var e in _options.Settings)
                _env.Set(e.Key, GetCommandLineValue(e.Value), Disambiguation.Override);

            _env.Set("uno", _config.GetFullPath("uno.command").QuoteSpace());

            if (Log.HasErrors)
                return null;

            using (Log.StartProfiler(typeof(UXProcessor)))
                UXProcessor.Build(_compiler.Disk, _input.Bundles, _transpiler);

            if (Log.HasErrors)
                return null;

            try
            {
                _compiler.Load();
            }
            finally
            {
                StopAnim();
            }

            if (Log.HasErrors)
                return null;

            var defines = _compiler.Data.Extensions.Defines;
            var stuff = GetDirtyStuff(_env, defines, _input.Bundles);

            if (stuff.Count > 0)
            {
                using (Log.StartAnimation("Installing dependencies"))
                {
                    foreach (var f in stuff)
                        if (!Installer.Install(Log, f, 0, defines))
                            Log.Error(new Source(f), null, "Failed to install dependencies");
                }
            }

            if (Log.HasErrors)
                return null;

            using (Log.StartAnimation("Compiling syntax tree"))
                _compiler.Compile();

            if (Log.HasErrors)
                return null;

            _anim = Log.StartAnimation("Generating " + _compiler.Backend.What);

            try
            {
                return _compiler.Generate(_target.Configure);
            }
            finally
            {
                // Add flag to avoid repeating warnings when this bundle is reused in following builds (uno doctor).
                _compiler.Input.Bundle.Flags |= SourceBundleFlags.Verified;

                _file.Product = _env.GetString("product");
                _file.BuildCommand = _env.GetString("commands.build");
                _file.RunCommand = _env.GetString("commands.run");
                _file.UnoVersion = UnoVersion.InformationalVersion;

                if (!Log.HasErrors)
                {
                    _file.Save(GetHashCode());
                    _target.DeleteOutdated(_compiler.Disk, _env);
                }

                StopAnim();
            }
        }

        public void BuildNative()
        {
            using (Log.StartAnimation("Building " + _target.ToString().ToLower() + " " + (
                        _env.IsTest
                            ? "test" :
                        _env.IsConsole
                            ? "console app" :
                        _env.IsLibrary
                            ? "library"
                            : "app"
                    ) +
                    " (" + _compilerOptions.Configuration.ToString().ToLower() + ")"))
            {
                if (!_target.Build(_compiler.Shell, _file, _options.NativeArguments))
                    Log.Error(Source.Unknown, ErrorCode.E0200, _target + " build failed");
                else
                    _file.TouchProduct();
            }
        }

        public void PrintInternals()
        {
            var properties = _compiler.Data.Extensions.Properties.ToArray();
            Array.Sort(properties, (a, b) => string.Compare(a.Key, b.Key, StringComparison.InvariantCulture));

            var defines = _compiler.Data.Extensions.Defines.ToArray();
            Array.Sort(defines);

            Log.H1("Build internals");

            foreach (var p in properties)
                Log.WriteLine(p.Key.PadRight(40) + " " + Clamp(_env.GetString(p.Key)));

            Log.H2("Defines");
            Log.WriteLine(string.Join(" ", defines));
        }

        void PrintRow(string description, IEnumerable<object> paths)
        {
            if (!Log.IsVerbose)
                return;

            PrintRow(description, string.Join(", ", paths.Select(x => x.ToString().ToRelativePath())));
        }

        void PrintRow(string description, string path)
        {
            if (!Log.IsVerbose)
                return;

            Log.WriteLine(description + ":" + new string(' ', 13 - description.Length) + path.ToRelativePath());
        }

        public override int GetHashCode()
        {
            var hash = 13 * _options.GetHashCode() + _target.Identifier.GetHashCode();

            foreach (var bundle in _compiler.Input.Bundles)
                if (bundle.IsCached && bundle.Version != null)
                    hash = hash * 13 + bundle.Version.GetHashCode();

            return hash * 13 + UnoVersion.InformationalVersion.GetHashCode();
        }

        public BuildResult GetResult(BackendResult result)
            => new BuildResult(Log, _project, _target, _compiler, _options, _file, result);

        List<string> GetDirtyStuff(
            IEnvironment env,
            IEnumerable<string> defines,
            IEnumerable<SourceBundle> bundles)
        {
            var stuff = new List<string>();
            foreach (var bundle in bundles)
                foreach (var file in bundle.StuffFiles)
                    if (env.Test(bundle.Source, file.Condition) &&
                            !Installer.IsUpToDate(Log, Path.Combine(bundle.SourceDirectory, file.NativePath), 0, defines))
                        stuff.Add(Path.Combine(bundle.SourceDirectory, file.NativePath));
            return stuff;
        }

        SourceValue GetConfigValue(string key, StuffItem value)
            => new SourceValue(new Source(value.File.Filename, value.LineNumber), _config.GetString(key));

        SourceValue GetCommandLineValue(string s)
            => new SourceValue(new Source(Path.Combine(Directory.GetCurrentDirectory(), "uno")), s);

        string Clamp(string value)
            => ClampSingleLine(
                value != null && value.Contains('\n') && !Log.IsVeryVerbose
                    ? "(multi-line) " + value.Substring(0, value.IndexOf('\n')).TrimEnd()
                    : value);

        string ClampSingleLine(string value, int maxLength = 64)
            => value != null && value.Length > maxLength && !Log.IsVerbose
                ? value.Substring(0, maxLength - 3) + "..."
                : value;
    }
}
