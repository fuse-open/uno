using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Members
{
    public sealed class AstReqStatement : SourceObject
    {
        public readonly AstExpression Type;
        public readonly AstIdentifier Name;
        public readonly uint Offset;
        public readonly string Tag;

        public AstReqStatement(Source src, uint offset, AstIdentifier name, AstExpression type, string tag)
            : base(src)
        {
            Name = name;
            Type = type;
            Offset = offset;
            Tag = tag;
        }
    }
}
