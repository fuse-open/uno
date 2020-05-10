using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Uno.Compiler;
using Uno.Compiler.API;
using Uno.Compiler.Core;
using Uno.Logging;
using Uno.UX.Markup.Common;

namespace Uno.UX.Markup.CompilerReflection
{
    public static class ILCache
    {
        public static Compiler.Core.Compiler Create(Log log, SourcePackage project)
        {
            var backend = new ILCacheBackend();

            var compiler = new Compiler.Core.Compiler(log, backend, project, new CompilerOptions
            {
                Debug = true,
                CodeCompletionMode = true,
                OutputDirectory = Path.Combine(project.SourceDirectory, "Output"),
                BuildTarget = "UXValidator",
                Strip = false
            });

            ParseUxDummyFiles(compiler, project);
            compiler.ParseSourceFiles();
            compiler.InitializeIL();
            compiler.Backend.Begin(compiler);

            try
            {
                compiler.TypeBuilder.BuildTypes();
            }
            catch (Exception)
            {
            }

            return compiler;
        }

        static void EmitResources(string file, Log log, XElement elm, StringBuilder code)
        {
            var globalRes = Attributes.TryGetUXAttrib(elm, UxAttribute.Global) ?? Attributes.TryGetUXAttrib(elm, UxAttribute.GlobalResource);
            var staticRes = globalRes ?? Attributes.TryGetUXAttrib(elm, UxAttribute.StaticResource);

            if (staticRes != null)
            {
                if (globalRes != null)
                {
                    code.AppendLine("[Uno.UX.UXGlobalResource(\"" + globalRes.Value + "\")]");
                }

                if (elm.Name.NamespaceName != Configuration.DefaultNamespace)
                {
                    code.AppendLine("public static readonly " + elm.Name.NamespaceName + "." + elm.Name.LocalName + " " + staticRes.Value.ToIdentifier() + ";");
                }
                else
                {
                    code.AppendLine("public static readonly " + elm.Name.LocalName + " " + staticRes.Value.ToIdentifier() + ";");
                }
            }

            foreach (var c in elm.Elements()) EmitResources(file, log, c, code);
        }

        static void FindNamespaces(Compiler.Core.Compiler comp, SourcePackage package, string path, XElement elm, List<string> result)
        {
            foreach (var ns in elm.Attributes()
                .Where(x => x.IsNamespaceDeclaration && x.Value != Configuration.UXNamespace)
                .SelectMany(x => x.Value.Split(new[] { "," }, StringSplitOptions.None).Select(y => y.Trim())))
                result.Add(ns);

            var foo = result.Where(x => x.Contains(":") && x != Configuration.UXNamespace);
            if (foo.Any())
            {
                comp.Log.FatalError(new Source(package, path, ((IXmlLineInfo)elm).LineNumber), ErrorCode.E0000, "Invalid URL namespace: '" + foo.First() + "'. The only supported URL namespaces are: \n\n\t- " + Configuration.UXNamespace + "\n");
            }

            if (!elm.Attributes().Any(x => x.IsNamespaceDeclaration && x.Name == "xmlns"))
                foreach (var ns in Configuration.DefaultNamespaces)
                    result.Add(ns);
        }

        static void ParseUxDummyFiles(Compiler.Core.Compiler compiler, SourcePackage package)
        {
            foreach (var ux in package.UXFiles)
            {
                var code = new StringBuilder();
                var fullUxPath = Path.Combine(package.SourceDirectory, ux.NativePath);

                XElement elm;
                try
                {
                    elm = fullUxPath.ReadAllXml(LoadOptions.None, true).Root;
                }
                catch (XmlException e)
                {
                    compiler.Log.Error(new Source(package, fullUxPath, e.LineNumber), ErrorCode.E0000, e.Message);
                    continue;
                }
                catch (MarkupException)
                {
                    continue;
                }

                var namespaces = new List<string>();
                FindNamespaces(compiler, package, fullUxPath, elm, namespaces);

                foreach (var ns in namespaces)
                    code.AppendLine("using " + ns + ";");

                code.AppendLine();

                foreach (var ns in namespaces)
                    code.AppendLine("namespace " + ns + " {}");

                code.AppendLine();

                EmitClassPredeclaration(true, fullUxPath, elm, code, compiler);
                compiler.ParseSourceCode(((ICompiler) compiler).Input.Package, fullUxPath, code.ToString());
            }
        }

        static int baseClassCounter;

        static void EmitClassPredeclaration(bool isRoot, string ux, XElement elm, StringBuilder code, Compiler.Core.Compiler compiler)
        {
            foreach (var c in elm.Elements())
            {
                EmitClassPredeclaration(false, ux, c, code, compiler);
            }


            var classNameAttr = Attributes.TryGetUXAttrib(elm, UxAttribute.ClassName);

            classNameAttr = Attributes.TryGetUXAttrib(elm, UxAttribute.Class) ?? classNameAttr;

            if (classNameAttr == null && !isRoot) return;

            var genMode = Attributes.TryGetUXAttrib(elm, UxAttribute.Generate);

            var baseClassName = elm.Name.LocalName;

            var className = System.IO.Path.GetFileNameWithoutExtension(ux);

            if (classNameAttr == null)
            {
                if (elm.Name.LocalName != "App" ||
                    elm.Name.LocalName == "ExportedViews")
                {
                    return;
                }
            }
            else
            {
                className = classNameAttr.Value;
            }

            className = className.Replace('-', '_');

            // Hack to avoid name collisions when class is the same name as something in UX default namespace pool
            var baseClassProxy = "__baseClass" + baseClassCounter++;
            code.AppendLine("public abstract class " + baseClassProxy + " : " + baseClassName + "{}");

            var cns = new TypeNameHelper(className).Namespace;
            var ct = new TypeNameHelper(className).Surname;

            if (!string.IsNullOrEmpty(cns))
            {
                code.AppendLine("namespace " + cns);
                code.AppendLine("{");
            }


            if (genMode != null && genMode.Value == "Template")
            {
                code.AppendLine("    public partial class " + ct + ": global::Uno.UX.Template<" + baseClassName + ">");
                code.AppendLine("{");
            }
            else
            {
                code.AppendLine("    public partial class " + ct + ": " + baseClassProxy);
                code.AppendLine("{");
            }

            EmitResources(ux, compiler.Log, elm, code);

            code.AppendLine("}");

            if (!string.IsNullOrEmpty(cns))
                code.AppendLine("}");
        }
    }

    enum ErrorCode
    {
        E0000,
        I0000,
        E8001,
        W8002
    }
}
