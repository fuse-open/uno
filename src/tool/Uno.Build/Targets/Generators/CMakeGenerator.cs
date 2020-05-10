using System.Collections.Generic;
using Uno.Collections;
using Uno.Compiler.API;

namespace Uno.Build.Targets.Generators
{
    class CMakeGenerator
    {
        readonly IEnvironment _env;
        readonly ListDictionary<string, string> _groups = new ListDictionary<string, string>();

        public CMakeGenerator(IEnvironment env)
        {
            _env = env;
        }

        public void Configure()
        {
            foreach (var f in _env.GetSet("HeaderFile", true))
                Add("Header Files", "@(HeaderDirectory)", f);
            foreach (var f in _env.GetSet("SourceFile", true))
                Add("Source Files", "@(SourceDirectory)", f);

            var lines = new List<string>();

            foreach (var g in _groups)
                lines.Add("source_group(" + g.Key.QuoteSpace() + " FILES \"" + string.Join("\" \"", g.Value) + "\")");
            
            _env.Set("CMake.SourceGroups", string.Join("\n", lines));
        }

        void Add(string group, string dir, string name)
        {
            _groups.Add(
                (group + "/" + name).UnixDirectoryName().UnixToNative().Replace("\\", "\\\\"),
                dir + "/" + name);
        }
    }
}
