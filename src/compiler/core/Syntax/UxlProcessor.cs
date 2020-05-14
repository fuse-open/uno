using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using Uno.Collections;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.API.Domain.UXL;
using Uno.Compiler.Core.IL.Utilities;
using Uno.IO;
using Uno.Logging;
using Uno.Macros;

namespace Uno.Compiler.Core.Syntax
{
    public class UxlProcessor : LogObject
    {
        readonly Disk _disk;
        readonly ExtensionRoot _root;
        readonly BuildEnvironment _env;
        readonly UxlBackendType _backendType;
        readonly Namespace _il;
        readonly ILFactory _ilf;
        readonly List<UxlDocument> _documents = new List<UxlDocument>();
        readonly Dictionary<string, string> _deprecated = new Dictionary<string, string>();

        public UxlProcessor(
            Disk disk,
            string backendName,
            Namespace il,
            ExtensionRoot root,
            BuildEnvironment env,
            ILFactory ilf)
            : base(disk)
        {
            _disk = disk;
            _il = il;
            _root = root;
            _env = env;
            _ilf = ilf;
            Enum.TryParse(backendName, true, out _backendType);
        }

        internal void AddRange(IEnumerable<UxlDocument> uxl)
        {
            foreach (var doc in uxl)
                if (doc.Backend == _backendType &&
                        _env.Test(doc.Package.Source, doc.Package.BuildCondition))
                    _documents.Add(doc);
        }

        public void CompileDefines()
        {
            var input = _documents.ToArray();
            var carry = new List<UxlDocument>();
            _documents.Clear();

            while (CompileDefines(input, carry) && carry.Count > 0)
            {
                input = carry.ToArray();
                carry.Clear();
            }

            foreach (var doc in _documents)
                if (Test(doc.Condition))
                    foreach (var e in doc.Defines)
                        if (!_env.IsDefined(e.Name) && Test(e.Condition))
                            _env.Define(e.Name);
        }

        bool CompileDefines(UxlDocument[] input, List<UxlDocument> carry)
        {
            var retval = false;

            foreach (var doc in input)
            {
                if (!Test(doc.Condition))
                {
                    carry.Add(doc);
                    continue;
                }

                _documents.Add(doc);

                foreach (var e in doc.Defines)
                {
                    if (!Test(e.Condition))
                        continue;

                    if (_env.IsDefined(e.Name))
                        Log.Warning(e.Condition.Source, ErrorCode.W0000, e.Name.Quote() + " is already defined");

                    _env.Define(e.Name);
                    retval = true;
                }
            }

            return retval;
        }

        public void CompileDocuments()
        {
            foreach (var doc in _documents)
            {
                foreach (var e in doc.Declarations)
                {
                    if (!Test(e.Condition))
                        continue;

                    var def = GetDeclarationSet(e.Type);

                    if (def == null)
                    {
                        Log.Error(e.Source, ErrorCode.E0000, "Unknown 'Type' attribute on <Declare> element (" + e.Type + ")");
                        continue;
                    }

                    if (def.Contains(e.Key.String))
                        Log.Error(e.Key.Source, ErrorCode.E0000, "A <Declare> element for " + e.Key.String.Quote() + " already exists");
                    else
                        def.Add(e.Key.String);
                }

                foreach (var e in doc.Deprecations)
                {
                    if (_deprecated.ContainsKey(e.OldName))
                        Log.Error(e.Source, ErrorCode.E0000, "A <Deprecate> element for " + e.OldName.Quote() + " already exists");
                    else
                        _deprecated.Add(e.OldName, e.NewName);
                }
            }

            foreach (var doc in _documents)
            {
                var usings = CompileUsings(doc.Usings);

                foreach (var e in doc.Templates)
                    CompileTemplate(e, usings);

                foreach (var e in doc.Types)
                    CompileType(e, doc.Package, usings);

                foreach (var e in doc.Elements)
                {
                    string key;
                    if (!TryGetKey(e, out key) || !Test(e.Condition))
                        continue;

                    if (e.Type == UxlElementType.Set)
                        Apply("Set", key, new Element(e.Value.Source, e.Value.String, e.Disambiguation, usings), _root.Properties);
                    else
                        CompileExtensionElement(_root, "Extensions", key, e, usings);
                }

                CompileFiles(_root, doc);
            }
        }

