using Uno;
using Uno.Collections;
using Uno.Graphics;

namespace UnoTest
{
    public class Batch
    {
        int maxVertices, maxIndices;
        bool staticBatch;

        public Batch(int maxVertices, int maxIndices, bool staticBatch)
        {
            this.maxVertices = maxVertices;
            this.maxIndices = maxIndices;
            this.staticBatch = staticBatch;
        }

        BatchVertexBuffer positions;
        BatchVertexBuffer texCoord0s;
        BatchVertexBuffer texCoord1s;
        BatchVertexBuffer texCoord2s;
        BatchVertexBuffer texCoord3s;
        BatchVertexBuffer texCoord4s;
        BatchVertexBuffer texCoord5s;
        BatchVertexBuffer texCoord6s;
        BatchVertexBuffer texCoord7s;
        BatchVertexBuffer normals;
        BatchVertexBuffer binormals;
        BatchVertexBuffer tangents;
        BatchVertexBuffer colors;
        BatchVertexBuffer instanceIndex;
        BatchVertexBuffer boneIndices;
        BatchVertexBuffer boneWeights;

        BatchVertexBuffer attrib0;
        BatchVertexBuffer attrib1;
        BatchVertexBuffer attrib2;
        BatchVertexBuffer attrib3;
        BatchVertexBuffer attrib4;
        BatchVertexBuffer attrib5;
        BatchVertexBuffer attrib6;
        BatchVertexBuffer attrib7;

        public BatchVertexBuffer Positions { get { return positions ?? (positions = new BatchVertexBuffer(VertexAttributeType.Float3, maxVertices, staticBatch)); } set { positions = value; }}
        public BatchVertexBuffer TexCoord0s { get { return texCoord0s ?? (texCoord0s = new BatchVertexBuffer(VertexAttributeType.Float2, maxVertices, staticBatch)); } set { texCoord0s = value; }}
        public BatchVertexBuffer TexCoord1s { get { return texCoord1s ?? (texCoord1s = new BatchVertexBuffer(VertexAttributeType.Float2, maxVertices, staticBatch)); } set { texCoord1s = value; }}
        public BatchVertexBuffer TexCoord2s { get { return texCoord2s ?? (texCoord2s = new BatchVertexBuffer(VertexAttributeType.Float2, maxVertices, staticBatch)); } set { texCoord2s = value; }}
        public BatchVertexBuffer TexCoord3s { get { return texCoord3s ?? (texCoord3s = new BatchVertexBuffer(VertexAttributeType.Float2, maxVertices, staticBatch)); } set { texCoord3s = value; }}
        public BatchVertexBuffer TexCoord4s { get { return texCoord4s ?? (texCoord4s = new BatchVertexBuffer(VertexAttributeType.Float2, maxVertices, staticBatch)); } set { texCoord4s = value; }}
        public BatchVertexBuffer TexCoord5s { get { return texCoord5s ?? (texCoord5s = new BatchVertexBuffer(VertexAttributeType.Float2, maxVertices, staticBatch)); } set { texCoord5s = value; }}
        public BatchVertexBuffer TexCoord6s { get { return texCoord6s ?? (texCoord6s = new BatchVertexBuffer(VertexAttributeType.Float2, maxVertices, staticBatch)); } set { texCoord6s = value; }}
        public BatchVertexBuffer TexCoord7s { get { return texCoord7s ?? (texCoord7s = new BatchVertexBuffer(VertexAttributeType.Float2, maxVertices, staticBatch)); } set { texCoord7s = value; }}
        public BatchVertexBuffer Normals { get { return normals ?? (normals = new BatchVertexBuffer(VertexAttributeType.Float3, maxVertices, staticBatch)); } set { normals = value; }}
        public BatchVertexBuffer Binormals { get { return binormals ?? (binormals = new BatchVertexBuffer(VertexAttributeType.Float3, maxVertices, staticBatch)); } set { binormals = value; }}
        public BatchVertexBuffer Tangents { get { return tangents ?? (tangents = new BatchVertexBuffer(VertexAttributeType.Float4, maxVertices, staticBatch)); } set { tangents = value; }}
        public BatchVertexBuffer Colors { get { return colors ?? (colors = new BatchVertexBuffer(VertexAttributeType.Float4, maxVertices, staticBatch)); } set { colors = value; }}
        public BatchVertexBuffer InstanceIndices { get { return instanceIndex ?? (instanceIndex = new BatchVertexBuffer(VertexAttributeType.UShort, maxVertices, staticBatch)); } set { instanceIndex = value; }}
        public BatchVertexBuffer BoneIndexBuffer { get { return boneIndices ?? (boneIndices = new BatchVertexBuffer(VertexAttributeType.Byte4, maxVertices, staticBatch)); } set { boneIndices = value; }}
        public BatchVertexBuffer BoneWeightBuffer { get { return boneWeights ?? (boneWeights = new BatchVertexBuffer(VertexAttributeType.Byte4Normalized, maxVertices, staticBatch)); } set { boneWeights = value; }}

