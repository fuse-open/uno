using System;
using System.Collections.Generic;
using System.Xml;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.UXL;
using Uno.Compiler.Frontend.Xml;
using Uno.Logging;

namespace Uno.Compiler.Frontend
{
    class UxlParser : XmlParserBase
    {
        public static bool Parse(Log log, SourceBundle bundle, string filename, List<UxlDocument> result)
        {
            try
            {
                return new UxlParser(log, bundle, filename).Parse(result);
            }
            catch (XmlException e)
            {
                log.Error(new Source(bundle, filename, e.LineNumber, e.LinePosition), ErrorCode.E0000, "UXL: " + e.Message);
                return false;
            }
            catch (Exception e)
            {
                log.Error(new Source(bundle, filename), ErrorCode.E3332, "UXL: " + e.Message);
                return false;
            }
        }

        UxlParser(Log log, SourceBundle bundle, string filename)
            : base(log, bundle, filename)
        {
        }

        bool Parse(List<UxlDocument> result)
        {
            switch (FindRootElement().ToLowerCamelCase())
            {
                case "extensions":
                    result.Add(ParseDocument());
                    return Log.ErrorCount - StartErrorCount == 0;

                case "package":
                    ParseAttributes(x => false);
                    ParseElements(name =>
                    {
                        switch (name.ToLowerCamelCase())
                        {
                            case "extensions":
                                result.Add(ParseDocument());
                                return true;
                            default:
                                return false;
                        }
                    });

                    return Log.ErrorCount - StartErrorCount == 0;

                default:
                    Log.Error(GetSource(), ErrorCode.E0000, "Expected <extensions> element");
                    return false;
            }
        }

        UxlDocument ParseDocument()
        {
            SourceValue? backend = null;
            SourceValue? cond = null;

            ParseAttributes(
                name =>
                {
                    switch (name.ToLowerCamelCase())
                    {
                        case "backend":
                            backend = GetValue();
                            return true;
                        case "condition":
                            cond = GetValue();
                            return true;
                        default:
                            return false;
                    }
                });

            UxlBackendType backendType = 0;

            if (backend == null)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected 'backend' attribute on <extensions>");
            else if (!Enum.TryParse(backend.Value.String, true, out backendType) || backendType == UxlBackendType.Unknown)
                Log.Error(backend.Value.Source, ErrorCode.E0000, "Unknown backend " + backend.Value.String.Quote());

            var result = new UxlDocument(File.Bundle, backendType, cond);

            ParseElements(
                name =>
                {
                    switch (name.ToLowerCamelCase())
                    {
                        case "using":
                            ParseUsing(result);
                            return true;
                        case "define":
                            ParseDefine(result);
                            return true;
                        case "declare":
                            ParseDeclare(result);
                            return true;
                        case "deprecate":
                            ParseDeprecate(result);
                            return true;
                        case "definition":
                            Log.Warning(GetSource(), ErrorCode.W0000, "<definition> is deprecated, replace with <declare>");
                            ParseDeclare(result);
                            return true;
                        case "type":
                            ParseType(name, result);
                            return true;
                        case "template":
                            ParseTemplate(name, result.Templates);
                            return true;
                        case "library":
                            Log.Warning(GetSource(), ErrorCode.W0000, "<library> is deprecated, replace with <template>");
                            ParseTemplate(name, result.Templates);
                            return true;
                        case "property":
                            Log.Warning(GetSource(), ErrorCode.W0000, "<property> is deprecated, replace with <set>");
                            ParseSet(name, result.Elements);
                            return true;
                        default:
                            return TryParseEntity(name, result);
                    }
                });

            return result;
        }

        void ParseUsing(UxlDocument doc)
        {
            var foundNamespace = false;

            ParseAttributes(
                name =>
                {
                    switch (name.ToLowerCamelCase())
                    {
                        case "namespace":
                            doc.Usings.Add(GetValue());
                            return foundNamespace = true;
                        default:
                            return false;
                    }
                });

            if (!foundNamespace)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected 'namespace' attribute on <using>");

            ParseEmptyElement();
        }

