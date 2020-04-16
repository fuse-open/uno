using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Uno.Collections;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends.Decompilers;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Backends.CPlusPlus
{
    class CppGenerator : IDisposable
    {
        public static void ExportType(
            IEnvironment environment,
            IEssentials essentials,
            CppBackend backend,
            DataType dt)
        {
            using (var w = new CppGenerator(
                    environment,
                    essentials,
                    backend.GetNamespaceNames(dt),
                    backend))
                w.ExportType(dt);
        }

        public static void ExportTypes(
            IEnvironment environment,
            IEssentials essentials,
            CppBackend backend,
            Namespace ns,
            string key,
            List<DataType> list)
        {
            using (var w = new CppGenerator(
                    environment,
                    essentials,
                    backend.GetNamespaceNames(ns),
                    backend))
                w.ExportTypes(key, list);
        }

        readonly IEnvironment _env;
        readonly IEssentials _essentials;
        readonly CppBackend _backend;
        readonly List<string> _namespaces;
        readonly Dictionary<string, int> _stringConsts = new Dictionary<string, int>();
        readonly Dictionary<DataType, int> _typeObjects = new Dictionary<DataType, int>();
        CppWriter _cpp;
        CppWriter _h;

        CppGenerator(
            IEnvironment env,
            IEssentials essentials,
            List<string> namespaces,
            CppBackend backend)
        {
            _env = env;
            _essentials = essentials;
            _namespaces = namespaces;
            _backend = backend;
        }

        public void Dispose()
        {
            _cpp?.Dispose();
            _h?.Dispose();
        }

        void ExportType(DataType dt)
        {
            var type = _backend.GetType(dt);

            _typeObjects.Clear();
            _stringConsts.Clear();
            foreach (var e in type.StringConsts)
                if (!_stringConsts.ContainsKey(e))
                    _stringConsts[e] = _stringConsts.Count;
            foreach (var e in type.TypeObjects)
                if (!_typeObjects.ContainsKey(e))
                    _typeObjects[e] = _typeObjects.Count;

            var basename = _backend.GetExportName(dt);
            var cppFilename = basename + "." + _backend.GetFileExtension(dt);
            var hFilename = basename + ".h";

            _env.Require("SourceFile", cppFilename);
            _env.Require("HeaderFile", hFilename);

            _cpp = CreateWriter(Path.Combine(_backend.SourceDirectory, cppFilename.UnixToNative()));
            _h = CreateWriter(Path.Combine(_backend.HeaderDirectory, hFilename.UnixToNative()));

            _cpp.WriteOrigin(dt.Source);
            _h.WriteOrigin(dt.Source);
            _h.WriteLine("#pragma once");

            foreach (var decl in type.Declarations.Header)
                _h.WriteLine(decl);

            _h.Skip();

            foreach (var decl in type.Declarations.Source)
                _cpp.WriteLine(decl);

            // Negate is correct here.
            // When true these are emitted into the header file.
            if (!HasInlineMethods(type))
                foreach (var decl in type.Declarations.Inline)
                    _cpp.WriteLine(decl);

            _cpp.DeclareGlobals();
            _cpp.Skip();

            _cpp.BeginNamespace(_namespaces);
            _h.BeginNamespace(_namespaces);

            EmitType(type, dt);

            _cpp.EndNamespace(_namespaces);
            _h.EndNamespace(_namespaces);
        }

        void ExportTypes(string key, List<DataType> list)
        {
            // Group types by file extension (you know, there could be some Obj-C here)
            var extTypes = new ListDictionary<string, DataType>();

            foreach (var dt in list)
                extTypes.Add(_backend.GetFileExtension(dt), dt);

            foreach (var ext in extTypes)
            {
                var count = ext.Value.Count;
                var types = new CppType[count];
                var declSet = new HashSet<string>();
                ext.Value.Sort();

                _typeObjects.Clear();
                _stringConsts.Clear();

                for (int i = 0; i < ext.Value.Count; i++)
                {
                    types[i] = _backend.GetType(ext.Value[i]);

                    foreach (var e in types[i].Declarations.Source)
                        declSet.Add(e);

                    // Negate is correct here.
                    // When true these are emitted into the header file.
                    if (!HasInlineMethods(types[i]))
                        foreach (var e in types[i].Declarations.Inline)
                            declSet.Add(e);

                    foreach (var e in types[i].StringConsts)
                        if (!_stringConsts.ContainsKey(e))
                            _stringConsts[e] = _stringConsts.Count;
                    foreach (var e in types[i].TypeObjects)
                        if (!_typeObjects.ContainsKey(e))
                            _typeObjects[e] = _typeObjects.Count;
                }

                var declarations = declSet.ToArray();
                Array.Sort(declarations);

                var cppFilename = key + "." + ext.Key;
                _env.Require("SourceFile", cppFilename);

                _cpp?.Dispose();
                _cpp = CreateWriter(Path.Combine(_backend.SourceDirectory, cppFilename.UnixToNative()));
                _cpp.WriteOrigin(new Source("(multiple files)"));

                foreach (var e in declarations)
                    _cpp.WriteLine(e);

                _cpp.DeclareGlobals();
                _cpp.Skip();
                _cpp.BeginNamespace(_namespaces);

                for (int i = 0; i < ext.Value.Count; i++)
                {
                    var dt = ext.Value[i];
                    var hFilename = _backend.GetExportName(dt) + ".h";
                    _env.Require("HeaderFile", hFilename);

                    _h?.Dispose();
                    _h = CreateWriter(Path.Combine(_backend.HeaderDirectory, hFilename.UnixToNative()));
                    _h.WriteOrigin(dt.Source);
                    _h.WriteLine("#pragma once");

                    foreach (var decl in types[i].Declarations.Header)
                        _h.WriteLine(decl);

                    _h.Skip();
                    _h.BeginNamespace(_namespaces);

                    _cpp.WriteOrigin(dt.Source);
                    EmitType(types[i], dt);

                    _h.EndNamespace(_namespaces);
                }

                _cpp.EndNamespace(_namespaces);
            }
        }

        void EmitType(CppType type, DataType dt)
        {
            switch (dt.TypeType)
            {
                case TypeType.Interface:
                    EmitInterface(type, dt);
                    break;
                case TypeType.Delegate:
                    EmitDelegate(type, dt);
                    break;
                case TypeType.Enum:
                    EmitEnum(type, dt);
                    break;
                default:
                    EmitStruct(type, dt);
                    break;
            }
        }

        void EmitStruct(CppType type, DataType dt)
        {
            _cpp.BeginType(type, dt);
            _h.BeginType(type, dt);

            // typeof()
            if (type.EmitTypeObject)
            {
                RequireTypeOf(dt);

                if (dt.Initializer != null)
                    DefineFunction(dt.Initializer);
                if (dt.Finalizer != null)
                    DefineFunction(dt.Finalizer);

                foreach (var m in dt.InterfaceMethods)
                    DefineInterfaceAdapter(m.Value);

                if (type.EmitTypeStruct)
                {
                    _h.WriteLine(
                        "struct " + type.TypeOfType + " : " + (
                            dt.IsValueType
                                ? "uStructType"
                                : _backend.GetTypeOfType(dt.Base ?? _essentials.Object, dt)
                        ));
                    _h.BeginScope();

                    // Interfaces
                    for (int i = dt.Base?.Interfaces.Length ?? 0;
                             i < dt.Interfaces.Length;
                             i++)
                        _h.WriteLine(_backend.GetStaticName(dt.Interfaces[i]) + " interface" + i + ";");

                    // V-table
                    foreach (var m in type.VTable)
                        DeclareFunctionPointer(m);

                    // typeof()
                    _h.EndScopeSemicolon();
                }

                if (type.EmitTypeOfDeclaration)
                    _h.WriteLine(type.TypeOfType + "* " + type.TypeOfFunction + "();");

                _cpp.WriteLine("static void " + type.StructName + "_build(uType* type)");
                _cpp.BeginScope();

                foreach (var e in type.StringConsts)
                {
                    int index;
                    if (_stringConsts.TryGetValue(e, out index))
                        _cpp.WriteLine("::STRINGS[" + index + "] = uString::Const(\"" + _cpp.EscapeString(e) + "\");");
                }

                var typeCache = new TypeCache(new Dictionary<DataType, int>());
                foreach (var e in type.TypeObjects)
                {
                    int index;
                    if (_typeObjects.TryGetValue(e, out index))
                    {
                        _cpp.WriteLine("::TYPES[" + index + "] = " + _backend.GetTypeOf(e, dt, "type", null, typeCache) + ";");
                        typeCache.Global.Add(e, index);
                    }
                }

                if (dt.Base != null && dt.Base.IsFlattenedParameterization)
                    _cpp.WriteLine("type->SetBase(" + _backend.GetTypeOf(dt.Base, dt, "type") + ");");

                if (type.Dependencies.Count > 0)
                {
                    _cpp.BeginLine("type->SetDependencies(");
                    _cpp.Indent();

                    var first = true;
                    foreach (var e in type.Dependencies)
                    {
                        _cpp.EndLine(first ? "" : ",");
                        _cpp.BeginLine(_backend.GetTypeOf(e, dt, "type"));
                        first = false;
                    }

                    _cpp.EndLine(");");
                    _cpp.Unindent();
                }

                if (type.PrecalcedTypes.Count > 0)
                {
                    _cpp.BeginLine("type->SetPrecalc(");
                    _cpp.Indent();

                    var first = true;
                    foreach (var e in type.PrecalcedTypes)
                    {
                        _cpp.EndLine(first ? "" : ",");
                        _cpp.BeginLine(_backend.GetTypeOf(e, dt, "type"));
                        first = false;
                    }

                    _cpp.EndLine(");");
                    _cpp.Unindent();
                }

                foreach (var gtype in type.MethodTypes)
                {
                    var obj = "type->MethodTypes[" + gtype.MethodIndex + "]";

                    if (gtype.Dependencies.Count > 0)
                    {
                        _cpp.BeginLine(obj + "->SetDependencies(");
                        _cpp.Indent();

                        var first = true;
                        foreach (var e in gtype.Dependencies)
                        {
                            _cpp.EndLine(first ? "" : ",");
                            _cpp.BeginLine(_backend.GetTypeOf(e, dt, obj, null, typeCache));
                            first = false;
                        }

                        _cpp.EndLine(");");
                        _cpp.Unindent();
                    }

                    if (gtype.PrecalcedTypes.Count > 0)
                    {
                        _cpp.BeginLine(obj + "->SetPrecalc(");
                        _cpp.Indent();

                        var first = true;
                        foreach (var e in gtype.PrecalcedTypes)
                        {
                            _cpp.EndLine(first ? "" : ",");
                            _cpp.BeginLine(_backend.GetTypeOf(e, dt, obj, null, typeCache));
                            first = false;
                        }

                        _cpp.EndLine(");");
                        _cpp.Unindent();
                    }
                }

                if (dt.Interfaces.Length > 0)
                {
                    _cpp.BeginLine("type->SetInterfaces(");
                    _cpp.Indent();

                    for (int i = 0; i < dt.Interfaces.Length; i++)
                    {
                        _cpp.EndLine(i == 0 ? "" : ",");
                        _cpp.BeginLine(_backend.GetTypeOf(dt.Interfaces[i], dt, "type", null, typeCache) + ", offsetof(" + type.TypeOfType + ", interface" + i + ")");
                    }

                    _cpp.EndLine(");");
                    _cpp.Unindent();
                }

                if (type.FlattenedFields.Count > 0)
                {
                    _cpp.BeginLine("type->SetFields(" + type.InheritedFieldCount);
                    _cpp.Indent();

                    for (int i = type.InheritedFieldCount; i < type.FlattenedFields.Count; i++)
                    {
                        var f = type.FlattenedFields[i];
                        _cpp.EndLine(",");
                        _cpp.BeginLine(_backend.GetTypeOf(f.ReturnType, dt, "type", null, typeCache) + ", " + GetFieldOffset(f) + ", " + GetFieldFlags(f));
                    }

                    _cpp.EndLine(");");
                    _cpp.Unindent();
                }

                if (type.ReflectedFields.Count > 0)
                {
                    _cpp.BeginLine("type->Reflection.SetFields(" + type.ReflectedFields.Count);
                    _cpp.Indent();

                    foreach (var f in type.ReflectedFields)
                    {
                        _cpp.EndLine(",");
                        _cpp.BeginLine("new uField(" + f.UnoName.ToLiteral() +
                                       ", " + _backend.GetIndex(f) + ")");
                    }

                    _cpp.EndLine(");");
                    _cpp.Unindent();
                }

                if (type.ReflectedFunctions.Count > 0)
                {
                    _cpp.BeginLine("type->Reflection.SetFunctions(" + type.ReflectedFunctions.Count);
                    _cpp.Indent();

                    foreach (var f in type.ReflectedFunctions)
                    {
                        var obj = f.GenericType != null
                                ? "type->MethodTypes[" + _backend.GetType(f.GenericType).MethodIndex + "]"
                                : "type";

                        _cpp.EndLine(",");
                        _cpp.BeginLine("new uFunction(" + f.Prototype.NameAndSuffix.ToLiteral() +
                                       ", " + (
                                           _backend.HasTypeParameter(f)
                                               ? _backend.GetTypeOf(f.GenericType ?? f.DeclaringType, dt, obj, null, typeCache)
                                               : "nullptr"
                                       ) +
                                       ", " + (
                                           f.IsVirtual
                                               ? "nullptr, offsetof(" + type.TypeOfType + ", fp_" + f.Name + ")"
                                               : "(void*)" + _backend.GetFunctionPointer(f, dt) + ", 0"
                                       ) +
                                       ", " + f.IsStatic.ToLiteral() +
                                       ", " + _backend.GetTypeOf(f.ReturnType, dt, obj, null, typeCache) +
                                       ", " + f.Parameters.Length);

                        foreach (var p in f.Parameters)
                            _cpp.Write(", " + _backend.GetTypeOf(p, dt, obj, typeCache));

                        _cpp.Write(")");
                    }

                    _cpp.EndLine(");");
                    _cpp.Unindent();
                }

                _cpp.EndScope();

                _cpp.WriteLine(type.TypeOfType + "* " + type.TypeOfFunction + "()");
                _cpp.BeginScope();
                _cpp.WriteLine("static uSStrong" + _backend.GetTemplateString(type.TypeOfType + "*") + " type;");
                _cpp.WriteLine("if (type != nullptr) return type;");
                _cpp.Skip();

                _cpp.WriteLine("uTypeOptions options;");
                if (dt.Base != null && dt.Base != _essentials.Object)
                    _cpp.WriteLine("options.BaseDefinition = " + _backend.GetTypeOf(dt.Base.MasterDefinition, dt) + ";");
                if (type.FlattenedFields.Count > 0)
                    _cpp.WriteLine("options.FieldCount = " + type.FlattenedFields.Count + ";");
                if (dt.IsFlattenedDefinition)
                    _cpp.WriteLine("options.GenericCount = " + dt.FlattenedParameters.Length + ";");
                if (dt.Interfaces.Length > 0)
                    _cpp.WriteLine("options.InterfaceCount = " + dt.Interfaces.Length + ";");
                if (type.MethodTypes.Count > 0)
                    _cpp.WriteLine("options.MethodTypeCount = " + type.MethodTypes.Count + ";");
                if (type.Dependencies.Count > 0)
                    _cpp.WriteLine("options.DependencyCount = " + type.Dependencies.Count + ";");
                if (type.PrecalcedTypes.Count > 0)
                    _cpp.WriteLine("options.PrecalcCount = " + type.PrecalcedTypes.Count + ";");
                if (!dt.IsStatic && !_backend.IsConstrained(dt))
                {
                    if (dt.IsValueType)
                    {
                        _cpp.WriteLine("options.Alignment = alignof(" + _backend.GetTypeName(dt, dt) + ");");
                        _cpp.WriteLine("options.ValueSize = sizeof(" + _backend.GetTypeName(dt, dt) + ");");
                    }
                    else
                        _cpp.WriteLine("options.ObjectSize = sizeof(" + type.StructName + ");");
                }
                _cpp.WriteLine("options.TypeSize = sizeof(" + type.TypeOfType + ");");
                _cpp.WriteLine("type = " + (
                        type.TypeOfType != "uStructType" &&
                        type.TypeOfType != "uClassType" &&
                        type.TypeOfType != "uType"
                            ? "(" + type.TypeOfType + "*)"
                            : null
                    ) + (
                        dt.IsValueType
                            ? "uStructType"
                            : "uClassType"
                    ) + "::New(" + type.ReflectedName.ToLiteral() + ", options);");

                foreach (var e in type.MethodTypes)
                    _cpp.WriteLine("type->MethodTypes[" + e.MethodIndex + "] = type->NewMethodType(" + e.MethodRank + ", " + e.PrecalcedTypes.Count + "," + e.Dependencies.Count + ");");

                _cpp.WriteLine("type->fp_build_ = " + type.StructName + "_build;");

                var ctor = dt.TryGetDefaultConstructor();
                if (ctor != null)
                    _cpp.WriteLine("type->fp_ctor_ = (void*)" + _backend.GetFunctionPointer(ctor, dt) + ";");

                if (dt.Initializer != null)
                    AssignFunctionPointer(dt.Initializer);
                if (dt.Finalizer != null)
                    AssignFunctionPointer(dt.Finalizer);

                foreach (var m in type.VTable)
                    AssignFunctionPointer(m);
                foreach (var e in dt.InterfaceMethods)
                    AssignInterfacePointer(e.Key, e.Value, dt);

                _cpp.WriteLine("return type;");
                _cpp.EndScope();
            }

            // Concrete functions
            foreach (var f in type.Functions)
                DefineFunction(f);

            // Early-out on empty opaque structs
            if (dt.IsStruct &&
                type.Fields.Count == 0 &&
                type.InstanceMethods.Count == 0 &&
                type.StaticMethods.Count == 0 &&
                _env.HasProperty(dt, "TypeName"))
            {
                _cpp.EndType();
                _h.EndType();
                return;
            }

            _h.Skip();

            // Struct declaration
            // {
            _h.WriteTemplate(dt);
            _h.WriteLine("struct " + type.StructName + (
                !dt.IsValueType && !_env.HasProperty(dt, "TypeName")
                    ? " : " + _backend.GetBaseType(dt.Base ?? _essentials.Object, dt)
                    : ""
                ));
            _h.BeginScope();

            // Fields
            foreach (var f in type.Fields)
                DefineField(f);

            _cpp.Skip();
            _h.Skip();

            // Instance methods
            foreach (var m in type.InstanceMethods)
                DeclareInstanceMethod(type, m);

            // Static methods
            foreach (var m in type.StaticMethods)
                DeclareStaticMethod(type, m);

            _h.EndScopeSemicolon();
            // };

            // Inline methods
            if (HasInlineMethods(type))
            {
                _h.EndNamespace(_namespaces);

                foreach (var inc in type.Declarations.Inline)
                    _h.WriteLine(inc);

                _h.BeginNamespace(_namespaces);

                foreach (var m in type.InstanceMethods)
                    DefineInstanceMethod(type, m);
                foreach (var m in type.StaticMethods)
                    DefineStaticMethod(type, m);
            }

            // Struct methods
            foreach (var m in type.StaticMethods)
                DefineStructMethod(m);

            _cpp.EndType();
            _h.EndType();
        }

        void EmitInterface(CppType type, DataType dt)
        {
            _cpp.BeginType(type, dt);
            _h.BeginType(type, dt);
            RequireTypeOf(dt);

            // typeof()
            _h.WriteLine("uInterfaceType* " + type.TypeOfFunction + "();");
            _cpp.WriteLine("uInterfaceType* " + type.TypeOfFunction + "()");
            _cpp.BeginScope();
            _cpp.WriteLine("static uSStrong<uInterfaceType*> type;");
            _cpp.WriteLine("if (type != nullptr) return type;");
            _cpp.Skip();
            _cpp.WriteLine("type = uInterfaceType::New(" + type.ReflectedName.ToLiteral() +
                ", " + (dt.IsFlattenedDefinition ? dt.FlattenedParameters.Length : 0) +
                ", " + type.MethodTypes.Count + ");");

            foreach (var e in type.MethodTypes)
                _cpp.WriteLine("type->MethodTypes[" + e.MethodIndex + "] = type->NewMethodType(" + e.MethodRank + ");");

            if (type.ReflectedFunctions.Count > 0)
            {
                _cpp.BeginLine("type->Reflection.SetFunctions(" + type.ReflectedFunctions.Count);
                _cpp.Indent();

                foreach (var f in type.ReflectedFunctions)
                {
                    var obj = f.GenericType != null
                            ? "type->MethodTypes[" + _backend.GetType(f.GenericType).MethodIndex + "]"
                            : "type";
                
                    _cpp.EndLine(",");
                    _cpp.BeginLine("new uFunction(" + f.Prototype.NameAndSuffix.ToLiteral() +
                                   ", " + (
                                        _backend.HasTypeParameter(f)
                                            ? _backend.GetTypeOf(f.GenericType ?? f.DeclaringType, dt, obj)
                                            : "nullptr"
                                   ) +
                                   ", " + (
                                        f.IsVirtual
                                            ? "nullptr, offsetof(" + type.StructName + ", fp_" + f.Name + ")"
                                            : "(void*)" + _backend.GetFunctionPointer(f, dt) + ", 0"
                                   ) +
                                   ", " + f.IsStatic.ToLiteral() +
                                   ", " + _backend.GetTypeOf(f.ReturnType, dt, obj) +
                                   ", " + f.Parameters.Length);

                    foreach (var p in f.Parameters)
                        _cpp.Write(", " + _backend.GetTypeOf(p, dt, obj));

                    _cpp.Write(")");
                }

                _cpp.EndLine(");");
                _cpp.Unindent();
            }

            _cpp.WriteLine("return type;");
            _cpp.EndScope();
            _h.Skip();

            // Struct declaration
            // {
            _h.WriteLine("struct " + type.StructName);
            _h.BeginScope();

            // Function pointers
            foreach (var m in type.VTable)
                DeclareFunctionPointer(m);

            // Instance methods
            foreach (var m in type.InstanceMethods)
                DeclareInterfaceMethod(type, m);

            _h.EndScopeSemicolon();
            // };

            // Inline methods
            if (HasInlineMethods(type))
            {
                _h.EndNamespace(_namespaces);

                foreach (var inc in type.Declarations.Inline)
                    _h.WriteLine(inc);

                _h.BeginNamespace(_namespaces);

                foreach (var m in type.InstanceMethods)
                    DefineInterfaceMethod(type, m);
            }

            _cpp.EndType();
            _h.EndType();
        }

        void EmitDelegate(CppType type, DataType dt)
        {
            _cpp.WriteComment(dt);
            _h.WriteComment(dt);
            RequireTypeOf(dt);

            // typeof()
            _h.WriteLine("uDelegateType* " + type.TypeOfFunction + "();");
            _h.Skip();

            _cpp.WriteLine("uDelegateType* " + type.TypeOfFunction + "()");
            _cpp.BeginScope();
            _cpp.WriteLine("static uSStrong<uDelegateType*> type;");
            _cpp.WriteLine("if (type != nullptr) return type;");
            _cpp.Skip();

            _cpp.WriteLine("type = uDelegateType::New(" + type.ReflectedName.ToLiteral() +
                           ", " + dt.Parameters.Length +
                           ", " + (dt.IsFlattenedDefinition ? dt.FlattenedParameters.Length : 0) + ");");
            _cpp.BeginLine("type->SetSignature(" + _backend.GetTypeOf(dt.ReturnType, dt, "type"));
            _cpp.Indent();

            foreach (var p in dt.Parameters)
            {
                _cpp.EndLine(",");
                _cpp.BeginLine(_backend.GetTypeOf(p, dt, "type"));
            }

            _cpp.EndLine(");");
            _cpp.Unindent();
            _cpp.WriteLine("return type;");
            _cpp.EndScope();
        }

        void EmitEnum(CppType type, DataType dt)
        {
            _cpp.WriteComment(dt);
            _h.WriteComment(dt);
            RequireTypeOf(dt);

            // typeof()
            _h.WriteLine("uEnumType* " + type.TypeOfFunction + "();");
            _h.Skip();

            _cpp.WriteLine("uEnumType* " + type.TypeOfFunction + "()");
            _cpp.BeginScope();
            _cpp.WriteLine("static uSStrong<uEnumType*> type;");
            _cpp.WriteLine("if (type != nullptr) return type;");
            _cpp.Skip();

            _cpp.WriteLine("type = uEnumType::New(" + type.ReflectedName.ToLiteral() + ", " + _backend.GetTypeOf(dt.Base, dt, "type") + ", " + dt.Literals.Count + ");");

            if (dt.Literals.Count > 0)
            {
                _cpp.BeginLine("type->SetLiterals(");
                _cpp.Indent();

                var first = true;
                foreach (var p in dt.Literals)
                {
                    _cpp.EndLine(first ? "" : ",");
                    _cpp.BeginLine(p.Name.ToLiteral() + ", " + GetLong(p.Value) + "LL");
                    first = false;
                }

                _cpp.Unindent();
                _cpp.EndLine(");");
            }

            _cpp.WriteLine("return type;");
            _cpp.EndScope();
        }

        void RequireTypeOf(DataType dt)
        {
            _env.Require("TypeObjects.Declaration", _backend.GetTypeOfDeclaration(dt));
            _env.Require("TypeObjects.FunctionPointer", _backend.GetTypeOfPointer(dt));
        }

        void DefineField(Field f)
        {
            if (f.IsStatic)
            {
                if (!f.DeclaringType.IsClosed)
                    _h.WriteLine("static uTField " + f.Name + "(uType* type) { return " + (
                            f.DeclaringType.HasInitializer
                                ? "type->Init(), "
                                : null
                        ) + "type->Field(" + _backend.GetIndex(f) + "); }");
                else
                {
                    _cpp.WriteLine(_backend.GetFieldType(f, f.DeclaringType) + " " + _backend.GetStaticName(f, f.DeclaringType) + "_;");
                    _h.WriteLine("static " + _backend.GetFieldType(f, f.DeclaringType) + " " + _backend.GetMemberName(f) + "_;");
                    _h.WriteLine("static " + _backend.GetFieldType(f, f.DeclaringType) + "& " +
                        _backend.GetMemberName(f) + "() { return " + (
                            f.DeclaringType.HasInitializer
                                ? _backend.GetTypeOf(f.DeclaringType, f.DeclaringType) + "->Init(), "
                                : null
                        ) + _backend.GetMemberName(f) + "_; }");
                }
            }
            else
                _h.WriteLine(_backend.IsConstrained(f) && !_backend.IsConstrained(f.DeclaringType)
                    ? "uTField " + f.Name + "() { return __type->Field(this, " + _backend.GetIndex(f) + "); }"
                    : _backend.GetFieldType(f, f.DeclaringType) + " " + f.Name + ";");
        }

        string GetFieldOffset(Field f)
        {
            return f.IsStatic
                ? !f.DeclaringType.IsClosed
                    ? "(uintptr_t)0"
                    : "(uintptr_t)&" + _backend.GetStaticName(f, f.DeclaringType) + "_"
                : _backend.IsConstrained(f)
                    ? "(uintptr_t)0"
                    : "offsetof(" + _backend.GetTypeName(f.DeclaringType, f.DeclaringType).Replace("*", "") + ", " + f.Name + ")";
        }

        string GetFieldFlags(Field f)
        {
            var flags = new List<string>();

            if (_backend.IsConstrained(f))
                flags.Add("uFieldFlagsConstrained");
            if (f.IsStatic)
                flags.Add("uFieldFlagsStatic");
            if (_backend.GetReferenceType(f) == ReferenceType.Weak)
                flags.Add("uFieldFlagsWeak");

            return flags.Count > 0
                ? string.Join(" | ", flags)
                : "0";
        }

        void DefineFunction(Function f)
        {
            _cpp.WriteComment(f);
            var decl = GetFunctionDeclaration(f);

            if (!f.IsConstructor && !f.IsFinalizer)
                _h.WriteLine(decl + ";");

            _cpp.WriteLine(f.IsConstructor || f.IsFinalizer
                    ? "static " + decl
                    : decl);

            if (_backend.PassByRef(f))
                _cpp.WriteMethodBody(f);
            else
            {
                var obj = f.IsStatic ? null : new This(f.Source, f.DeclaringType).Address;
                var args = new Expression[f.Parameters.Length];

                for (int i = 0; i < f.Parameters.Length; i++)
                {
                    var p = f.Parameters[i];
                    args[i] = new StringExpression(p.Source, p.Type, p.Type.IsReferenceType || p.IsReference ? p.Name : "*" + p.Name);
                }

                _cpp.SetContext(f);
                _cpp.BeginScope();
                _cpp.BeginLine();

                if (!f.ReturnType.IsVoid)
                    _cpp.Write(_backend.IsConstrained(f.ReturnType)
                                ? "__retval.Store(" + _backend.GetTypeOf(f.ReturnType, f.DeclaringType, f) + ", "
                                : "*__retval = ");

                _cpp.WriteCall(f.Source, f, obj, args);
                _cpp.EndLine(";");
                _cpp.EndScope();
            }
        }

        void DeclareFunctionPointer(Method f)
        {
            if (f.OverriddenMethod != null)
                return;

            _h.BeginLine(
                "void(*fp_" + f.Name +
                ")(" + _backend.GetTypeDef(f.DeclaringType, null, TypeFlags.ThisByRef));
            _h.WriteWhen(f.IsGenericMethod, ", uType*");

            foreach (var p in f.Parameters)
                _h.Write(", " + _backend.GetParameterType(p));

            if (!f.ReturnType.IsVoid)
                _h.Write(", " + _backend.GetTypeDef(f.ReturnType, null, TypeFlags.ReturnByRef));

            _h.EndLine(");");
        }

        void AssignFunctionPointer(Constructor f)
        {
            _cpp.WriteLine("type->fp_cctor_ = " + _backend.GetFunctionPointer(f, f.DeclaringType) + ";");
        }

        void AssignFunctionPointer(Finalizer f)
        {
            _cpp.WriteLine(
                "type->fp_" + f.Name +
                " = (void(*)(" + _backend.GetTypeDef(_essentials.Object, f.DeclaringType, TypeFlags.ThisByRef) +
                "))" + _backend.GetFunctionPointer(f, f.DeclaringType) +
                ";");
        }

        void AssignFunctionPointer(Method f)
        {
            if (f.IsAbstract)
                return;

            _cpp.BeginLine(
                "type->fp_" + f.Name + (
                    f.DeclaringType.IsValueType
                        ? "_struct"
                        : null
                ) + " = ");

            var m = f.VirtualBase.MasterDefinition;
            if (m != f)
            {
                _cpp.Write(
                    "(void(*)(" + (
                        f.DeclaringType.IsValueType
                            ? "void*"
                            : _backend.GetTypeDef(m.DeclaringType, f.DeclaringType, TypeFlags.ThisByRef)
                    ));
                _cpp.WriteWhen(_backend.HasTypeParameter(f), ", uType*");

                foreach (var p in m.Parameters)
                    _cpp.Write(", " + _backend.GetParameterType(p, f.DeclaringType));

                if (!f.ReturnType.IsVoid)
                    _cpp.Write(", " + _backend.GetTypeDef(m.ReturnType, f.DeclaringType, TypeFlags.ReturnByRef));

                _cpp.Write("))");
            }

            _cpp.EndLine(_backend.GetFunctionPointer(f, f.DeclaringType) + ";");
        }

        void AssignInterfacePointer(Method decl, Method impl, DataType parent)
        {
            if (impl.IsAbstract)
                return;

            _cpp.BeginLine("type->interface" +
                GetInterfaceIndex(decl.DeclaringType, parent) +
                ".fp_" + decl.Name + " = ");

            var master = decl.MasterDefinition;

            if (!impl.DeclaringType.IsStruct || 
                !master.CompareParameters(impl) ||
                master.ReturnType != impl.ReturnType)
            {
                _cpp.Write("(void(*)(uObject*");

                if (_backend.HasTypeParameter(master))
                    _cpp.Write(", uType*");

                foreach (var p in master.Parameters)
                    _cpp.Write(", " + _backend.GetParameterType(p, parent));

                if (!decl.ReturnType.IsVoid)
                    _cpp.Write(", " + _backend.GetTypeDef(master.ReturnType, parent, TypeFlags.ReturnByRef));

                _cpp.Write("))");
            }

            _cpp.EndLine(
                _backend.GetFunctionPointer(impl, parent,
                    impl.DeclaringType.IsStruct
                        ? "_ex" 
                        : "_fn"
                    ) + ";");
        }

        void DefineInterfaceAdapter(Method f)
        {
            if (!f.DeclaringType.IsStruct)
                return;

            _cpp.WriteComment(f, "adapter");
            _cpp.BeginLine("static void " + _backend.GetFunctionPointer(f, f.DeclaringType, "_ex") + "(uObject* __this");

            foreach (var p in f.Parameters)
                _cpp.Write(", " + _backend.GetParameterType(p, f.DeclaringType) + " " + p.Name);

            if (!f.ReturnType.IsVoid)
                _cpp.Write(", " + _backend.GetTypeDef(f.ReturnType, f.DeclaringType, TypeFlags.ReturnByRef) + " __retval");

            _cpp.EndLine(")");
            _cpp.BeginScope();
            _cpp.BeginLine(_backend.GetFunctionPointer(f, f.DeclaringType) + "((" +
                           _backend.GetTypeName(f.DeclaringType, f.DeclaringType) +
                           "*)((uint8_t*)__this + sizeof(uObject))");
            _cpp.WriteWhen(_backend.HasTypeParameter(f), ", __this->__type");

            foreach (var p in f.Parameters)
                _cpp.Write(", " + p.Name);

            _cpp.WriteWhen(!f.ReturnType.IsVoid, ", __retval");
            _cpp.EndLine(");");
            _cpp.EndScope();
        }

        int GetInterfaceIndex(DataType dt, DataType parent)
        {
            for (int i = 0; i < parent.Interfaces.Length; i++)
                if (parent.Interfaces[i] == dt)
                    return i;

            _backend.Log.Error("C++: Failed to get index of " + dt.Quote() + " in " + parent.Quote());
            return -1;
        }

        void DefineStructMethod(Function f)
        {
            if (!_backend.IsStructMethod(f))
                return;

            if (_backend.PassByRef(f))
            {
                var t = _h.WriteTemplate(f);
                _h.BeginLine((!t ? "inline " : "") +
                    _backend.GetTypeDef(f.ReturnType, f.DeclaringType, TypeFlags.ReturnInline) +
                    " " + _backend.GetStaticName(f, f.DeclaringType) + GetParameterList(f, ParameterFlags.ObjectByRef));
                _h.WriteStaticMethodBody(f, BodyFlags.Inline);
            }
            else
            {
                _h.WriteLine(_backend.GetTypeDef(f.ReturnType, f.DeclaringType, TypeFlags.ReturnInline) +
                    " " + _backend.GetStaticName(f, f.DeclaringType) + GetParameterList(f, ParameterFlags.ObjectByRef) + ";");
                _cpp.WriteComment(f, "static");
                _cpp.WriteLine(_backend.GetTypeDef(f.ReturnType, f.DeclaringType, TypeFlags.Return) +
                    " " + _backend.GetStaticName(f, f.DeclaringType) + GetParameterList(f, ParameterFlags.ObjectByRef));
                _cpp.WriteStaticMethodBody(f);
            }
        }

        void DeclareStaticMethod(CppType type, Function f)
        {
            if (_backend.IsStructMethod(f))
                return;

            if (_backend.PassByRef(f))
            {
                _h.WriteTemplate(f);
                _h.BeginLine("static " +
                    _backend.GetTypeDef(f.ReturnType, f.DeclaringType, TypeFlags.ReturnInline) +
                    " " + _backend.GetMemberName(f) + GetParameterList(f, ParameterFlags.ObjectByRef));

                if (!NeedsInlineImplementation(type, f))
                    _h.WriteStaticMethodBody(f, BodyFlags.Inline);
                else
                    _h.EndLine(";");
            }
            else
            {
                _h.WriteLine("static " + _backend.GetTypeDef(f.ReturnType, f.DeclaringType, TypeFlags.ReturnInline) +
                    " " + _backend.GetMemberName(f) + GetParameterList(f, ParameterFlags.ObjectByRef) + ";");
                _cpp.WriteComment(f, "static");
                _cpp.WriteLine(_backend.GetTypeDef(f.ReturnType, f.DeclaringType, TypeFlags.ReturnInline) +
                    " " + _backend.GetStaticName(f, f.DeclaringType) + GetParameterList(f, ParameterFlags.ObjectByRef));
                _cpp.WriteStaticMethodBody(f);
            }
        }

        void DefineStaticMethod(CppType type, Function f)
        {
            if (_backend.IsStructMethod(f) ||
                !NeedsInlineImplementation(type, f))
                return;

            var t = _h.WriteTemplate(f);
            _h.BeginLine((!t ? "inline " : null) +
                _backend.GetTypeDef(f.ReturnType, f.DeclaringType, TypeFlags.ReturnInline) +
                " " + _backend.GetStaticName(f.DeclaringType, f.DeclaringType) +
                "::" + _backend.GetMemberName(f) + GetParameterList(f, ParameterFlags.ObjectByRef));
            _h.WriteStaticMethodBody(f, BodyFlags.Inline);
        }

        void DeclareInstanceMethod(CppType type, Method f)
        {
            if (_backend.PassByRef(f))
            {
                var t = _h.WriteTemplate(f);
                _h.BeginLine((type.IsOpaque ? "static " : null) +
                     _backend.GetTypeDef(f.ReturnType, f.DeclaringType, TypeFlags.ReturnInline) +
                     " " + _backend.GetMemberName(f) + GetParameterList(f, type.ParameterFlags));

                if (!NeedsInlineImplementation(type, f))
                    _h.WriteInstanceMethodBody(f, BodyFlags.Inline);
                else
                    _h.EndLine(";");

                if (t && f.IsVirtual)
                {
                    _h.BeginLine((type.IsOpaque ? "static " : null) + "void " +
                        _backend.GetMemberName(f, f) + GetParameterList(f, type.ParameterFlags | ParameterFlags.ReturnByRef));
                    _h.WriteInstanceMethodBody(f, BodyFlags.InlineReturnByRef);
                }
            }
            else
            {
                _h.WriteLine((type.IsOpaque ? "static " : null) +
                    _backend.GetTypeDef(f.ReturnType, f.DeclaringType, TypeFlags.ReturnInline) +
                    " " + _backend.GetMemberName(f) + GetParameterList(f, type.ParameterFlags) + ";");
                _cpp.WriteComment(f, "instance");
                _cpp.WriteLine(_backend.GetTypeDef(f.ReturnType, f.DeclaringType, TypeFlags.Return) +
                    " " + _backend.GetStaticName(f.DeclaringType, f.DeclaringType) + "::" + _backend.GetMemberName(f) + GetParameterList(f, type.ParameterFlags));
                _cpp.WriteInstanceMethodBody(f);
            }
        }

        void DefineInstanceMethod(CppType type, Method f)
        {
            if (!NeedsInlineImplementation(type, f))
                return;

            var t = _h.WriteTemplate(f);
            _h.BeginLine((!t ? "inline " : null) +
                _backend.GetTypeDef(f.ReturnType, f.DeclaringType, TypeFlags.ReturnInline) +
                " " + _backend.GetStaticName(f.DeclaringType, f.DeclaringType) +
                "::" + _backend.GetMemberName(f) + GetParameterList(f, type.ParameterFlags));
            _h.WriteInstanceMethodBody(f, BodyFlags.Inline);
        }

        void DeclareInterfaceMethod(CppType type, Method f)
        {
            var t = _h.WriteTemplate(f);
            _h.BeginLine("static " +
                _backend.GetTypeDef(f.ReturnType, f.DeclaringType, TypeFlags.ReturnInline) +
                " " + _backend.GetMemberName(f) + GetParameterList(f, ParameterFlags.ObjectByRef));

            if (!NeedsInlineImplementation(type, f))
                _h.WriteInterfaceMethodBody(f);
            else
                _h.EndLine(";");

            if (t)
            {
                _h.BeginLine("static void " + _backend.GetMemberName(f, f) + GetParameterList(f, ParameterFlags.ObjectAndReturnByRef));
                _h.WriteInterfaceMethodBody(f, BodyFlags.InlineReturnByRef);
            }
        }

        void DefineInterfaceMethod(CppType type, Method f)
        {
            if (!NeedsInlineImplementation(type, f))
                return;

            var t = _h.WriteTemplate(f);
            _h.BeginLine((!t ? "inline " : null) +
                _backend.GetTypeDef(f.ReturnType, f.DeclaringType, TypeFlags.ReturnInline) +
                " " + _backend.GetStaticName(f.DeclaringType, f.DeclaringType) +
                "::" + _backend.GetMemberName(f) + GetParameterList(f, ParameterFlags.ObjectByRef));
            _h.WriteInterfaceMethodBody(f);
        }

        string GetFunctionDeclaration(Function f, string suffix = "_fn")
        {
            var comma = false;
            var sb = new StringBuilder("void " + _backend.GetFunctionPointer(f, f.DeclaringType, suffix) + "(");

            if (!f.IsStatic)
            {
                sb.Append(_backend.GetTypeDef(f.DeclaringType, f.DeclaringType, TypeFlags.ThisByRefDeclaration) + " __this");
                comma = true;
            }

            if (_backend.HasTypeParameter(f))
            {
                sb.CommaWhen(comma);
                sb.Append("uType* __type");
                comma = true;
            }

            foreach (var p in f.Parameters)
            {
                sb.CommaWhen(comma);
                sb.Append(_backend.GetParameterType(p, f.DeclaringType) + " " + p.Name);
                comma = true;
            }

            if (!f.ReturnType.IsVoid)
            {
                sb.CommaWhen(comma);
                sb.Append(_backend.GetTypeDef(f.ReturnType, f.DeclaringType, TypeFlags.ReturnByRef) + " __retval");
            }

            sb.Append(")");
            return sb.ToString();
        }

        string GetParameterList(Function f, ParameterFlags flags = 0)
        {
            var comma = false;
            var sb = new StringBuilder("(");

            if (flags.HasFlag(ParameterFlags.ObjectByRef) && !f.IsStatic)
            {
                sb.Append(_backend.GetTypeDef(f.DeclaringType, f.DeclaringType, TypeFlags.ThisInlineDeclaration) + " __this");
                comma = true;
            }

            if (_backend.HasTypeParameter(f))
            {
                sb.CommaWhen(comma);
                sb.Append("uType* __type");
                comma = true;
            }

            foreach (var p in f.Parameters)
            {
                sb.CommaWhen(comma);
                sb.Append(_backend.GetParameterType(p, f.DeclaringType, !flags.HasFlag(ParameterFlags.ReturnByRef)) + " " + p.Name);
                comma = true;
            }

            if (flags.HasFlag(ParameterFlags.ReturnByRef) && !f.ReturnType.IsVoid)
            {
                sb.CommaWhen(comma);
                sb.Append(_backend.GetTypeDef(f.ReturnType, f.DeclaringType, TypeFlags.ReturnByRef) + " __retval");
            }

            sb.Append(")");
            return sb.ToString();
        }

        bool HasInlineMethods(CppType type)
        {
            if (type.Declarations.Inline.Length > 0)
            {
                foreach (var m in type.InstanceMethods)
                    if (NeedsInlineImplementation(type, m))
                        return true;
                foreach (var m in type.StaticMethods)
                    if (NeedsInlineImplementation(type, m))
                        return true;
            }

            return false;
        }

        bool NeedsInlineImplementation(CppType type, Function f)
        {
            if (type.Declarations.Inline.Length == 0 || !_backend.PassByRef(f))
                return false;

            if (f.ReturnType.IsStruct && !_env.HasProperty(f.ReturnType, "TypeName"))
                return true;

            foreach (var p in f.Parameters)
                if (!p.IsReference && p.Type.IsStruct && !_env.HasProperty(p.Type, "TypeName"))
                    return true;

            return false;
        }

        long GetLong(object value)
        {
            dynamic d = value;
            return (long)d;
        }

        CppWriter CreateWriter(string filename)
        {
            return new CppWriter(_backend, filename, _stringConsts, _typeObjects);
        }
    }
}
