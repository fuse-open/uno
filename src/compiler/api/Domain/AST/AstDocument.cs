using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST
{
    public sealed class AstDocument : AstNamespace
    {
        public AstDocument(Source src)
            : base(new AstIdentifier(src, "-"))
        {
        }
    }
}