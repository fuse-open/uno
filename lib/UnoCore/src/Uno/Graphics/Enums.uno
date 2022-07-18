using Uno.Compiler.ExportTargetInterop;

namespace Uno.Graphics
{
    public enum BufferUsage
    {
        Immutable,
        Dynamic,
        Stream,
    }

    public enum PrimitiveType
    {
        Triangles = 1,
        Lines,
        Points,
        TriangleStrip,
        LineStrip
    }

    public enum IndexType
    {
        Undefined = 0,
        Byte = 1,
        UShort = 2,
        UInt = 4,
    }

    public enum VertexAttributeType
    {
        Undefined = 0,
        Float,
        Float2,
        Float3,
        Float4,
        Short,
        ShortNormalized,
        Short2,
        Short2Normalized,
        Short4,
        Short4Normalized,
        UShort,
        UShortNormalized,
        UShort2,
        UShort2Normalized,
        UShort4,
        UShort4Normalized,
        SByte4,
        SByte4Normalized,
        Byte4,
        Byte4Normalized,
    }

    public enum CubeFace
    {
        PositiveX,
        NegativeX,
        PositiveY,
        NegativeY,
        PositiveZ,
        NegativeZ,
    }

    public enum BlendOperand
    {
        Zero,
        One,
        SrcAlpha,
        OneMinusSrcAlpha,
        SrcColor,
        OneMinusSrcColor,
        DstAlpha,
        OneMinusDstAlpha,
        DstColor,
        OneMinusDstColor
    }

    public enum BlendEquation
    {
        Add,
        Subtract,
        ReverseSubtract,
        Min,
        Max
    }

    public enum CompareFunc
    {
        Always,
        Less,
        LessOrEqual,
        Equal,
        NotEqual,
        GreaterOrEqual,
        Greater,
        Never
    }

    public enum StencilOp
    {
        Null,
        Keep,
        Replace,
        Increase,
        Decrease,
        Invert
    }

    public enum PolygonFace
    {
        None,
        Front,
        Back,
        Both
    }

    public enum PolygonWinding
    {
        Clockwise,
        CounterClockwise,
    }

    public enum Format
    {
        Unknown = 0,
        L8 = 1,
        LA88 = 2,
        RGBA8888 = 3,
        RGBA4444 = 4,
        RGBA5551 = 5,
        RGB565 = 6
    }
}
