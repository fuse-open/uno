using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Uno.Compiler.Frontend.Analysis;
using Uno.UX.Markup.Common;

namespace Uno.UX.Markup.AST
{
    public class Parser
    {
        static Element Parse(string projName, string p, out XDocument xdoc, Common.IMarkupErrorLog log)
        {
            using (var stream = File.OpenRead(p))
            {
                xdoc = XmlHelpers.ReadAllXml(stream, LoadOptions.SetLineInfo, true);
                return Parse(projName, p, xdoc, log);
            }
        }

        public static Element Parse(string projName, string p, XDocument xdoc, Common.IMarkupErrorLog log)
        {
            try
            {
                return Parse(new Parser(projName, p, xdoc, log));
            }
            catch (XmlException e)
            {
                log.ReportError(p, e.LineNumber, e.Message);
                throw;
            }
        }

        static Element Parse(Parser parser)
        {
            if (parser._doc == null)
            {
                return null;
            }

            return parser.ParseRoot(parser._doc.Root);
        }

        readonly string _projectName;
        readonly string _filePath;
        readonly XDocument _doc;
        readonly Common.IMarkupErrorLog _log;
        readonly HashSet<XAttribute> VisitedUXAttribs = new HashSet<XAttribute>();

        Parser(string projectName, string filePath, XDocument doc, Common.IMarkupErrorLog log)
        {
            _projectName = projectName;
            _filePath = filePath;
            _log = log;
            _doc = doc;
        }

        void ReportError(string message)
        {
            _log.ReportError(_filePath, 1, message);
        }
        
        void ReportError(IXmlLineInfo xml, string message)
        {
            _log.ReportError(_filePath, xml.LineNumber, message);
        }

        void ReportWarning(IXmlLineInfo xml, string message)
        {
            _log.ReportWarning(_filePath, xml.LineNumber, message);
        }

        string GetUXAttrib(XElement elm, UxAttribute attrib, string defaultValue)
        {
            var name = attrib.ToString();
            var attr = Attributes.TryGetUXAttrib(elm, attrib);
            if (attr != null)
            {
                VisitedUXAttribs.Add(attr);
                return attr.Value;
            }
            else return defaultValue;
        }

        void ReportErrorOnUnvisitedUXAttribs(XElement elm)
        {
            var attr = elm.Attributes().Where(x => x.Name.NamespaceName == Configuration.UXNamespace && !VisitedUXAttribs.Contains(x));
            foreach (var a in attr)
            {
                ReportError(elm, "Invalid UX attribute: ux:" + a.Name.LocalName);
            }
        }

        Element ParseRoot(XNode node)
        {
            var r = Parse(node) as Element;

            if (r == null)
            {
                ReportError("Root element cannot be a reference");
                return null;
            }

            return r;
        }
        
        
        Node Parse(XNode node)
        {
            var text = node as XText;
            if (!string.IsNullOrWhiteSpace(text?.Value))
                return new Text(text.Value, new FileSourceInfo(_filePath, ((IXmlLineInfo)text).LineNumber));

            var elm = node as XElement;
            if (elm != null)
                return ParseElement(elm);

            return null;
        }

