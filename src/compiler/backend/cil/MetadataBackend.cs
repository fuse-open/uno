using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Backends.CIL
{
    public class MetadataBackend : Backend
    {
        CilLinker _linker;

        public override string Name => "Meta";

        public MetadataBackend(ShaderBackend shaderBackend)
            : base(shaderBackend)
        {
        }

        public override void Configure()
        {
            _linker = new CilLinker(Log, Essentials, true);
        }

        public override bool CanLink(SourcePackage upk)
        {
            return Environment.IsUpToDate(upk, upk.Name + ".dll");
        }

        public override bool CanLink(DataType dt)
        {
            return dt.Package.CanLink;
        }

        public override bool CanLink(Function function)
        {
            return true;
        }

        public override BackendResult Build(SourcePackage package)
        {
            if (package.CanLink)
            {
                package.Tag = _linker.AddAssemblyFile(Environment.Combine(package.Name + ".dll"));
                return null;
            }

            var g = new MetadataGenerator(Disk, Data, Essentials,
                                          this, _linker, package,
                                          Environment.OutputDirectory);
            g.Configure();
            g.Generate();
            g.Save();
            return null;
        }
    }
}
