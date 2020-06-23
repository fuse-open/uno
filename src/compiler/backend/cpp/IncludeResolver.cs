using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Backends.CPlusPlus
{
    class IncludeResolver
    {
        readonly CppBackend _backend;
        readonly IEnvironment _env;

        internal IncludeResolver(
            CppBackend backend,
            IEnvironment env)
        {
            _backend = backend;
            _env = env;
        }

        public string[] GetIncludes(Function f)
        {
            var types = new HashSet<DataType>();
            var includes = new HashSet<string>();

            Add(types, _backend.GetDependencies(f));

            foreach (var t in types)
                includes.Add(_backend.GetIncludeFilename(t));

            var result = includes.ToArray();
            Array.Sort(result);
            return result;
        }

        public Declarations GetDeclarations(DataType dt, CppType type)
        {
            var baseTypes = new HashSet<DataType>();
            var fieldTypes = new HashSet<DataType>();
            var hIncludes = new HashSet<string>();
            var hDeclarations = new HashSet<string>();
            var hTypes = new HashSet<DataType>();
            var incIncludes = new HashSet<string>();
            var incTypes = new HashSet<DataType>();
            var cppTypes = new HashSet<DataType>();
            var cppIncludes = new HashSet<string>();
            var cppDeclarations = new HashSet<string>();

            Resolve(dt, baseTypes, fieldTypes, hTypes, incTypes, cppTypes);

            foreach (var t in type.Dependencies)
                Add(cppTypes, t);
            foreach (var t in type.PrecalcedTypes)
                Add(cppTypes, t);
            foreach (var m in type.MethodTypes)
            {
                foreach (var t in m.Dependencies)
                    Add(cppTypes, t);
                foreach (var t in m.PrecalcedTypes)
                    Add(cppTypes, t);
            }

            if (_backend.HasForwardDeclaration(dt))
                hDeclarations.Add(_backend.GetForwardDeclaration(dt));

            foreach (var e in fieldTypes)
                if (e.IsStruct && !_backend.IsOpaque(e))
                    baseTypes.Add(e.MasterDefinition);
                else
                    hTypes.Add(e.MasterDefinition);

            foreach (var e in baseTypes)
            {
                hIncludes.Add(_backend.GetIncludeFilename(e));
                hTypes.Remove(e.MasterDefinition);
                incTypes.Remove(e.MasterDefinition);
                cppTypes.Add(e.MasterDefinition);
            }

            foreach (var e in hTypes)
            {
                hDeclarations.Add(_backend.GetForwardDeclaration(e));
                cppTypes.Add(e.MasterDefinition);
            }

            foreach (var e in incTypes)
                incIncludes.Add(_backend.GetIncludeFilename(e));
            foreach (var e in cppTypes)
                cppIncludes.Add(_backend.GetIncludeFilename(e));

            if (hIncludes.Count == 0)
                hIncludes.Add("uno.h");

            foreach (var e in _env.GetSet(dt, "Header.Declaration"))
                hDeclarations.Add(e.Trim());
            foreach (var e in _env.GetSet(dt, "Header.Include"))
                hIncludes.Add(e.Trim());
            foreach (var e in _env.GetSet(dt, "Header.Import"))
                hDeclarations.Add("#import <" + e.Trim() + ">");

            foreach (var e in _env.GetSet(dt, "Source.Declaration"))
                cppDeclarations.Add(e.Trim());
            foreach (var e in _env.GetSet(dt, "Source.Include"))
                cppIncludes.Add(e.Trim());
            foreach (var e in _env.GetSet(dt, "Source.Import"))
                cppDeclarations.Add("#import <" + e.Trim() + ">");

            SourceValue val;
            if (_env.TryGetValue(dt, "ForwardDeclaration", out val))
                hDeclarations.Add(val.String.Trim());
            if (_env.TryGetValue(dt, "Include", out val))
                hIncludes.Add(val.String.Trim());

            foreach (var e in hIncludes)
            {
                cppIncludes.Remove(e);
                incIncludes.Remove(e);
            }
            foreach (var e in incIncludes)
                cppIncludes.Remove(e);

            foreach (var e in hIncludes)
                hDeclarations.Add("#include <" + e + ">");
            foreach (var e in cppIncludes)
                cppDeclarations.Add("#include <" + e + ">");

            hDeclarations.Remove(null);
            cppDeclarations.Remove(null);
            incIncludes.Remove(null);

            var inlineDeclarations = incIncludes.ToArray();
            for (int i = 0; i < inlineDeclarations.Length; i++)
                inlineDeclarations[i] = "#include <" + inlineDeclarations[i] + ">";

            var result = new Declarations
            {
                Header = hDeclarations.ToArray(),
                Source = cppDeclarations.ToArray(),
                Inline = inlineDeclarations,
            };

            Array.Sort(result.Header);
            Array.Sort(result.Source);
            Array.Sort(result.Inline);
            return result;
        }

        void Resolve(DataType dt, HashSet<DataType> baseTypes, HashSet<DataType> fieldTypes, HashSet<DataType> hTypes, HashSet<DataType> incTypes, HashSet<DataType> cppTypes)
        {
            if (dt.Base != null)
                Add(baseTypes, dt.Base);

            foreach (var it in dt.Interfaces)
                Add(baseTypes, it);

            switch (dt.TypeType)
            {
                case TypeType.Delegate:
                {
                    Add(cppTypes, dt.ReturnType);

                    foreach (var p in dt.Parameters)
                        Add(cppTypes, p.Type);
                    break;
                }
                default:
                {
                    foreach (var f in dt.EnumerateFields())
                        Add(f.IsStatic 
                                ? hTypes 
                                : fieldTypes,
                            f.ReturnType);

                    foreach (var f in dt.EnumerateFunctions())
                    {
                        Add(hTypes, f.ReturnType);

                        foreach (var p in f.Parameters)
                            Add(hTypes, p.Type);

                        if (!f.CanLink)
                            Add(cppTypes, _backend.GetDependencies(f));

                        if (f.ReturnType.IsStruct &&
                            !_env.HasProperty(f.ReturnType, "TypeName"))
                            Add(incTypes, f.ReturnType);

                        foreach (var p in f.Parameters)
                            if (!p.IsReference && p.Type.IsStruct &&
                                !_env.HasProperty(p.Type, "TypeName"))
                                Add(incTypes, p.Type);
                    }

                    break;
                }
            }

            baseTypes.Remove(dt.MasterDefinition);
            fieldTypes.Remove(dt.MasterDefinition);
            hTypes.Remove(dt.MasterDefinition);
            incTypes.Remove(dt.MasterDefinition);
            cppTypes.Add(dt.MasterDefinition);
        }

        void Add(HashSet<DataType> set, DataType dt)
        {
            for (;;)
            {
                switch (dt.TypeType)
                {
                    case TypeType.GenericParameter:
                        dt = dt.Base;
                        continue;
                    case TypeType.RefArray:
                        dt = dt.ElementType;
                        continue;
                    case TypeType.Enum:
                    case TypeType.Struct:
                    case TypeType.Class:
                    case TypeType.Delegate:
                    case TypeType.Interface:
                        if (dt.IsFlattenedParameterization)
                            foreach (var a in dt.FlattenedArguments)
                                Add(set, a);

                        set.Add(dt.IsGenericMethodType
                            ? dt.ParentType.MasterDefinition
                            : dt.MasterDefinition);
                        return;
                    default:
                        return;
                }
            }
        }

        void Add(HashSet<DataType> set, IEnumerable<IEntity> entities)
        {
            foreach (var dt in entities)
                if (dt is DataType)
                    Add(set, dt as DataType);
        }
    }
}

