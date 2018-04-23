using Uno.Compiler.API.Plugins;

namespace Uno.Compiler.API
{
    public interface IPlugins
    {
        IEnvironment Environment { get; }

        void Add(Plugin plugin);
    }
}