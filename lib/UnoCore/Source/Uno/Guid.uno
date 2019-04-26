using Uno.Compiler.ExportTargetInterop;
using Uno.Text;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.Guid")]
    [Require("Source.Include", "sstream")]
    [Require("Source.Include", "iostream")]
    [extern(APPLE) Set("FileExtension", "mm")]
    public struct Guid
    {
        uint Data1;
        ushort Data2;
        ushort Data3;
        byte Data4_1;
        byte Data4_2;
        byte Data4_3;
        byte Data4_4;
        byte Data4_5;
        byte Data4_6;
        byte Data4_7;
        byte Data4_8;

        public static readonly Guid Empty = new Guid();

        //Note: This constructor casts signed integers to unsigned integers
        //For C++ backends, the result of this is implementation defined for negative values
        public Guid(int d1, short d2, short d3, byte d4_1, byte d4_2, byte d4_3, byte d4_4, byte d4_5, byte d4_6, byte d4_7, byte d4_8)
        {
            Data1 = (uint)d1;
            Data2 = (ushort)d2;
            Data3 = (ushort)d3;
            Data4_1 = d4_1;
            Data4_2 = d4_2;
            Data4_3 = d4_3;
            Data4_4 = d4_4;
            Data4_5 = d4_5;
            Data4_6 = d4_6;
            Data4_7 = d4_7;
            Data4_8 = d4_8;
        }

        public Guid(uint d1, ushort d2, ushort d3, byte d4_1, byte d4_2, byte d4_3, byte d4_4, byte d4_5, byte d4_6, byte d4_7, byte d4_8)
        {
            Data1 = d1;
            Data2 = d2;
            Data3 = d3;
            Data4_1 = d4_1;
            Data4_2 = d4_2;
            Data4_3 = d4_3;
            Data4_4 = d4_4;
            Data4_5 = d4_5;
            Data4_6 = d4_6;
            Data4_7 = d4_7;
            Data4_8 = d4_8;
        }

        //Note: This constructor casts signed integers to unsigned integers
        //For C++ backends, the result of this is implementation defined for negative values
        public Guid(int d1, short d2, short d3, byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length != 8)
            {
                throw new ArgumentException("The length of the 'bytes' array for Guid(byte[] bytes) was " + bytes.Length + ", it must be 8.");
            }

            Data1 = (uint)d1;
            Data2 = (ushort)d2;
            Data3 = (ushort)d3;
            Data4_1 = bytes[0];
            Data4_2 = bytes[1];
            Data4_3 = bytes[2];
            Data4_4 = bytes[3];
            Data4_5 = bytes[4];
            Data4_6 = bytes[5];
            Data4_7 = bytes[6];
            Data4_8 = bytes[7];
        }

        public Guid(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length != 16)
            {
                throw new ArgumentException("The length of the 'bytes' array for Guid(byte[] bytes) was " + bytes.Length + ", it must be 16.");
            }

            Data1 = ((uint)bytes[3] << 24 | (uint)bytes[2] << 16 | (uint)bytes[1] << 8 | (uint)bytes[0]);
            Data2 = (ushort)((ushort)bytes[5] << 8 | (ushort)bytes[4]);
            Data3 = (ushort)((ushort)bytes[7] << 8 | (ushort)bytes[6]);
            Data4_1 = bytes[8];
            Data4_2 = bytes[9];
            Data4_3 = bytes[10];
            Data4_4 = bytes[11];
            Data4_5 = bytes[12];
            Data4_6 = bytes[13];
            Data4_7 = bytes[14];
            Data4_8 = bytes[15];
        }

        //Note: This constructor only accepts the format used by ToString()
        //Example: dddddddd-dddd-dddd-dddd-dddddddddddd
        public Guid(string g)
        {
            if defined(CPLUSPLUS)
            {
                if (g == null)
                {
                    throw new ArgumentNullException(nameof(g));
                }
                try
                {
                    var parts = ValidateGuid(g);
                    Data1 = ToUint(parts[0]);
                    Data2 = (ushort)ToUint(parts[1]);
                    Data3 = (ushort)ToUint(parts[2]);
                    Data4_1 = (byte)ToUint(parts[3].Substring(0,2));
                    Data4_2 = (byte)ToUint(parts[3].Substring(2,2));
                    Data4_3 = (byte)ToUint(parts[4].Substring(0,2));
                    Data4_4 = (byte)ToUint(parts[4].Substring(2,2));
                    Data4_5 = (byte)ToUint(parts[4].Substring(4,2));
                    Data4_6 = (byte)ToUint(parts[4].Substring(6,2));
                    Data4_7 = (byte)ToUint(parts[4].Substring(8,2));
                    Data4_8 = (byte)ToUint(parts[4].Substring(10,2));
                } catch (Exception e)
                {
                    throw new FormatException("Unrecognised Guid format, requires format 'dddddddd-dddd-dddd-dddd-dddddddddddd'");
                }
            }
            else
            {
                throw new Exception("Guid.Guid(string) is not supported on this platform");
            }
        }

        [TargetSpecificImplementation]
        extern(DOTNET)
        public static Guid NewGuid();

        [Require("LinkLibrary", "Rpcrt4")]
        [Require("Source.Include", "Uno/WinAPIHelper.h")]
        extern(MSVC)
        public static Guid NewGuid()
        @{
            UUID guid;
            UuidCreate(&guid);
            // relies of msvc's ulong being the same size as uno's int
            // https://msdn.microsoft.com/en-us/library/s3f49ktz.aspx
            return @{Uno.Guid(uint, ushort, ushort, byte, byte, byte, byte, byte, byte, byte, byte):New(
                    (@{uint})guid.Data1,
                    (@{ushort})guid.Data2,
                    (@{ushort})guid.Data3,
                    (@{byte})guid.Data4[0],
                    (@{byte})guid.Data4[1],
                    (@{byte})guid.Data4[2],
                    (@{byte})guid.Data4[3],
                    (@{byte})guid.Data4[4],
                    (@{byte})guid.Data4[5],
                    (@{byte})guid.Data4[6],
                    (@{byte})guid.Data4[7])};
        @}

        [Foreign(Language.Java)]
        extern(Android)
        public static Guid NewGuid()
        @{
            java.util.UUID guid = java.util.UUID.randomUUID();
            java.nio.ByteBuffer buffer = java.nio.ByteBuffer.allocate(16);
            long highSig = guid.getMostSignificantBits();
            buffer.order(java.nio.ByteOrder.LITTLE_ENDIAN);
            buffer.putInt((int)(highSig >> 32));
            buffer.putShort((short)(highSig >> 16));
            buffer.putShort((short)highSig);
            buffer.order(java.nio.ByteOrder.BIG_ENDIAN);
            buffer.putLong(guid.getLeastSignificantBits());
            byte[] result = buffer.array();
            return @{Guid(byte[]):New(new ByteArray(result))};
        @}

        [Require("Source.Include", "Foundation/Foundation.h")]
        extern(APPLE)
        public static Guid NewGuid()
        @{
            NSUUID* guid = [[NSUUID alloc] init];
            unsigned char buf[16];
            [guid getUUIDBytes:buf];
            return @{Uno.Guid(uint, ushort, ushort, byte, byte, byte, byte, byte, byte, byte, byte):New(
                              (@{uint})CFSwapInt32(*(uint32_t*)&buf),
                              (@{ushort})CFSwapInt16(*(uint16_t*)&buf[4]),
                              (@{ushort})CFSwapInt16(*(uint16_t*)&buf[6]),
                              (@{byte})buf[8],
                              (@{byte})buf[9],
                              (@{byte})buf[10],
                              (@{byte})buf[11],
                              (@{byte})buf[12],
                              (@{byte})buf[13],
                              (@{byte})buf[14],
                              (@{byte})buf[15])};
        @}

        [Require("Source.Include", "uuid/uuid.h")]
        [Require("LinkLibrary", "uuid")]
        extern(LINUX)
        public static Guid NewGuid()
        @{
            unsigned char buf[16];
            uuid_generate_time_safe(buf);
            return @{Uno.Guid(uint, ushort, ushort, byte, byte, byte, byte, byte, byte, byte, byte):New(
                              (@{uint})*(uint32_t*)&buf,
                              (@{ushort})*(uint16_t*)&buf[4],
                              (@{ushort})*(uint16_t*)&buf[6],
                              (@{byte})buf[8],
                              (@{byte})buf[9],
                              (@{byte})buf[10],
                              (@{byte})buf[11],
                              (@{byte})buf[12],
                              (@{byte})buf[13],
                              (@{byte})buf[14],
                              (@{byte})buf[15])};
        @}

        string[] ValidateGuid(string guid)
        {
            var parts = guid.Split('-');
            if (parts.Length != 5)
            {
                throw new Exception();
            }
            ValidatePart(parts[0], 8);
            ValidatePart(parts[1], 4);
            ValidatePart(parts[2], 4);
            ValidatePart(parts[3], 4);
            ValidatePart(parts[4], 12);
            return parts;
        }

        void ValidatePart(string part, int expectedLength)
        {
            if (part.Length != expectedLength)
            {
                throw new Exception();
            }
            foreach(var c in part)
            {
                if (!Char.IsDigit(c) && c != 'a' && c != 'b' && c != 'c' && c != 'd' && c != 'e' && c != 'f')
                {
                    throw new Exception();
                }
            }
        }

        public byte[] ToByteArray()
        {
            var arr = new byte[16];
            arr[0] = (byte)Data1;
            arr[1] = (byte)(Data1 >> 8);
            arr[2] = (byte)(Data1 >> 16);
            arr[3] = (byte)(Data1 >> 24);
            arr[4] = (byte)Data2;
            arr[5] = (byte)(Data2 >> 8);
            arr[6] = (byte)Data3;
            arr[7] = (byte)(Data3 >> 8);
            arr[8] = Data4_1;
            arr[9] = Data4_2;
            arr[10] = Data4_3;
            arr[11] = Data4_4;
            arr[12] = Data4_5;
            arr[13] = Data4_6;
            arr[14] = Data4_7;
            arr[15] = Data4_8;
            return arr;
        }

        extern(CPLUSPLUS) uint ToUint(string str)
        @{
            uCString cstr(str);
            unsigned long i;
            std::stringstream ss;
            ss << std::hex << cstr.Ptr;
            ss >> i;
            return i;
        @}

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(String.Format("{0:X8}", Data1));
            sb.Append("-");
            sb.Append(String.Format("{0:X4}", Data2));
            sb.Append("-");
            sb.Append(String.Format("{0:X4}", Data3));
            sb.Append("-");
            sb.Append(String.Format("{0:X2}", Data4_1));
            sb.Append(String.Format("{0:X2}", Data4_2));
            sb.Append("-");
            sb.Append(String.Format("{0:X2}", Data4_3));
            sb.Append(String.Format("{0:X2}", Data4_4));
            sb.Append(String.Format("{0:X2}", Data4_5));
            sb.Append(String.Format("{0:X2}", Data4_6));
            sb.Append(String.Format("{0:X2}", Data4_7));
            sb.Append(String.Format("{0:X2}", Data4_8));
            return sb.ToString().ToLower();
        }

        public override bool Equals(object other)
        {
            if (other == null || !(other is Guid))
            {
                return false;
            }
            return Equals((Guid)other);
        }

        public bool Equals(Guid other)
        {
            return
                Data1 == other.Data1 &&
                Data2 == other.Data2 &&
                Data3 == other.Data3 &&
                Data4_1 == other.Data4_1 &&
                Data4_2 == other.Data4_2 &&
                Data4_3 == other.Data4_3 &&
                Data4_4 == other.Data4_4 &&
                Data4_5 == other.Data4_5 &&
                Data4_6 == other.Data4_6 &&
                Data4_7 == other.Data4_7 &&
                Data4_8 == other.Data4_8 ;
        }

        public override int GetHashCode()
        {
            return (int)Data1 ^
            ((int)Data2 << 16 | (short)Data3) ^
            ((int)Data4_1 << 24 | (int)Data4_2 << 16 | (short)Data4_3 << 8 | Data4_4) ^
            ((int)Data4_5 << 24 | (int)Data4_6 << 16 | (short)Data4_7 << 8 | Data4_8);
        }

        public static bool operator==(Guid g1, Guid g2)
        {
            return g1.Equals(g2);
        }

        public static bool operator!=(Guid g1, Guid g2)
        {
            return !(g1 == g2);
        }
    }
}