        public void CompileRequirements()
        {
            CompileRequirements(_root);

            foreach (var typeExt in _root.TypeExtensions)
            {
                CompileRequirements(typeExt.Value);

                foreach (var methodExt in typeExt.Value.MethodExtensions)
                    CompileRequirements(methodExt.Value);
            }

            foreach (var template in _root.Templates)
                CompileRequirements(template.Value);
        }

        void CompileRequirements(ExtensionEntity ext)
        {
            ListDictionary<string, Element> deprecated = null;

            foreach (var e in ext.Requirements)
            {
                string key;
                if (_deprecated.TryGetValue(e.Key, out key))
                {
                    foreach (var req in e.Value)
                        Log.Warning(req.Source, ErrorCode.W0000, e.Key.Quote() + " is deprecated -- please replace with " + key.Quote());

                    if (deprecated == null)
                        deprecated = new ListDictionary<string, Element>();

                    deprecated.Add(key, e.Value);
                }
            }

            if (deprecated != null)
                foreach (var e in deprecated)
                    ext.Requirements.AddRange(e.Key, e.Value);

            List<Element> elms;
            if (ext.Requirements.TryGetValue("Entity", out elms))
                foreach (var e in elms)
                    ext.RequiredEntities.Add(_ilf.GetEntity(e.Source, e.String, e.Usings));

            if (ext.Requirements.TryGetValue("Template", out elms))
            {
                foreach (var e in elms)
                {
                    ExtensionEntity template;
                    if (!_root.Templates.TryGetValue(e.String, out template))
                        Log.Error(e.Source, ErrorCode.E0000, "Template " + e.String.Quote() + " is not defined (on this backend)");
                    else
                        ext.RequiredTemplates.Add(template);
                }
            }
        }

        void CompileTemplate(UxlTemplate uxl, Namescope[] usings)
        {
            if (!Test(uxl.Condition))
                return;

            var template = new ExtensionEntity(uxl.Name.Source, uxl.Name.String, uxl.Disambiguation);
            Apply("Template", uxl.Name.String, template, _root.Templates);

            foreach (var e in uxl.Elements)
            {
                string key;
                if (!TryGetKey(e, out key) || !Test(e.Condition))
                    continue;

                CompileExtensionElement(template, "Template", key, e, usings);
            }

            CompileFiles(template, uxl);
        }

        void CompileType(UxlType uxl, SourcePackage upk, Namescope[] usings)
        {
            if (!Test(uxl.Condition))
                return;

            var dt = _ilf.GetEntity(uxl.Name.Source, uxl.Name.String, usings) as DataType;

            if (dt == DataType.Invalid)
                return;

            if (dt == null)
            {
                Log.Error(uxl.Name.Source, ErrorCode.E0000, uxl.Name.String.Quote() + " is not a type name");
                return;
            }

            if (!dt.IsIntrinsic &&
                !dt.HasAttribute(_ilf.Essentials.TargetSpecificImplementationAttribute) &&
                !dt.HasAttribute(_ilf.Essentials.TargetSpecificTypeAttribute))
                Log.Error(uxl.Name.Source, ErrorCode.E0000, dt.Quote() + " cannot be extended because it does not specify " + _ilf.Essentials.TargetSpecificImplementationAttribute.AttributeString);

            if (upk != dt.Source.Package)
                Log.Error(uxl.Name.Source, ErrorCode.E0000, dt.Quote() + " cannot be extended from outside its package " + dt.Source.Package.Quote());

            if (!Test(uxl.Condition))
                return;

            var typeScopes = GetTypeScopes(dt, usings);
            var typeExt = new TypeExtension(uxl.Name.Source, dt, uxl.Disambiguation);
            Apply("Type", dt, typeExt, _root.TypeExtensions);

            foreach (var e in uxl.Methods)
                CompileMethod(e, dt, typeExt, typeScopes);

            foreach (var e in uxl.Elements)
            {
                string key;
                if (!TryGetKey(e, out key) || !Test(e.Condition))
                    continue;

                if (e.Type == UxlElementType.Set && _root.TypePropertyDefinitions.Contains(key))
                    Apply("Set", key, new Element(e.Value.Source, e.Value.String, e.Disambiguation, typeScopes), typeExt.Properties);
                else
                    CompileTypeElement(typeExt, "Type", key, e, typeScopes);
            }

            CompileFiles(typeExt, uxl);
        }

