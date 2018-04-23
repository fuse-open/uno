using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Plugins;
using Uno.IO;
using Uno.Logging;

namespace Uno.Compiler.Core
{
    public class PluginCache : Dictionary<string, Plugin>, IPlugins
    {
        readonly Log _log;
        readonly IBundle _bundle;
        readonly IILFactory _ilf;
        public IEnvironment Environment { get; }

        public PluginCache(
            Log log,
            IBundle bundle,
            IILFactory ilf,
            IEnvironment env)
        {
            _log = log;
            _bundle = bundle;
            _ilf = ilf;
            Environment = env;
        }

        public void Add(Plugin plugin)
        {
            plugin.Initialize(_log, _bundle, _ilf);
            Add(plugin.Class, plugin);
        }

        public bool TryGetBlockFactory(DataType dt, out BlockFactory result)
        {
            Plugin plugin;
            if (!TryGetValue(dt.ToString(), out plugin))
            {
                result = null;
                return false;
            }

            result = (BlockFactory) plugin;
            return true;
        }

        public bool TryGetImporter(DataType dt, out Importer result)
        {
            Plugin plugin;
            if (!TryGetValue(dt.ToString(), out plugin))
            {
                result = null;
                return false;
            }

            result = (Importer) plugin;
            return true;
        }

        public void Load(string filename)
        {
            GetLoader(filename)?.Invoke(this);
        }

        static Action<IPlugins> GetLoader(string filename)
        {
            lock (Register)
            {
                Action<IPlugins> result;
                var full = filename.ToFullPath();
                if (Register.TryGetValue(full, out result))
                    return result;

                if (!File.Exists(filename))
                    throw new FileNotFoundException("Uno plugin not found: " + filename);

                var asm = Assembly.LoadFrom(filename);
                foreach (var type in asm.GetTypes())
                {
                    var method = type.GetMethod(
                        "Load",
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                        null,
                        new[] {typeof(IPlugins)},
                        null);

                    if (method != null)
                        return Register[full] =
                            (Action<IPlugins>) method.CreateDelegate(typeof(Action<IPlugins>));
                }

                return Register[full] = null;
            }
        }

        static readonly Dictionary<string, Action<IPlugins>> Register = new Dictionary<string, Action<IPlugins>>();
    }
}