namespace Uno.Build.Targets.Browser
{
    public class FirefoxOSBuild : WebGLBuild
    {
        public override string Identifier => "FirefoxOS";
        public override string Description => "JavaScript/WebGL code and FFOS app.";
        public override bool IsExperimental => true;
        public override bool IsObsolete => true;

        public override bool CanBuild(BuildFile file)
        {
            return false;
        }

        public override bool CanRun(BuildFile file)
        {
            return false;
        }
    }
}
