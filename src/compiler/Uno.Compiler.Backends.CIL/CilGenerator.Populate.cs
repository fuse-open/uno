using IKVM.Reflection;
using IKVM.Reflection.Emit;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Backends.PInvoke;
using ParameterModifier = Uno.Compiler.API.Domain.ParameterModifier;

namespace Uno.Compiler.Backends.CIL
{
    partial class CilGenerator
    {
        void PopulateClassStructOrInterface(CilType data)
        {
            if (data.Definition is ClassType && data.Definition.Base != null)
                data.Builder.SetParent(_linker.GetType(data.Definition.Base));

            foreach (var it in data.Definition.Interfaces)
                data.Builder.AddInterfaceImplementation(_linker.GetType(it));

            foreach (var m in data.Definition.Fields)
                _linker.AddField(m, data.DefineField(m, _linker.GetType(m.ReturnType)));

            foreach (var m in data.Definition.Events)
            {
                var eb = data.DefineEvent(m, _linker.GetType(m.ReturnType));

                if (m.ImplicitField != null)
                    _linker.AddField(m.ImplicitField,
                        data.DefineField(m.ImplicitField, _linker.GetType(m.ImplicitField.ReturnType)));

                if (m.AddMethod != null)
                {
                    var mb = data.DefineMethod(m.AddMethod, MethodAttributes.SpecialName, _linker.GetType(m.AddMethod.ReturnType), _linker.GetParameterTypes(m.AddMethod.Parameters));
                    _linker.AddMethod(m.AddMethod, mb);
                    eb.SetAddOnMethod(mb);
                }

                if (m.RemoveMethod != null)
                {
                    var mb = data.DefineMethod(m.RemoveMethod, MethodAttributes.SpecialName, _linker.GetType(m.RemoveMethod.ReturnType), _linker.GetParameterTypes(m.RemoveMethod.Parameters));
                    _linker.AddMethod(m.RemoveMethod, mb);
                    eb.SetRemoveOnMethod(mb);
                }
            }

            foreach (var m in data.Definition.Properties)
            {
                var pb = data.DefineProperty(m, m.Parameters.Length > 0 ? PropertyAttributes.SpecialName : 0, _linker.GetType(m.ReturnType), _linker.GetParameterTypes(m.Parameters));

                if (m.ImplicitField != null)
                    _linker.AddField(m.ImplicitField,
                        data.DefineField(m.ImplicitField, _linker.GetType(m.ImplicitField.ReturnType)));

                if (m.GetMethod != null)
                {
                    var mb = data.DefineMethod(m.GetMethod, MethodAttributes.SpecialName, _linker.GetType(m.GetMethod.ReturnType), _linker.GetParameterTypes(m.GetMethod.Parameters));
                    _linker.AddMethod(m.GetMethod, mb);
                    pb.SetGetMethod(mb);
                }

                if (m.SetMethod != null)
                {
                    var mb = data.DefineMethod(m.SetMethod, MethodAttributes.SpecialName, _linker.GetType(m.SetMethod.ReturnType), _linker.GetParameterTypes(m.SetMethod.Parameters));
                    _linker.AddMethod(m.SetMethod, mb);
                    pb.SetSetMethod(mb);
                }
            }

            foreach (var m in data.Definition.Methods)
            {
                if (_backend.IsPInvokable(_essentials, m))
                {
                    var mb = PInvokeBackend.CreateCilPInvokeMethod(
                        _linker.Universe,
                        _essentials,
                        data.Builder,
                        m,
                        m.CilMethodAttributes(),
                        _linker.GetType(m.ReturnType),
                        _linker.GetParameterTypes(m.Parameters));
                    m.SetBody(null);
                    data.Methods.Add(new CilMember<MethodBuilder, Function>(mb, m));
                    _linker.AddMethod(m, mb);
                }
                else
                {
                    var mb = data.DefineMethod(m);

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

            foreach (var m in data.Definition.Constructors)
            {
                var cb = data.DefineConstructor(m, _linker.GetParameterTypes(m.Parameters));
                _linker.AddConstructor(m, cb);
            }

            foreach (var m in data.Definition.Casts)
            {
                var mb = data.DefineMethod(m, MethodAttributes.SpecialName, _linker.GetType(m.ReturnType), _linker.GetParameterTypes(m.Parameters));
                _linker.AddMethod(m, mb);
            }

            foreach (var m in data.Definition.Operators)
            {
                var mb = data.DefineMethod(m, MethodAttributes.SpecialName, _linker.GetType(m.ReturnType), _linker.GetParameterTypes(m.Parameters));
                _linker.AddMethod(m, mb);
            }

            if (data.Definition.Initializer != null)
                data.DefineTypeInitializer(data.Definition.Initializer);

            if (data.Definition.Finalizer != null)
                data.DefineTypeFinalizer(data.Definition.Finalizer);

            // Add dummy field if [TargetSpecificType]
            if (data.Definition.HasAttribute(_essentials.TargetSpecificTypeAttribute))
                data.Builder.DefineField("__ptr", _linker.System_IntPtr, FieldAttributes.Private);
        }

        void PopulateDelegate(CilType data)
        {
            var dt = (DelegateType)data.Definition;
            var cb = data.Builder.DefineConstructor(
                MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public,
                CallingConventions.Standard, new[] {_linker.System_Object, _linker.System_IntPtr});

            cb.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

            var mb = data.Builder.DefineMethod("Invoke",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                _linker.GetType(dt.ReturnType), _linker.GetParameterTypes(dt.Parameters));

            mb.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

            if (_backend.IsPInvokable(_essentials, dt))
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

            _linker.AddConstructor(data.Definition, cb);
            _linker.AddMethod(data.Definition, mb);
        }

        void PopulateEnum(CilType data)
        {
            data.Builder.DefineField("value__", _linker.GetType(data.Definition.Base), FieldAttributes.Public | FieldAttributes.SpecialName | FieldAttributes.RTSpecialName);

            for (int i = 0, l = data.Definition.Literals.Count; i < l; i++)
            {
                var v = data.Definition.Literals[i];
                var f = data.Builder.DefineField(v.UnoName, data.Builder, FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.Literal);
                f.SetConstant(v.Value);
            }
        }

        void PopulateParameter(Parameter p, ParameterBuilder pb)
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
    }
}
