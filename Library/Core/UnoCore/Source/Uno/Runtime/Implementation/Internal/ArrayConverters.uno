namespace Uno.Runtime.Implementation.Internal
{
    public static class ArrayConverters
    {
        [Obsolete]
        public static float4x4[] ToFloat4x4Array(Buffer b)
        {
            return BufferReader.ReadArray<float4x4>(new BufferReader(b), b.SizeInBytes / 64, BufferReader.ReadFloat4x4);
        }
    }
}
