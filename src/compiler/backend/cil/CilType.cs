using System;
using System.Collections.Generic;
using IKVM.Reflection;
using IKVM.Reflection.Emit;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Logging;
using ParameterModifier = Uno.Compiler.API.Domain.ParameterModifier;
using Type = IKVM.Reflection.Type;

namespace Uno.Compiler.Backends.CIL
{
    public class CilType : LogObject
    {
        readonly Backend _backend;
        readonly IEssentials _essentials;
        readonly CilLinker _linker;
        public readonly TypeBuilder Builder;
        public readonly DataType Definition;
        public readonly List<CilMember<GenericTypeParameterBuilder, GenericParameterType>> GenericParameters = new List<CilMember<GenericTypeParameterBuilder,GenericParameterType>>();
        public readonly List<CilMember<FieldBuilder, Field>> Fields = new List<CilMember<FieldBuilder, Field>>();
        public readonly List<CilMember<PropertyBuilder, Property>> Properties = new List<CilMember<PropertyBuilder, Property>>();
        public readonly List<CilMember<EventBuilder, Event>> Events = new List<CilMember<EventBuilder, Event>>();
        public readonly List<CilMember<MethodBuilder, Function>> Methods = new List<CilMember<MethodBuilder, Function>>();
        public readonly List<CilMember<ConstructorBuilder, Constructor>> Constructors = new List<CilMember<ConstructorBuilder, Constructor>>();

        internal CilType(Backend backend, IEssentials essentials, CilLinker linker, TypeBuilder builder, DataType definition)
            : base(backend.Log)
        {
            _backend = backend;
            _essentials = essentials;
            _linker = linker;
            Builder = builder;
            Definition = definition;
        }

        public void Populate()
        {
            foreach (var g in GenericParameters)
                SetConstraints(g.Builder, g.Definition);

            switch (Definition.TypeType)
            {
                case TypeType.Enum:
                    PopulateEnum();
                    break;
                case TypeType.Delegate:
                    PopulateDelegate();
                    break;
                default:
                    PopulateClassStructOrInterface();
                    break;
            }
        }

