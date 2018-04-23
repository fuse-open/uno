using Uno.Compiler.API;
using Uno.Compiler.Extensions.Plugins;

namespace Uno.Compiler.Extensions
{
    public static class PluginLoader
    {
        public static void Load(IPlugins plugins)
        {
            plugins.Add(new BundleFileImporter());
        }
    }
}
