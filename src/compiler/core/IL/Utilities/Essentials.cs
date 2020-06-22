using Uno.Compiler.API;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Core.IL.Utilities
{
    public class Essentials : IEssentials
    {
        public DataType[] BuiltinTypes = new DataType[(int) BuiltinType.Max];

        public DataType Void { get; private set; }
        public DataType Bool { get; private set; }
        public DataType Char { get; private set; }
        public DataType Byte { get; private set; }
        public DataType SByte { get; private set; }
        public DataType Short { get; private set; }
        public DataType UShort { get; private set; }
        public DataType Int { get; private set; }
        public DataType Int2 { get; private set; }
        public DataType Int3 { get; private set; }
        public DataType Int4 { get; private set; }
        public DataType UInt { get; private set; }
        public DataType Long { get; private set; }
        public DataType ULong { get; private set; }
        public DataType Double { get; private set; }
        public DataType Float { get; private set; }
        public DataType Float2 { get; private set; }
        public DataType Float2x2 { get; private set; }
        public DataType Float3 { get; private set; }
        public DataType Float3x3 { get; private set; }
        public DataType Float4 { get; private set; }
        public DataType Float4x4 { get; private set; }

        public DataType Type { get; private set; }
        public DataType Object { get; private set; }
        public DataType String { get; private set; }
        public DataType Array { get; private set; }
        public DataType Attribute { get; private set; }
        public DataType Delegate { get; private set; }
        public DataType Exception { get; private set; }
        public DataType Application { get; private set; }
        public DataType CoreApp { get; private set; }
        public DataType Texture2D { get; private set; }
        public DataType TextureCube { get; private set; }
        public DataType Sampler2D { get; private set; }
        public DataType SamplerCube { get; private set; }
        public DataType SamplerState { get; private set; }
        public DataType VertexAttributeType { get; private set; }
        public DataType IndexType { get; private set; }
        public DataType VertexBuffer { get; private set; }
        public DataType IndexBuffer { get; private set; }
        public DataType VideoTexture { get; private set; }
        public DataType VideoSampler { get; private set; }
        public DataType IDisposable { get; private set; }
        public DataType IEnumerable_T { get; private set; }
        public DataType ArrayEnumerable_T { get; private set; }
        public DataType Bundle { get; private set; }
        public DataType Monitor { get; private set; }
        public DataType ValueType { get; private set; }

        public DataType DontExportAttribute { get; private set; }
        public DataType StageInlineAttribute { get; private set; }
        public DataType RequireShaderStageAttribute { get; private set; }
        public DataType MainClassAttribute { get; private set; }
        public DataType IgnoreMainClassAttribute { get; private set; }
        public DataType DotNetTypeAttribute { get; private set; }
        public DataType DotNetOverrideAttribute { get; private set; }
        public DataType TargetSpecificTypeAttribute { get; private set; }
        public DataType TargetSpecificImplementationAttribute { get; private set; }
        public DataType CallerFilePathAttribute { get; private set; }
        public DataType CallerLineNumberAttribute { get; private set; }
        public DataType CallerMemberNameAttribute { get; private set; }
        public DataType CallerPackageNameAttribute { get; private set; }
        public DataType WeakReferenceAttribute { get; private set; }
        public DataType OptionalAttribute { get; private set; }
        public DataType ObsoleteAttribute { get; private set; }
        public DataType AttributeUsageAttribute { get; private set; }
        public DataType NativeClassAttribute { get; private set; }
        public DataType UxGeneratedAttribute { get; private set; }
        public DataType ForeignAttribute { get; private set; }
        public DataType ForeignTypeNameAttribute { get; private set; }
        public DataType ForeignIncludeAttribute { get; private set; }
        public DataType ForeignAnnotationAttribute { get; private set; }
        public DataType ProcessFileAttribute { get; private set; }
        public DataType RequireAttribute { get; private set; }
        public DataType SetAttribute { get; private set; }
        public DataType Language { get; private set; }

        internal void Resolve(ILFactory ilf)
        {
            Void = ilf.GetType("void");
            BuiltinTypes[(int) BuiltinType.Bool] = Bool = ilf.GetType("Uno.Bool");
            BuiltinTypes[(int) BuiltinType.Char] = Char = ilf.GetType("Uno.Char");
            BuiltinTypes[(int) BuiltinType.Byte] = Byte = ilf.GetType("Uno.Byte");
            BuiltinTypes[(int) BuiltinType.Byte2] = ilf.GetType("Uno.Byte2");
            BuiltinTypes[(int) BuiltinType.Byte4] = ilf.GetType("Uno.Byte4");
            BuiltinTypes[(int) BuiltinType.SByte] = SByte = ilf.GetType("Uno.SByte");
            BuiltinTypes[(int) BuiltinType.SByte2] = ilf.GetType("Uno.SByte2");
            BuiltinTypes[(int) BuiltinType.SByte4] = ilf.GetType("Uno.SByte4");
            BuiltinTypes[(int) BuiltinType.Short] = Short = ilf.GetType("Uno.Short");
            BuiltinTypes[(int) BuiltinType.Short2] = ilf.GetType("Uno.Short2");
            BuiltinTypes[(int) BuiltinType.Short4] = ilf.GetType("Uno.Short4");
            BuiltinTypes[(int) BuiltinType.UShort] = UShort = ilf.GetType("Uno.UShort");
            BuiltinTypes[(int) BuiltinType.UShort2] = ilf.GetType("Uno.UShort2");
            BuiltinTypes[(int) BuiltinType.UShort4] = ilf.GetType("Uno.UShort4");
            BuiltinTypes[(int) BuiltinType.Int] = Int = ilf.GetType("Uno.Int");
            BuiltinTypes[(int) BuiltinType.Int2] = Int2 = ilf.GetType("Uno.Int2");
            BuiltinTypes[(int) BuiltinType.Int3] = Int3 = ilf.GetType("Uno.Int3");
            BuiltinTypes[(int) BuiltinType.Int4] = Int4 = ilf.GetType("Uno.Int4");
            BuiltinTypes[(int) BuiltinType.UInt] = UInt = ilf.GetType("Uno.UInt");
            BuiltinTypes[(int) BuiltinType.Long] = Long = ilf.GetType("Uno.Long");
            BuiltinTypes[(int) BuiltinType.ULong] = ULong = ilf.GetType("Uno.ULong");
            BuiltinTypes[(int) BuiltinType.Double] = Double = ilf.GetType("Uno.Double");
            BuiltinTypes[(int) BuiltinType.Float] = Float = ilf.GetType("Uno.Float");
            BuiltinTypes[(int) BuiltinType.Float2] = Float2 = ilf.GetType("Uno.Float2");
            BuiltinTypes[(int) BuiltinType.Float2x2] = Float2x2 = ilf.GetType("Uno.Float2x2");
            BuiltinTypes[(int) BuiltinType.Float3] = Float3 = ilf.GetType("Uno.Float3");
            BuiltinTypes[(int) BuiltinType.Float3x3] = Float3x3 = ilf.GetType("Uno.Float3x3");
            BuiltinTypes[(int) BuiltinType.Float4] = Float4 = ilf.GetType("Uno.Float4");
            BuiltinTypes[(int) BuiltinType.Float4x4] = Float4x4 = ilf.GetType("Uno.Float4x4");
            BuiltinTypes[(int) BuiltinType.Object] = Object = ilf.GetType("Uno.Object");
            BuiltinTypes[(int) BuiltinType.String] = String = ilf.GetType("Uno.String");

            Type = ilf.GetType("Uno.Type");
            Array = ilf.GetType("Uno.Array");
            Attribute = ilf.GetType("Uno.Attribute");
            Delegate = ilf.GetType("Uno.Delegate");
            Exception = ilf.GetType("Uno.Exception");
            Application = ilf.GetType("Uno.Application");
            CoreApp = ilf.GetType("Uno.Platform.CoreApp");
            BuiltinTypes[(int) BuiltinType.Texture2D] = Texture2D = ilf.GetType("Uno.Graphics.Texture2D");
            BuiltinTypes[(int) BuiltinType.TextureCube] = TextureCube = ilf.GetType("Uno.Graphics.TextureCube");
            BuiltinTypes[(int) BuiltinType.Sampler2D] = Sampler2D = ilf.GetType("Uno.Graphics.Sampler2D");
            BuiltinTypes[(int) BuiltinType.SamplerCube] = SamplerCube = ilf.GetType("Uno.Graphics.SamplerCube");
            BuiltinTypes[(int) BuiltinType.VideoTexture] = VideoTexture = ilf.GetType("Uno.Graphics.VideoTexture");
            BuiltinTypes[(int) BuiltinType.VideoSampler] = VideoSampler = ilf.GetType("Uno.Graphics.VideoSampler");
            SamplerState = ilf.GetType("Uno.Graphics.SamplerState");
            BuiltinTypes[(int) BuiltinType.Framebuffer] = ilf.GetType("Uno.Graphics.Framebuffer");
            VertexAttributeType = ilf.GetType("Uno.Graphics.VertexAttributeType");
            IndexType = ilf.GetType("Uno.Graphics.IndexType");
            VertexBuffer = ilf.GetType("Uno.Graphics.VertexBuffer");
            IndexBuffer = ilf.GetType("Uno.Graphics.IndexBuffer");
            IDisposable = ilf.GetType("Uno.IDisposable");
            IEnumerable_T = ilf.GetType("Uno.Collections.IEnumerable<>");
            ArrayEnumerable_T = ilf.GetType("Uno.Internal.ArrayEnumerable<>");
            Bundle = ilf.GetType("Uno.IO.Bundle");
            Monitor = ilf.GetType("Uno.Threading.Monitor");
            ValueType = ilf.GetType("Uno.ValueType");

            DontExportAttribute = ilf.GetType("Uno.Compiler.ExportTargetInterop.DontExportAttribute");
            StageInlineAttribute = ilf.GetType("Uno.Compiler.ShaderGenerator.ShaderStageInlineAttribute");
            RequireShaderStageAttribute = ilf.GetType("Uno.Compiler.ShaderGenerator.RequireShaderStageAttribute");
            MainClassAttribute = ilf.GetType("Uno.Compiler.MainClassAttribute");
            IgnoreMainClassAttribute = ilf.GetType("Uno.Compiler.IgnoreMainClassAttribute");
            DotNetTypeAttribute = ilf.GetType("Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute");
            DotNetOverrideAttribute = ilf.GetType("Uno.Compiler.ExportTargetInterop.DotNetOverrideAttribute");
            TargetSpecificTypeAttribute = ilf.GetType("Uno.Compiler.ExportTargetInterop.TargetSpecificTypeAttribute");
            TargetSpecificImplementationAttribute = ilf.GetType("Uno.Compiler.ExportTargetInterop.TargetSpecificImplementationAttribute");
            CallerFilePathAttribute = ilf.GetType("Uno.Compiler.CallerFilePathAttribute");
            CallerLineNumberAttribute = ilf.GetType("Uno.Compiler.CallerLineNumberAttribute");
            CallerMemberNameAttribute = ilf.GetType("Uno.Compiler.CallerMemberNameAttribute");
            CallerPackageNameAttribute = ilf.GetType("Uno.Compiler.CallerPackageNameAttribute");
            WeakReferenceAttribute = ilf.GetType("Uno.WeakReferenceAttribute");
            OptionalAttribute = ilf.GetType("Uno.Compiler.ExportTargetInterop.OptionalAttribute");
            ObsoleteAttribute = ilf.GetType("Uno.ObsoleteAttribute");
            AttributeUsageAttribute = ilf.GetType("Uno.AttributeUsageAttribute");
            NativeClassAttribute = ilf.GetType("Uno.Compiler.ExportTargetInterop.NativeClassAttribute");
            UxGeneratedAttribute = ilf.GetType("Uno.Compiler.UxGeneratedAttribute");
            ForeignAttribute = ilf.GetType ("Uno.Compiler.ExportTargetInterop.ForeignAttribute");
            ForeignIncludeAttribute = ilf.GetType ("Uno.Compiler.ExportTargetInterop.ForeignIncludeAttribute");
            ForeignAnnotationAttribute = ilf.GetType("Uno.Compiler.ExportTargetInterop.ForeignAnnotationAttribute");
            ForeignTypeNameAttribute = ilf.GetType("Uno.Compiler.ExportTargetInterop.ForeignTypeNameAttribute");
            ProcessFileAttribute = ilf.GetType("Uno.Compiler.ExportTargetInterop.ProcessFileAttribute");
            RequireAttribute = ilf.GetType("Uno.Compiler.ExportTargetInterop.RequireAttribute");
            SetAttribute = ilf.GetType("Uno.Compiler.ExportTargetInterop.SetAttribute");
            Language = ilf.GetType("Uno.Compiler.ExportTargetInterop.Language");
        }
    }
}
