using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.IO;
using IKVM.Reflection;
using IKVM.Reflection.Emit;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.IL;
using Uno.Diagnostics;
using Uno.IO;

namespace Uno.Compiler.Backends.CIL
{
    partial class CilGenerator : DiskObject
    {
        readonly SourceBundle _bundle;
        readonly IBuildData _data;
        readonly IEssentials _essentials;
        readonly CilLinker _linker;
        readonly CilBackend _backend;
        readonly ModuleBuilder _module;
        readonly AssemblyBuilder _assembly;
        readonly CilTypeFactory _types;
        readonly List<DataType> _linkedTypes = new();
        readonly Dictionary<string, ISymbolDocumentWriter> _documents = new();
        readonly string _outputDir;

        public readonly SortedSet<Location> Locations = new();
        public Assembly Assembly => _assembly;

        public CilGenerator(Disk disk, IBuildData data, IEssentials essentials,
                            CilBackend backend, CilLinker linker, SourceBundle bundle,
                            string outputDir)
            : base(disk)
        {
            _data = data;
            _essentials = essentials;
            _backend = backend;
            _bundle = bundle;
            _linker = linker;
            _outputDir = outputDir;
            _assembly = _linker.Universe.DefineDynamicAssembly(
                new AssemblyName(bundle.Name) {Version = bundle.ParseVersion(Log)},
                AssemblyBuilderAccess.Save,
                outputDir);
            _module = _assembly.DefineDynamicModule(
                bundle.Name,
                bundle.Name + ".dll",
                true);
            _types = new CilTypeFactory(backend, essentials, linker, _module);
        }

        public void Configure(bool debug)
        {
            if (debug)
                AddAssemblyAttribute(
                    _linker.System_Diagnostics_DebuggableAttribute_ctor,
                    DebuggableAttribute.DebuggingModes.DisableOptimizations | DebuggableAttribute.DebuggingModes.Default);

            foreach (var name in _bundle.InternalsVisibleTo)
                AddAssemblyAttribute(
                    _linker.System_Runtime_CompilerServices_InternalsVisibleToAttribute_ctor,
                    name);

            AddAssemblyAttribute(
                _linker.System_Reflection_AssemblyMetadataAttribute_ctor,
                "Uno.Version", UnoVersion.InformationalVersion);
        }


        public void Save()
        {
            // Whether to enable DllImportResolver in app-loaders
            AddAssemblyAttribute(
                _linker.System_Reflection_AssemblyMetadataAttribute_ctor,
                "Uno.DllImportResolver", _linker.PInvoke.ToString());

            var streams = new List<Stream>();

            try
            {
                // Embed resources (bundle files)
                foreach (var file in _data.Extensions.BundleFiles)
                {
                    if (file.Bundle != _bundle)
                        continue;

                    var stream = File.OpenRead(file.SourcePath);
                    streams.Add(stream);
                    _module.DefineManifestResource(file.TargetName, stream, ResourceAttributes.Public);
                }

                // Save assembly
                Disk.CreateDirectory(_outputDir);
                _assembly.Save(_assembly.GetName().Name + ".dll");
            }
            finally
            {
                // Clean up
                foreach (var stream in streams)
                    stream.Dispose();

                streams.Clear();
            }

            // Copy referenced assemblies
            if (_bundle.IsStartup)
            {
                foreach (var asm in _linker.CopyAssemblies)
                {
                    foreach (var m in asm.GetModules())
                    {
                        var dll = m.FullyQualifiedName;
                        var pdb = string.Concat(dll.AsSpan(dll.Length - 4), ".pdb");

                        if (File.Exists(dll))
                            Disk.CopyFile(dll, Path.Combine(_outputDir, Path.GetFileName(dll)));

                        if (File.Exists(pdb))
                            Disk.CopyFile(pdb, Path.Combine(_outputDir, Path.GetFileName(pdb)));
                    }
                }
            }
        }

        void AddAssemblyAttribute(ConstructorInfo ctor, params object[] args)
        {
            _assembly.SetCustomAttribute(new CustomAttributeBuilder(ctor, args));
        }

        void AddLocation(int ilOffset, string path, int line, int column)
        {
            Locations.Add(new Location(ilOffset, path, line, column));
        }

        ISymbolDocumentWriter GetDocument(string path)
        {
            if (_documents.TryGetValue(path, out ISymbolDocumentWriter result))
                return result;

            result = _module.DefineDocument(path, Guid.Empty, Guid.Empty, Guid.Empty);
            _documents.Add(path, result);
            return result;
        }
    }
}
