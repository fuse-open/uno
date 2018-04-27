using Uno.Compiler.API;
using Uno.Compiler.API.Backends;

namespace Uno.Build.Targets.Uno
{
    public class DocsBuild : BuildTarget
    {
        public override string Identifier => "docs";
        public override string FormerName => "UnoDoc";
        public override string Description => "Uno documentation files.";
        public override bool IsExperimental => true;
        public override bool DefaultStrip => false;

        public override Backend CreateBackend()
        {
            return BackendFactory.NewUnoDocBackend();
        }

        public override void Initialize(IEnvironment env)
        {
            base.Initialize(env);
            /*
            env.Define("ANDROID");
            env.Define("CPLUSPLUS");
            env.Define("DOTNET");
            env.Define("IOS");
            env.Define("JAVASCRIPT");
            env.Define("MAC");
            env.Define("WIN32");
            */
        }
    }
}
