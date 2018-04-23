namespace Uno.Compiler.Core.Syntax.Binding
{
    public enum PartialExpressionType
    {
        Namespace,
        Type,
        Block,
        Value,
        This,
        Variable,
        Parameter,
        Field,
        Event,
        Property,
        Indexer,
        ArrayElement,
        MethodGroup,
        ExtensionGroup,
    }
}