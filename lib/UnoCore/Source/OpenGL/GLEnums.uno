using Uno.Compiler.ExportTargetInterop;
using Uno;

namespace OpenGL
{
    public extern(OPENGL) enum GLIntegerName
    {
        MaxTextureSize = 0x0D33,
    }

    public extern(OPENGL) enum GLInteger4Name
    {
        ScissorBox = 0x0C10,
        Viewport = 0x0BA2,
    }

    public extern(OPENGL) enum GLShaderType
    {
        FragmentShader = 0x8B30,
        VertexShader = 0x8B31,
    }

    public extern(OPENGL) enum GLShaderPrecision
    {
        LowFloat = 0x8DF0,
        MediumFloat = 0x8DF1,
        HighFloat = 0x8DF2,
        LowInt = 0x8DF3,
        MediumInt = 0x8DF4,
        HighInt = 0x8DF5,
    }

    public extern(OPENGL) enum GLBufferTarget
    {
        ArrayBuffer = 0x8892,
        ElementArrayBuffer = 0x8893,
    }

    public extern(OPENGL) enum GLBufferUsage
    {
        StreamDraw = 0x88E0,
        StaticDraw = 0x88E4,
        DynamicDraw = 0x88E8,
    }

    public extern(OPENGL) enum GLTextureUnit
    {
        Texture0 = 0x84C0,
        Texture1 = 0x84C1,
        Texture2 = 0x84C2,
        Texture3 = 0x84C3,
        Texture4 = 0x84C4,
        Texture5 = 0x84C5,
        Texture6 = 0x84C6,
        Texture7 = 0x84C7,
    }

    public extern(OPENGL) enum GLTextureTarget
    {
        Texture2D = 0x0DE1,
        TextureCubeMap = 0x8513,
        TextureCubeMapPositiveX = 0x8515,
        TextureCubeMapNegativeX = 0x8516,
        TextureCubeMapPositiveY = 0x8517,
        TextureCubeMapNegativeY = 0x8518,
        TextureCubeMapPositiveZ = 0x8519,
        TextureCubeMapNegativeZ = 0x851A,
        TextureExternalOES = 0x8D65,
    }

    public extern(OPENGL) enum GLTextureParameterName
    {
        WrapS = 0x2802,
        WrapT = 0x2803,
        MagFilter = 0x2800,
        MinFilter = 0x2801,
    }

    public extern(OPENGL) enum GLTextureParameterValue
    {
        Repeat = 0x2901,
        ClampToEdge = 0x812F,

        Nearest = 0x2600,
        Linear = 0x2601,

        NearestMipmapNearest = 0x2700,
        LinearMipmapNearest = 0x2701,
        NearestMipmapLinear = 0x2702,
        LinearMipmapLinear = 0x2703,
    }

    public extern(OPENGL) enum GLEnableCap
    {
        Blend = 0x0BE2,
        DepthTest = 0x0B71,
        CullFace = 0x0B44,
        ScissorTest = 0x0C11,
    }

    public extern(OPENGL) enum GLDepthFunction
    {
        Never = 0x0200,
        Less = 0x0201,
        Equal = 0x0202,
        Lequal = 0x0203,
        Greater = 0x0204,
        Notequal = 0x0205,
        Gequal = 0x0206,
        Always = 0x0207,
    }

    public extern(OPENGL) enum GLCullFaceMode
    {
        None = 0,
        Front = 0x0404,
        Back = 0x0405,
        FrontAndBack = 0x0408,
    }

    public extern(OPENGL) enum GLFrontFaceDirection
    {
        Cw = 0x0900,
        Ccw = 0x0901,
    }

    public extern(OPENGL) enum GLPrimitiveType
    {
        Points = 0x0000,
        Lines = 0x0001,
        LineLoop = 0x0002,
        LineStrip = 0x0003,
        Triangles = 0x0004,
        TriangleStrip = 0x0005,
        TriangleFan = 0x0006,
    }

    public extern(OPENGL) enum GLDataType
    {
        Byte = 0x1400,
        UnsignedByte = 0x1401,
        Short = 0x1402,
        UnsignedShort = 0x1403,
        Int = 0x1404,
        UnsignedInt = 0x1405,
        Float = 0x1406,
    }

