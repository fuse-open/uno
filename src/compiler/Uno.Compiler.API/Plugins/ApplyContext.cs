namespace Uno.Compiler.API.Plugins
{
    public struct ApplyContext
    {
        public readonly Source Source;
        public readonly object[] Arguments;
        public readonly SourcePackage Package;

        public ApplyContext(Source src, object[] arguments)
        {
            Source = src;
            Arguments = arguments;
            Package = src.Package;
        }
    }
}
