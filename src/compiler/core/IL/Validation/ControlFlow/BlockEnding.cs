namespace Uno.Compiler.Core.IL.Validation.ControlFlow
{
    public enum BlockEnding
    {
        Open,

        RetNonVoid,
        RetVoid,

        Br,
        Throw,

        CondBr,
    }
}