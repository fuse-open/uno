namespace Uno.Compiler.API.Domain.Serialization
{
    public enum StringFlags : byte
    {
        Null,
        Empty,
        Index,
        String,
        SpaceSeparated,
        SlashSeparated,
        LeftSeparated,
        RightSeparated,
        DashSeparated,
        DotSeparated,
        CommaSeparated,
        ColonSeparated,
        BacktickSeparated,
        SingleQuoteSeparated,
        DoubleQuoteSeparated,
        AssignSeparated,
        Max,
    }
}