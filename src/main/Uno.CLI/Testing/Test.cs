using Uno.Configuration;

namespace Uno.CLI.Testing
{
    class Test : DotNetCommand
    {
        public override string Exe => UnoConfig.Current.GetFullPath("Assemblies.Test");
        public override string Name => "test";
        public override string Description => "Run unit test project(s) and print results.";
    }
}
