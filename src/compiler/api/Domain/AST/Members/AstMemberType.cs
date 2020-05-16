namespace Uno.Compiler.API.Domain.AST.Members
{
    public enum AstMemberType
    {
        Field,
        Constructor,
        Finalizer,
        Method,
        Property,
        Class,
        Indexer,
        Operator,
        Converter,
        Enum,
        Delegate,
        Event,

        // draw statements
        MetaMethod,
        MetaProperty,
        NodeBlock,
        Block,
        ApplyStatement,
    }
}