        void CompileMethod(UxlMethod uxl, DataType dt, TypeExtension typeExt, Namescope[] typeScopes)
        {
            if (!Test(uxl.Condition))
                return;

            var signatureString = uxl.Signature.String;
            var colonIndex = signatureString.MacroIndexOf(':');
            string returnTypeString = null;

            if (colonIndex != -1)
            {
                returnTypeString = signatureString.Substring(colonIndex + 1);
                signatureString = signatureString.Substring(0, colonIndex);
            }

            var entity = _ilf.GetEntity(uxl.Signature.Source, signatureString, typeScopes);

            if (entity is InvalidType)
                return;

            if (!(entity is Function))
            {
                Log.Error(uxl.Signature.Source, ErrorCode.E0000, uxl.Signature.String.Quote() + " is not a method signature");
                return;
            }

            var method = entity as Function;
            var methodScopes = GetMethodScopes(method, typeScopes);

            if (returnTypeString != null)
            {
                var rt = _ilf.GetEntity(uxl.Signature.Source, returnTypeString, methodScopes);

                if (!(rt is InvalidType) && method.ReturnType != rt)
                    Log.Error(uxl.Signature.Source, ErrorCode.E0000, "Inconsistent return type for " + method.Quote() + " (" + method.ReturnType + ")");
            }
            else if (!method.ReturnType.IsVoid)
                Log.Error(uxl.Signature.Source, ErrorCode.E0000, "Requiring return type for " + method.Quote() + " (" + method.ReturnType + ")");

            if (!method.HasAttribute(_ilf.Essentials.TargetSpecificImplementationAttribute) &&
                !method.IsIntrinsic)
                Log.Error(uxl.Signature.Source, ErrorCode.E0000, method.Quote() + " cannot be extended because it does not specify " + _ilf.Essentials.TargetSpecificImplementationAttribute.AttributeString);

            if (dt.MasterDefinition != method.DeclaringType.MasterDefinition)
                Log.Error(uxl.Signature.Source, ErrorCode.E0000, method.Quote() + " cannot be extended from outside its declaring type " + method.DeclaringType.Quote());

            if (!Test(uxl.Condition))
                return;

            var methodExt = new FunctionExtension(uxl.Signature.Source, method, uxl.Disambiguation, methodScopes);
            Apply("Method", method, methodExt, typeExt.MethodExtensions);

            foreach (var impl in uxl.Implementations)
            {
                if (!Test(impl.Condition))
                    continue;

                if (!methodExt.HasImplementation ||
                    methodExt.IsDefaultImplementation && !impl.IsDefault)
                    methodExt.SetImplementation(impl.Body.Source ?? impl.Source, impl.Type, impl.Body.String, impl.IsDefault);
                else if (methodExt.IsDefaultImplementation || !impl.IsDefault)
                    Log.Error(impl.Source, ErrorCode.E0000, "An implementation is already provided for " + method.Quote());
            }

            foreach (var e in uxl.Elements)
            {
                string key;
                if (!TryGetKey(e, out key) || !Test(e.Condition))
                    continue;

                if (e.Type == UxlElementType.Set && _root.MethodPropertyDefinitions.Contains(key))
                    Apply("Set", key, new Element(e.Value.Source, e.Value.String, e.Disambiguation, methodScopes), methodExt.Properties);
                else
                    CompileTypeElement(methodExt, "Method", key, e, methodScopes);
            }

            CompileFiles(typeExt, uxl);
        }

