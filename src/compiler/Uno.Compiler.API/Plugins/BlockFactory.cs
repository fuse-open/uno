using Uno.Compiler.API.Domain.AST;

namespace Uno.Compiler.API.Plugins
{
    public abstract class BlockFactory : Plugin
    {
        public abstract AstBlock Create(ApplyContext context);
    }

    public abstract class BlockFactory<T> : BlockFactory
    {
        public abstract AstBlock Create(ApplyContext context, T arg);

        public override AstBlock Create(ApplyContext context)
        {
            return Create(context, (T)context.Arguments[0]);
        }
    }
}