namespace Uno.Compiler.API.Domain.Bytecode
{
    public enum Opcodes
    {
        Nop,

        MarkSource,
        MarkLabel,

        Constrained,

        BeginExceptionBlock,
        EndExceptionBlock,
        BeginCatchBlock,
        BeginFinallyBlock,

        Null,

        /// <summary>
        /// Used in shader bytecode
        /// </summary>
        GetShaderConst,
        LoadShaderUniform,
        LoadShaderPixelSampler,
        LoadShaderVertexAttrib,
        LoadShaderVarying,

        NewObject,
        NewArray,
        NewDelegate,

        Box,
        Unbox,
        UnboxAny,

        Constant,
        DefaultInit,
        TypeOf,

        This,

        Call,
        CallDelegate,
        CallVirtual,

        Pop,
        Dup,

        LoadObj,
        StoreObj,

        LoadFunction,
        LoadFunctionVirtual,

        LoadArg,
        LoadArgAddress,
        StoreArg,

        LoadLocal,
        LoadLocalAddress,
        StoreLocal,

        LoadField,
        LoadFieldAddress,
        StoreField,

        LoadStaticfield,
        LoadStaticFieldAddress,
        StoreStaticField,

        LoadArrayElement,
        LoadArrayElementAddress,
        StoreArrayElement,

        LoadArrayLength,

        ConvByte,
        ConvSByte,
        ConvUShort,
        ConvShort,
        ConvUInt,
        ConvInt,
        ConvULong,
        ConvLong,
        ConvFloat,
        ConvDouble,
        ConvChar,

        AsClass,
        CastClass,

        Eq,
        Neq,
        Lt,
        Lt_Unsigned,
        Lte,
        Lte_Unsigned,
        Gt,
        Gt_Unsigned,
        Gte,
        Gte_Unsigned,

        Add,
        Sub,
        Mul,
        Div,
        Div_Un,
        Rem,
        Rem_Un,
        And,
        Or,
        Xor,
        Shl,
        Shr,
        Shr_Un,
        BitwiseNot,
        LogNot,
        Neg,

        Leave,

        Br,
        BrEq,
        BrNeq,
        BrTrue,
        BrFalse,
        BrLt,
        BrLte,
        BrGt,
        BrGte,
        BrLt_Unsigned,
        BrLte_Unsigned,
        BrGt_Unsigned,
        BrGte_Unsigned,
        BrNull,
        BrNotNull,

        Ret,

        Throw,
    }
}