        void CompileFiles(ExtensionEntity ext, UxlEntity uxl)
        {
            foreach (var e in uxl.CopyFiles)
            {
                if (!Test(e.Condition))
                    continue;

                if (e.Type != null)
                {
                    string newType;
                    if (!_deprecated.TryGetValue(e.Type.Value.String, out newType))
                        newType = e.Type.Value.String;

                    ext.CopyFiles.Add(new CopyFile(e.SourceName, e.Flags, e.TargetName, e.Condition, new SourceValue(e.Type.Value.Source, newType)));
                }
                else
                    ext.CopyFiles.Add(e);
            }

            foreach (var e in uxl.ImageFiles)
                if (Test(e.Condition))
                    ext.ImageFiles.Add(e);
        }

        void CompileTypeElement(ExtensionEntity ext, string type, string key, UxlElement elm, Namescope[] usings)
        {
            if (elm.Type == UxlElementType.Require && _root.TypeElementDefinitions.Contains(key))
                ext.Requirements.Add(key, new Element(elm.Value.Source, elm.Value.String, elm.Disambiguation, usings));
            else
                CompileExtensionElement(ext, type, key, elm, usings);
        }

        void CompileExtensionElement(ExtensionEntity ext, string type, string key, UxlElement elm, Namescope[] usings)
        {
            if (elm.Type == UxlElementType.Require && _root.ElementDefinitions.Contains(key))
                ext.Requirements.Add(key, new Element(elm.Value.Source, elm.Value.String, elm.Disambiguation, usings));
            else
                Log.Error(elm.Source, ErrorCode.E0000, "<" + elm.Type + " Key=\"" + key + "\"> is not valid in <" + type + "> (on this backend)");
        }

        bool TryGetKey(UxlElement e, out string key)
        {
            if (string.IsNullOrEmpty(e.Key.String))
            {
                Log.Error(e.Key.Source, ErrorCode.E0000, "<" + e.Type + "> must provide a non-empty 'Key' attribute");
                key = null;
                return false;
            }

            if (_deprecated.TryGetValue(e.Key.String, out key))
                Log.Warning(e.Key.Source, ErrorCode.W0000, e.Key.String.Quote() + " is deprecated -- please replace with " + key.Quote());
            else
                key = e.Key.String;

            return true;
        }

        void Apply<TKey, TValue>(string elmType, TKey key, TValue value, Dictionary<TKey, TValue> map)
            where TValue : IDisambiguable
        {
            TValue old;
            if (!map.TryGetValue(key, out old))
                map.Add(key, value);
            else if (old.Disambiguation != Disambiguation.Override && (
                        value.Source.Package != old.Source.Package ||
                        value.Disambiguation == Disambiguation.Override ||
                        old.Disambiguation == Disambiguation.Default && value.Disambiguation != Disambiguation.Default ||
                        old.Disambiguation != Disambiguation.Condition && value.Disambiguation == Disambiguation.Condition)
                )
                map[key] = value;
            else if (old.Disambiguation == Disambiguation.Condition && value.Disambiguation != Disambiguation.Condition ||
                     old.Disambiguation == Disambiguation.Override && value.Disambiguation != Disambiguation.Override)
                map[key] = old;
            else if (old.Disambiguation == Disambiguation.Default || value.Disambiguation != Disambiguation.Default)
                Log.Error(value.Source, ErrorCode.E0000, "A <" + elmType + "> for " + key.Quote() + " already exists");
        }

        bool Test(SourceValue? cond)
        {
            return !cond.HasValue || _env.Test(cond.Value.Source, cond.Value.String);
        }

