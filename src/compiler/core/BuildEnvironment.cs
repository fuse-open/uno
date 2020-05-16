using System;
using System.Collections.Generic;
using System.IO;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.Core.IL.Utilities;
using Uno.Compiler.Core.Syntax.Macros;
using Uno.Compiler.Frontend.Preprocessor;
using Uno.Logging;

namespace Uno.Compiler.Core
{
    public class BuildEnvironment : LogObject, IEnvironment
    {
        readonly Essentials Essentials;
        readonly ExtensionRoot Extensions;
        readonly MacroExpander MacroExpander;
        readonly Compiler Compiler;
        internal readonly CompilerOptions Options;
        internal BuildStep Step;

        public bool Debug => Options.Debug;
        public bool Parallel => Options.Parallel;
        public bool Strip => Options.Strip;
        public bool IsGeneratingCode => Step == BuildStep.Generating;
        public bool CanCacheIL => Options.CanCacheIL;

        public string CacheDirectory { get; }
        public string BundleDirectory { get; }
        public string OutputDirectory { get; }

        readonly Dictionary<string, bool> _cachedTests = new Dictionary<string, bool>();
        readonly Dictionary<Key, Result> _cachedProperties = new Dictionary<Key, Result>();

        internal BuildEnvironment(
            Backend backend,
            SourcePackage project,
            CompilerOptions options,
            ExtensionRoot extensions,
            ILFactory ilf,
            Compiler compiler)
            : base(compiler)
        {
            if (string.IsNullOrEmpty(options.Configuration))
                options.Configuration = "Debug";
            if (string.IsNullOrEmpty(options.BuildTarget))
                throw new ArgumentNullException(nameof(options.BuildTarget));
            if (string.IsNullOrEmpty(options.OutputDirectory))
                throw new ArgumentNullException(nameof(options.OutputDirectory));

            Extensions = extensions;
            Options = options;
            BundleDirectory = Path.Combine(project.CacheDirectory, "bundle");
            CacheDirectory = Path.Combine(project.CacheDirectory, "builds", (options.BuildTarget + options.Configuration).GetHashCode().ToString("x8"));
            Essentials = ilf.Essentials;
            MacroExpander = new MacroExpander(backend, this, extensions, ilf, compiler);
            Compiler = compiler;

            Set("Target", options.BuildTarget);
            Set("Configuration", options.Configuration);
            Set("OutputDirectory", OutputDirectory = ExpandSingleLine(options.OutputDirectory).UnixToNative());
        }

        public bool IsUpToDate(SourcePackage upk, string filename)
        {
            if (!upk.IsCached)
                return false;

            var file = Path.Combine(OutputDirectory, filename);
            var lib = Path.Combine(upk.CacheDirectory, "lib." + Compiler.Input.AstCache.MagicString);
            return File.Exists(file) && File.Exists(lib) && 
                   File.GetLastWriteTime(file) > File.GetLastWriteTime(lib);
        }

        public void Set(string key, SourceValue value, Disambiguation disamg = 0)
        {
            Extensions.Properties[key] = new Element(value.Source, value.String, disamg);
        }

        public void Set(string key, string value)
        {
            Extensions.Properties[key] = new Element(Source.Unknown, value);
        }

        public void Require(string key, SourceValue value)
        {
            Extensions.Requirements.Add(key, new Element(value.Source, value.String));
        }

        public void Require(string key, string value)
        {
            Extensions.Requirements.Add(key, new Element(Source.Unknown, value));
        }

        public void Define(params string[] defines)
        {
            foreach (var def in defines)
                Define(def);
        }

        public void Define(string def)
        {
            if (!string.IsNullOrEmpty(def) &&
                (def[0] == '_' || char.IsLetter(def[0])) &&
                Extensions.Defines.Add(def.ToUpperInvariant()))
                _cachedTests.Clear();
        }

        public void Undefine(string def)
        {
            if (Extensions.Defines.Remove(def.ToUpperInvariant()))
                _cachedTests.Clear();
        }

        public bool IsDefined(string def)
        {
            return Extensions.Defines.Contains(def.ToUpperInvariant());
        }