        Node ParseElement(XElement elm)
        {
            var source = new FileSourceInfo(_filePath, ((IXmlLineInfo)elm).LineNumber);

            var ns = elm.Name.NamespaceName;

            var typeName = elm.Name.LocalName.Replace('-', '_');
            var autoBind = GetUXAttrib(elm, UxAttribute.AutoBind, "true");
            var explicitAutoBindFalse = !bool.Parse(autoBind);
            var generator = ParseGenerator(elm, ref explicitAutoBindFalse);

            var attr = Attributes.TryGetUXAttrib(elm.Name);
            if (attr != null)
            {
                if (attr == UxAttribute.Include)
                {
                    var fileAttr = elm.Attributes().FirstOrDefault(x => x.Name == "File");
                    if (fileAttr == null)
                    {
                        ReportError(elm, "ux:Include missing required 'File' attribute");
                        return null;
                    }

                    var dir = Path.GetDirectoryName(_filePath);
                    var f = Path.Combine(dir, fileAttr.Value);

                    XDocument d;
                    var e = Parse(_projectName, f, out d, _log);

                    if (e.Generator is ClassGenerator)
                    {
                        var cg = (ClassGenerator)e.Generator;
                        if (!cg.IsInnerClass)
                        {
                            ReportError("You cannot ux:Include the file '" + fileAttr.Value + "' because it's root node is marked with ux:Class. You can instantiate the class with <" + cg.ClassName + " /> instead. ");
                            return null;
                        }
                    }

                    return e;
                }
                else if (attr == UxAttribute.Resources)
                {
                    typeName = "object";
                    ns = Configuration.DefaultNamespace;
                    generator = new ClassGenerator(false, _projectName.ToIdentifier() + "_" + System.IO.Path.GetFileNameWithoutExtension(_filePath).ToIdentifier() + "_res", true, true);
                }
                else
                {
                    ReportError("Unrecognized ux:-directive: 'ux:" + elm.Name + "'. Did you mean ux:Include ? ");
                    return null;
                }
            }

            var uxDependency = GetUXAttrib(elm, UxAttribute.Dependency, null);
            var uxTemplate = GetUXAttrib(elm, UxAttribute.Template, null);
            var uxName = GetUXAttrib(elm, UxAttribute.Name, null);
            var uxProperty = GetUXAttrib(elm, UxAttribute.Property, null);
            var uxValue = GetUXAttrib(elm, UxAttribute.Value, null);
            var uxKey = GetUXAttrib(elm, UxAttribute.Key, null);
            var binding = GetUXAttrib(elm, UxAttribute.Binding, null);

            if (uxDependency != null && uxName != null)
                ReportError(elm, "Cannot specify both ux:Dependency and ux:Name");

            if (uxProperty != null && uxName != null)
                ReportError(elm, "Cannot specify both ux:Property and ux:Name");

            var elmType =
                uxTemplate != null ? ElementType.Template :
                uxProperty != null ? ElementType.Property :
                uxDependency != null ? ElementType.Dependency :
                ElementType.Object;

            uxName = uxName ?? uxTemplate ?? uxDependency ?? uxProperty;

            if (uxName != null)
            {
                if (IsKeyword(uxName))
                {
                    if (uxName == "this")
                    {
                        ReportError(elm, "'this' is the implicit name of class nodes and can not be specified explicitly");
                    }
                    else
                    {
                        ReportError(elm, "'" + uxName + "' is a Uno language keyword and can not be used as name");
                    }
                }
            }

            var resourceRef = GetUXAttrib(elm, UxAttribute.Resource, null);
            if (resourceRef != null)
            {
                ReportError(elm, "'ux:Resource' is deprecated - use 'ux:Ref' instead (works for both local and global references now)");
            }

            var refId = GetUXAttrib(elm, UxAttribute.Ref, null);
            if (refId != null)
            {
                return new ReferenceNode(ns, typeName, uxName, binding, refId, explicitAutoBindFalse, source);
            }
            var props = elm
                .Attributes()
                .Where(x => x.Name.NamespaceName != Configuration.UXNamespace)
                .Where(x => !x.IsNamespaceDeclaration)
                .Select(x => new Property(x.Name.LocalName, x.Value, source, x.Name.NamespaceName))
                .ToArray();

            var uxPath = GetUXAttrib(elm, UxAttribute.Path, null);
            var clearColor = GetUXAttrib(elm, UxAttribute.ClearColor, null);
            var children = elm.Nodes().Select(Parse).Where(x => x != null).ToArray();
            var condition = GetUXAttrib(elm, UxAttribute.Condition, null);

            if (uxProperty != null)
            {
                if (props.Length > 0)
                {
                    ReportError(elm, "Nodes marked with ux:Property can not specify other properties. To assign a default object to this property, create a separate node of the desired type with ux:Binding=\"" + uxProperty + "\".");
                }
                else if (uxValue != null)
                {
                    ReportError(elm, "Nodes marked with ux:Property can not specify ux:Value. To assign a default value to '" + uxProperty + "', put '" + uxProperty + "=\"" + uxValue + "\"' on the containing class node.");
                }
                else if (children.Length > 0)
                {
                    ReportError(elm, "Nodes marked with ux:Property can not contain child nodes. To assign a default object to this property, create a separate node of the desired type with ux:Binding=\"" + uxProperty + "\", and place the child nodes within.");
                }
                else if (binding != null)
                {
                    ReportError(elm, "Nodes marked with ux:Property can not specify ux:Binding, because they don't represent objects.");
                }
            }

            ReportErrorOnUnvisitedUXAttribs(elm);

            ValidateIdentifier(elm, uxName, false, "ux:Name or ux:Global", false);
            ValidateIdentifier(elm, uxProperty, true, "ux:Property", false);

            return new Element(ns, typeName, condition, uxName, elmType,  uxKey, binding, uxPath, uxValue, clearColor, props, generator, children, explicitAutoBindFalse, source);
        }

        static readonly string identifierChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0987654321_";

        void ValidateIdentifier(XNode elm, string s, bool doThrow, string usage, bool allowPeriod)
        {
            if (s == null) return;

            if (IsKeyword(s))
            {
                ReportError(elm, "'" + s + "' is a Uno language keyword, and can not be used as " + usage);
            }

            for (int i = 0; i < s.Length; i++)
            {
                if (!(identifierChars.Contains(s[i]) || (allowPeriod && s[i] == '.')))
                {
                    if (doThrow)
                    {
                        var msg = "'" + s + "' contains the character '" + s[i] + "' which is not a valid Uno identifier character. ";
                        ReportError(elm, msg + "This name can not be used as " + usage);
                    }
                }
            }
        }

        bool IsKeyword(string s)
        {
            return Tokens.IsReserved(s);
        }

