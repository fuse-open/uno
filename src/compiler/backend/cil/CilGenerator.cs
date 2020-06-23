using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.IO;
using IKVM.Reflection;
using IKVM.Reflection.Emit;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.IO;

namespace Uno.Compiler.Backends.CIL
{
    partial class CilGenerator : DiskObject
    {
        readonly SourcePackage _package;
        readonly IBuildData _data;
        readonly IEssentials _essentials;
        readonly CilLinker _linker;
        readonly CilBackend _backend;
        readonly ModuleBuilder _module;
        readonly AssemblyBuilder _assembly;
        readonly CilTypeFactory _types;
        readonly List<DataType> _linkedTypes = new List<DataType>();
        readonly Dictionary<string, ISymbolDocumentWriter> _documents = new Dictionary<string, ISymbolDocumentWriter>();
        readonly string _outputDir;

        public readonly SortedSet<Location> Locations = new SortedSet<Location>();
        public Assembly Assembly => _assembly;

        public CilGenerator(Disk disk, IBuildData data, IEssentials essentials,
                            CilBackend backend, CilLinker linker, SourcePackage package,
                            string outputDir)
            : base(disk)
        {
            _data = data;
            _essentials = essentials;
            _backend = backend;
            _package = package;
            _linker = linker;
            _outputDir = outputDir;
            _assembly = _linker.Universe.DefineDynamicAssembly(
                new AssemblyName(package.Name) {Version = package.ParseVersion(Log)},
                AssemblyBuilderAccess.Save,
                outputDir);
            _module = _assembly.DefineDynamicModule(
                package.Name, 
                package.Name + ".dll", 
                true);
            _types = new CilTypeFactory(backend, essentials, linker, _module);
        }

        public void Configure(bool debug)
        {
            if (debug)
                _assembly.SetCustomAttribute(
                    new CustomAttributeBuilder(
                        _linker.System_Diagnostics_DebuggableAttribute_ctor, 
                        new object[] {DebuggableAttribute.DebuggingModes.DisableOptimizations | DebuggableAttribute.DebuggingModes.Default}));

            foreach (var name in _package.InternalsVisibleTo)
                _assembly.SetCustomAttribute(
                    new CustomAttributeBuilder(
                        _linker.System_Runtime_CompilerServices_InternalsVisibleToAttribute_ctor,
                        new object[] {name}));
        }

        public void Save()
        {
            var streams = new List<Stream>();

            try
            {
                // Embed resources (bundle files)
                foreach (var file in _data.Extensions.BundleFiles)
                {
                    if (file.Package != _package)
                        continue;

                    var stream = File.OpenRead(file.SourcePath);
                    streams.Add(stream); // Add it here before we give the stream away
                    _module.DefineManifestResource(file.TargetName, stream, ResourceAttributes.Public);
                }

                // Output assembly
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
            if (_package.IsStartup)
            {
                foreach (var asm in _linker.CopyAssemblies)
                {
                    foreach (var m in asm.GetModules())
                    {
                        var dll = m.FullyQualifiedName;
                        var pdb = dll.Substring(dll.Length - 4) + ".pdb";

                        if (File.Exists(dll))
                            Disk.CopyFile(dll, Path.Combine(_outputDir, Path.GetFileName(dll)));

                        if (File.Exists(pdb))
                            Disk.CopyFile(pdb, Path.Combine(_outputDir, Path.GetFileName(pdb)));
                    }
                }
            }
        }

        void AddLocation(int ilOffset, string path, int line, int column)
        {
            Locations.Add(new Location(ilOffset, path, line, column));
        }

        ISymbolDocumentWriter GetDocument(string path)
        {
            ISymbolDocumentWriter result;
            if (_documents.TryGetValue(path, out result))
                return result;

            result = _module.DefineDocument(path, Guid.Empty, Guid.Empty, Guid.Empty);
            _documents.Add(path, result);
            return result;
        }
    }
}
