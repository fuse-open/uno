using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.UX.Markup.Reflection
{
    sealed class AttachedProperty
    {
        public Compiler.API.Domain.IL.Members.Method GetMethod { get; set; }
        public Compiler.API.Domain.IL.Members.Method SetMethod { get; set; }
        public Compiler.API.Domain.IL.Members.Method StyleSetMethod { get; set; }
        public string Name { get; set; }

        public Compiler.API.Domain.IL.DataType OwnerType
        {
            get
            {
                if (GetMethod != null) return GetMethod.Parameters[0].Type;
                else if (SetMethod != null) return SetMethod.Parameters[0].Type;
                else throw new Exception("Incomplete attached property: " + Name);
            }
        }

        public Compiler.API.Domain.IL.DataType DataType
        {
            get
            {
                if (GetMethod != null) return GetMethod.ReturnType;
                else if (SetMethod != null) return SetMethod.Parameters[1].Type;
                else throw new Exception("Incomplete attached property: " + Name);
            }
        }

        public AttachedProperty(string name, Method getter, Method setter, Method styleSetter)
        {
            GetMethod = getter;
            SetMethod = setter;
            StyleSetMethod = styleSetter;

            if (GetMethod != null && SetMethod != null)
            {
                if (GetMethod.ReturnType != SetMethod.Parameters[1].Type)
                    throw new Exception("Get and set methods of attached property " + name + " has inconsistent types: '" + GetMethod.ReturnType.Name + "', and '" + SetMethod.Parameters[1].Type + "'");
            }

            if (SetMethod != null && StyleSetMethod != null)
            {
                if (SetMethod.Parameters[1].Type != StyleSetMethod.Parameters[1].Type)
                    throw new Exception("Set and StyleSet methods of attached property " + name + " has inconsistent types: '" + SetMethod.Parameters[1].Type + "', and '" + StyleSetMethod.Parameters[1].Type + "'");
            }

            Name = name;
        }
    }

    sealed class AttachedEvent
    {
        public Compiler.API.Domain.IL.Members.Method AddMethod { get; set; }
        public Compiler.API.Domain.IL.Members.Method RemoveMethod { get; set; }
        public string Name { get; set; }

        public Compiler.API.Domain.IL.DataType OwnerType
        {
            get
            {
                if (AddMethod != null) return AddMethod.Parameters[0].Type;
                else if (RemoveMethod != null) return RemoveMethod.Parameters[0].Type;
                else throw new Exception("Incomplete attached event: " + Name);
            }
        }

        public Compiler.API.Domain.IL.DataType DataType
        {
            get
            {
                if (AddMethod != null) return AddMethod.Parameters[1].Type;
                else if (RemoveMethod != null) return RemoveMethod.Parameters[1].Type;
                else throw new Exception("Incomplete attached event: " + Name);
            }
        }

        public AttachedEvent(string name)
        {
            Name = name;
        }
    }

    sealed class AttachedMemberRegistry
    {
        readonly Dictionary<string, AttachedProperty> _attachedProperties = new Dictionary<string, AttachedProperty>();
        readonly Dictionary<string, AttachedEvent> _attachedEvents = new Dictionary<string, AttachedEvent>();

        internal IEnumerable<AttachedProperty> AttachedProperties => _attachedProperties.Values;
        internal IEnumerable<AttachedEvent> AttachedEvents => _attachedEvents.Values;

        public void Populate(Method[] allMethodsEver)
        {
            FindAttachedProperties(allMethodsEver);
            FindAttachedEvents(allMethodsEver);
        }

        void FindAttachedProperties(Method[] allMethodsEver)
        {
            foreach (var m in allMethodsEver)
            {
                var name = SetterAttributeName(m);
                if (name != null)
                {
                    if (!_attachedProperties.ContainsKey(name))
                        _attachedProperties.Add(name, new AttachedProperty(name, FindGetter(name, allMethodsEver), m, FindStyleSetter(name, allMethodsEver)));
                }
            }
        }

        Method FindGetter(string name, Method[] allMethodsEver)
        {
            foreach (var m in allMethodsEver)
            {
                if (GetterAttributeName(m) == name) return m;
            }
            return null;
        }

        Method FindStyleSetter(string name, Method[] allMethodsEver)
        {
            foreach (var m in allMethodsEver)
            {
                if (StyleSetterAttributeName(m) == name) return m;
            }
            return null;
        }

        void FindAttachedEvents(Method[] allMethodsEver)
        {
            foreach (var m in allMethodsEver)
            {
                var name = AdderAttributeName(m);
                if (name != null)
                {
                    if (!_attachedEvents.ContainsKey(name))
                        _attachedEvents.Add(name, new AttachedEvent(name));

                    _attachedEvents[name].AddMethod = m;
                }
            }

            foreach (var m in allMethodsEver)
            {
                var name = RemoverAttributeName(m);
                if (name != null)
                {
                    if (!_attachedEvents.ContainsKey(name))
                        _attachedEvents.Add(name, new AttachedEvent(name));

                    _attachedEvents[name].RemoveMethod = m;
                }
            }
        }

        static string GetterAttributeName(Method method)
        {
            return NameOrSomething(GetterAttribute(method));
        }

        static string SetterAttributeName(Method method)
        {
            return NameOrSomething(SetterAttribute(method));
        }

        static string StyleSetterAttributeName(Method method)
        {
            return NameOrSomething(StyleSetterAttribute(method));
        }

        static string AdderAttributeName(Method method)
        {
            return NameOrSomething(AdderAttribute(method));
        }

        static string RemoverAttributeName(Method method)
        {
            return NameOrSomething(RemoverAttribute(method));
        }

        static string NameOrSomething(NewObject method)
        {
            var arg0 = (Constant) method?.Arguments[0];
            return arg0 == null ? null : (string) arg0.Value;
        }

        static NewObject GetterAttribute(Method method)
        {
            return method.Attributes.FirstOrDefault(x => x.ReturnType.FullName == "Uno.UX.UXAttachedPropertyGetterAttribute");
        }

        static NewObject SetterAttribute(Method method)
        {
            return method.Attributes.FirstOrDefault(x => x.ReturnType.FullName == "Uno.UX.UXAttachedPropertySetterAttribute");
        }

        static NewObject StyleSetterAttribute(Method method)
        {
            return method.Attributes.FirstOrDefault(x => x.ReturnType.FullName == "Uno.UX.UXAttachedPropertyStyleSetterAttribute");
        }

        static NewObject AdderAttribute(Method method)
        {
            return method.Attributes.FirstOrDefault(x => x.ReturnType.FullName == "Uno.UX.UXAttachedEventAdderAttribute");
        }

        static NewObject RemoverAttribute(Method method)
        {
            return method.Attributes.FirstOrDefault(x => x.ReturnType.FullName == "Uno.UX.UXAttachedEventRemoverAttribute");
        }
    }
}