        public BatchVertexBuffer Attrib0Buffer { get { return attrib0 ?? (attrib0 = new BatchVertexBuffer(VertexAttributeType.Float4, maxVertices, staticBatch)); } set { attrib0 = value; }}
        public BatchVertexBuffer Attrib1Buffer { get { return attrib1 ?? (attrib1 = new BatchVertexBuffer(VertexAttributeType.Float4, maxVertices, staticBatch)); } set { attrib1 = value; }}
        public BatchVertexBuffer Attrib2Buffer { get { return attrib2 ?? (attrib2 = new BatchVertexBuffer(VertexAttributeType.Float4, maxVertices, staticBatch)); } set { attrib2 = value; }}
        public BatchVertexBuffer Attrib3Buffer { get { return attrib3 ?? (attrib3 = new BatchVertexBuffer(VertexAttributeType.Float4, maxVertices, staticBatch)); } set { attrib3 = value; }}
        public BatchVertexBuffer Attrib4Buffer { get { return attrib4 ?? (attrib4 = new BatchVertexBuffer(VertexAttributeType.Float4, maxVertices, staticBatch)); } set { attrib4 = value; }}
        public BatchVertexBuffer Attrib5Buffer { get { return attrib5 ?? (attrib5 = new BatchVertexBuffer(VertexAttributeType.Float4, maxVertices, staticBatch)); } set { attrib5 = value; }}
        public BatchVertexBuffer Attrib6Buffer { get { return attrib6 ?? (attrib6 = new BatchVertexBuffer(VertexAttributeType.Float4, maxVertices, staticBatch)); } set { attrib6 = value; }}
        public BatchVertexBuffer Attrib7Buffer { get { return attrib7 ?? (attrib7 = new BatchVertexBuffer(VertexAttributeType.Float4, maxVertices, staticBatch)); } set { attrib7 = value; }}

        bool hasExplicitVertexCount = false;
        int explicitVertexCount;
        public int VertexCount
        {
            get { return hasExplicitVertexCount ? explicitVertexCount : maxIndices; }
            set { hasExplicitVertexCount = true; explicitVertexCount = value; }
        }


        BatchIndexBuffer indexBuffer;
        public BatchIndexBuffer Indices { get { return indexBuffer ?? (indexBuffer = new BatchIndexBuffer(IndexType.UShort, maxIndices, staticBatch)); } set { indexBuffer = value; } }

