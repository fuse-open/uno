using Uno.Configuration;

namespace Uno.CLI.Testing
{
    class TestGen : DotNetCommand
    {
        public override string Exe => UnoConfig.Current.GetFullPath("Assemblies.TestGen");
        public override string Name => "test-gen";
        public override string Description => "Generate compilation tests.";
        public override bool IsExperimental => true;
    }
}