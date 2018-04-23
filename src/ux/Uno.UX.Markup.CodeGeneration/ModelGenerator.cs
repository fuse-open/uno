using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Uno.Compiler.API.Utilities;
using Uno.Compiler.API.IL;
using Uno.Compiler.API.IL.Blocks;
using Uno.Compiler.API.IL.Expressions;
using Uno.Compiler.API.IL.Members;
using Uno.Compiler.API.IL.Statements;
using Uno.Compiler.API.IL.Types;
using Uno.Compiler;
using Uno.Compiler.Core;


namespace Uno.UX.Markup.CodeGeneration
{
    class ModelGenerator
    {
        Compiler _compiler;
        Compiler Compiler { get { return _compiler; } }
        Namespace IL { get { return _compiler.IL; } }

        Dictionary<string, string> builtInTypeMap = new Dictionary<string, string>()
        {
            { "string", "System.String"},
            { "bool", "System.Boolean" },
            { "float", "System.Single" },
            { "double", "System.Double" },
            { "long", "System.Int64" },
            { "ulong", "System.UInt64" },
            { "int", "System.Int32" },
            { "uint", "System.UInt32" },
            { "short", "System.Int16" },
            { "ushort", "System.UInt16" },
            { "byte", "System.Byte" },
            { "sbyte", "System.SByte" }
        };

        Dictionary<DataType, Type> _typeMap = new Dictionary<DataType, Type>();

        Type UnoTypeToCilType(IXmlLineInfo obj, DataType dt)
        {
            Type t;
            if (!_typeMap.TryGetValue(dt, out t))
            {
                t = typeof(Uno.Float2).Assembly.GetTypes().Where(x => x.FullName == dt.QualifiedName).FirstOrDefault();

                if (dt is EnumType)
                {
                    _typeMap.Add(dt, typeof(int));
                    return typeof(int);
                }

                if (t != null) _typeMap.Add(dt, t);
                else
                {
                    var sysname = dt.FullName;
                    if (builtInTypeMap.ContainsKey(sysname)) sysname = builtInTypeMap[sysname];

                    t = typeof(int).Assembly.GetTypes().Where(x => x.FullName == sysname).FirstOrDefault();
                    if (t != null) _typeMap.Add(dt, t);
                    else throw Error(obj, "Illegal atomic type: " + dt.FullName);
                }
            }
            return t;
        }

        Field[] _styleProperties;
        
        internal ModelGenerator(Compiler compiler)
        {
            _compiler = compiler;

            var stylePropertyType = compiler.ResolveString(Source.Unknown, "Uno.Styling.StyleProperty");

            if (stylePropertyType != null)
            {
                _styleProperties = _compiler.FindAllDataTypes()
                    .SelectMany(x => x.Fields)
                    .Distinct()
                    .Where(x => x.DataType.FullName.StartsWith("Uno.Styling.StyleProperty") && x.AstName.EndsWith("Property"))
                    .ToArray();
            }
        }


        Dictionary<string, Namespace[]> _namespaces = new Dictionary<string, Namespace[]>();

        string _uxFilename;

        Source GetSource(IXmlLineInfo obj) { return new Source(_uxFilename, obj.LineNumber); }

        Namespace[] GetNamespaces(string names, Namespace extra = null)
        {
            Namespace[] namespaces;
            if (!_namespaces.TryGetValue(names, out namespaces))
            {
                var nss = names.Split(',').Select(x => x.Trim()).ToArray();
                var k = nss.Select(x => Compiler.ResolveString(Source.Unknown, x) as Namespace).Where(x => x != null);
                if (extra != null) k = k.Concat(new[] { extra });

                namespaces = k.ToArray();
                _namespaces.Add(names, namespaces);
            }

            return namespaces;
        }

        DataType GetDataType(XElement element)
        {
            var namespaces = GetNamespaces(element.Name.NamespaceName);
            return Compiler.ResolveString(Source.Unknown, element.Name.LocalName, namespaces) as DataType;
        }

        bool IsUXNamespaceElement(XElement element)
        {
            return element.Name.Namespace == Configuration.UXNamespace;
        }

        bool IsUXNamespaceAttribute(XAttribute element)
        {
            return element.Name.Namespace == Configuration.UXNamespace;
        }

        Exception Error(IXmlLineInfo obj, string message)
        {
            Compiler.Log.ReportError(GetSource(obj), ErrorCode.EUNKNOWN, message);
            return new Exception(message);
        }

        internal ModelNode GenerateModel(string uxFilename)
        {
            _uxFilename = uxFilename;
            var root = System.Xml.Linq.XElement.Load(uxFilename);
            
            // Register root namespace as part of the default namespace
            GetNamespaces(root.GetDefaultNamespace().NamespaceName, IL);

            if (IsUXNamespaceElement(root))
            {
                throw Error(root, "Invalid UX root element");
            }
            else
            {
                return Convert(null, root);
            }            
        }

        T GetUXAttribute<T>(XElement elm, string name, T defaultValue)
        {
            var v = elm.Attributes()
                .Where(x => x.Name.NamespaceName == Configuration.UXNamespace)
                .Where(x => x.Name.LocalName == name).FirstOrDefault();

            if (v != null) return (T)ValueMarshal.Parse(v.Value, typeof(T));
            else return defaultValue;
        }

