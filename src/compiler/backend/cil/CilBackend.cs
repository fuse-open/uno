using System;
using System.IO;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.IL;
using Uno.IO;

namespace Uno.Compiler.Backends.CIL
{
    public class CilBackend : Backend
    {
        CilLinker _linker;
        string _outputDir;

        internal bool EnableReflection { get; private set; }
        internal DataType TypeAliasAttribute { get; private set; }

        public override string Name => "CIL";

        public CilBackend(ShaderBackend shaderBackend)
            : base(shaderBackend)
        {
            FunctionOptions =
                FunctionOptions.DecodeEnumOps |
                FunctionOptions.DecodeDelegateOps |
                FunctionOptions.DecodeSwizzles |
                FunctionOptions.ClosureConvert |
                FunctionOptions.Bytecode;
        }

        public override void Configure()
        {
            _outputDir = Environment.Combine(
                Environment.ExpandSingleLine("@(AssemblyDirectory || '.')")).TrimPath();
            _linker = new CilLinker(Log, Essentials);
            Scheduler.AddTransform(new CilTransform(this));
            EnableReflection = Environment.IsDefined("REFLECTION");
            TypeAliasAttribute = EnableReflection
                                ? ILFactory.GetType("Uno.Reflection.TypeAliasAttribute")
                                : DataType.Invalid;
        }

        public override bool CanLink(SourcePackage upk)
        {
            return Environment.IsUpToDate(upk, upk.Name + ".dll");
        }

        public override bool CanLink(DataType dt)
        {
            // Can't check DotNetTypeAttribute because we don't know if any members with DotNetOverrideAttriute exist.
            return dt.Package.CanLink;
        }

        public override bool CanLink(Function f)
        {
            return f.DeclaringType.CanLink ||
                f.DeclaringType.HasAttribute(Essentials.DotNetTypeAttribute, true) && 
                    !f.HasAttribute(Essentials.DotNetOverrideAttribute);
        }

        public override void BeginBuild()
        {
            foreach (var e in Environment.Enumerate("Assembly"))
            {
                try
                {
                    var fullPath = e.GetFullPath();
                    if (File.Exists(fullPath))
                        _linker.AddAssemblyFile(fullPath, true);
                    else
                        _linker.AddAssembly(e.String);
                }
                catch (Exception ex)
                {
                    Log.Error(e.Source, ErrorCode.E0000, "Failed to load assembly " + e.String.Quote() + ": " + ex.Message);
                    Log.Trace(ex);
                }
            }
        }

        public override void EndBuild()
        {
            if (Environment.IsDefined("X64"))
                foreach (var e in Environment.Enumerate("UnmanagedLibrary.x64"))
                    Environment.Require("UnmanagedLibrary", e);
            else if (Environment.IsDefined("X86"))
                foreach (var e in Environment.Enumerate("UnmanagedLibrary.x86"))
                    Environment.Require("UnmanagedLibrary", e);

            // Copy native libraries
            foreach (var e in Environment.Enumerate("UnmanagedLibrary"))
                Disk.CopyFile(e, _outputDir.UnixToNative());

            // Check if we need AppLoader
            if (Environment.IsDefined("LIBRARY") ||
                    string.IsNullOrEmpty(Environment.GetString("AppLoader.Assembly")))
                return;

            // Create an executable for given architecture (-DX86 or -DX64)
            using (Log.StartProfiler(typeof(AppLoader)))
            {
                var loader = new AppLoader(Environment.GetString("AppLoader.Assembly"));
                var executable = Environment.Combine(Environment.GetString("Product").TrimPath());

                if (Environment.IsDefined("X64"))
                    loader.SetX64();
                else if (Environment.IsDefined("X86"))
                    loader.SetX86();

                Log.Verbose("Creating executable: " + executable.ToRelativePath() + " (" + loader.Architecture + ")");
                loader.SetAssemblyInfo(Input.Package.Name + "-loader",
                    Input.Package.ParseVersion(Log),
                    Environment.GetString);
                loader.SetMainClass(Data.MainClass.CilTypeName(),
                    Path.Combine(_outputDir, Input.Package.Name + ".dll"),
                    Environment.GetString("AppLoader.Class"),
                    Environment.GetString("AppLoader.Method"));
                loader.ClearPublicKey();
                loader.Save(executable);
            }
        }

        public override BackendResult Build(SourcePackage package)
        {
            if (package.CanLink)
            {
                package.Tag = _linker.AddAssemblyFile(Path.Combine(_outputDir, package.Name + ".dll"));
                return null;
            }

            var g = new CilGenerator(Disk, Data, Essentials,
                                     this, _linker, package, _outputDir);
            g.Configure(Environment.Debug);

            using (Log.StartProfiler(g.GetType().FullName + ".Generate"))
                g.Generate();

            if (Log.HasErrors)
                return null;

            using (Log.StartProfiler(g.GetType().FullName + ".Save"))
                g.Save();

            return new CilResult(g.Assembly, _linker.TypeMap, g.Locations);
        }
    }
}
