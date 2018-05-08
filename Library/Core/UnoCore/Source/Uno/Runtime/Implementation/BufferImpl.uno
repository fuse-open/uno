using Uno.Compiler.ExportTargetInterop;

namespace Uno.Runtime.Implementation
{
    [extern(CPLUSPLUS) Require("Source.Include", "Uno/Support.h")]
    public static class BufferImpl
    {
        public static short GetShort(this byte[] buffer, int offset, bool littleEndian)
        {
            if defined(CPLUSPLUS)
            @{
                return uLoadBytes<int16_t>((uint8_t*)$0->_ptr + $1, $2);
            @}
            else if defined(DOTNET)
            {
                return System.BitConverter.ToInt16(buffer.GetBytes(offset, 2, littleEndian), 0);
            }
            else
                build_error;
        }

        public static void SetShort(this byte[] buffer, int offset, short value, bool littleEndian)
        {
            if defined(CPLUSPLUS)
            @{
                uStoreBytes((uint8_t*)$0->_ptr + $1, $2, $3);
            @}
            else if defined(DOTNET)
            {
                buffer.SetBytes(offset, System.BitConverter.GetBytes(value), littleEndian);
            }
            else
                build_error;
        }

        public static ushort GetUShort(this byte[] buffer, int offset, bool littleEndian)
        {
            if defined(CPLUSPLUS)
            @{
                return uLoadBytes<uint16_t>((uint8_t*)$0->_ptr + $1, $2);
            @}
            else if defined(DOTNET)
            {
                return System.BitConverter.ToUInt16(buffer.GetBytes(offset, 2, littleEndian), 0);
            }
            else
                build_error;
        }

        public static void SetUShort(this byte[] buffer, int offset, ushort value, bool littleEndian)
        {
            if defined(CPLUSPLUS)
            @{
                uStoreBytes((uint8_t*)$0->_ptr + $1, $2, $3);
            @}
            else if defined(DOTNET)
            {
                buffer.SetBytes(offset, System.BitConverter.GetBytes(value), littleEndian);
            }
            else
                build_error;
        }

        public static int GetInt(this byte[] buffer, int offset, bool littleEndian)
        {
            if defined(CPLUSPLUS)
            @{
                return uLoadBytes<int>((uint8_t*)$0->_ptr + $1, $2);
            @}
            else if defined(DOTNET)
            {
                return System.BitConverter.ToInt32(buffer.GetBytes(offset, 4, littleEndian), 0);
            }
            else
                build_error;
        }

        public static void SetInt(this byte[] buffer, int offset, int value, bool littleEndian)
        {
            if defined(CPLUSPLUS)
            @{
                uStoreBytes((uint8_t*)$0->_ptr + $1, $2, $3);
            @}
            else if defined(DOTNET)
            {
                buffer.SetBytes(offset, System.BitConverter.GetBytes(value), littleEndian);
            }
            else
                build_error;
        }

        public static uint GetUInt(this byte[] buffer, int offset, bool littleEndian)
        {
            if defined(CPLUSPLUS)
            @{
                return uLoadBytes<uint32_t>((uint8_t*)$0->_ptr + $1, $2);
            @}
            else if defined(DOTNET)
            {
                return System.BitConverter.ToUInt32(buffer.GetBytes(offset, 4, littleEndian), 0);
            }
            else
                build_error;
        }

        public static void SetUInt(this byte[] buffer, int offset, uint value, bool littleEndian)
        {
            if defined(CPLUSPLUS)
            @{
                uStoreBytes((uint8_t*)$0->_ptr + $1, $2, $3);
            @}
            else if defined(DOTNET)
            {
                buffer.SetBytes(offset, System.BitConverter.GetBytes(value), littleEndian);
            }
            else
                build_error;
        }

        public static long GetLong(this byte[] buffer, int offset, bool littleEndian)
        {
            if defined(CPLUSPLUS)
            @{
                return uLoadBytes<int64_t>((uint8_t*)$0->_ptr + $1, $2);
            @}
            else if defined(DOTNET)
            {
                return System.BitConverter.ToInt64(buffer.GetBytes(offset, 8, littleEndian), 0);
            }
            else
                build_error;
        }

