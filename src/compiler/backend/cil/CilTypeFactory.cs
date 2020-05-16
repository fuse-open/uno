using IKVM.Reflection;
using IKVM.Reflection.Emit;
using System.Collections.Generic;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Logging;
using Type = IKVM.Reflection.Type;

namespace Uno.Compiler.Backends.CIL
{
    public class CilTypeFactory : List<CilType>
    {
        readonly Backend _backend;
        readonly IEssentials _essentials;
        readonly CilLinker _linker;
        readonly ModuleBuilder _module;

        public CilTypeFactory(Backend backend, IEssentials essentials, CilLinker linker, ModuleBuilder module)
        {
            _backend = backend;
            _essentials = essentials;
            _linker = linker;
            _module = module;
        }

        public void DefineType(DataType dt, TypeBuilder parent = null)
        {
            switch (dt.TypeType)
            {
                case TypeType.Class:
                    parent = CreateTypeBuilder(dt, parent, TypeAttributes.Class, _linker.System_Object);
                    break;
                case TypeType.Struct:
                    parent = CreateTypeBuilder(dt, parent, TypeAttributes.Sealed | TypeAttributes.SequentialLayout | TypeAttributes.Serializable, _linker.System_ValueType);
                    break;
                case TypeType.Interface:
                    parent = CreateTypeBuilder(dt, parent, TypeAttributes.Interface | TypeAttributes.AnsiClass | TypeAttributes.AutoClass, null);
                    break;
                case TypeType.Delegate:
                    parent = CreateTypeBuilder(dt, parent, TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.AnsiClass | TypeAttributes.AutoClass, _linker.System_MulticastDelegate);
                    break;
                case TypeType.Enum:
                    parent = CreateTypeBuilder(dt, parent, TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.AnsiClass | TypeAttributes.AutoClass, _linker.System_Enum);
                    break;
                default:
                    throw new FatalException(dt.Source, ErrorCode.I0000, "Unsupported .NET type: " + dt.TypeType);
            }

            for (int i = 0, l = dt.NestedTypes.Count; i < l; i++)
                DefineType(dt.NestedTypes[i], parent);
        }

        TypeBuilder CreateTypeBuilder(DataType dt, TypeBuilder parent, TypeAttributes typeAttrs, Type typeBase)
        {
            var result = parent?.DefineNestedType(dt.CilTypeName(), dt.CilTypeAttributes(true) | typeAttrs, typeBase) ?? _module.DefineType(dt.CilTypeName(), dt.CilTypeAttributes(false) | typeAttrs, typeBase);
            var data = new CilType(_backend, _essentials, _linker, result, dt);

            if (!dt.IsEnum && dt.IsFlattenedDefinition)
            {
                var flatParams = dt.FlattenedParameters;
                var flatParamNames = new string[flatParams.Length];

                for (int i = 0, l = flatParamNames.Length; i < l; i++)
                    flatParamNames[i] = flatParams[i].Name;

                var resultParams = result.DefineGenericParameters(flatParamNames);

                for (int i = 0; i < resultParams.Length; i++)
                    data.GenericParameters.Add(new CilMember<GenericTypeParameterBuilder, GenericParameterType>(resultParams[i], flatParams[i]));

                if (dt.IsGenericDefinition)
                {
                    var dtParams = dt.GenericParameters;

                    for (int i = 0, l = dtParams.Length; i < l; i++)
                        _linker.AddType(dtParams[i], resultParams[resultParams.Length - dtParams.Length + i]);
                }
            }

            _linker.AddType(dt, result);
            Add(data);
            return result;
        }
    }
}