        void ParseDefine(UxlDocument doc)
        {
            string define = null;
            SourceValue? expression = null;

            ParseAttributes(
                name =>
                {
                    if (define == null && name.ToLowerCamelCase() != "condition")
                    {
                        define = name;
                        expression = GetValue();

                        // VALUE == NAME, when VALUE is omitted from XML
                        if (expression.Value.String == define)
                            expression = new SourceValue(GetSource(), "1");

                        return true;
                    }

                    return false;
                });

            if (define == null || !expression.HasValue)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected a (name)=\"(expression)\" attribute on <define>");
            else
                doc.Defines.Add(new UxlDefine(define, expression.Value));

            ParseEmptyElement();
        }

        void ParseDeclare(UxlDocument doc)
        {
            Source src = null;
            UxlDeclareType type = 0;
            SourceValue? key = null;
            SourceValue? cond = null;
            SourceValue? targetDir = null;

            ParseAttributes(
                name =>
                {
                    switch (name.ToLowerCamelCase())
                    {
                        case "condition":
                            cond = GetValue();
                            return true;
                        case "targetDirectory":
                            targetDir = GetValue();
                            return true;
                        default:
                            UxlDeclareType et;
                            if (Enum.TryParse(name, true, out et) && et != UxlDeclareType.Unknown)
                            {
                                if (type != 0)
                                    Log.Error(GetSource(), ErrorCode.E0000, "A (elementType)=\"(key)\" attribute was already found on <declare>");

                                src = GetSource();
                                type = et;
                                key = GetValue();
                                return true;
                            }

                            return false;
                    }
                });

            if (type == 0)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected a (elementType)=\"(key)\" attribute on <declare>");
            else
                doc.Declarations.Add(new UxlDeclare(src, type, key ?? new SourceValue(), cond));

            if (targetDir != null && key != null)
                doc.Elements.Add(new UxlElement(UxlElementType.Set, new SourceValue(targetDir.Value.Source, key.Value.String + ".targetDirectory"), targetDir.Value, null, false));

