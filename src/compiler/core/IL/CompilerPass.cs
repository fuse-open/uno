using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.Core.IL.Utilities;
using Uno.Compiler.Core.Syntax.Binding;
using Uno.Compiler.Core.Syntax.Builders;
using Uno.IO;

namespace Uno.Compiler.Core.IL
{
    public class CompilerPass : Pass
    {
        protected readonly Backend Backend;
        protected readonly SourcePackage Package;
        protected readonly TypeBuilder TypeBuilder;
        protected new readonly BuildData Data;
        protected new readonly BuildEnvironment Environment;
        protected new readonly Essentials Essentials;
        protected new readonly ILFactory ILFactory;
        protected readonly NameResolver NameResolver;

        internal CompilerPass(
            Disk disk,
            BuildData data,
            BuildEnvironment environment,
            ILFactory ilf,
            Backend backend,
            SourcePackage package,
            TypeBuilder typeBuilder,
            NameResolver resolver)
            : base(disk, data, environment, ilf)
        {
            Backend = backend;
            Package = package;
            TypeBuilder = typeBuilder;
            Data = data;
            Environment = environment;
            Essentials = ilf.Essentials;
            ILFactory = ilf;
            NameResolver = resolver;
        }

        public CompilerPass(CompilerPass parent)
            : base(parent)
        {
            Backend = parent.Backend;
            Package = parent.Package;
            TypeBuilder = parent.TypeBuilder;
            Data = parent.Data;
            Environment = parent.Environment;
            Essentials = parent.Essentials;
            ILFactory = parent.ILFactory;
            NameResolver = parent.NameResolver;
        }
    }
}