        public float3 VertexPosition:   vertex_attrib<float3>(Positions.DataType,       Positions.VertexBuffer,     Positions.StrideInBytes,        0, IndexType, IndexBuffer);
        public float2 TexCoord0:        vertex_attrib<float2>(TexCoord0s.DataType,      TexCoord0s.VertexBuffer,    TexCoord0s.StrideInBytes,       0, IndexType, IndexBuffer);
        public float2 TexCoord1:        vertex_attrib<float2>(TexCoord1s.DataType,      TexCoord1s.VertexBuffer,    TexCoord1s.StrideInBytes,       0, IndexType, IndexBuffer);
        public float2 TexCoord2:        vertex_attrib<float2>(TexCoord2s.DataType,      TexCoord2s.VertexBuffer,    TexCoord2s.StrideInBytes,       0, IndexType, IndexBuffer);
        public float2 TexCoord3:        vertex_attrib<float2>(TexCoord3s.DataType,      TexCoord3s.VertexBuffer,    TexCoord3s.StrideInBytes,       0, IndexType, IndexBuffer);
        public float2 TexCoord4:        vertex_attrib<float2>(TexCoord4s.DataType,      TexCoord4s.VertexBuffer,    TexCoord4s.StrideInBytes,       0, IndexType, IndexBuffer);
        public float2 TexCoord5:        vertex_attrib<float2>(TexCoord5s.DataType,      TexCoord5s.VertexBuffer,    TexCoord5s.StrideInBytes,       0, IndexType, IndexBuffer);
        public float2 TexCoord6:        vertex_attrib<float2>(TexCoord6s.DataType,      TexCoord6s.VertexBuffer,    TexCoord6s.StrideInBytes,       0, IndexType, IndexBuffer);
        public float2 TexCoord7:        vertex_attrib<float2>(TexCoord7s.DataType,      TexCoord7s.VertexBuffer,    TexCoord7s.StrideInBytes,       0, IndexType, IndexBuffer);
        public float3 VertexNormal:     vertex_attrib<float3>(Normals.DataType,         Normals.VertexBuffer,       Normals.StrideInBytes,          0, IndexType, IndexBuffer);
        public float4 VertexTangentSpace: vertex_attrib<float4>(Tangents.DataType,          Tangents.VertexBuffer,      Tangents.StrideInBytes,             0, IndexType, IndexBuffer);
        public float3 VertexTangent:    VertexTangentSpace.XYZ;
        public float3 VertexBinormal:   Vector.Cross(VertexTangent, VertexNormal) * VertexTangentSpace.W;
        public float4 VertexColor:      vertex_attrib<float4>(Colors.DataType,          Colors.VertexBuffer,        Colors.StrideInBytes,           0, IndexType, IndexBuffer);
        public float InstanceIndex:     vertex_attrib<float>(InstanceIndices.DataType,      InstanceIndices.VertexBuffer,   InstanceIndices.StrideInBytes,  0, IndexType, IndexBuffer);
        public float4 BoneIndices:      vertex_attrib<float4>(BoneIndexBuffer.DataType,     BoneIndexBuffer.VertexBuffer,   BoneIndexBuffer.StrideInBytes,  0, IndexType, IndexBuffer);
        public float4 BoneWeights:      vertex_attrib<float4>(BoneWeightBuffer.DataType,    BoneWeightBuffer.VertexBuffer,  BoneWeightBuffer.StrideInBytes, 0, IndexType, IndexBuffer);
        public float4 Attrib0:          vertex_attrib<float4>(Attrib0Buffer.DataType,       Attrib0Buffer.VertexBuffer,     Attrib0Buffer.StrideInBytes,    0, IndexType, IndexBuffer);
        public float4 Attrib1:          vertex_attrib<float4>(Attrib1Buffer.DataType,       Attrib1Buffer.VertexBuffer,     Attrib1Buffer.StrideInBytes,    0, IndexType, IndexBuffer);
        public float4 Attrib2:          vertex_attrib<float4>(Attrib2Buffer.DataType,       Attrib2Buffer.VertexBuffer,     Attrib2Buffer.StrideInBytes,    0, IndexType, IndexBuffer);
        public float4 Attrib3:          vertex_attrib<float4>(Attrib3Buffer.DataType,       Attrib3Buffer.VertexBuffer,     Attrib3Buffer.StrideInBytes,    0, IndexType, IndexBuffer);
        public float4 Attrib4:          vertex_attrib<float4>(Attrib4Buffer.DataType,       Attrib4Buffer.VertexBuffer,     Attrib4Buffer.StrideInBytes,    0, IndexType, IndexBuffer);
        public float4 Attrib5:          vertex_attrib<float4>(Attrib5Buffer.DataType,       Attrib5Buffer.VertexBuffer,     Attrib5Buffer.StrideInBytes,    0, IndexType, IndexBuffer);
        public float4 Attrib6:          vertex_attrib<float4>(Attrib6Buffer.DataType,       Attrib6Buffer.VertexBuffer,     Attrib6Buffer.StrideInBytes,    0, IndexType, IndexBuffer);
        public float4 Attrib7:          vertex_attrib<float4>(Attrib7Buffer.DataType,       Attrib7Buffer.VertexBuffer,     Attrib7Buffer.StrideInBytes,    0, IndexType, IndexBuffer);


        public IndexBuffer IndexBuffer
        {
            get { return indexBuffer == null ? null : Indices.IndexBuffer; }
        }

        public IndexType IndexType
        {
            get { return indexBuffer == null ? IndexType.Undefined : Indices.DataType; }
        }

        public float2 TexCoord: TexCoord0;
    }
}
