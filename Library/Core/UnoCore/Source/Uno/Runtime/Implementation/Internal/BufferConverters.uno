namespace Uno.Runtime.Implementation.Internal
{
    [Obsolete]
    public static class BufferConverters
    {
        public static Buffer ToBuffer(float[] data)
        {
            var result = new Buffer(data.Length * 4);

            for (int i = 0; i < data.Length; i++)
                result.Set(i * 4, data[i]);

            return result;
        }

        public static Buffer ToBuffer(float2[] data)
        {
            var result = new Buffer(data.Length * 8);

            for (int i = 0; i < data.Length; i++)
                result.Set(i * 8, data[i]);

            return result;
        }

        public static Buffer ToBuffer(float3[] data)
        {
            var result = new Buffer(data.Length * 12);

            for (int i = 0; i < data.Length; i++)
                result.Set(i * 12, data[i]);

            return result;
        }

        public static Buffer ToBuffer(float4[] data)
        {
            var result = new Buffer(data.Length * 16);

            for (int i = 0; i < data.Length; i++)
                result.Set(i * 16, data[i]);

            return result;
        }

        public static Buffer ToBuffer(sbyte4[] data)
        {
            var result = new Buffer(data.Length * 4);

            for (int i = 0; i < data.Length; i++)
                result.Set(i * 4, data[i]);

            return result;
        }

        public static Buffer ToBuffer(byte[] data)
        {
            var result = new Buffer(data.Length);

            for (int i = 0; i < data.Length; i++)
                result.Set(i, data[i]);

            return result;
        }

        public static Buffer ToBuffer(ushort[] data)
        {
            var result = new Buffer(data.Length * 2);

            for (int i = 0; i < data.Length; i++)
                result.Set(i * 2, data[i]);

            return result;
        }

        public static Buffer ToBuffer(uint[] data)
        {
            var result = new Buffer(data.Length * 4);

            for (int i = 0; i < data.Length; i++)
                result.Set(i * 4, data[i]);

            return result;
        }

        public static Buffer ToBuffer(byte4[] data)
        {
            var result = new Buffer(data.Length * 4);

            for (int i = 0; i < data.Length; i++)
                result.Set(i * 4, data[i]);

            return result;
        }

        public static Buffer ToBuffer(short[] data)
        {
            var result = new Buffer(data.Length * 2);

            for (int i = 0; i < data.Length; i++)
                result.Set(i * 2, data[i]);

            return result;
        }
    }
}