        void PopulateClassStructOrInterface()
        {
            if (Definition is ClassType && Definition.Base != null)
                Builder.SetParent(_linker.GetType(Definition.Base));

            foreach (var it in Definition.Interfaces)
                Builder.AddInterfaceImplementation(_linker.GetType(it));

            foreach (var m in Definition.Fields)
                _linker.AddField(m, DefineField(m, _linker.GetType(m.ReturnType)));

            foreach (var m in Definition.Events)
            {
                var eb = DefineEvent(m, _linker.GetType(m.ReturnType));

                if (m.ImplicitField != null)
                    _linker.AddField(m.ImplicitField,
                        DefineField(m.ImplicitField, _linker.GetType(m.ImplicitField.ReturnType)));

                if (m.AddMethod != null)
                {
                    var mb = DefineMethod(m.AddMethod, MethodAttributes.SpecialName, _linker.GetType(m.AddMethod.ReturnType), _linker.GetParameterTypes(m.AddMethod.Parameters));
                    _linker.AddMethod(m.AddMethod, mb);
                    eb.SetAddOnMethod(mb);
                }

                if (m.RemoveMethod != null)
                {
                    var mb = DefineMethod(m.RemoveMethod, MethodAttributes.SpecialName, _linker.GetType(m.RemoveMethod.ReturnType), _linker.GetParameterTypes(m.RemoveMethod.Parameters));
                    _linker.AddMethod(m.RemoveMethod, mb);
                    eb.SetRemoveOnMethod(mb);
                }
            }

            foreach (var m in Definition.Properties)
            {
                var pb = DefineProperty(m, m.Parameters.Length > 0 ? PropertyAttributes.SpecialName : 0, _linker.GetType(m.ReturnType), _linker.GetParameterTypes(m.Parameters));

                if (m.ImplicitField != null)
                    _linker.AddField(m.ImplicitField,
                        DefineField(m.ImplicitField, _linker.GetType(m.ImplicitField.ReturnType)));

                if (m.GetMethod != null)
                {
                    var mb = DefineMethod(m.GetMethod, MethodAttributes.SpecialName, _linker.GetType(m.GetMethod.ReturnType), _linker.GetParameterTypes(m.GetMethod.Parameters));
                    _linker.AddMethod(m.GetMethod, mb);
                    pb.SetGetMethod(mb);
                }

                if (m.SetMethod != null)
                {
                    var mb = DefineMethod(m.SetMethod, MethodAttributes.SpecialName, _linker.GetType(m.SetMethod.ReturnType), _linker.GetParameterTypes(m.SetMethod.Parameters));
                    _linker.AddMethod(m.SetMethod, mb);
                    pb.SetSetMethod(mb);
                }
            }

            foreach (var m in Definition.Methods)
            {
                if (m.IsPInvokable(_essentials, Log))
                {
                    var mb = PInvokeBackend.CreateCilPInvokeMethod(
                        _linker.Universe,
                        _essentials,
                        Builder,
                        m,
                        m.CilMethodAttributes(),
                        _linker.GetType(m.ReturnType),
                        _linker.GetParameterTypes(m.Parameters));
                    m.SetBody(null);
                    Methods.Add(new CilMember<MethodBuilder, Function>(mb, m));
                    _linker.AddMethod(m, mb);
                }
                else
                {
                    var mb = DefineMethod(m);

                    if (m.IsGenericDefinition)
                    {
                        var paramNames = new string[m.GenericParameters.Length];
                        for (int i = 0; i < paramNames.Length; i++)
                            paramNames[i] = m.GenericParameters[i].UnoName;

                        var paramTypes = mb.DefineGenericParameters(paramNames);
                        for (int i = 0; i < paramTypes.Length; i++)
                            _linker.AddType(m.GenericParameters[i], paramTypes[i]);
                        for (int i = 0; i < paramTypes.Length; i++)
                            SetConstraints(paramTypes[i], m.GenericParameters[i]);
                    }

                    mb.SetReturnType(_linker.GetType(m.ReturnType));
                    mb.SetParameters(_linker.GetParameterTypes(m.Parameters));
                    _linker.AddMethod(m, mb);

                    if (m.Prototype != null && m.Prototype != m &&
                        m.Prototype.HasAttribute(_essentials.DotNetOverrideAttribute))
                        _linker.AddMethod(m.Prototype, mb);
                }
            }

            foreach (var m in Definition.Constructors)
            {
                var cb = DefineConstructor(m, _linker.GetParameterTypes(m.Parameters));
                _linker.AddConstructor(m, cb);
            }

            foreach (var m in Definition.Casts)
            {
                var mb = DefineMethod(m, MethodAttributes.SpecialName, _linker.GetType(m.ReturnType), _linker.GetParameterTypes(m.Parameters));
                _linker.AddMethod(m, mb);
            }

            foreach (var m in Definition.Operators)
            {
                var mb = DefineMethod(m, MethodAttributes.SpecialName, _linker.GetType(m.ReturnType), _linker.GetParameterTypes(m.Parameters));
                _linker.AddMethod(m, mb);
            }

            if (Definition.Initializer != null)
                DefineTypeInitializer(Definition.Initializer);

            if (Definition.Finalizer != null)
                DefineTypeFinalizer(Definition.Finalizer);

            // Add dummy field if [TargetSpecificType]
            if (Definition.HasAttribute(_essentials.TargetSpecificTypeAttribute))
                Builder.DefineField("__ptr", _linker.System_IntPtr, FieldAttributes.Private);

            if (Definition.IsStatic)
                Builder.SetCustomAttribute(
                    new CustomAttributeBuilder(_linker.System_Runtime_CompilerServices_ExtensionAttribute_ctor, new object[0]));
        }

