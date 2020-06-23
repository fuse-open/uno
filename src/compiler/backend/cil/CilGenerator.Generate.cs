using System;
using IKVM.Reflection;
using IKVM.Reflection.Emit;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using Type = IKVM.Reflection.Type;

namespace Uno.Compiler.Backends.CIL
{
    partial class CilGenerator
    {
        public void Generate()
        {
            Process(_data.IL);

            foreach (var e in _types)
                e.Populate();
            foreach (var e in _types)
                CompileType(e);
            foreach (var e in _linkedTypes)
                ValidateType(e);

            // Emit TypeAlias attributes, used at run-time by Fuse Simulator.
            if (_backend.EnableReflection)
                EmitTypeAliasAttributes();

            Log.Verbose("Generated " + _types.Count + " .NET type".Plural(_types) + " for " + _assembly.GetName().Name.Quote() + " assembly");
        }

        void EmitTypeAliasAttributes()
        {
            var attribute = _linker.GetType(_backend.TypeAliasAttribute)
                    .GetConstructor(new[] {
                        _linker.GetType(_essentials.String),
                        _linker.GetType(_essentials.String)});

            foreach (var alias in _linker.TypeMap)
                if (alias.Key.Package == _package &&
                        !alias.Key.IsArray &&
                        !alias.Key.IsGenericParameter &&
                        !alias.Key.IsGenericParameterization &&
                        !string.IsNullOrEmpty(alias.Value.FullName) &&
                        alias.Key.FullName != alias.Value.FullName.Replace("+", "."))
                    _assembly.SetCustomAttribute(
                        new CustomAttributeBuilder(
                            attribute,
                            new object[] {
                                alias.Key.FullName,
                                alias.Value.FullName}));
        }

        void Process(Namespace root)
        {
            for (int i = 0, l = root.Types.Count; i < l; i++)
            {
                var dt = root.Types[i];
                if (dt.Package != _package)
                    continue;

                var dotNetName = dt.HasAttribute(_essentials.DotNetTypeAttribute)
                    ? dt.TryGetAttributeString(_essentials.DotNetTypeAttribute) ?? dt.CilTypeName()
                    : null;
                var prebuiltType = _linker.TryGetType(dotNetName);

                if (dotNetName != null && prebuiltType == null)
                    Log.Error(dt.Source, ErrorCode.E0000, "Unable to resolve .NET type " + dotNetName.Quote());
                else if (prebuiltType != null)
                    LinkType(dt, prebuiltType);
                else
                    _types.DefineType(dt);
            }

            for (int i = 0, l = root.Namespaces.Count; i < l; i++)
                Process(root.Namespaces[i]);
        }

        void LinkType(DataType dt, Type prebuiltType)
        {
            if (!dt.Package.CanLink)
                _linkedTypes.Add(dt);

            _linker.AddType(dt, prebuiltType);
        }

        void ValidateType(DataType dt)
        {
            foreach (var it in dt.NestedTypes)
                ValidateType(it);

            switch (dt.TypeType)
            {
                case TypeType.Class:
                case TypeType.Struct:
                case TypeType.Interface:
                {
                    if (!dt.IsPublic && !dt.IsProtected)
                        break;

                    try
                    {
                        var netType = _linker.GetType(dt);

                        if (netType.GetTypeInfo().IsInterface && !dt.IsInterface)
                            Log.Error(dt.Source, ErrorCode.E0000, "Uno type is " + dt.TypeType.ToLiteral() + ", but .NET expected an interface");
                        else if (!netType.GetTypeInfo().IsInterface && dt.IsInterface)
                            Log.Error(dt.Source, ErrorCode.E0000, "Uno interface " + dt.Quote() + " is not a .NET interface");
                    }
                    catch (Exception e)
                    {
                        Log.Trace(e);
                        Log.Error(dt.Source, ErrorCode.E0000, "Failed to link .NET type " + dt.Quote() + ": " + e.Message);
                    }

                    foreach (var m in dt.EnumerateFields())
                    {
                        if (!m.IsPublic)
                            continue;

                        try
                        {
                            ValidateField(m, _linker.GetField(m));
                        }
                        catch (Exception e)
                        {
                            Log.Trace(e);
                            Log.Error(m.Source, ErrorCode.E0000, "Failed to link .NET field " + m.Quote() + ": " + e.Message);
                        }
                    }

                    foreach (var m in dt.EnumerateFunctions())
                    {
                        if (!m.IsPublic && !m.IsProtected ||
                                m.IsIntrinsic || m.IsGenerated ||
                                !m.CanLink)
                            continue;

                        try
                        {
                            if (m is Constructor)
                                _linker.GetConstructor(m as Constructor);
                            else
                                ValidateMethod(m, _linker.GetMethod(m));
                        }
                        catch (Exception e)
                        {
                            Log.Trace(e);
                            Log.Error(m.Source, ErrorCode.E0000, "Failed to link .NET function " + m.Quote() + ": " + e.Message);
                        }
                    }

                    break;
                }
            }
        }

        void ValidateField(Field m, FieldInfo fi)
        {
            var rt = _linker.GetType(m.ReturnType);
            if (!AreCompatible(fi.FieldType, rt))
                Log.Error(m.Source, ErrorCode.E0000, m.Quote() + " did not have expected .NET field type " + fi.FieldType.Quote() + " -- found " + rt.Quote());
        }

        void ValidateMethod(Function m, MethodInfo mi)
        {
            var rt = _linker.GetType(m.ReturnType);
            if (!AreCompatible(mi.ReturnType, rt))
                Log.Error(m.Source, ErrorCode.E0000, m.Quote() + " did not have expected .NET return type " + mi.ReturnType.Quote() + " -- found " + rt.Quote());
        }

        bool AreCompatible(Type a, Type b)
        {
            if (a.IsGenericParameter)
                return a.Name == b.Name;

            if (b.IsInterface)
                return ImplementsInterface(a, b);

            for (;;)
            {
                if (Compare(a, b))
                    return true;

                a = a.BaseType;

                if (a == null)
                    return false;
            }
        }

        bool ImplementsInterface(Type a, Type b)
        {
            if (Compare(a, b))
                return true;

            foreach (var i in a.GetInterfaces())
                if (Compare(a, b) || ImplementsInterface(i, b))
                    return true;

            return false;
        }

        bool Compare(Type a, Type b)
        {
            return a == b ||
                   a.IsGenericType && b.IsGenericType &&
                   a.GetGenericTypeDefinition() == b.GetGenericTypeDefinition();
        }
    }
}