        public bool Test(Source src, string optionalCond)
        {
            if (string.IsNullOrEmpty(optionalCond))
                return true;

            bool result;
            if (_cachedTests.TryGetValue(optionalCond, out result))
                return result;

            string id;
            while (optionalCond.FindIdentifier(out id) != -1)
                optionalCond = optionalCond.ReplaceWord(id, IsDefined(id) ? "1" : "0");

            if (Preprocessor.TryParseCondition(optionalCond, out result))
                return _cachedTests[optionalCond] = result;

            Log.Error(src, ErrorCode.E0000, "Invalid condition " + optionalCond.Quote());
            return false;
        }

        public string Expand(Source src, string text, bool escape = false, Function context = null, params Namescope[] usings)
        {
            return MacroExpander.ExpandMacrosAndDirectives(src, text, escape, context, usings);
        }

        public string Expand(Source src, ExpandInterceptor intercept, string text, bool escape, Function context, params Namescope[] usings)
        {
            return MacroExpander.ExpandMacrosAndDirectives(src, text, escape, context, usings, intercept);
        }

        public string ExpandSingleLine(Source src, string text, bool escape = false, Function context = null, params Namescope[] usings)
        {
            return MacroExpander.ExpandMacros(src, text, escape, context, usings);
        }

        public string ExpandSingleLine(string text)
        {
            return MacroExpander.ExpandMacros(Source.Unknown, text, false, null, new Namescope[0]);
        }

        public bool TryGetExtension(DataType dt, out TypeExtension result)
        {
            return Extensions.TypeExtensions.TryGetValue(dt.Prototype, out result);
        }

        public bool TryGetExtension(Function functions, out FunctionExtension result)
        {
            result = null;
            TypeExtension typeExt;
            return TryGetExtension(functions.DeclaringType, out typeExt) && typeExt.MethodExtensions.TryGetValue(functions.Prototype.MasterDefinition, out result);
        }

        public TypeExtension GetExtension(DataType dt)
        {
            TypeExtension result;
            if (!Extensions.TypeExtensions.TryGetValue(dt.Prototype, out result))
            {
                result = new TypeExtension(dt.Source, dt);
                Extensions.TypeExtensions[dt] = result;
            }

            return result;
        }

        public FunctionExtension GetExtension(Function function)
        {
            var typeExt = GetExtension(function.DeclaringType);

            FunctionExtension result;
            if (!typeExt.MethodExtensions.TryGetValue(function.Prototype.MasterDefinition, out result))
            {
                result = new FunctionExtension(function.Source, function, 0, Compiler.NameResolver.GetUsings(function.DeclaringType, function.Source));
                typeExt.MethodExtensions[function] = result;
            }

            return result;
        }

        public Element CreateElement(Source src, string value, Namescope scope)
        {
            return new Element(src, value, 0, Compiler.NameResolver.GetUsings(scope, src));
        }

        public void Require(DataType dt, string key, params Element[] elements)
        {
            GetExtension(dt).Requirements.AddRange(key, elements);
        }

        public void Require(DataType dt, string key, Source src, string value)
        {
            GetExtension(dt).Requirements.Add(key, CreateElement(src, value, dt));
        }

        public void ProcessFile(DataType dt, string type, Source src, string name, string targetName = null)
        {
            GetExtension(dt).CopyFiles.Add(new CopyFile(new SourceValue(src, name), CopyFileFlags.ProcessFile, targetName != null ? new SourceValue(src, targetName) : null, null, new SourceValue(src, type)));
        }

        public void Set(DataType dt, string key, Source src, string value)
        {
            GetExtension(dt).Properties[key] = CreateElement(src, value, dt);
        }

        public bool HasImplementation(Function function)
        {
            FunctionExtension methodExt;
            return TryGetExtension(function, out methodExt) && methodExt.HasImplementation;
        }

        public void Require(Function function, string key, params Element[] elements)
        {
            GetExtension(function).Requirements.AddRange(key, elements);
        }

