namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public enum ExpressionType
    {
        Invalid,
        NoOp,
        Constant,
        AllocObject,
        Default,
        AddressOf,
        TypeOf,
        This,
        Base,
        BranchOp,
        ConditionalOp,
        ReferenceOp,
        SequenceOp,
        NullOp,
        CastOp,
        FixOp,
        IsOp,
        AsOp,
        LoadLocal,
        LoadField,
        LoadElement,
        LoadArgument,
        StoreLocal,
        StoreArgument,
        StoreField,
        StoreElement,
        StoreThis,
        Swizzle,
        GetProperty,
        SetProperty,
        AddListener,
        RemoveListener,
        NewObject,
        NewDelegate,
        NewArray,
        CallCast,
        CallConstructor,
        CallMethod,
        CallDelegate,
        CallBinOp,
        CallUnOp,
        UncompiledLambda,
        Lambda,

        // Name resolving
        ExtensionGroup,
        MethodGroup,

        // Meta properties
        CapturedLocal,
        CapturedArgument,
        GetMetaProperty,
        GetMetaObject,
        PlaceholderValue,
        PlaceholderReference,
        PlaceholderArray,
        PlaceholderArgument,
        StageOp,

        // Shaders
        CallShader,
        LoadUniform,
        LoadPixelSampler,
        LoadVertexAttrib,
        LoadVarying,
        NewVertexAttrib,
        NewPixelSampler,
        RuntimeConst,

        // Native
        ExternOp,
        ExternString,
        LoadPtr,
        ZeroMemory,

        // Other
        Other
    }
}