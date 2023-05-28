﻿using IKVM.Reflection;
using IKVM.Reflection.Emit;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.IL;
using Uno.IO;

namespace Uno.Compiler.Backends.CIL
{
    class MetadataGenerator : DiskObject
    {
        readonly SourceBundle _bundle;
        readonly IBuildData _data;
        readonly IEssentials _essentials;
        readonly CilLinker _linker;
        readonly MetadataBackend _backend;
        readonly AssemblyBuilder _assembly;
        readonly CilTypeFactory _types;
        readonly string _outputDir;

        public MetadataGenerator(Disk disk, IBuildData data, IEssentials essentials,
                                 MetadataBackend backend, CilLinker linker, SourceBundle bundle,
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
            var module = _assembly.DefineDynamicModule(
                bundle.Name,
                bundle.Name + ".dll",
                true);
            _types = new CilTypeFactory(backend, essentials, linker, module);
        }

        public void Configure()
        {
            foreach (var name in _bundle.InternalsVisibleTo)
                _assembly.SetCustomAttribute(
                    new CustomAttributeBuilder(
                        _linker.System_Runtime_CompilerServices_InternalsVisibleToAttribute_ctor,
                        new object[] {name}));
        }

        public void Save()
        {
            Disk.CreateDirectory(_outputDir);
            _assembly.Save(_assembly.GetName().Name + ".dll");
        }

        public void Generate()
        {
            Process(_data.IL);

            foreach (var e in _types)
                e.Populate();
            foreach (var e in _types)
                e.Builder.CreateType();

            Log.Verbose("Generated " + _types.Count + " .NET type".Plural(_types) + " for " + _assembly.GetName().Name.Quote() + " assembly");
        }

        void Process(Namespace root)
        {
            foreach (var dt in root.Types)
                if (dt.Source.Bundle == _bundle)
                    _types.DefineType(dt);

            foreach (var ns in root.Namespaces)
                Process(ns);
        }
    }
}
