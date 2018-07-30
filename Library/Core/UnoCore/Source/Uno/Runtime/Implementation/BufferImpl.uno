using Uno.Compiler.ExportTargetInterop;

namespace Uno.Runtime.Implementation
{
    [Obsolete("Use extension methods on Uno.ByteArrayExtensions instead")]
    public static class BufferImpl
    {
        public static short GetShort(byte[] buffer, int offset, bool littleEndian)
        {
            return ByteArrayExtensions.GetShort(buffer, offset, littleEndian);
        }

        public static void SetShort(byte[] buffer, int offset, short value, bool littleEndian)
        {
            ByteArrayExtensions.Set(buffer, offset, value, littleEndian);
        }

        public static ushort GetUShort(byte[] buffer, int offset, bool littleEndian)
        {
            return ByteArrayExtensions.GetUShort(buffer, offset, littleEndian);
        }

        public static void SetUShort(byte[] buffer, int offset, ushort value, bool littleEndian)
        {
            ByteArrayExtensions.Set(buffer, offset, value, littleEndian);
        }

        public static int GetInt(byte[] buffer, int offset, bool littleEndian)
        {
            return ByteArrayExtensions.GetInt(buffer, offset, littleEndian);
        }

        public static void SetInt(byte[] buffer, int offset, int value, bool littleEndian)
        {
            ByteArrayExtensions.Set(buffer, offset, value, littleEndian);
        }

        public static uint GetUInt(byte[] buffer, int offset, bool littleEndian)
        {
            return ByteArrayExtensions.GetUInt(buffer, offset, littleEndian);
        }

        public static void SetUInt(byte[] buffer, int offset, uint value, bool littleEndian)
        {
            ByteArrayExtensions.Set(buffer, offset, value, littleEndian);
        }

        public static long GetLong(byte[] buffer, int offset, bool littleEndian)
        {
            return ByteArrayExtensions.GetLong(buffer, offset, littleEndian);
        }

        public static void SetLong(byte[] buffer, int offset, long value, bool littleEndian)
        {
            ByteArrayExtensions.Set(buffer, offset, value, littleEndian);
        }

        public static ulong GetULong(byte[] buffer, int offset, bool littleEndian)
        {
            return ByteArrayExtensions.GetULong(buffer, offset, littleEndian);
        }

        public static void SetULong(byte[] buffer, int offset, ulong value, bool littleEndian)
        {
            ByteArrayExtensions.Set(buffer, offset, value, littleEndian);
        }

        public static float GetFloat(byte[] buffer, int offset, bool littleEndian)
        {
            return ByteArrayExtensions.GetFloat(buffer, offset, littleEndian);
        }

        public static void SetFloat(byte[] buffer, int offset, float value, bool littleEndian)
        {
            ByteArrayExtensions.Set(buffer, offset, value, littleEndian);
        }

        public static double GetDouble(byte[] buffer, int offset, bool littleEndian)
        {
            return ByteArrayExtensions.GetDouble(buffer, offset, littleEndian);
        }

        public static void SetDouble(byte[] buffer, int offset, double value, bool littleEndian)
        {
            ByteArrayExtensions.Set(buffer, offset, value, littleEndian);
        }
    }
}
