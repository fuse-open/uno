namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public sealed class AstMember: AstExpression
    {
        public readonly AstExpression Base;
        public readonly AstIdentifier Name;

        public override AstExpressionType ExpressionType => AstExpressionType.Member;

        public AstMember(AstExpression @base, AstIdentifier name)
            : base(name.Source)
        {
            Base = @base;
            Name = name;
        }

        public override string ToString()
        {
            return Base + "." + Name;
        }
        
        public static AstExpression Create(Source src, params string[] qualifier)
        {
            AstExpression @base = new AstSymbol(src, AstSymbolType.Global);
            foreach (var id in qualifier)
                @base = new AstMember(@base,
                    new AstIdentifier(src, id));
            return @base;
        }
    }
}