        HashSet<string> GetDeclarationSet(UxlDeclareType type)
        {
            switch (type)
            {
                case UxlDeclareType.Element:
                    return _root.ElementDefinitions;
                case UxlDeclareType.TypeElement:
                    return _root.TypeElementDefinitions;
                case UxlDeclareType.TypeProperty:
                    return _root.TypePropertyDefinitions;
                case UxlDeclareType.MethodProperty:
                    return _root.MethodPropertyDefinitions;
                default:
                    return null;
            }
        }

        Namescope[] CompileUsings(List<SourceValue> usings)
        {
            var result = new Namescope[usings.Count];
            int i = 0;

            foreach (var e in usings)
            {
                var ns = _ilf.GetEntity(e.Source, e.String);

                if (ns is Namespace)
                    result[i++] = ns as Namespace;
                else if (!(ns is InvalidType))
                    Log.Error(e.Source, ErrorCode.E0000, e.String.Quote() + " is not a namespace");
            }

            while (i < result.Length)
                result[i++] = _il;

            return result;
        }

        Namescope[] GetTypeScopes(DataType dt, Namescope[] usings)
        {
            var result = new List<Namescope>();

            if (dt != null)
                for (Namescope p = dt; !p.IsRoot; p = p.Parent)
                    result.Add(p);

            foreach (var u in usings)
                if (!result.Contains(u))
                    result.Add(u);

            return result.ToArray();
        }

        Namescope[] GetMethodScopes(Function func, Namescope[] typeScopes)
        {
            var result = typeScopes;
            var method = func as Method;

            if (method != null && method.IsGenericDefinition)
            {
                result = new Namescope[typeScopes.Length + 1];
                result[0] = method.GenericType;
                Array.Copy(typeScopes, 0, result, 1, typeScopes.Length);
            }

            return result;
        }

        internal void FlattenExtensions()
        {
            foreach (var typeItem in _root.TypeExtensions)
            {
                if (!typeItem.Key.IsStripped)
                {
                    foreach (var methodItem in typeItem.Value.MethodExtensions)
                    {
                        if (!methodItem.Key.IsStripped)
                        {
                            foreach (var e in methodItem.Value.Requirements)
                                if (_root.TypeElementDefinitions.Contains(e.Key))
                                    typeItem.Value.Requirements.AddRange(e.Key, e.Value);

                            FlattenExtensionEntity(typeItem.Value, methodItem.Value);
                        }
                    }

                    FlattenExtensionEntity(_root, typeItem.Value);
                }
            }

            // Use ToArray() because FlattenTemplate() might add more.
            foreach (var e in _root.RequiredTemplates.ToArray())
                FlattenTemplate(e);
        }

        void FlattenTemplate(ExtensionEntity template)
        {
            // For templates to support requiring other templates we need some special behaviour.
            // If the template is introducing a new required template, flatten the new one first. Otherwise it would be lost.
            foreach (var e in template.RequiredTemplates)
            {
                if (!_root.RequiredTemplates.Contains(e))
                {
                    _root.RequiredTemplates.Add(e);
                    FlattenTemplate(e);
                }
            }

            FlattenExtensionEntity(_root, template);
        }

        void FlattenExtensionEntity(ExtensionEntity parent, ExtensionEntity child)
        {
            parent.CopyFiles.AddRange(child.CopyFiles);
            parent.ImageFiles.AddRange(child.ImageFiles);

            foreach (var e in child.Requirements)
                if (_root.ElementDefinitions.Contains(e.Key))
                    parent.Requirements.AddRange(e.Key, e.Value);

            foreach (var e in child.RequiredEntities)
                parent.RequiredEntities.Add(e);

            foreach (var e in child.RequiredTemplates)
                parent.RequiredTemplates.Add(e);
        }

        internal void WriteTypedFiles()
        {
            foreach (var f in _root.CopyFiles)
                if (f.Type != null)
                    WriteCopyFile(f);
        }

        internal void WriteUntypedFiles()
        {
            foreach (var f in _root.CopyFiles)
                if (f.Type == null)
                    WriteCopyFile(f);

            foreach (var f in _root.ImageFiles)
                WriteImageFile(f);
        }