    public extern(OPENGL) enum GLIndexType
    {
        UnsignedByte = 0x1401,
        UnsignedShort = 0x1403,
    }

    public extern(OPENGL) enum GLPixelFormat
    {
        DepthComponent = 0x1902,
        Alpha = 0x1906,
        Bgr = 0x80E0,
        Bgra = 0x80E1,
        Red = 0x1903,
        Rg = 0x8227,
        Rgb = 0x1907,
        Rgba = 0x1908,
        Luminance = 0x1909,
        LuminanceAlpha = 0x190A,
    }

    public extern(OPENGL) enum GLPixelType
    {
        Byte = 0x1400,
        UnsignedByte = 0x1401,
        Short = 0x1402,
        UnsignedShort = 0x1403,
        Int = 0x1404,
        UnsignedInt = 0x1405,
        Float = 0x1406,
        UnsignedShort4444 = 0x8033,
        UnsignedShort5551 = 0x8034,
        UnsignedShort565 = 0x8363,
    }

    public extern(OPENGL) enum GLPixelStoreParameter
    {
        UnpackAlignment = 3317,
        PackAlignment = 3333,
    }

    public extern(OPENGL) enum GLRenderbufferStorage
    {
        DepthComponent16 = 0x81A5,
    }

    public extern(OPENGL) enum GLBlendEquation
    {
        FuncAdd = 0x8006,
        FuncSubtract = 0x800A,
        FuncReverseSubtract = 0x800B,
    }

    public extern(OPENGL) enum GLBlendingFactor
    {
        Zero = 0,
        One = 1,
        SrcColor = 0x0300,
        OneMinusSrcColor = 0x0301,
        SrcAlpha = 0x0302,
        OneMinusSrcAlpha = 0x0303,
        DstAlpha = 0x0304,
        OneMinusDstAlpha = 0x0305,
        DstColor = 0x0306,
        OneMinusDstColor = 0x0307,
        SrcAlphaSaturate = 0x0308,
    }

    public extern(OPENGL) enum GLFramebufferAttachment
    {
        ColorAttachment0 = 0x8CE0,
        DepthAttachment = 0x8D00,
        StencilAttachment = 0x8D20,
    }

    public extern(OPENGL) enum GLFramebufferTarget
    {
        Framebuffer = 0x8D40,
    }

    public extern(OPENGL) enum GLRenderbufferTarget
    {
        Renderbuffer = 0x8D41,
    }

    public extern(OPENGL) enum GLError
    {
        NoError = 0,
        InvalidEnum = 0x0500,
        InvalidValue = 0x0501,
        InvalidOperation = 0x0502,
        OutOfMemory = 0x0505,
        InvalidFramebufferOperation = 0x0506,
    }

    public extern(OPENGL) enum GLFramebufferStatus
    {
        FramebufferComplete = 0x8CD5,
        FramebufferIncompleteAttachment = 0x8CD6,
        FramebufferIncompleteMissingAttachment = 0x8CD7,
        FramebufferIncompleteDimensions = 0x8CD9,
        FramebufferUnsupported = 0x8CDD,
    }

    public extern(OPENGL) enum GLClearBufferMask
    {
        DepthBufferBit = 0x00000100,
        StencilBufferBit = 0x00000400,
        ColorBufferBit = 0x00004000,
    }

    public extern(OPENGL) enum GLShaderParameter
    {
        ShaderType = 35663,
        DeleteStatus = 35712,
        CompileStatus = 35713,
    }

    public extern(OPENGL) enum GLProgramParameter
    {
        DeleteStatus = 35712,
        LinkStatus = 35714,
        AttachedShaders = 35717,
        ActiveUniforms = 35718,
        ActiveAttributes = 35721,
    }

    public extern(OPENGL) enum GLStringName
    {
        Vendor = 7936,
        Renderer = 7937,
        Version = 7938,
        Extensions = 7939,
        ShadingLanguageVersion = 35724,
    }
}
