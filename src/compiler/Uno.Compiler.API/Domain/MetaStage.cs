namespace Uno.Compiler.API.Domain
{
    public enum MetaStage : byte
    {
        Undefined,
        Const,
        ReadOnly, // Same name as operator
        Volatile, // Same name as operator
        Vertex,
        Pixel,
        Max
    }
}