        void WriteCopyFile(CopyFile f)
        {
            var src = f.SourceName.String.IsValidPath() &&
                File.Exists(Path.Combine(Path.GetDirectoryName(f.SourceName.Source.FullPath), f.SourceName.String))
                    ? f.SourceName.String
                    : _env.ExpandSingleLine(f.SourceName.Source, f.SourceName.String).NativeToUnix();
            var dst = _env.Combine(GetTargetName(f.SourceName, f.TargetName, f.Type));

            if (!_disk.GetFullPath(
                    f.SourceName.Source,
                    ref src,
                    PathFlags.AllowAbsolutePath | (f.Flags.HasFlag(CopyFileFlags.IsDirectory) ? PathFlags.IsDirectory : 0)) ||
                    f.Flags.HasFlag(CopyFileFlags.NoOverwrite) && File.Exists(dst))
                return;

            if (f.Flags.HasFlag(CopyFileFlags.ProcessFile))
                using (var w = _disk.CreateBufferedText(dst))
                    w.Write(_env.Expand(new Source(f.SourceName.Source.Package, src, 1, 1), f.Preprocess(File.ReadAllText(src).Replace("\r\n", "\n"))));
            else if (f.Flags.HasFlag(CopyFileFlags.IsDirectory))
                _disk.CopyDirectory(src, dst);
            else
                _disk.CopyFile(src, dst);

            if (f.Flags.HasFlag(CopyFileFlags.IsExecutable))
                _disk.MakeExecutable(dst);
        }

        void WriteImageFile(ImageFile f)
        {
            var src = f.SourceName.String.IsValidPath() &&
                File.Exists(Path.Combine(Path.GetDirectoryName(f.SourceName.Source.FullPath), f.SourceName.String))
                    ? f.SourceName.String
                    : _env.ExpandSingleLine(f.SourceName.Source, f.SourceName.String).NativeToUnix();
            var dst = _env.Combine(GetTargetName(f.SourceName, f.TargetName));

            if (!_disk.GetFullPath(
                    f.SourceName.Source,
                    ref src,
                    PathFlags.AllowAbsolutePath))
                return;

            try
            {
                if (!Disk.IsNewer(src, dst))
                    return;

                Log.Event(IOEvent.Write, dst);
                _disk.CreateDirectory(Path.GetDirectoryName(dst));

                using (var originalImg = Image.FromFile(src))
                {
                    if (_env.IsDefined("iOS") && Image.IsAlphaPixelFormat(originalImg.PixelFormat) )
                    {
                        var source = new Source(src);
                        Log.Warning(source, ErrorCode.W0000, "iOS App Store doesn't accept Images with transparency.");
                    }

                    int width = f.TargetWidth ?? f.TargetHeight ?? originalImg.Width;
                    int height = f.TargetHeight ?? f.TargetWidth ?? originalImg.Height;

                    using (var resizedImg = new Bitmap(width, height, originalImg.PixelFormat))
                    {
                        using (var graphics = Graphics.FromImage(resizedImg))
                        {
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            graphics.DrawImage(originalImg, new Rectangle(0, 0, width, height));
                            resizedImg.Save(dst);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(new Source(src), ErrorCode.E0000, e.Message);
                Log.Error(f.SourceName.Source, ErrorCode.E0000, "Failed to convert " + src.Quote());
            }
        }

        string GetTargetName(SourceValue sourceName, SourceValue? targetName, SourceValue? type = null)
        {
            if (targetName != null && targetName.Value != null)
                return _env.ExpandSingleLine(targetName.Value.Source, targetName.Value.String).UnixToNative();

            var src = _env.ExpandSingleLine(sourceName.Source, sourceName.String).UnixToNative();

            SourceValue dir;
            if (type != null && _env.TryGetValue(type.Value.String + ".TargetDirectory", out dir))
                return Path.Combine(dir.String.UnixToNative(), src);

            return src;
        }
    }
}