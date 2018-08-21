using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    public static class ByteArrayExtensions
    {
        public static sbyte GetSByte(this byte[] bytes, int offset)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(byte))
                throw new ArgumentOutOfRangeException(nameof(offset));

            return (sbyte)bytes[offset];
        }

        public static void Set(this byte[] bytes, int offset, sbyte value)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(byte))
                throw new ArgumentOutOfRangeException(nameof(offset));

            bytes[offset] = (byte)value;
        }

        public static byte GetByte(this byte[] bytes, int offset)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(byte))
                throw new ArgumentOutOfRangeException(nameof(offset));

            return bytes[offset];
        }

        public static void Set(this byte[] bytes, int offset, byte value)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(byte))
                throw new ArgumentOutOfRangeException(nameof(offset));

            bytes[offset] = value;
        }

        public static short GetShort(this byte[] bytes, int offset, bool littleEndian = true)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(short))
                throw new ArgumentOutOfRangeException(nameof(offset));

            if defined(CPLUSPLUS)
            @{
                @{short} result;
                memcpy(&result, (uint8_t*)$0->_ptr + $1, sizeof(result));
                if (!$2)
                    @{ReverseBytes(IntPtr, ulong):Call(&result, sizeof(result))};
                return result;
            @}
            else if defined(DOTNET)
            {
                return System.BitConverter.ToInt16(bytes.GetBytes(offset, 2, littleEndian), 0);
            }
            else
                build_error;
        }

        public static void Set(this byte[] bytes, int offset, short value, bool littleEndian = true)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(short))
                throw new ArgumentOutOfRangeException(nameof(offset));

            if defined(CPLUSPLUS)
            @{
                if (!$3)
                    @{ReverseBytes(IntPtr, ulong):Call(&$2, sizeof($2))};
                memcpy((uint8_t*)$0->_ptr + $1, &$2, sizeof($2));
            @}
            else if defined(DOTNET)
            {
                bytes.SetBytes(offset, System.BitConverter.GetBytes(value), littleEndian);
            }
            else
                build_error;
        }

        public static ushort GetUShort(this byte[] bytes, int offset, bool littleEndian = true)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(ushort))
                throw new ArgumentOutOfRangeException(nameof(offset));

            if defined(CPLUSPLUS)
            @{
                @{ushort} result;
                memcpy(&result, (uint8_t*)$0->_ptr + $1, sizeof(result));
                if (!$2)
                    @{ReverseBytes(IntPtr, ulong):Call(&result, sizeof(result))};
                return result;
            @}
            else if defined(DOTNET)
            {
                return System.BitConverter.ToUInt16(bytes.GetBytes(offset, 2, littleEndian), 0);
            }
            else
                build_error;
        }

        public static void Set(this byte[] bytes, int offset, ushort value, bool littleEndian = true)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(ushort))
                throw new ArgumentOutOfRangeException(nameof(offset));

            if defined(CPLUSPLUS)
            @{
                if (!$3)
                    @{ReverseBytes(IntPtr, ulong):Call(&$2, sizeof($2))};
                memcpy((uint8_t*)$0->_ptr + $1, &$2, sizeof($2));
            @}
            else if defined(DOTNET)
            {
                bytes.SetBytes(offset, System.BitConverter.GetBytes(value), littleEndian);
            }
            else
                build_error;
        }

        public static int GetInt(this byte[] bytes, int offset, bool littleEndian = true)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(int))
                throw new ArgumentOutOfRangeException(nameof(offset));

            if defined(CPLUSPLUS)
            @{
                @{int} result;
                memcpy(&result, (uint8_t*)$0->_ptr + $1, sizeof(result));
                if (!$2)
                    @{ReverseBytes(IntPtr, ulong):Call(&result, sizeof(result))};
                return result;
            @}
            else if defined(DOTNET)
            {
                return System.BitConverter.ToInt32(bytes.GetBytes(offset, 4, littleEndian), 0);
            }
            else
                build_error;
        }

        public static void Set(this byte[] bytes, int offset, int value, bool littleEndian = true)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(int))
                throw new ArgumentOutOfRangeException(nameof(offset));

            if defined(CPLUSPLUS)
            @{
                if (!$3)
                    @{ReverseBytes(IntPtr, ulong):Call(&$2, sizeof($2))};
                memcpy((uint8_t*)$0->_ptr + $1, &$2, sizeof($2));
            @}
            else if defined(DOTNET)
            {
                bytes.SetBytes(offset, System.BitConverter.GetBytes(value), littleEndian);
            }
            else
                build_error;
        }

        public static uint GetUInt(this byte[] bytes, int offset, bool littleEndian = true)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(uint))
                throw new ArgumentOutOfRangeException(nameof(offset));

            if defined(CPLUSPLUS)
            @{
                @{uint} result;
                memcpy(&result, (uint8_t*)$0->_ptr + $1, sizeof(result));
                if (!$2)
                    @{ReverseBytes(IntPtr, ulong):Call(&result, sizeof(result))};
                return result;
            @}
            else if defined(DOTNET)
            {
                return System.BitConverter.ToUInt32(bytes.GetBytes(offset, 4, littleEndian), 0);
            }
            else
                build_error;
        }

        public static void Set(this byte[] bytes, int offset, uint value, bool littleEndian = true)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(uint))
                throw new ArgumentOutOfRangeException(nameof(offset));

            if defined(CPLUSPLUS)
            @{
                if (!$3)
                    @{ReverseBytes(IntPtr, ulong):Call(&$2, sizeof($2))};
                memcpy((uint8_t*)$0->_ptr + $1, &$2, sizeof($2));
            @}
            else if defined(DOTNET)
            {
                bytes.SetBytes(offset, System.BitConverter.GetBytes(value), littleEndian);
            }
            else
                build_error;
        }

        public static long GetLong(this byte[] bytes, int offset, bool littleEndian = true)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(long))
                throw new ArgumentOutOfRangeException(nameof(offset));

            if defined(CPLUSPLUS)
            @{
                @{long} result;
                memcpy(&result, (uint8_t*)$0->_ptr + $1, sizeof(result));
                if (!$2)
                    @{ReverseBytes(IntPtr, ulong):Call(&result, sizeof(result))};
                return result;
            @}
            else if defined(DOTNET)
            {
                return System.BitConverter.ToInt64(bytes.GetBytes(offset, 8, littleEndian), 0);
            }
            else
                build_error;
        }

        public static void Set(this byte[] bytes, int offset, long value, bool littleEndian = true)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(long))
                throw new ArgumentOutOfRangeException(nameof(offset));

            if defined(CPLUSPLUS)
            @{
                if (!$3)
                    @{ReverseBytes(IntPtr, ulong):Call(&$2, sizeof($2))};
                memcpy((uint8_t*)$0->_ptr + $1, &$2, sizeof($2));
            @}
            else if defined(DOTNET)
            {
                bytes.SetBytes(offset, System.BitConverter.GetBytes(value), littleEndian);
            }
            else
                build_error;
        }

        public static ulong GetULong(this byte[] bytes, int offset, bool littleEndian = true)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(ulong))
                throw new ArgumentOutOfRangeException(nameof(offset));

            if defined(CPLUSPLUS)
            @{
                @{ulong} result;
                memcpy(&result, (uint8_t*)$0->_ptr + $1, sizeof(result));
                if (!$2)
                    @{ReverseBytes(IntPtr, ulong):Call(&result, sizeof(result))};
                return result;
            @}
            else if defined(DOTNET)
            {
                return System.BitConverter.ToUInt64(bytes.GetBytes(offset, 8, littleEndian), 0);
            }
            else
                build_error;
        }

        public static void Set(this byte[] bytes, int offset, ulong value, bool littleEndian = true)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(ulong))
                throw new ArgumentOutOfRangeException(nameof(offset));

            if defined(CPLUSPLUS)
            @{
                if (!$3)
                    @{ReverseBytes(IntPtr, ulong):Call(&$2, sizeof($2))};
                memcpy((uint8_t*)$0->_ptr + $1, &$2, sizeof($2));
            @}
            else if defined(DOTNET)
            {
                bytes.SetBytes(offset, System.BitConverter.GetBytes(value), littleEndian);
            }
            else
                build_error;
        }

        public static float GetFloat(this byte[] bytes, int offset, bool littleEndian = true)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(float))
                throw new ArgumentOutOfRangeException(nameof(offset));

            if defined(CPLUSPLUS)
            @{
                @{float} result;
                memcpy(&result, (uint8_t*)$0->_ptr + $1, sizeof(result));
                if (!$2)
                    @{ReverseBytes(IntPtr, ulong):Call(&result, sizeof(result))};
                return result;
            @}
            else if defined(DOTNET)
            {
                return System.BitConverter.ToSingle(bytes.GetBytes(offset, 4, littleEndian), 0);
            }
            else
                build_error;
        }

        public static void Set(this byte[] bytes, int offset, float value, bool littleEndian = true)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(float))
                throw new ArgumentOutOfRangeException(nameof(offset));

            if defined(CPLUSPLUS)
            @{
                if (!$3)
                    @{ReverseBytes(IntPtr, ulong):Call(&$2, sizeof($2))};
                memcpy((uint8_t*)$0->_ptr + $1, &$2, sizeof($2));
            @}
            else if defined(DOTNET)
            {
                bytes.SetBytes(offset, System.BitConverter.GetBytes(value), littleEndian);
            }
            else
                build_error;
        }

        public static double GetDouble(this byte[] bytes, int offset, bool littleEndian = true)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(double))
                throw new ArgumentOutOfRangeException(nameof(offset));

            if defined(CPLUSPLUS)
            @{
                @{double} result;
                memcpy(&result, (uint8_t*)$0->_ptr + $1, sizeof(result));
                if (!$2)
                    @{ReverseBytes(IntPtr, ulong):Call(&result, sizeof(result))};
                return result;
            @}
            else if defined(DOTNET)
            {
                return System.BitConverter.ToDouble(bytes.GetBytes(offset, 8, littleEndian), 0);
            }
            else
                build_error;
        }

        public static void Set(this byte[] bytes, int offset, double value, bool littleEndian = true)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(double))
                throw new ArgumentOutOfRangeException(nameof(offset));

            if defined(CPLUSPLUS)
            @{
                if (!$3)
                    @{ReverseBytes(IntPtr, ulong):Call(&$2, sizeof($2))};
                memcpy((uint8_t*)$0->_ptr + $1, &$2, sizeof($2));
            @}
            else if defined(DOTNET)
            {
                bytes.SetBytes(offset, System.BitConverter.GetBytes(value), littleEndian);
            }
            else
                build_error;
        }

        // Vector types

        public static sbyte2 GetSByte2(this byte[] bytes, int offset)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(sbyte2))
                throw new ArgumentOutOfRangeException(nameof(offset));

            return sbyte2(
                (sbyte) bytes[offset    ], 
                (sbyte) bytes[offset + 1]);
        }

        public static void Set(this byte[] bytes, int offset, sbyte2 value)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(sbyte2))
                throw new ArgumentOutOfRangeException(nameof(offset));

            bytes[offset    ] = (byte) value.X;
            bytes[offset + 1] = (byte) value.Y;
        }

        public static sbyte4 GetSByte4(this byte[] bytes, int offset)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(sbyte4))
                throw new ArgumentOutOfRangeException(nameof(offset));

            return sbyte4(
                (sbyte) bytes[offset    ], 
                (sbyte) bytes[offset + 1], 
                (sbyte) bytes[offset + 2], 
                (sbyte) bytes[offset + 3]);
        }

        public static void Set(this byte[] bytes, int offset, sbyte4 value)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(sbyte4))
                throw new ArgumentOutOfRangeException(nameof(offset));

            bytes[offset    ] = (byte) value.X;
            bytes[offset + 1] = (byte) value.Y;
            bytes[offset + 2] = (byte) value.Z;
            bytes[offset + 3] = (byte) value.W;
        }

        public static byte2 GetByte2(this byte[] bytes, int offset)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(byte2))
                throw new ArgumentOutOfRangeException(nameof(offset));

            return byte2(bytes[offset], bytes[offset + 1]);
        }

        public static void Set(this byte[] bytes, int offset, byte2 value)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(byte2))
                throw new ArgumentOutOfRangeException(nameof(offset));

            bytes[offset    ] = value.X;
            bytes[offset + 1] = value.Y;
        }

        public static byte4 GetByte4(this byte[] bytes, int offset)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(byte4))
                throw new ArgumentOutOfRangeException(nameof(offset));

            return byte4(bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3]);
        }

        public static void Set(this byte[] bytes, int offset, byte4 value)
        {
            if (offset < 0 || bytes.Length < offset + sizeof(byte4))
                throw new ArgumentOutOfRangeException(nameof(offset));

            bytes[offset    ] = value.X;
            bytes[offset + 1] = value.Y;
            bytes[offset + 2] = value.Z;
            bytes[offset + 3] = value.W;
        }

        public static short2 GetShort2(this byte[] bytes, int offset, bool littleEndian = true)
        {
            return short2(bytes.GetShort(offset, littleEndian), bytes.GetShort(offset + 2, littleEndian));
        }

        public static void Set(this byte[] bytes, int offset, short2 value, bool littleEndian = true)
        {
            bytes.Set(offset    , value.X, littleEndian);
            bytes.Set(offset + 2, value.Y, littleEndian);
        }

        public static short4 GetShort4(this byte[] bytes, int offset, bool littleEndian = true)
        {
            return short4(
                bytes.GetShort(offset    , littleEndian), 
                bytes.GetShort(offset + 2, littleEndian), 
                bytes.GetShort(offset + 4, littleEndian), 
                bytes.GetShort(offset + 6, littleEndian));
        }

        public static void Set(this byte[] bytes, int offset, short4 value, bool littleEndian = true)
        {
            bytes.Set(offset    , value.X, littleEndian);
            bytes.Set(offset + 2, value.Y, littleEndian);
            bytes.Set(offset + 4, value.Z, littleEndian);
            bytes.Set(offset + 6, value.W, littleEndian);
        }

        public static ushort2 GetUShort2(this byte[] bytes, int offset, bool littleEndian = true)
        {
            return ushort2(bytes.GetUShort(offset    , littleEndian), bytes.GetUShort(offset + 2, littleEndian));
        }

        public static void Set(this byte[] bytes, int offset, ushort2 value, bool littleEndian = true)
        {
            bytes.Set(offset    , value.X, littleEndian);
            bytes.Set(offset + 2, value.Y, littleEndian);
        }

        public static ushort4 GetUShort4(this byte[] bytes, int offset, bool littleEndian = true)
        {
            return ushort4(
                bytes.GetUShort(offset    , littleEndian), 
                bytes.GetUShort(offset + 2, littleEndian), 
                bytes.GetUShort(offset + 4, littleEndian), 
                bytes.GetUShort(offset + 6, littleEndian));
        }

        public static void Set(this byte[] bytes, int offset, ushort4 value, bool littleEndian = true)
        {
            bytes.Set(offset    , value.X, littleEndian);
            bytes.Set(offset + 2, value.Y, littleEndian);
            bytes.Set(offset + 4, value.Z, littleEndian);
            bytes.Set(offset + 6, value.W, littleEndian);
        }

        public static int2 GetInt2(this byte[] bytes, int offset, bool littleEndian = true)
        {
            return int2(bytes.GetInt(offset    , littleEndian), bytes.GetInt(offset + 4, littleEndian));
        }

        public static void Set(this byte[] bytes, int offset, int2 value, bool littleEndian = true)
        {
            bytes.Set(offset    , value.X, littleEndian);
            bytes.Set(offset + 4, value.Y, littleEndian);
        }

        public static int3 GetInt3(this byte[] bytes, int offset, bool littleEndian = true)
        {
            return int3(
                bytes.GetInt(offset    , littleEndian), 
                bytes.GetInt(offset + 4, littleEndian), 
                bytes.GetInt(offset + 8, littleEndian));
        }

        public static void Set(this byte[] bytes, int offset, int3 value, bool littleEndian = true)
        {
            bytes.Set(offset    , value.X, littleEndian);
            bytes.Set(offset + 4, value.Y, littleEndian);
            bytes.Set(offset + 8, value.Z, littleEndian);
        }

        public static int4 GetInt4(this byte[] bytes, int offset, bool littleEndian = true)
        {
            return int4(
                bytes.GetInt(offset     , littleEndian), 
                bytes.GetInt(offset +  4, littleEndian), 
                bytes.GetInt(offset +  8, littleEndian), 
                bytes.GetInt(offset + 12, littleEndian));
        }

        public static void Set(this byte[] bytes, int offset, int4 value, bool littleEndian = true)
        {
            bytes.Set(offset     , value.X, littleEndian);
            bytes.Set(offset +  4, value.Y, littleEndian);
            bytes.Set(offset +  8, value.Z, littleEndian);
            bytes.Set(offset + 12, value.W, littleEndian);
        }

        public static float2 GetFloat2(this byte[] bytes, int offset, bool littleEndian = true)
        {
            return float2(bytes.GetFloat(offset    , littleEndian), bytes.GetFloat(offset + 4, littleEndian));
        }

        public static void Set(this byte[] bytes, int offset, float2 value, bool littleEndian = true)
        {
            bytes.Set(offset    , value.X, littleEndian);
            bytes.Set(offset + 4, value.Y, littleEndian);
        }

        public static float3 GetFloat3(this byte[] bytes, int offset, bool littleEndian = true)
        {
            return float3(
                bytes.GetFloat(offset    , littleEndian), 
                bytes.GetFloat(offset + 4, littleEndian), 
                bytes.GetFloat(offset + 8, littleEndian));
        }

        public static void Set(this byte[] bytes, int offset, float3 value, bool littleEndian = true)
        {
            bytes.Set(offset    , value.X, littleEndian);
            bytes.Set(offset + 4, value.Y, littleEndian);
            bytes.Set(offset + 8, value.Z, littleEndian);
        }

        public static float4 GetFloat4(this byte[] bytes, int offset, bool littleEndian = true)
        {
            return float4(
                bytes.GetFloat(offset     , littleEndian), 
                bytes.GetFloat(offset +  4, littleEndian), 
                bytes.GetFloat(offset +  8, littleEndian), 
                bytes.GetFloat(offset + 12, littleEndian));
        }

        public static void Set(this byte[] bytes, int offset, float4 value, bool littleEndian = true)
        {
            bytes.Set(offset     , value.X, littleEndian);
            bytes.Set(offset +  4, value.Y, littleEndian);
            bytes.Set(offset +  8, value.Z, littleEndian);
            bytes.Set(offset + 12, value.W, littleEndian);
        }

        // Matrix types

        public static float3x3 GetFloat3x3(this byte[] bytes, int offset, bool littleEndian = true)
        {
            return float3x3(
                bytes.GetFloat(offset     , littleEndian), bytes.GetFloat(offset +  4, littleEndian), bytes.GetFloat(offset +  8, littleEndian),
                bytes.GetFloat(offset + 12, littleEndian), bytes.GetFloat(offset + 16, littleEndian), bytes.GetFloat(offset + 20, littleEndian),
                bytes.GetFloat(offset + 24, littleEndian), bytes.GetFloat(offset + 28, littleEndian), bytes.GetFloat(offset + 32, littleEndian));
        }

        public static void Set(this byte[] bytes, int offset, float3x3 value, bool littleEndian = true)
        {
            bytes.Set(offset     , value.M11, littleEndian);
            bytes.Set(offset +  4, value.M12, littleEndian);
            bytes.Set(offset +  8, value.M13, littleEndian);
            bytes.Set(offset + 12, value.M21, littleEndian);
            bytes.Set(offset + 16, value.M22, littleEndian);
            bytes.Set(offset + 20, value.M23, littleEndian);
            bytes.Set(offset + 24, value.M31, littleEndian);
            bytes.Set(offset + 28, value.M32, littleEndian);
            bytes.Set(offset + 32, value.M33, littleEndian);
        }

        public static float4x4 GetFloat4x4(this byte[] bytes, int offset, bool littleEndian = true)
        {
            return float4x4(
                bytes.GetFloat(offset     , littleEndian), bytes.GetFloat(offset +  4, littleEndian), bytes.GetFloat(offset +  8, littleEndian), bytes.GetFloat(offset + 12, littleEndian),
                bytes.GetFloat(offset + 16, littleEndian), bytes.GetFloat(offset + 20, littleEndian), bytes.GetFloat(offset + 24, littleEndian), bytes.GetFloat(offset + 28, littleEndian),
                bytes.GetFloat(offset + 32, littleEndian), bytes.GetFloat(offset + 36, littleEndian), bytes.GetFloat(offset + 40, littleEndian), bytes.GetFloat(offset + 44, littleEndian),
                bytes.GetFloat(offset + 48, littleEndian), bytes.GetFloat(offset + 52, littleEndian), bytes.GetFloat(offset + 56, littleEndian), bytes.GetFloat(offset + 60, littleEndian));
        }

        public static void Set(this byte[] bytes, int offset, float4x4 value, bool littleEndian = true)
        {
            bytes.Set(offset     , value.M11, littleEndian);
            bytes.Set(offset +  4, value.M12, littleEndian);
            bytes.Set(offset +  8, value.M13, littleEndian);
            bytes.Set(offset + 12, value.M14, littleEndian);
            bytes.Set(offset + 16, value.M21, littleEndian);
            bytes.Set(offset + 20, value.M22, littleEndian);
            bytes.Set(offset + 24, value.M23, littleEndian);
            bytes.Set(offset + 28, value.M24, littleEndian);
            bytes.Set(offset + 32, value.M31, littleEndian);
            bytes.Set(offset + 36, value.M32, littleEndian);
            bytes.Set(offset + 40, value.M33, littleEndian);
            bytes.Set(offset + 44, value.M34, littleEndian);
            bytes.Set(offset + 48, value.M41, littleEndian);
            bytes.Set(offset + 52, value.M42, littleEndian);
            bytes.Set(offset + 56, value.M43, littleEndian);
            bytes.Set(offset + 60, value.M44, littleEndian);
        }

        // .NET utilities

        extern(DOTNET)
        static byte[] GetBytes(this byte[] bytes, int offset, int count, bool littleEndian = true)
        {
            var result = new byte[count];
            Array.Copy((Array) bytes, offset, result, 0, count);

            if (!littleEndian)
                Array.Reverse(result);

            return result;
        }

        extern(DOTNET)
        static void SetBytes(this byte[] bytes, int offset, byte[] value, bool littleEndian = true)
        {
            Array.Copy((Array) value, 0, bytes, offset, value.Length);

            if (!littleEndian)
                Array.Reverse(bytes, offset, value.Length);
        }

        // C++ utilities

        extern(CPLUSPLUS)
        static void ReverseBytes(IntPtr ptr, ulong size)
        @{
            uint64_t tmp;
            uint8_t* dst = (uint8_t*)$0;
            uint8_t* src = (uint8_t*)&tmp;

            switch ($1)
            {
            case 2:
                memcpy(src, dst, 2);
                dst[0] = src[1];
                dst[1] = src[0];
                break;
            case 4:
                memcpy(src, dst, 4);
                dst[0] = src[3];
                dst[1] = src[2];
                dst[2] = src[1];
                dst[3] = src[0];
                break;
            case 8:
                memcpy(src, dst, 8);
                dst[0] = src[7];
                dst[1] = src[6];
                dst[2] = src[5];
                dst[3] = src[4];
                dst[4] = src[3];
                dst[5] = src[2];
                dst[6] = src[1];
                dst[7] = src[0];
                break;
            default:
                U_FATAL();
            }
        @}
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
