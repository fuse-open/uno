namespace Uno.Graphics
{
    public static class IndexTypeHelpers
    {
        public static int GetStrideInBytes(IndexType type)
        {
            switch (type)
            {
                case IndexType.Byte:
                case IndexType.UShort:
                case IndexType.UInt:
                    return (int)type;

                default:
                    throw new FormatException("Invalid IndexType <" + type + ">");
            }
        }
    }
}
