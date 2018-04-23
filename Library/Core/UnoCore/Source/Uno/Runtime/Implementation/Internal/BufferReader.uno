using Uno.Text;

namespace Uno.Runtime.Implementation.Internal
{
    [Obsolete]
    public class BufferReader
    {
        Buffer _buffer;
        int _position;

        public BufferReader(Buffer buffer)
        {
            _buffer = buffer;
        }

        public int Length
        {
            get { return _buffer.SizeInBytes; }
        }

        public int Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public byte ReadByte()
        {
            _position += 1;
            return _buffer.GetByte(_position - 1);
        }

        public byte2 ReadByte2()
        {
            _position += 2;
            return _buffer.GetByte2(_position - 2);
        }

        public byte4 ReadByte4()
        {
            _position += 4;
            return _buffer.GetByte4(_position - 4);
        }

        public double ReadDouble(bool littleEndian = true)
        {
            _position += 8;
            return _buffer.GetDouble(_position - 8, littleEndian);
        }

        public float ReadFloat(bool littleEndian = true)
        {
            _position += 4;
            return _buffer.GetFloat(_position - 4, littleEndian);
        }

        public float2 ReadFloat2(bool littleEndian = true)
        {
            _position += 8;
            return _buffer.GetFloat2(_position - 8, littleEndian);
        }

        public float3 ReadFloat3(bool littleEndian = true)
        {
            _position += 12;
            return _buffer.GetFloat3(_position - 12, littleEndian);
        }

        public float4 ReadFloat4(bool littleEndian = true)
        {
            _position += 16;
            return _buffer.GetFloat4(_position - 16, littleEndian);
        }

        public float3x3 ReadFloat3x3(bool littleEndian = true)
        {
            _position += 36;
            return _buffer.GetFloat3x3(_position - 36, littleEndian);
        }

        public float4x4 ReadFloat4x4(bool littleEndian = true)
        {
            _position += 64;
            return _buffer.GetFloat4x4(_position - 64, littleEndian);
        }

        public int ReadInt(bool littleEndian = true)
        {
            _position += 4;
            return _buffer.GetInt(_position - 4, littleEndian);
        }

        public int2 ReadInt2(bool littleEndian = true)
        {
            _position += 8;
            return _buffer.GetInt2(_position - 8, littleEndian);
        }

        public int3 ReadInt3(bool littleEndian = true)
        {
            _position += 12;
            return _buffer.GetInt3(_position - 12, littleEndian);
        }

        public int4 ReadInt4(bool littleEndian = true)
        {
            _position += 16;
            return _buffer.GetInt4(_position - 16, littleEndian);
        }

        public sbyte ReadSByte()
        {
            _position += 1;
            return _buffer.GetSByte(_position - 1);
        }

        public sbyte2 ReadSByte2()
        {
            _position += 2;
            return _buffer.GetSByte2(_position - 2);
        }

        public sbyte4 ReadSByte4()
        {
            _position += 4;
            return _buffer.GetSByte4(_position - 4);
        }

        public short ReadShort(bool littleEndian = true)
        {
            _position += 2;
            return _buffer.GetShort(_position - 2, littleEndian);
        }

        public short2 ReadShort2(bool littleEndian = true)
        {
            _position += 4;
            return _buffer.GetShort2(_position - 4, littleEndian);
        }

        public short4 ReadShort4(bool littleEndian = true)
        {
            _position += 8;
            return _buffer.GetShort4(_position - 8, littleEndian);
        }

        public uint ReadUInt(bool littleEndian = true)
        {
            _position += 4;
            return _buffer.GetUInt(_position - 4, littleEndian);
        }

        public ushort ReadUShort(bool littleEndian = true)
        {
            _position += 2;
            return _buffer.GetUShort(_position - 2, littleEndian);
        }

        public ushort2 ReadUShort2(bool littleEndian = true)
        {
            _position += 4;
            return _buffer.GetUShort2(_position - 4, littleEndian);
        }

        public ushort4 ReadUShort4(bool littleEndian = true)
        {
            _position += 8;
            return _buffer.GetUShort4(_position - 8, littleEndian);
        }

        public byte[] ReadBytes(int count)
        {
            var result = new byte[count];

            for (int i = 0; i < count; i++)
                result[i] = ReadByte();

            return result;
        }

        public int ReadCompressedInt()
        {
            int count = 0;
            int shift = 0;

            while (shift != 35)
            {
                var b = this.ReadByte();
                count |= (int)(b & 127) << shift;
                shift += 7;

                if ((b & 128) == 0)
                  return count;
            }

            throw new FormatException("Invalid 7 bit encoded int");
        }

        public string ReadString()
        {
            return Utf8.GetString(ReadBytes(ReadCompressedInt()));
        }

        public static float ReadFloat(BufferReader r)
        {
            return r.ReadFloat();
        }

        public static float2 ReadFloat2(BufferReader r)
        {
            return r.ReadFloat2();
        }

        public static float3 ReadFloat3(BufferReader r)
        {
            return r.ReadFloat3();
        }

        public static float4 ReadFloat4(BufferReader r)
        {
            return r.ReadFloat4();
        }

        public static float4x4 ReadFloat4x4(BufferReader r)
        {
            return r.ReadFloat4x4();
        }

        public static byte ReadByte(BufferReader r)
        {
            return r.ReadByte();
        }

        public static byte4 ReadByte4(BufferReader r)
        {
            return r.ReadByte4();
        }

        public static sbyte ReadSByte(BufferReader r)
        {
            return r.ReadSByte();
        }

        public static sbyte4 ReadSByte4(BufferReader r)
        {
            return r.ReadSByte4();
        }

        public static short ReadShort(BufferReader r)
        {
            return r.ReadShort();
        }

        public static short2 ReadShort2(BufferReader r)
        {
            return r.ReadShort2();
        }

        public static short4 ReadShort4(BufferReader r)
        {
            return r.ReadShort4();
        }

        public static ushort ReadUShort(BufferReader r)
        {
            return r.ReadUShort();
        }

        public static ushort2 ReadUShort2(BufferReader r)
        {
            return r.ReadUShort2();
        }

        public static ushort4 ReadUShort4(BufferReader r)
        {
            return r.ReadUShort4();
        }

        public static int ReadInt(BufferReader r)
        {
            return r.ReadInt();
        }

        public static int2 ReadInt2(BufferReader r)
        {
            return r.ReadInt2();
        }

        public static int3 ReadInt3(BufferReader r)
        {
            return r.ReadInt3();
        }

        public static int4 ReadInt4(BufferReader r)
        {
            return r.ReadInt4();
        }

        public static uint ReadUInt(BufferReader r)
        {
            return r.ReadUInt();
        }

        [Obsolete]
        public static T[] ReadArray<T>(BufferReader r, int count, Func<BufferReader, T> readElement)
        {
            var result = new T[count];

            for (int i = 0; i < count; i++)
                result[i] = readElement(r);

            return result;
        }
    }
}