        public void Require(Function function, string key, Source src, string value)
        {
            GetExtension(function).Requirements.Add(key, CreateElement(src, value, function.DeclaringType));
        }

        public void ProcessFile(Function function, string type, Source src, string name, string targetName = null)
        {
            GetExtension(function).CopyFiles.Add(new CopyFile(new SourceValue(src, name), CopyFileFlags.ProcessFile, targetName != null ? new SourceValue(src, targetName) : null, null, new SourceValue(src, type)));
        }

        public void Set(Function function, string key, Source src, string value)
        {
            GetExtension(function).Properties[key] = CreateElement(src, value, function.DeclaringType);
        }

        public bool HasProperty(string propertyName)
        {
            return Extensions.Properties.ContainsKey(propertyName);
        }

        public bool TryGetValue(string propertyName, out SourceValue result, bool escape = false)
        {
            Result value;
            Key key = new Key(propertyName, escape);
            if (_cachedProperties.TryGetValue(key, out value))
            {
                result = value.Value;
                return value.HasValue;
            }

            Element elm;
            if (Extensions.Properties.TryGetValue(propertyName, out elm))
            {
                result = new SourceValue(elm.Source, ExpandSingleLine(elm.Source, elm.String, escape, null, elm.Usings));
                _cachedProperties[key] = new Result(result, true);
                return true;
            }

            result = default(SourceValue);
            _cachedProperties[key] = new Result(result, false);
            return false;
        }

        internal void ClearPropertyCache()
        {
            _cachedProperties.Clear();
        }

        public string GetString(string propertyName, bool escape = false)
        {
            SourceValue result;
            return TryGetValue(propertyName, out result, escape) ? result.String : null;
        }

        public bool GetBool(string propertyName, bool defaultValue = false)
        {
            SourceValue result;
            return TryGetValue(propertyName, out result) ? CompileBool(result) : defaultValue;
        }

        public string GetOutputPath(string key)
        {
            return Path.Combine(OutputDirectory, GetString(key).UnixToNative());
        }

        public string Combine(string path0, string path1, string path2)
        {
            return Path.Combine(OutputDirectory, path0, path1, path2);
        }

        public string Combine(string path0, string path1)
        {
            return Path.Combine(OutputDirectory, path0, path1);
        }

        public string Combine(string path)
        {
            return Path.Combine(OutputDirectory, path);
        }

        public bool HasProperty(DataType dt, string propertyName)
        {
            TypeExtension typeExt;
            return TryGetExtension(dt, out typeExt) && typeExt.Properties.ContainsKey(propertyName);
        }

        public bool TryGetValue(DataType dt, string propertyName, out SourceValue result, bool escape = false)
        {
            Element elm;
            TypeExtension typeExt;
            if (Extensions.TypeExtensions.TryGetValue(dt.Prototype, out typeExt) &&
                typeExt.Properties.TryGetValue(propertyName, out elm))
            {
                result = new SourceValue(elm.Source, ExpandSingleLine(elm.Source, elm.String, escape, null, elm.Usings));
                return true;
            }

            result = default(SourceValue);
            return false;
        }

        public string GetString(DataType dt, string propertyName, bool escape = false)
        {
            SourceValue result;
            return TryGetValue(dt, propertyName, out result, escape) ? result.String : null;
        }

        public bool GetBool(DataType dt, string propertyName, bool defaultValue = false)
        {
            SourceValue result;
            return TryGetValue(dt, propertyName, out result) ? CompileBool(result) : defaultValue;
        }

        public bool HasProperty(Function f, string propertyName)
        {
            FunctionExtension methodExt;
            return TryGetExtension(f, out methodExt) && methodExt.Properties.ContainsKey(propertyName);
        }

        public bool TryGetValue(Function f, string propertyName, out SourceValue result, bool escape = false)
        {
            Element elm;
            TypeExtension typeExt;
            FunctionExtension methodExt;
            if (Extensions.TypeExtensions.TryGetValue(f.DeclaringType.Prototype, out typeExt) &&
                typeExt.MethodExtensions.TryGetValue(f.Prototype, out methodExt) &&
                methodExt.Properties.TryGetValue(propertyName, out elm))
            {
                result = new SourceValue(elm.Source, ExpandSingleLine(elm.Source, elm.String, escape, f, elm.Usings));
                return true;
            }

            result = default(SourceValue);
            return false;
        }

