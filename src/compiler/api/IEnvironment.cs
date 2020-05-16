using System;
using System.Collections.Generic;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API
{
    public interface IEnvironment : IFrontendEnvironment
    {
        bool IsGeneratingCode { get; }
        bool Debug { get; }
        bool Strip { get; }
        string OutputDirectory { get; }
        string CacheDirectory { get; }
        string BundleDirectory { get; }

        bool IsUpToDate(SourcePackage upk, string filename);

        void Define(string def);
        void Undefine(string def);
        bool IsDefined(string def);

        void Set(string key, SourceValue value, Disambiguation disamg = 0);
        void Set(string key, string value);

        void Require(string key, SourceValue value);
        void Require(string key, string value);

        string Expand(Source src, string block, bool escape = false, Function context = null, params Namescope[] usings);
        string Expand(Source src, ExpandInterceptor intercept, string block, bool escape = false, Function context = null, params Namescope[] usings);
        string ExpandSingleLine(Source src, string line, bool escape = false, Function context = null, params Namescope[] usings);
        string ExpandSingleLine(string line);

        TypeExtension GetExtension(DataType dt);
        FunctionExtension GetExtension(Function function);
        bool TryGetExtension(DataType dt, out TypeExtension result);
        bool TryGetExtension(Function func, out FunctionExtension result);
        bool HasImplementation(Function function);

        void Require(DataType dt, string key, params Element[] elements);
        void Require(DataType dt, string key, Source src, string value);
        void Require(Function function, string key, params Element[] elements);
        void Require(Function function, string key, Source src, string value);

        bool HasProperty(string key);
        bool TryGetValue(string key, out SourceValue result, bool escape = false);
        string GetString(string key, bool escape = false);
        bool GetBool(string key, bool defaultValue = false);

        string GetOutputPath(string key);
        string Combine(string path0, string path1, string path2);
        string Combine(string path0, string path1);
        string Combine(string path);

        bool HasProperty(DataType dt, string key);
        bool TryGetValue(DataType dt, string key, out SourceValue result, bool escape = false);
        string GetString(DataType dt, string key, bool escape = false);
        bool GetBool(DataType dt, string key, bool defaultValue = false);

        bool HasProperty(Function func, string key);
        bool TryGetValue(Function func, string key, out SourceValue result, bool escape = false);
        string GetString(Function func, string key, bool escape = false);
        bool GetBool(Function func, string key, bool defaultValue = false);

        IEnumerable<SourceValue> Enumerate(string key, bool escape = false);
        IEnumerable<SourceValue> Enumerate(DataType dt, string key, bool escape = false);

        List<string> GetSet(string key, bool escape = false);
        List<string> GetSet(DataType dt, string key, bool escape = false);
        HashSet<string> GetWords(string key);
    }
}
