using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Uno.UX.Markup.Common
{
    public enum UxAttribute
    {
        AutoBind,
        AutoCtor,
        Binding,
        Case, // Deprecated
        Class,
        ClassName,
        ClearColor,
        Condition,
        DefaultCase, // Deprecated
        DefaultTemplate,
        Dependency,
        Generate,
        Global,
        GlobalResource,
        Include,
        InnerClass,
        Key,
        Name,
        Path,
        Property,
        Ref,
        Resource,
        Resources,
        StaticResource,
        Template,
        Test,
        Value,
        Simulate // Set to false to disable simulator for this class
    }

    public static class Attributes
    {
        // Used by code completion engine to enumerate possible ux: values
        public static string[] Values =
            ((IEnumerable<UxAttribute>)Enum.GetValues(typeof(UxAttribute)))
            .Select(x => x.ToString())
            .ToArray();

        public static bool HasUXAttrib(XElement elm, UxAttribute attrib)
        {
            var name = attrib.ToString();
            var attr = elm.Attributes().FirstOrDefault(x => x.Name.NamespaceName == Configuration.UXNamespace && x.Name.LocalName == name);
            return attr != null;
        }

        public static UxAttribute? TryGetUXAttrib(XName name)
        {
            return name.NamespaceName == Configuration.UXNamespace
                ? ((IEnumerable<UxAttribute>)Enum.GetValues(typeof(UxAttribute))).Select<UxAttribute, UxAttribute?>(x => x).FirstOrDefault(x => x.ToString() == name.LocalName)
                : null;
        }

        public static XAttribute TryGetUXAttrib(XElement elm, UxAttribute attrib)
        {
            var name = attrib.ToString();
            return elm.Attributes().FirstOrDefault(x => x.Name.NamespaceName == Configuration.UXNamespace && x.Name.LocalName == name);
        }
    }
}