        void PopulateDelegate()
        {
            var dt = (DelegateType)Definition;
            var cb = Builder.DefineConstructor(
                MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public,
                CallingConventions.Standard, new[] { _linker.System_Object, _linker.System_IntPtr });

            cb.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

            var mb = Builder.DefineMethod("Invoke",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                _linker.GetType(dt.ReturnType), _linker.GetParameterTypes(dt.Parameters));

            mb.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

            if (dt.IsPInvokable(_essentials))
            {
                for (int i = 0; i < dt.Parameters.Length; i++)
                {
                    var p = dt.Parameters[i];
                    var pb = PInvokeBackend.DefineCilDelegateParameter(_linker.Universe, _essentials, mb, p, i);
                    PopulateParameter(p, pb);
                }
            }
            else
            {
                for (int i = 0; i < dt.Parameters.Length; i++)
                {
                    var p = dt.Parameters[i];
                    PopulateParameter(p, mb.DefineParameter(i + 1, p.CilParameterAttributes(), p.Name));
                }
            }

            _linker.AddConstructor(Definition, cb);
            _linker.AddMethod(Definition, mb);
        }

        void PopulateEnum()
        {
            Builder.DefineField("value__", _linker.GetType(Definition.Base), FieldAttributes.Public | FieldAttributes.SpecialName | FieldAttributes.RTSpecialName);

            for (int i = 0, l = Definition.Literals.Count; i < l; i++)
            {
                var v = Definition.Literals[i];
                var f = Builder.DefineField(v.UnoName, Builder, FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.Literal);
                f.SetConstant(v.Value);
            }
        }

        public void PopulateParameter(Parameter p, ParameterBuilder pb)
        {
            try
            {
                if (p.OptionalDefault is Constant)
                    pb.SetConstant(p.OptionalDefault.ConstantValue);
            }
            catch
            {
                // TODO
            }

            if (p.Modifier == ParameterModifier.Params)
                pb.SetCustomAttribute(new CustomAttributeBuilder(_linker.System_ParamAttribute_ctor, new object[0]));
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

        FieldBuilder DefineField(Field m, Type type)
        {
            var b = Builder.DefineField(m.Name, type, m.CilFieldAttributes());
            Fields.Add(new CilMember<FieldBuilder, Field>(b, m));
            return b;
        }

        EventBuilder DefineEvent(Event m, Type type)
        {
            var b = Builder.DefineEvent(m.Name, EventAttributes.None, type);
            Events.Add(new CilMember<EventBuilder, Event>(b, m));
            return b;
        }

        PropertyBuilder DefineProperty(Property m, PropertyAttributes attributes, Type returnType, Type[] paramTypes)
        {
            var b = Builder.DefineProperty(m.Name, attributes, returnType, paramTypes);
            Properties.Add(new CilMember<PropertyBuilder, Property>(b, m));
            return b;
        }

        MethodBuilder DefineMethod(Function m, MethodAttributes attributes, Type returnType, Type[] paramTypes)
        {
            var b = DefineMethod(m, attributes);
            b.SetReturnType(returnType);
            b.SetParameters(paramTypes);
            return b;
        }

        MethodBuilder DefineMethod(Function m, MethodAttributes attributes = 0)
        {
            if ((m as Method)?.ImplementedMethod != null)
                attributes |= MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Final;

            if (m.DeclaringType.IsInterface)
                attributes &= ~MethodAttributes.Final;

            var b = Builder.DefineMethod(m.Name, m.CilMethodAttributes() | attributes);
            Methods.Add(new CilMember<MethodBuilder, Function>(b, m));
            return b;
        }

        ConstructorBuilder DefineConstructor(Constructor m, Type[] paramTypes)
        {
            var b = Builder.DefineConstructor(m.CilMethodAttributes() | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, paramTypes);
            Constructors.Add(new CilMember<ConstructorBuilder, Constructor>(b, m));
            return b;
        }

        void DefineTypeInitializer(Constructor m)
        {
            Constructors.Add(new CilMember<ConstructorBuilder, Constructor>(Builder.DefineTypeInitializer(), m));
        }

        void DefineTypeFinalizer(Finalizer m)
        {
            Methods.Add(new CilMember<MethodBuilder, Function>(Builder.DefineMethod("Finalize", MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig, CallingConventions.Standard, _linker.System_Void, Type.EmptyTypes), m));
        }
    }
}