        public static void SetLong(this byte[] buffer, int offset, long value, bool littleEndian)
        {
            if defined(CPLUSPLUS)
            @{
                uStoreBytes((uint8_t*)$0->_ptr + $1, $2, $3);
            @}
            else if defined(DOTNET)
            {
                buffer.SetBytes(offset, System.BitConverter.GetBytes(value), littleEndian);
            }
            else
                build_error;
        }

        public static ulong GetULong(this byte[] buffer, int offset, bool littleEndian)
        {
            if defined(CPLUSPLUS)
            @{
                return uLoadBytes<uint64_t>((uint8_t*)$0->_ptr + $1, $2);
            @}
            else if defined(DOTNET)
            {
                return System.BitConverter.ToUInt64(buffer.GetBytes(offset, 8, littleEndian), 0);
            }
            else
                build_error;
        }

        public static void SetULong(this byte[] buffer, int offset, ulong value, bool littleEndian)
        {
            if defined(CPLUSPLUS)
            @{
                uStoreBytes((uint8_t*)$0->_ptr + $1, $2, $3);
            @}
            else if defined(DOTNET)
            {
                buffer.SetBytes(offset, System.BitConverter.GetBytes(value), littleEndian);
            }
            else
                build_error;
        }

        public static float GetFloat(this byte[] buffer, int offset, bool littleEndian)
        {
            if defined(CPLUSPLUS)
            @{
                return uLoadBytes<float>((uint8_t*)$0->_ptr + $1, $2);
            @}
            else if defined(DOTNET)
            {
                return System.BitConverter.ToSingle(buffer.GetBytes(offset, 4, littleEndian), 0);
            }
            else
                build_error;
        }

        public static void SetFloat(this byte[] buffer, int offset, float value, bool littleEndian)
        {
            if defined(CPLUSPLUS)
            @{
                uStoreBytes((uint8_t*)$0->_ptr + $1, $2, $3);
            @}
            else if defined(DOTNET)
            {
                buffer.SetBytes(offset, System.BitConverter.GetBytes(value), littleEndian);
            }
            else
                build_error;
        }

        public static double GetDouble(this byte[] buffer, int offset, bool littleEndian)
        {
            if defined(CPLUSPLUS)
            @{
                return uLoadBytes<double>((uint8_t*)$0->_ptr + $1, $2);
            @}
            else if defined(DOTNET)
            {
                return System.BitConverter.ToDouble(buffer.GetBytes(offset, 8, littleEndian), 0);
            }
            else
                build_error;
        }

        public static void SetDouble(this byte[] buffer, int offset, double value, bool littleEndian)
        {
            if defined(CPLUSPLUS)
            @{
                uStoreBytes((uint8_t*)$0->_ptr + $1, $2, $3);
            @}
            else if defined(DOTNET)
            {
                buffer.SetBytes(offset, System.BitConverter.GetBytes(value), littleEndian);
            }
            else
                build_error;
        }

        extern(DOTNET)
        static byte[] GetBytes(this byte[] buffer, int offset, int count, bool littleEndian)
        {
            var result = new byte[count];
            Array.Copy((Array) buffer, offset, result, 0, count);

            if (!littleEndian)
                Array.Reverse(result);

            return result;
        }

        extern(DOTNET)
        static void SetBytes(this byte[] buffer, int offset, byte[] value, bool littleEndian)
        {
            Array.Copy((Array) value, 0, buffer, offset, value.Length);

            if (!littleEndian)
                Array.Reverse(buffer, offset, value.Length);
        }
    }
}

namespace System
{
    [DotNetType]
    extern(DOTNET)
    class BitConverter
    {
        public static extern byte[] GetBytes(short value);
        public static extern byte[] GetBytes(ushort value);
        public static extern byte[] GetBytes(int value);
        public static extern byte[] GetBytes(uint value);
        public static extern byte[] GetBytes(long value);
        public static extern byte[] GetBytes(ulong value);
        public static extern byte[] GetBytes(float value);
        public static extern byte[] GetBytes(double value);

        public static extern short ToInt16(byte[] value, int startIndex);
        public static extern ushort ToUInt16(byte[] value, int startIndex);
        public static extern int ToInt32(byte[] value, int startIndex);
        public static extern uint ToUInt32(byte[] value, int startIndex);
        public static extern long ToInt64(byte[] value, int startIndex);
        public static extern ulong ToUInt64(byte[] value, int startIndex);
        public static extern float ToSingle(byte[] value, int startIndex);
        public static extern double ToDouble(byte[] value, int startIndex);
    }
}