        Generator ParseGenerator(XElement elm, ref bool explicitAutoBindFalse)
        {
            if (Attributes.HasUXAttrib(elm, UxAttribute.Case)) ReportError("ux:Case is deprecated, use ux:Template instead (means the same)");
            if (Attributes.HasUXAttrib(elm, UxAttribute.DefaultCase)) ReportError("ux:DefaultCase is deprecated, use ux:DefaultTemplate instead (means the same)"); 

            var templateAttrib = GetUXAttrib(elm, UxAttribute.Template, null);
            var defaultTemplateAttrib = bool.Parse(GetUXAttrib(elm, UxAttribute.DefaultTemplate, "false"));

            var genModeAttrib = GetUXAttrib(elm, UxAttribute.Generate, null);

            if (templateAttrib != null || defaultTemplateAttrib)
            {
                if (genModeAttrib != null) ReportError(elm, "Cannot specify both ux:Generate and ux:Template / ux:TemplateCase");

                genModeAttrib = "Template";
            }

            bool autoCtor = true;
            var autoCtorString = GetUXAttrib(elm, UxAttribute.AutoCtor, "true");

            if (autoCtorString != null && !bool.TryParse(autoCtorString, out autoCtor))
            {
                ReportError(elm, "Unable to parse 'ux:AutoCtor', must be 'true' or 'false'.");
            }

            bool simulate = true;
            var simulateString = GetUXAttrib(elm, UxAttribute.Simulate, "true");

            if (simulateString != null && !bool.TryParse(simulateString, out simulate))
            {
                ReportError(elm, "Unable to parse 'ux:Simulate', must be 'true' or 'false'.");
            }

            bool isInnerClass = false;
            var className = GetUXAttrib(elm, UxAttribute.ClassName, null);
            
            if (className != null)
            {
                ReportWarning(elm, "'ux:ClassName' is deprecated. Please use 'ux:Class' to specify the class name, and optionally ux:AutoCtor=\"false\" to get equivalent behavior. By default ux:AutoCtor is true, which means you don't need a code behind with a default constructor. This warning will be an error in a future version.");
                
                autoCtor = false; // Compatibility with old scheme
            }
            else
            {
                className = GetUXAttrib(elm, UxAttribute.InnerClass, null);
                if (className != null)
                {
                    if (className.Contains('.'))
                    {
                        ReportError("Inner class names must be a plain name. It can not contain periods '.'");
                    }
                    isInnerClass = true;
                }
            }

            var tn = GetUXAttrib(elm, UxAttribute.Test, null);
            if (tn != null)
                return new TestGenerator(tn);

            var cn = GetUXAttrib(elm, UxAttribute.Class, null);

            if (className != null && cn != null)
            {
                ReportError(elm, "Cannot specify both ux:InnerClass and ux:Class");
            }

            className = className ?? cn;

            if (className != null)
            {
                // Auto-convert dash to underscore to support angular-style tag names
                className = className.Replace('-', '_');

                ValidateIdentifier(elm, className, true, "class name", true);

                if (genModeAttrib != null)
                {
                    ReportError(elm, "'" + className + "' cannot be both class and template at the same time. Inline this template in the style where it's being used.");
                }
            }
            
            if (genModeAttrib != null)
            {
                switch (genModeAttrib)
                {
                    case "Template":
                        return new TemplateGenerator(className, templateAttrib, defaultTemplateAttrib);

                    default:
                        {
                            ReportError(elm, "Unsupported/deprecated generate mode: '" + genModeAttrib + "'");
                            return new InstanceGenerator();
                        }
                }
            }

            if (GetUXAttrib(elm, UxAttribute.StaticResource, null) != null) 
            {
                ReportError(elm, "ux:StaticResource is deprecated. Use ux:Global instead.");
            }

            var globalResourceName = GetUXAttrib(elm, UxAttribute.GlobalResource, null);
            if (globalResourceName != null)
            {
                ReportWarning(elm, "ux:GlobalResource is deprecated. Use the shorter form ux:Global instead.");
            }

            globalResourceName = GetUXAttrib(elm, UxAttribute.Global, null) ?? globalResourceName;

            ValidateIdentifier(elm, globalResourceName, false, "ux:Global", false);

            if (globalResourceName != null)
            {
                if (IsKeyword(globalResourceName))
                {
                    ReportError(elm, "'" + globalResourceName + "' is a Uno language keyword and can not be used as a global name");
                }

                explicitAutoBindFalse = true;
                return new GlobalInstanceGenerator(globalResourceName);
            }

            if (className != null)
            {
                return new ClassGenerator(isInnerClass, className, autoCtor, simulate);
            }

            if (elm == _doc.Root)
            {
                if (elm.Name.LocalName == "App" ||
                    elm.Name.LocalName == "ExportedViews")
                {
                    return new ClassGenerator(isInnerClass, Path.GetFileNameWithoutExtension(_filePath), autoCtor, simulate);
                }
            }

            return new UnspecifiedGenerator(templateAttrib, defaultTemplateAttrib);
        }
    }
}
