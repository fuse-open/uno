using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API
{
    public interface IEssentials
    {
		DataType Void { get; }
        DataType Bool { get; }
        DataType Char { get; }
        DataType Byte { get; }
        DataType SByte { get; }
        DataType Short { get; }
        DataType UShort { get; }
        DataType Int { get; }
        DataType Int2 { get; }
        DataType Int3 { get; }
        DataType Int4 { get; }
        DataType UInt { get; }
        DataType Long { get; }
        DataType ULong { get; }
        DataType Double { get; }
        DataType Float { get; }
        DataType Float2 { get; }
        DataType Float2x2 { get; }
        DataType Float3 { get; }
        DataType Float3x3 { get; }
        DataType Float4 { get; }
        DataType Float4x4 { get; }

        DataType Type { get; }
        DataType Object { get; }
        DataType String { get; }
        DataType Attribute { get; }
        DataType Delegate { get; }
        DataType Exception { get; }
        DataType Sampler2D { get; }
        DataType SamplerCube { get; }
        DataType Texture2D { get; }
        DataType TextureCube { get; }
        DataType VideoSampler { get; }
        DataType VideoTexture { get; }

        DataType DotNetTypeAttribute { get; }
        DataType DotNetOverrideAttribute { get; }
        DataType TargetSpecificTypeAttribute { get; }
        DataType TargetSpecificImplementationAttribute { get; }
        DataType WeakReferenceAttribute { get; }
        DataType NativeClassAttribute { get; }
        DataType ForeignAttribute { get; }
        DataType ForeignTypeNameAttribute { get; }
        DataType ForeignIncludeAttribute { get; }
		DataType ForeignAnnotationAttribute { get; }
        DataType Language { get; }
    }
}
