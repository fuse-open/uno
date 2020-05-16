using System;
using System.Collections.Generic;
using System.IO;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.Extensions;

namespace Uno.Build.Targets.Generators
{
    class AndroidGenerator
    {
        readonly IEnvironment _env;
        readonly ExtensionRoot _extensions;

        public AndroidGenerator(IEnvironment env, ExtensionRoot extensions)
        {
            _env = env;
            _extensions = extensions;
        }

        public void Configure()
        {
            // List of string literals given to @(Activity).java for dlload()-ing
            var jniLibs = GetLibraries("LoadLibrary");
            jniLibs.AddRange(GetLibraries("SharedLibrary"));

            for (int i = 0; i < jniLibs.Count; i++)
                jniLibs[i] = GetModuleName(jniLibs[i]).ToLiteral();

            _env.Set("LoadLibraryStrings", string.Join(", ", jniLibs));
            _env.Set("LinkOrderedStaticLibraries", string.Join("\n", GetLibraries("StaticLibrary", true)));
        }

        List<string> GetLibraries(string type, bool reverse = false)
        {
            var result = new List<string>();
            var foundLibs = new HashSet<string>();
            var foundTemplates = new HashSet<ExtensionEntity>();

            foreach (var e in _extensions.Requirements.GetList(type))
                AddLibrary(type, e, foundLibs, foundTemplates, result);

            if (reverse)
                result.Reverse();

            return result;
        }

        void AddLibrary(string type, Element lib, HashSet<string> foundLibs, HashSet<ExtensionEntity> foundTemplates, List<string> result)
        {
            var name = _env.ExpandSingleLine(lib.Source, lib.String);

            if (foundLibs.Contains(name))
                return;

            foundLibs.Add(name);

            foreach (var t in _extensions.Templates)
                foreach (var e in t.Value.Requirements.GetList(type))
                    if (_env.ExpandSingleLine(e.Source, e.String) == name)
                        AddDependencies(type, t.Value, foundLibs, foundTemplates, result);

            result.Add(name);
        }

        void AddDependencies(string type, ExtensionEntity template, HashSet<string> foundLibs, HashSet<ExtensionEntity> foundTemplates, List<string> result)
        {
            if (foundTemplates.Contains(template))
                return;

            foundTemplates.Add(template);

            foreach (var t in template.RequiredTemplates)
                AddDependencies(type, t, foundLibs, foundTemplates, result);

            foreach (var e in template.Requirements.GetList(type))
                AddLibrary(type, e, foundLibs, foundTemplates, result);
        }

        static string GetModuleName(string f)
        {
            f = Path.GetFileNameWithoutExtension(f);
            return f.StartsWith("lib", StringComparison.InvariantCulture) ? f.Substring(3) : f;
        }
    }
}
