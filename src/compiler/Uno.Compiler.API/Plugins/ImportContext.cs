using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Plugins
{
    public struct ImportContext
    {
        public readonly Source Source;
        public readonly DataType[] TypeArguments;
        public readonly object[] Arguments;
        public readonly SourcePackage Package;

        public ImportContext(Source src, DataType[] types, object[] arguments)
        {
            Source = src;
            TypeArguments = types;
            Arguments = arguments;
            Package = src.Package;
        }
    }
}