        ModelNode Convert(ModelNode parent, XElement elm)
        {
            var dt = GetDataType(elm);
            if (dt == null)
            {
                throw Error(elm, "Data type '" + elm.Name.LocalName + "' could not be resolved");
            }

            var name = GetUXAttribute(elm, "Name", (string)null);
            var className = GetUXAttribute(elm, "ClassName", (string)null);
            var genMember = GetUXAttribute(elm, "Member", GenerateMemberMode.None);
            var autoBind = GetUXAttribute(elm, "AutoBind", true);
            var bindingString = GetUXAttribute(elm, "Binding", "");
            
            var bindings = (string[])null;
            if (bindingString.Length > 0) bindings = bindingString.Split(',');

            var node = new ModelNode(dt, name, genMember, className, autoBind);

            // Process explicit bindings
            if (parent == null)
            {
                Error(elm, "Illegal 'Binding': element has no parent");
            }
            else
            {
                if (bindings != null)
                {
                    foreach (var b in bindings)
                    {
                        var ps = FindPropertyOrStyleProperty(elm, parent.DataType, b);
                        parent.AddBinding(ps, node);
                    }
                }
            }

            // Process implicit bindings
            if (parent != null && autoBind)
            {
                AddAutoBindings(parent, node);
            }

            foreach (var attr in elm.Attributes())
            {
                if (IsUXNamespaceAttribute(attr)) continue;

                var prop = attr.Name.LocalName;

                var ps = FindPropertyOrStyleProperty(attr, node.DataType, prop);

                if (ps is Property)
                {
                    var value = Parse(attr, attr.Value, (ps as Property).DataType);
                    if (value != null)
                    {
                        node.PropertyValues[ps as Property] = value;
                    }
                }
                else if (ps is Field)
                {
                    var styleProp = ps as Field;

                    var value = Parse(attr, attr.Value, styleProp.DataType.GenericTypeArguments[1]);
                    if (value != null)
                    {
                        node.PropertyValues[styleProp] = value;
                    }
                }
            }
            
            foreach (var child in elm.Elements())
            {
                if (IsUXNamespaceElement(child))
                {

                }
                else
                {
                    node.Children.Add(Convert(node, child));
                }
            }

            return node;
        }

        public static IEnumerable<Property> EnumeratePropertiesRecursive(DataType dt)
        {
            foreach (var p in dt.Properties)
                yield return p;

            if (dt.BaseType != null)
                foreach (var p in EnumeratePropertiesRecursive(dt.BaseType))
                    yield return p;
        }

        IEnumerable<Property> FindPropertiesWithAttribute(DataType dt, string designername)
        {
            return EnumeratePropertiesRecursive(dt).Where(x => x.Attributes.Any(y => y.Constructor.DeclaringType.FullName == "Uno.Scenes.Designer." + designername + "Attribute"));
        }

        Property PrimaryProperty(ModelNode node)
        {
            return FindPropertiesWithAttribute(node.DataType, "Primary").FirstOrDefault();
        }

        Property ComponentsProperty(ModelNode node)
        {
            return FindPropertiesWithAttribute(node.DataType, "Components").FirstOrDefault();
        }

        IEnumerable<Property> SerializationChildProperties(ModelNode node)
        {
            return FindPropertiesWithAttribute(node.DataType, "SerializationChild");
        }

        DataType GetActualPropertyType(Property p)
        {
            if (p.DataType.IsListType()) return p.DataType.GetListElementType();
            else return p.DataType;
        }

        void AddAutoBindings(ModelNode parent, ModelNode child)
        {
            var components = ComponentsProperty(parent);
            if (components != null && child.DataType.IsCompatibleWith(GetActualPropertyType(components)))
            {
                parent.AddBinding(components, child);
                return;
            }

            var serailizationChildren = SerializationChildProperties(parent);
            foreach (var p in serailizationChildren)
            {
                if (child.DataType.IsCompatibleWith(GetActualPropertyType(p)))
                {
                    parent.AddBinding(p, child);
                    return;
                }
            }

            var primary = PrimaryProperty(parent);
            if (primary != null && child.DataType.IsCompatibleWith(GetActualPropertyType(primary)))
            {
                parent.AddBinding(primary, child);
            }
        }

        Member FindPropertyOrStyleProperty(IXmlLineInfo attr, DataType owner, string prop)
        {
            var styleProps = _styleProperties
                    .Where(x => owner.IsEqualOrSubClassOf(x.DataType.GenericTypeArguments[0]))
                    .Where(x => prop + "Property" == x.AstName);

            if (styleProps.Count() > 1)
            {
                Error(attr, "'" + prop + "' is an ambiguous style property, can be " +
                    styleProps.Select(a => a.DeclaringType.FullName + "." + a.AstName).Aggregate((a, b) => (a + " or " + b)));

                return null;
            }
            else if (styleProps.Count() == 1)
            {
                return styleProps.First();
            }
            else
            {
                var regularProp = owner.TryFindProperty(prop, true);
                if (regularProp == null)
                {
                    Error(attr, "'" + prop + "' is not a property or style property of '" + owner.FullName + "'");
                    return null;
                }

                return regularProp;
            }
        }

        object Parse(IXmlLineInfo attr, string value, DataType dt)
        {
            var cilType = UnoTypeToCilType(attr, dt);

            if (cilType == null)
            {
                Error(attr, "A value of type '" + dt.FullName + "' cannot be parsed");
            }

            try
            {
                return ValueMarshal.Parse(value, cilType);
            }
            catch (Exception e)
            {
                Error(attr, "Failed to parse value of type '" + dt.FullName + "': " + e.Message);
            }

            return null;
        }
    }
}
