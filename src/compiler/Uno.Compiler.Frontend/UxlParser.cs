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
        public static bool Parse(Log log, SourcePackage upk, string filename, List<UxlDocument> result)
        {
            try
            {
                return new UxlParser(log, upk, filename).Parse(result);
            }
            catch (XmlException e)
            {
                log.Error(new Source(upk, filename, e.LineNumber, e.LinePosition), ErrorCode.E0000, "UXL: " + e.Message);
                return false;
            }
            catch (Exception e)
            {
                log.Error(new Source(upk, filename), ErrorCode.E3332, "UXL: " + e.Message);
                return false;
            }
        }

        UxlParser(Log log, SourcePackage upk, string filename)
            : base(log, upk, filename)
        {
        }

        bool Parse(List<UxlDocument> result)
        {
            switch (FindRootElement())
            {
                case "Extensions":
                    result.Add(ParseDocument());
                    return Log.ErrorCount - StartErrorCount == 0;

                case "Package":
                    ParseAttributes(x => false);
                    ParseElements(name =>
                    {
                        switch (name)
                        {
                            case "Extensions":
                                result.Add(ParseDocument());
                                return true;
                            default:
                                return false;
                        }
                    });

                    return Log.ErrorCount - StartErrorCount == 0;

                default:
                    Log.Error(GetSource(), ErrorCode.E0000, "Expected <Extensions> element");
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
                    switch (name)
                    {
                        case "Backend":
                            backend = GetValue();
                            return true;
                        case "Condition":
                            cond = GetValue();
                            return true;
                        default:
                            return false;
                    }
                });

            UxlBackendType backendType = 0;

            if (backend == null)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected 'Backend' attribute on <Extensions>");
            else if (!Enum.TryParse(backend.Value.String, out backendType) || backendType == UxlBackendType.Unknown)
                Log.Error(backend.Value.Source, ErrorCode.E0000, "Unknown backend " + backend.Value.String.Quote());

            var result = new UxlDocument(File.Package, backendType, cond);

            ParseElements(
                name =>
                {
                    switch (name)
                    {
                        case "Using":
                            ParseUsing(result);
                            return true;
                        case "Define":
                            ParseDefine(result);
                            return true;
                        case "Declare":
                            ParseDeclare(result);
                            return true;
                        case "Deprecate":
                            ParseDeprecate(result);
                            return true;
                        case "Definition":
                            Log.Warning(GetSource(), ErrorCode.W0000, "<Definition> is deprecated, replace with <Declare>");
                            ParseDeclare(result);
                            return true;
                        case "Type":
                            ParseType(name, result);
                            return true;
                        case "Template":
                            ParseTemplate(name, result.Templates);
                            return true;
                        case "Library":
                            Log.Warning(GetSource(), ErrorCode.W0000, "<Library> is deprecated, replace with <Template>");
                            ParseTemplate(name, result.Templates);
                            return true;
                        case "Property":
                            Log.Warning(GetSource(), ErrorCode.W0000, "<Property> is deprecated, replace with <Set>");
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
                    switch (name)
                    {
                        case "Namespace":
                            doc.Usings.Add(GetValue());
                            return foundNamespace = true;
                        default:
                            return false;
                    }
                });

            if (!foundNamespace)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected 'Namespace' attribute on <Using>");

            ParseEmptyElement();
        }

        void ParseDefine(UxlDocument doc)
        {
            string define = null;
            SourceValue? expression = null;

            ParseAttributes(
                name =>
                {
                    if (define == null && name != "Condition")
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
                Log.Error(GetSource(), ErrorCode.E0000, "Expected a (Name)=\"(Expression)\" attribute on <Define>");
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
                    switch (name)
                    {
                        case "Condition":
                            cond = GetValue();
                            return true;
                        case "TargetDirectory":
                            targetDir = GetValue();
                            return true;
                        default:
                            UxlDeclareType et;
                            if (Enum.TryParse(name, out et) && et != UxlDeclareType.Unknown)
                            {
                                if (type != 0)
                                    Log.Error(GetSource(), ErrorCode.E0000, "A (ElementType)=\"(Key)\" attribute was already found on <Declare>");

                                src = GetSource();
                                type = et;
                                key = GetValue();
                                return true;
                            }

                            return false;
                    }
                });

            if (type == 0)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected a (ElementType)=\"(Key)\" attribute on <Declare>");
            else
                doc.Declarations.Add(new UxlDeclare(src, type, key ?? new SourceValue(), cond));

            if (targetDir != null && key != null)
                doc.Elements.Add(new UxlElement(UxlElementType.Set, new SourceValue(targetDir.Value.Source, key.Value.String + ".TargetDirectory"), targetDir.Value, null, false));

            ParseEmptyElement();
        }

        void ParseDeprecate(UxlDocument doc)
        {
            string oldName = null;
            string newName = null;

            ParseAttributes(
                name =>
                {
                    if (oldName == null && newName == null && name != "Condition")
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

        // TODO: Deprecated
        void ParseUnoReference(List<UxlElement> parent)
        {
            var foundEntity = false;

            ParseAttributes(
                name =>
                {
                    switch (name)
                    {
                        case "Entity":
                            parent.Add(new UxlElement(UxlElementType.Require, new SourceValue(GetSource(), "Entity"), GetValue(), null, false));
                            return foundEntity = true;
                        default:
                            return false;
                    }
                });

            if (!foundEntity)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected 'Entity' attribute on <UnoReference>");

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
                    switch (name)
                    {
                        case "Name":
                            sourceName = GetValue();
                            return true;
                        case "TargetName":
                            targetName = GetValue();
                            return true;
                        case "IsExecutable":
                            isExecutable = GetBool();
                            return true;
                        case "Overwrite":
                            overwrite = GetBool();
                            return true;
                        case "Condition":
                            cond = GetValue();
                            return true;
                        case "Type":
                            type = GetValue();
                            return true;
                        default:
                            if (type != null || sourceName != null)
                                Log.Error(GetSource(), ErrorCode.E0000, "A (Type)=\"(Name)\" was already found on <" + elmName + ">");

                            type = new SourceValue(GetSource(), name);
                            sourceName = GetValue();
                            return true;
                    }
                });

            if (sourceName == null)
            {
                Log.Error(GetSource(), ErrorCode.E0000, "Expected a 'Name' attribute on <" + elmName + ">");
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
                    switch (name)
                    {
                        case "Name":
                            sourceName = GetValue();
                            return true;
                        case "TargetName":
                            targetName = GetValue();
                            return true;
                        case "Condition":
                            cond = GetValue();
                            return true;
                        default:
                            return false;
                    }
                });

            if (sourceName == null)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected 'Name' attribute on <" + elmName + ">");
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
                    switch (name)
                    {
                        case "Name":
                            sourceName = GetValue();
                            return true;
                        case "TargetName":
                            targetName = GetValue();
                            return true;
                        case "TargetWidth":
                            targetWidth = GetInt();
                            return true;
                        case "TargetHeight":
                            targetHeight = GetInt();
                            return true;
                        case "Condition":
                            cond = GetValue();
                            return true;
                        default:
                            return false;
                    }
                });

            if (sourceName == null)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected 'Name' attribute on <" + elmName + ">");
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
                    switch (name)
                    {
                        case "Condition":
                            cond = GetValue();
                            return true;
                        case "Key":
                            key = GetValue();
                            Log.Warning(GetSource(), ErrorCode.W0000, "'Key' attribute is deprecated -- use (Key)=\"(Value)\" or set inner text");
                            return true;
                        case "Type":
                            Log.Warning(GetSource(), ErrorCode.W0000, "'Type' attribute is deprecated -- use (Type)=\"(Value)\" or set inner text");
                            key = GetValue();
                            return true;
                        case "Value":
                            Log.Error(GetSource(), ErrorCode.E0000, "'Value' attribute is not allowed -- use (Name)=\"(Value)\" or set inner text on <" + elmName + ">");
                            return true;
                        default:
                            if (key != null)
                                Log.Error(GetSource(), ErrorCode.E0000, "A (Name)=\"(Value)\" was attribute already found on <" + elmName + ">");

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
                    Log.Error(GetSource(), ErrorCode.E0000, "Expected (Name)=\"(Value)\" or inner text on <" + elmName + ">");
                else if (text != null)
                    value = new SourceValue(src, text);
            }
            else
                ParseEmptyElement();

            if (key == null)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected (Name)=\"(Value)\" or inner text on <" + elmName + ">");
            else
                parent.Add(new UxlElement(UxlElementType.Require, key.Value, value ?? new SourceValue(), cond, false));
        }

        // TODO: Deprecated method
        void ParseLinkLibrary(string elmName, List<UxlElement> parent)
        {
            SourceValue? libraryName = null;

            ParseAttributes(
                name =>
                {
                    switch (name)
                    {
                        case "Name":
                            libraryName = GetValue();
                            return true;
                        default:
                            return false;
                    }
                });

            if (libraryName == null)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected 'Name' attribute on <" + elmName + ">");
            else
                parent.Add(new UxlElement(UxlElementType.Require, new SourceValue(GetSource(), "Template"), libraryName.Value, null, false));

            ParseEmptyElement();
        }

        bool TryParseEntity(string elmName, UxlEntity parent)
        {
            switch (elmName)
            {
                case "Set":
                    ParseSet(elmName, parent.Elements);
                    return true;
                case "Require":
                    ParseRequire(elmName, parent.Elements);
                    return true;
                case "CopyFile":
                    ParseCopyFile(elmName, parent, false);
                    return true;
                case "ProcessFile":
                    ParseCopyFile(elmName, parent, true);
                    return true;
                case "CopyDirectory":
                    ParseCopyDirectory(elmName, parent.CopyFiles);
                    return true;
                case "ImageFile":
                    ParseImageFile(elmName, parent.ImageFiles);
                    return true;
                    // TODO: Remove deprecated stuff below
                case "Element":
                    Log.Warning(GetSource(), ErrorCode.W0000, "<Element> is deprecated, replace with <Require>");
                    ParseRequire(elmName, parent.Elements);
                    return true;
                case "LinkLibrary":
                    Log.Warning(GetSource(), ErrorCode.W0000, "<LinkLibrary> is deprecated, replace with <Require Template=\"$(Name)\">");
                    ParseLinkLibrary(elmName, parent.Elements);
                    return true;
                case "UnoReference":
                    Log.Warning(GetSource(), ErrorCode.W0000, "<UnoReference> is deprecated, replace with <Require Entity=\"$(Entity)\">");
                    ParseUnoReference(parent.Elements);
                    return true;
                default:
                    return false;
            }
        }

        // TODO: Deprecated method
        bool TryParseTypeEntity(string elmName, UxlEntity parent)
        {
            switch (elmName)
            {
                case "TypeElement":
                    Log.Warning(GetSource(), ErrorCode.W0000, "<TypeElement> is deprecated, replace with <Require>");
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
                    switch (name)
                    {
                        case "Name":
                            templateName = GetValue();
                            return true;
                        case "Condition":
                            cond = GetValue();
                            return true;
                        case "IsDefault":
                            def = GetBool();
                            return true;
                        default:
                            return false;
                    }
                });

            var result = new UxlTemplate(templateName ?? new SourceValue(), cond, def);

            if (templateName == null)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected 'Name' attribute on <" + elmName + ">");
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
                    switch (name)
                    {
                        case "Key":
                            key = GetValue();
                            Log.Warning(GetSource(), ErrorCode.W0000, "'Key' attribute is deprecated -- use (Key)=\"(Value)\" or inner text");
                            return true;
                        case "Name":
                            key = GetValue();
                            Log.Warning(GetSource(), ErrorCode.W0000, "'Name' attribute is deprecated -- use (Name)=\"(Value)\" or inner text");
                            return true;
                        case "Value":
                            Log.Error(GetSource(), ErrorCode.E0000, "'Value' attribute is not supported -- use (Name)=\"(Value)\" or inner text");
                            return true;
                        case "Condition":
                            cond = GetValue();
                            return true;
                        case "IsDefault":
                            def = GetBool();
                            return true;
                        default:
                            if (key != null)
                                Log.Error(GetSource(), ErrorCode.E0000, "A (Name)=\"(Value)\" attribute was already found on <" + elmName + ">");

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
                    Log.Error(GetSource(), ErrorCode.E0000, "Expected a (Name)=\"(Value)\" attribute or non-empty inner text on <" + elmName + ">");
                else if (text != null)
                    value = new SourceValue(src, text);
            }
            else
                ParseEmptyElement();

            if (key == null)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected a (Name) attribute on <" + elmName + ">");
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
                    switch (name)
                    {
                        case "Condition":
                            cond = GetValue();
                            return true;
                        case "IsDefault":
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
                    switch (name)
                    {
                        case "Signature":
                            sig = GetValue();
                            return true;
                        case "Condition":
                            cond = GetValue();
                            return true;
                        case "IsDefault":
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
                    switch (name)
                    {
                        case "Body":
                            ParseImplementation(ImplementationType.Body, result.Implementations);
                            return true;
                        case "EmptyBody":
                            ParseImplementation(ImplementationType.EmptyBody, result.Implementations);
                            return true;
                        case "Expression":
                            ParseImplementation(ImplementationType.Expression, result.Implementations);
                            return true;
                        case "MethodProperty":
                            Log.Warning(GetSource(), ErrorCode.W0000, "<MethodProperty> is deprecated, replace with <Set>");
                            ParseSet(name, result.Elements);
                            return true;
                        default:
                            return TryParseTypeEntity(name, result);
                    }
                });

            if (sig == null)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected 'Signature' attribute on <" + elmName + "> element");
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
                    switch (name)
                    {
                        case "Name":
                            typeName = GetValue();
                            return true;
                        case "Condition":
                            cond = GetValue();
                            return true;
                        case "IsDefault":
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
                    switch (name)
                    {
                        case "Method":
                            ParseMethod(name, result);
                            return true;
                        case "TypeProperty":
                            Log.Warning(GetSource(), ErrorCode.W0000, "<TypeProperty> is deprecated, replace with <Set>");
                            ParseSet(name, result.Elements);
                            return true;
                        default:
                            return TryParseTypeEntity(name, result);
                    }
                });

            if (typeName == null)
                Log.Error(GetSource(), ErrorCode.E0000, "Expected 'Name' attribute on <" + elmName + "> element");
            else
                doc.Types.Add(result);
        }
    }
}
