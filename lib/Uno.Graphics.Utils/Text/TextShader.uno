using Uno.Vector;

namespace Uno.Graphics.Utils.Text
{
    public sealed class TextShaderData
    {
        public float FontScale;
        public float FontSpread;

        public float4x4 ClipSpaceMatrix;
        public texture2D FontTexture;
        public PolygonFace DataCullFace;

        public int CharCount;

        public IndexType IndexType;
        public IndexBuffer IndexBuffer;

        public VertexAttributeInfo PositionInfo;
        public VertexAttributeInfo TexCoordInfo;
        public VertexAttributeInfo ColorInfo;

        internal TextShaderData(IndexBuffer ibo, VertexBuffer vbo)
        {
            FontScale = 1;
            ClipSpaceMatrix = float4x4.Identity;
            DataCullFace = PolygonFace.None;

            IndexType = IndexType.UShort;
            IndexBuffer = ibo;

            PositionInfo.Buffer = vbo;
            PositionInfo.BufferOffset = 0;
            PositionInfo.BufferStride = 16;
            PositionInfo.Type = VertexAttributeType.Float2;

            TexCoordInfo.Buffer = vbo;
            TexCoordInfo.BufferOffset = 8;
            TexCoordInfo.BufferStride = 16;
            TexCoordInfo.Type = VertexAttributeType.UShort2Normalized;

            ColorInfo.Buffer = vbo;
            ColorInfo.BufferOffset = 12;
            ColorInfo.BufferStride = 16;
            ColorInfo.Type = VertexAttributeType.Byte4Normalized;
        }
    }

    public abstract class TextShader
    {
        apply TextShaderData;

        BlendEnabled: true;
        BlendSrc: BlendOperand.SrcAlpha;
        BlendDst: BlendOperand.OneMinusSrcAlpha;

        DepthTestEnabled: false;
        CullFace: DataCullFace;
        VertexCount: CharCount * 6;

        public float2 Position: vertex_attrib<float2>(PositionInfo.Type, PositionInfo.Buffer, PositionInfo.BufferStride, PositionInfo.BufferOffset, IndexType, IndexBuffer);
        public float2 TexCoord: vertex_attrib<float2>(TexCoordInfo.Type, TexCoordInfo.Buffer, TexCoordInfo.BufferStride, TexCoordInfo.BufferOffset, IndexType, IndexBuffer);
        public float4 Color: vertex_attrib<float4>(ColorInfo.Type, ColorInfo.Buffer, ColorInfo.BufferStride, ColorInfo.BufferOffset, IndexType, IndexBuffer);

        ClipPosition: Transform(float4(Position, 0, 1), ClipSpaceMatrix);

        SamplerState SamplerState: SamplerState.TrilinearClamp;
        public float FontMask: sample(FontTexture, TexCoord, SamplerState).X;

        public abstract void Draw(TextShaderData data);
    }
}
