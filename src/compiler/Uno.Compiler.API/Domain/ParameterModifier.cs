namespace Uno.Compiler.API.Domain
{
    public enum ParameterModifier : byte
    {
        Out = 1,
        Ref,
        Const,
        Params,
        This,
    }
}