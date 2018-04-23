using System;
using System.Collections.Generic;
using IKVM.Reflection;
using IKVM.Reflection.Emit;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using Type = IKVM.Reflection.Type;

namespace Uno.Compiler.Backends.CIL
{
    class CilType
    {
        readonly CilLinker _linker;
        public readonly TypeBuilder Builder;
        public readonly DataType Definition;
        public readonly List<CilMember<GenericTypeParameterBuilder, GenericParameterType>> GenericParameters = new List<CilMember<GenericTypeParameterBuilder,GenericParameterType>>();
        public readonly List<CilMember<FieldBuilder, Field>> Fields = new List<CilMember<FieldBuilder, Field>>();
        public readonly List<CilMember<PropertyBuilder, Property>> Properties = new List<CilMember<PropertyBuilder, Property>>();
        public readonly List<CilMember<EventBuilder, Event>> Events = new List<CilMember<EventBuilder, Event>>();
        public readonly List<CilMember<MethodBuilder, Function>> Methods = new List<CilMember<MethodBuilder, Function>>();
        public readonly List<CilMember<ConstructorBuilder, Constructor>> Constructors = new List<CilMember<ConstructorBuilder, Constructor>>();
        public bool IsCompiled;

        public CilType(CilLinker linker, TypeBuilder builder, DataType definition)
        {
            _linker = linker;
            Builder = builder;
            Definition = definition;
        }

        public FieldBuilder DefineField(Field m, Type type)
        {
            var b = Builder.DefineField(m.Name, type, m.CilFieldAttributes());
            Fields.Add(new CilMember<FieldBuilder, Field>(b, m));
            return b;
        }

        public EventBuilder DefineEvent(Event m, Type type)
        {
            var b = Builder.DefineEvent(m.Name, EventAttributes.None, type);
            Events.Add(new CilMember<EventBuilder, Event>(b, m));
            return b;
        }

        public PropertyBuilder DefineProperty(Property m, PropertyAttributes attributes, Type returnType, Type[] paramTypes)
        {
            var b = Builder.DefineProperty(m.Name, attributes, returnType, paramTypes);
            Properties.Add(new CilMember<PropertyBuilder, Property>(b, m));
            return b;
        }

        public MethodBuilder DefineMethod(Function m, MethodAttributes attributes, Type returnType, Type[] paramTypes)
        {
            var b = DefineMethod(m, attributes);
            b.SetReturnType(returnType);
            b.SetParameters(paramTypes);
            return b;
        }

        public MethodBuilder DefineMethod(Function m, MethodAttributes attributes = 0)
        {
            if ((m as Method)?.ImplementedMethod != null)
                attributes |= MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Final;

            if (m.DeclaringType.IsInterface)
                attributes &= ~MethodAttributes.Final;

            var b = Builder.DefineMethod(m.Name, m.CilMethodAttributes() | attributes);
            Methods.Add(new CilMember<MethodBuilder, Function>(b, m));
            return b;
        }

        public ConstructorBuilder DefineConstructor(Constructor m, Type[] paramTypes)
        {
            var b = Builder.DefineConstructor(m.CilMethodAttributes() | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, paramTypes);
            Constructors.Add(new CilMember<ConstructorBuilder, Constructor>(b, m));
            return b;
        }

        public void DefineTypeInitializer(Constructor m)
        {
            Constructors.Add(new CilMember<ConstructorBuilder, Constructor>(Builder.DefineTypeInitializer(), m));
        }

        public void DefineTypeFinalizer(Finalizer m)
        {
            Methods.Add(new CilMember<MethodBuilder, Function>(Builder.DefineMethod("Finalize", MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig, CallingConventions.Standard, _linker.System_Void, Type.EmptyTypes), m));
        }
    }
}