            ParseEmptyElement();
        }

        void ParseDeprecate(UxlDocument doc)
        {
            string oldName = null;
            string newName = null;

            ParseAttributes(
                name =>
                {
                    if (oldName == null && newName == null && name.ToLowerCamelCase() != "condition")
                    {
                        oldName = name;
                        newName = GetValue().String;
                        return true;
                    }

                    return false;
                });

            doc.Deprecations.Add(new UxlDeprecate(GetSource(), oldName, newName));
            ParseEmptyElement();
        }

        // Deprecated
        void ParseUnoReference(List<UxlElement> parent)
        {
            var foundEntity = false;

            ParseAttributes(
                name =>
                {
                    switch (name.ToLowerCamelCase())
                    {
                        case "entity":
                            parent.Add(new UxlElement(UxlElementType.Require, new SourceValue(GetSource(), "entity"), GetValue(), null, false));
                            return foundEntity = true;
                        default:
                            return false;
                    }
                });

            if (!foundEntity)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected 'entity' attribute on <unoReference>");

            ParseEmptyElement();
        }

        void ParseCopyFile(string elmName, UxlEntity parent, bool process)
        {
            SourceValue? sourceName= null;
            SourceValue? targetName = null;
            var isExecutable = false;
            var overwrite = true;
            SourceValue? cond = null;
            SourceValue? type = null;

            ParseAttributes(
                name =>
                {
                    switch (name.ToLowerCamelCase())
                    {
                        case "name":
                            sourceName = GetValue();
                            return true;
                        case "targetName":
                            targetName = GetValue();
                            return true;
                        case "isExecutable":
                            isExecutable = GetBool();
                            return true;
                        case "overwrite":
                            overwrite = GetBool();
                            return true;
                        case "condition":
                            cond = GetValue();
                            return true;
                        case "type":
                            type = GetValue();
                            return true;
                        default:
                            if (type != null || sourceName != null)
                                Log.Error(GetSource(), ErrorCode.E0000, "A (type)=\"(name)\" was already found on <" + elmName + ">");

                            type = new SourceValue(GetSource(), name);
                            sourceName = GetValue();
                            return true;
                    }
                });

            if (sourceName == null)
            {
                Log.Error(GetSource(), ErrorCode.E0000, "Expected a 'name' attribute on <" + elmName + ">");
            }
            else
            {
                CopyFileFlags flags = 0;
                if (process)
                    flags |= CopyFileFlags.ProcessFile;
                if (isExecutable)
                    flags |= CopyFileFlags.IsExecutable;
                if (!overwrite)
                    flags |= CopyFileFlags.NoOverwrite;

                parent.CopyFiles.Add(new CopyFile(sourceName.Value, flags, targetName, cond, type));

                if (type != null)
                    parent.Elements.Add(new UxlElement(UxlElementType.Require, type.Value, sourceName.Value, cond, false));
            }

            ParseEmptyElement();
        }

        void ParseCopyDirectory(string elmName, List<CopyFile> parent)
        {
            SourceValue? sourceName = null;
            SourceValue? targetName = null;
            SourceValue? cond = null;

            ParseAttributes(
                name =>
                {
                    switch (name.ToLowerCamelCase())
                    {
                        case "name":
                            sourceName = GetValue();
                            return true;
                        case "targetName":
                            targetName = GetValue();
                            return true;
                        case "condition":
                            cond = GetValue();
                            return true;
                        default:
                            return false;
                    }
                });

            if (sourceName == null)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected 'name' attribute on <" + elmName + ">");
            else
                parent.Add(new CopyFile(sourceName.Value, CopyFileFlags.IsDirectory, targetName, cond));

            ParseEmptyElement();
        }

        void ParseImageFile(string elmName, List<ImageFile> parent)
        {
            SourceValue? sourceName = null;
            SourceValue? targetName = null;
            SourceValue? cond = null;
            int? targetWidth = null;
            int? targetHeight = null;

            ParseAttributes(
                name =>
                {
                    switch (name.ToLowerCamelCase())
                    {
                        case "name":
                            sourceName = GetValue();
                            return true;
                        case "targetName":
                            targetName = GetValue();
                            return true;
                        case "targetWidth":
                            targetWidth = GetInt();
                            return true;
                        case "targetHeight":
                            targetHeight = GetInt();
                            return true;
                        case "condition":
                            cond = GetValue();
                            return true;
                        default:
                            return false;
                    }
                });

            if (sourceName == null)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected 'name' attribute on <" + elmName + ">");
            else
                parent.Add(new ImageFile(sourceName.Value, cond, targetName, targetWidth, targetHeight));

            ParseEmptyElement();
        }

        void ParseRequire(string elmName, List<UxlElement> parent)
        {
            SourceValue? key = null;
            SourceValue? value = null;
            SourceValue? cond = null;

            ParseAttributes(
                name =>
                {
                    switch (name.ToLowerCamelCase())
                    {
                        case "condition":
                            cond = GetValue();
                            return true;
                        case "key":
                            key = GetValue();
                            Log.Warning(GetSource(), ErrorCode.W0000, "'key' attribute is deprecated -- use (key)=\"(value)\" or set inner text");
                            return true;
                        case "type":
                            Log.Warning(GetSource(), ErrorCode.W0000, "'Type' attribute is deprecated -- use (type)=\"(value)\" or set inner text");
                            key = GetValue();
                            return true;
                        case "value":
                            Log.Error(GetSource(), ErrorCode.E0000, "'value' attribute is not allowed -- use (name)=\"(value)\" or set inner text on <" + elmName + ">");
                            return true;
                        default:
                            if (key != null)
                                Log.Error(GetSource(), ErrorCode.E0000, "A (name)=\"(value)\" was attribute already found on <" + elmName + ">");

                            key = new SourceValue(GetSource(), name);
                            value = GetValue();
                            return true;
                    }
                });

            if (value == null || key != null && value.Value == key.Value)
            {
                Source src;
                string text;
                ParseTextElement(true, out src, out text);

                if (text == null && value == null)
                    Log.Error(GetSource(), ErrorCode.E0000, "Expected (name)=\"(value)\" or inner text on <" + elmName + ">");
                else if (text != null)
                    value = new SourceValue(src, text);
            }
            else
                ParseEmptyElement();

            if (key == null)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected (vame)=\"(value)\" or inner text on <" + elmName + ">");
            else
                parent.Add(new UxlElement(UxlElementType.Require, key.Value, value ?? new SourceValue(), cond, false));
        }

        // Deprecated
        void ParseLinkLibrary(string elmName, List<UxlElement> parent)
        {
            SourceValue? libraryName = null;

            ParseAttributes(
                name =>
                {
                    switch (name.ToLowerCamelCase())
                    {
                        case "name":
                            libraryName = GetValue();
                            return true;
                        default:
                            return false;
                    }
                });

            if (libraryName == null)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected 'name' attribute on <" + elmName + ">");
            else
                parent.Add(new UxlElement(UxlElementType.Require, new SourceValue(GetSource(), "template"), libraryName.Value, null, false));

            ParseEmptyElement();
        }

        bool TryParseEntity(string elmName, UxlEntity parent)
        {
            switch (elmName.ToLowerCamelCase())
            {
                case "set":
                    ParseSet(elmName, parent.Elements);
                    return true;
                case "require":
                    ParseRequire(elmName, parent.Elements);
                    return true;
                case "copyFile":
                    ParseCopyFile(elmName, parent, false);
                    return true;
                case "processFile":
                    ParseCopyFile(elmName, parent, true);
                    return true;
                case "copyDirectory":
                    ParseCopyDirectory(elmName, parent.CopyFiles);
                    return true;
                case "imageFile":
                    ParseImageFile(elmName, parent.ImageFiles);
                    return true;
                    // TODO: Remove deprecated stuff below
                case "element":
                    Log.Warning(GetSource(), ErrorCode.W0000, "<element> is deprecated, replace with <require>");
                    ParseRequire(elmName, parent.Elements);
                    return true;
                case "linkLibrary":
                    Log.Warning(GetSource(), ErrorCode.W0000, "<linkLibrary> is deprecated, replace with <require template=\"$(name)\">");
                    ParseLinkLibrary(elmName, parent.Elements);
                    return true;
                case "unoReference":
                    Log.Warning(GetSource(), ErrorCode.W0000, "<unoReference> is deprecated, replace with <require entity=\"$(name)\">");
                    ParseUnoReference(parent.Elements);
                    return true;
                default:
                    return false;
            }
        }

        // Deprecated
        bool TryParseTypeEntity(string elmName, UxlEntity parent)
        {
            switch (elmName.ToLowerCamelCase())
            {
                case "typeElement":
                    Log.Warning(GetSource(), ErrorCode.W0000, "<typeElement> is deprecated, replace with <require>");
                    ParseRequire(elmName, parent.Elements);
                    return true;
                default:
                    return TryParseEntity(elmName, parent);
            }
        }

        void ParseTemplate(string elmName, List<UxlTemplate> parent)
        {
            SourceValue? templateName = null;
            SourceValue? cond = null;
            var def = false;

            ParseAttributes(
                name =>
                {
                    switch (name.ToLowerCamelCase())
                    {
                        case "name":
                            templateName = GetValue();
                            return true;
                        case "condition":
                            cond = GetValue();
                            return true;
                        case "isDefault":
                            def = GetBool();
                            return true;
                        default:
                            return false;
                    }
                });

            var result = new UxlTemplate(templateName ?? new SourceValue(), cond, def);

            if (templateName == null)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected 'name' attribute on <" + elmName + ">");
            else
                parent.Add(result);

            ParseElements(name => TryParseEntity(name, result));
        }

        void ParseSet(string elmName, List<UxlElement> parent)
        {
            SourceValue? key = null;
            SourceValue? value = null;
            SourceValue? cond = null;
            var def = false;

            ParseAttributes(
                name =>
                {
                    switch (name.ToLowerCamelCase())
                    {
                        case "key":
                            key = GetValue();
                            Log.Warning(GetSource(), ErrorCode.W0000, "'key' attribute is deprecated -- use (key)=\"(value)\" or inner text");
                            return true;
                        case "name":
                            key = GetValue();
                            Log.Warning(GetSource(), ErrorCode.W0000, "'name' attribute is deprecated -- use (name)=\"(value)\" or inner text");
                            return true;
                        case "value":
                            Log.Error(GetSource(), ErrorCode.E0000, "'value' attribute is not supported -- use (name)=\"(value)\" or inner text");
                            return true;
                        case "condition":
                            cond = GetValue();
                            return true;
                        case "isDefault":
                            def = GetBool();
                            return true;
                        default:
                            if (key != null)
                                Log.Error(GetSource(), ErrorCode.E0000, "A (name)=\"(value)\" attribute was already found on <" + elmName + ">");

                            key = new SourceValue(GetSource(), name);
                            value = GetValue();
                            return true;
                    }
                });

            if (value == null || key != null && value.Value == key.Value)
            {
                Source src;
                string text;
                ParseTextElement(true, out src, out text);

                if (text == null && value == null)
                    Log.Error(GetSource(), ErrorCode.E0000, "Expected a (name)=\"(value)\" attribute or non-empty inner text on <" + elmName + ">");
                else if (text != null)
                    value = new SourceValue(src, text);
            }
            else
                ParseEmptyElement();

            if (key == null)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected a (name) attribute on <" + elmName + ">");
            else
                parent.Add(new UxlElement(UxlElementType.Set, key.Value, value ?? new SourceValue(), cond, def));
        }

        void ParseImplementation(ImplementationType type, List<UxlImplementation> parent)
        {
            var src = GetSource();
            var def = false;
            SourceValue? cond = null;

            ParseAttributes(
                name =>
                {
                    switch (name.ToLowerCamelCase())
                    {
                        case "condition":
                            cond = GetValue();
                            return true;
                        case "isDefault":
                            def = GetBool();
                            return true;
                        default:
                            return false;
                    }
                });

            if (type == ImplementationType.EmptyBody)
            {
                ParseEmptyElement();
                parent.Add(new UxlImplementation(src, type, new SourceValue(), cond, def));
            }
            else
            {
                Source implSrc;
                string implText;
                ParseTextElement(false, out implSrc, out implText);
                parent.Add(new UxlImplementation(src, type, new SourceValue(implSrc, implText), cond, def));
            }
        }

        void ParseMethod(string elmName, UxlType parent)
        {
            SourceValue? sig = null;
            SourceValue? cond = null;
            var elms = new List<UxlElement>();
            var def = false;

            ParseAttributes(
                name =>
                {
                    switch (name.ToLowerCamelCase())
                    {
                        case "signature":
                            sig = GetValue();
                            return true;
                        case "condition":
                            cond = GetValue();
                            return true;
                        case "isDefault":
                            def = GetBool();
                            return true;
                        default:
                            elms.Add(new UxlElement(UxlElementType.Set, new SourceValue(GetSource(), name), GetValue(), null, false));
                            return true;
                    }
                });

            var result = new UxlMethod(sig ?? new SourceValue(), cond, def, elms);

            ParseElements(
                name =>
                {
                    switch (name.ToLowerCamelCase())
                    {
                        case "body":
                            ParseImplementation(ImplementationType.Body, result.Implementations);
                            return true;
                        case "emptyBody":
                            ParseImplementation(ImplementationType.EmptyBody, result.Implementations);
                            return true;
                        case "expression":
                            ParseImplementation(ImplementationType.Expression, result.Implementations);
                            return true;
                        case "methodProperty":
                            Log.Warning(GetSource(), ErrorCode.W0000, "<methodProperty> is deprecated, replace with <set>");
                            ParseSet(name, result.Elements);
                            return true;
                        default:
                            return TryParseTypeEntity(name, result);
                    }
                });

            if (sig == null)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected 'signature' attribute on <" + elmName + "> element");
            else
                parent.Methods.Add(result);
        }

        void ParseType(string elmName, UxlDocument doc)
        {
            SourceValue? typeName = null;
            SourceValue? cond = null;
            var elms = new List<UxlElement>();
            var def = false;

            ParseAttributes(
                name =>
                {
                    switch (name.ToLowerCamelCase())
                    {
                        case "name":
                            typeName = GetValue();
                            return true;
                        case "condition":
                            cond = GetValue();
                            return true;
                        case "isDefault":
                            def = GetBool();
                            return true;
                        default:
                            elms.Add(new UxlElement(UxlElementType.Set, new SourceValue(GetSource(), name), GetValue(), null, false));
                            return true;
                    }
                });

            var result = new UxlType(typeName ?? new SourceValue(), cond, def, elms);

            ParseElements(
                name =>
                {
                    switch (name.ToLowerCamelCase())
                    {
                        case "method":
                            ParseMethod(name, result);
                            return true;
                        case "typeProperty":
                            Log.Warning(GetSource(), ErrorCode.W0000, "<typeProperty> is deprecated, replace with <Set>");
                            ParseSet(name, result.Elements);
                            return true;
                        default:
                            return TryParseTypeEntity(name, result);
                    }
                });

            if (typeName == null)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected 'name' attribute on <" + elmName + "> element");
            else
                doc.Types.Add(result);
        }
    }
}
