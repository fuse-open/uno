namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public class AstInvalid : AstIdentifier
    {
        public AstInvalid(Source src)
            : base(src, "<invalid>")
        {
        }
    }
}