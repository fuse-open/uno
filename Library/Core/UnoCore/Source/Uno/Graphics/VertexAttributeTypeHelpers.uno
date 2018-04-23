namespace Uno.Graphics
{
    public static class VertexAttributeTypeHelpers
    {
        public static int GetStrideInBytes(VertexAttributeType type)
        {
            switch (type)
            {
                case VertexAttributeType.Float:
                    return 4;

                case VertexAttributeType.Float2:
                    return 8;

                case VertexAttributeType.Float3:
                    return 12;

                case VertexAttributeType.Float4:
                    return 16;

                case VertexAttributeType.Short:
                case VertexAttributeType.ShortNormalized:
                case VertexAttributeType.UShort:
                case VertexAttributeType.UShortNormalized:
                    return 2;

                case VertexAttributeType.Short2:
                case VertexAttributeType.Short2Normalized:
                case VertexAttributeType.UShort2:
                case VertexAttributeType.UShort2Normalized:
                    return 4;

                case VertexAttributeType.Short4:
                case VertexAttributeType.Short4Normalized:
                case VertexAttributeType.UShort4:
                case VertexAttributeType.UShort4Normalized:
                    return 8;

                case VertexAttributeType.SByte4:
                case VertexAttributeType.SByte4Normalized:
                case VertexAttributeType.Byte4:
                case VertexAttributeType.Byte4Normalized:
                    return 4;

                default:
                    throw new FormatException("Invalid VertexAttributeType <" + type + ">");
            }
        }
    }
}
