using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Plugins
{
    public abstract class Importer : Plugin
    {
        public abstract Expression Import(ImportContext context);
    }

    public abstract class Importer<T> : Importer
    {
        public abstract Expression Import(ImportContext context, T arg);

        public override Expression Import(ImportContext context)
        {
            return Import(context, (T)context.Arguments[0]);
        }
    }
}