        public string GetString(Function f, string propertyName, bool escape = false)
        {
            SourceValue result;
            return TryGetValue(f, propertyName, out result, escape) ? result.String : null;
        }

        public bool GetBool(Function f, string propertyName, bool defaultValue = false)
        {
            SourceValue result;
            return TryGetValue(f, propertyName, out result) ? CompileBool(result) : defaultValue;
        }

        public IEnumerable<SourceValue> Enumerate(string elementType, bool escape = false)
        {
            List<Element> elms;
            if (Extensions.Requirements.TryGetValue(elementType, out elms))
                foreach (var e in elms)
                    yield return new SourceValue(e.Source, Expand(e.Source, e.String, escape, null, e.Usings));
        }

        public IEnumerable<SourceValue> Enumerate(DataType dt, string elementType, bool escape = false)
        {
            List<Element> elms;
            TypeExtension typeExt;
            if (TryGetExtension(dt, out typeExt) &&
                typeExt.Requirements.TryGetValue(elementType, out elms))
                foreach (var e in elms)
                    yield return new SourceValue(e.Source, Expand(e.Source, e.String, escape, null, e.Usings));
        }

        public List<string> GetSet(string elementType, bool escape = false)
        {
            return GetSet(Enumerate(elementType, escape));
        }

        public List<string> GetSet(DataType dt, string elementType, bool escape = false)
        {
            return GetSet(Enumerate(dt, elementType, escape));
        }

        List<string> GetSet(IEnumerable<SourceValue> values)
        {
            var set = new HashSet<string>();
            var result = new List<string>(set);

            foreach (var e in values)
            {
                if (!set.Contains(e.String))
                {
                    result.Add(e.String);
                    set.Add(e.String);
                }
            }

            return result;
        }

        public HashSet<string> GetWords(string key)
        {
            var result = new HashSet<string>();

            foreach (var word in GetString(key).Split(' '))
                result.Add(word.Trim());

            return result;
        }

        bool CompileBool(SourceValue sourceValue)
        {
            bool result;
            if (!string.IsNullOrEmpty(sourceValue.String) &&
                Preprocessor.TryParseCondition(sourceValue.String, out result))
                return result;

            Log.Error(sourceValue.Source, ErrorCode.E0000, sourceValue.String.Quote() + " could not be evaluated to 'bool'");
            return false;
        }

        public bool CanExport(IEntity entity)
        {
            if (entity is Member)
                return CanExport(entity as Member);
            if (entity is DataType)
                return CanExport(entity as DataType);

            return false;
        }

        public bool CanExport(Member m)
        {
            return !m.Prototype.MasterDefinition.HasAttribute(Essentials.DontExportAttribute) &&
                CanExport(m.DeclaringType);
        }

        public bool CanExport(DataType dt)
        {
            for (var st = dt.Prototype; st != null; st = st.ParentType)
            {
                if (st.IsGenericParameterization)
                    foreach (var pt in st.GenericArguments)
                        if (!CanExport(pt))
                            return false;

                if (st.MasterDefinition.HasAttribute(Essentials.DontExportAttribute))
                    return false;
            }

            return true;
        }

        int _verboseMessageCount;
        const int MaxVerboseMessages = 20;
        public bool SkipVerboseErrors => !Options.CodeCompletionMode && _verboseMessageCount++ > MaxVerboseMessages;

        struct Key
        {
            public readonly string String;
            public readonly bool Bool;

            public Key(string str, bool boo)
            {
                String = str;
                Bool = boo;
            }

            public override int GetHashCode()
            {
                return String.GetHashCode() << 1 |
                       (Bool ? 1 : 0);
            }
        }

        struct Result
        {
            public readonly SourceValue Value;
            public readonly bool HasValue;

            public Result(SourceValue value, bool hasValue)
            {
                Value = value;
                HasValue = hasValue;
            }
        }
    }
}
