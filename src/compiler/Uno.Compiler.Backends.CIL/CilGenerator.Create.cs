using IKVM.Reflection;
using IKVM.Reflection.Emit;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Logging;
using Type = IKVM.Reflection.Type;

namespace Uno.Compiler.Backends.CIL
{
    partial class CilGenerator
    {
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
            var data = new CilType(_linker, result, dt);

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
            _types.Add(data);
            return result;
        }

        void SetConstraints(GenericTypeParameterBuilder builder, GenericParameterType definition)
        {
            var attrs = GenericParameterAttributes.None;

            switch (definition.ConstraintType)
            {
                case GenericConstraintType.Class:
                    if (definition.Base != _essentials.Object)
                        builder.SetBaseTypeConstraint(_linker.GetType(definition.Base));
                    else
                        attrs |= GenericParameterAttributes.ReferenceTypeConstraint;
                    break;

                case GenericConstraintType.Struct:
                    attrs |= GenericParameterAttributes.NotNullableValueTypeConstraint;
                    break;
            }

            if (definition.Constructors.Count > 0)
                attrs |= GenericParameterAttributes.DefaultConstructorConstraint;

            if (attrs != GenericParameterAttributes.None)
                builder.SetGenericParameterAttributes(attrs);

            if (definition.Interfaces.Length > 0)
            {
                var interfaceTypes = new Type[definition.Interfaces.Length];

                for (int j = 0; j < definition.Interfaces.Length; j++)
                    interfaceTypes[j] = _linker.GetType(definition.Interfaces[j]);

                builder.SetInterfaceConstraints(interfaceTypes);
            }
        }
    }
}
