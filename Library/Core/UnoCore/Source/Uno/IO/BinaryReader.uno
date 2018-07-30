using Uno.Collections;
using Uno.Text;

namespace Uno.IO
{
    public class BinaryReader : IDisposable
    {
        Stream _stream;
        byte[] _buffer;

        const int BufferSize = 64;

        public BinaryReader(Stream stream)
        {
            _stream = stream;
            _buffer = new byte[BufferSize];
            LittleEndian = true;
        }

        public Stream BaseStream
        {
            get { return _stream; }
        }

        public bool LittleEndian
        {
            get;
            set;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        void FillBuffer(int byteCount)
        {
            if (byteCount < 0 || byteCount > BufferSize)
                throw new ArgumentOutOfRangeException(nameof(byteCount));

            var offset = 0;

            do
            {
                var read = _stream.Read(_buffer, offset, byteCount - offset);

                if (read == 0)
                    throw new EndOfStreamException();

                offset += read;
            }
            while (offset < byteCount);
        }

        public byte[] ReadBytes(int byteCount)
        {
            if (byteCount < 0)
                throw new ArgumentOutOfRangeException(nameof(byteCount));

            var buffer = new byte[byteCount];
            var offset = 0;

            do
            {
                var read = _stream.Read(buffer, offset, byteCount - offset);
                offset += read;

                if (read == 0)
                {
                    break;
                }
            }
            while (offset < byteCount);

            if (offset == byteCount)
            {
                return buffer;
            }

            var result = new byte[offset];
            for (int i = 0; i < offset; i++)
            {
                result[i] = buffer[i];
            }

            return result;
        }

        public bool ReadBoolean()
        {
            FillBuffer(1);
            return _buffer[0] != 0;
        }

        public sbyte ReadSByte()
        {
            FillBuffer(1);
            return (sbyte)_buffer[0];
        }

        public byte ReadByte()
        {
            FillBuffer(1);
            return _buffer[0];
        }

        public char ReadChar()
        {
            FillBuffer(1);
            var buffer = new List<byte>();
            buffer.Add(_buffer[0]);
            if ((buffer[0] & 128) != 0)
            {
                FillBuffer(1);
                buffer.Add(_buffer[0]);
            }
            return Utf8.GetString(buffer.ToArray())[0];
        }

        public short ReadShort()
        {
            FillBuffer(2);
            return _buffer.GetShort(0, LittleEndian);
        }

        public ushort ReadUShort()
        {
            FillBuffer(2);
            return _buffer.GetUShort(0, LittleEndian);
        }

        public int ReadInt()
        {
            FillBuffer(4);
            return _buffer.GetInt(0, LittleEndian);
        }

        public uint ReadUInt()
        {
            FillBuffer(4);
            return _buffer.GetUInt(0, LittleEndian);
        }

        public long ReadLong()
        {
            FillBuffer(8);
            return _buffer.GetLong(0, LittleEndian);
        }

        public ulong ReadULong()
        {
            FillBuffer(8);
            return _buffer.GetULong(0, LittleEndian);
        }

        public float ReadFloat()
        {
            FillBuffer(4);
            return _buffer.GetFloat(0, LittleEndian);
        }

        public double ReadDouble()
        {
            FillBuffer(8);
            return _buffer.GetDouble(0, LittleEndian);
        }

        public sbyte2 ReadSByte2()
        {
            FillBuffer(2);
            return sbyte2(
                (sbyte)_buffer[0],
                (sbyte)_buffer[1]);
        }

        public sbyte4 ReadSByte4()
        {
            FillBuffer(4);
            return sbyte4(
                (sbyte)_buffer[0],
                (sbyte)_buffer[1],
                (sbyte)_buffer[2],
                (sbyte)_buffer[3]);
        }

        public byte2 ReadByte2()
        {
            FillBuffer(2);
            return byte2(
                _buffer[0],
                _buffer[1]);
        }

        public byte4 ReadByte4()
        {
            FillBuffer(4);
            return byte4(
                _buffer[0],
                _buffer[1],
                _buffer[2],
                _buffer[3]);
        }

        public short2 ReadShort2()
        {
            FillBuffer(4);
            return short2(
                _buffer.GetShort(0, LittleEndian),
                _buffer.GetShort(2, LittleEndian));
        }

        public short4 ReadShort4()
        {
            FillBuffer(8);
            return short4(
                _buffer.GetShort(0, LittleEndian),
                _buffer.GetShort(2, LittleEndian),
                _buffer.GetShort(4, LittleEndian),
                _buffer.GetShort(6, LittleEndian));
        }

        public ushort2 ReadUShort2()
        {
            FillBuffer(4);
            return ushort2(
                _buffer.GetUShort(0, LittleEndian),
                _buffer.GetUShort(2, LittleEndian));
        }

        public ushort4 ReadUShort4()
        {
            FillBuffer(8);
            return ushort4(
                _buffer.GetUShort(0, LittleEndian),
                _buffer.GetUShort(2, LittleEndian),
                _buffer.GetUShort(4, LittleEndian),
                _buffer.GetUShort(6, LittleEndian));
        }

        public int2 ReadInt2()
        {
            FillBuffer(8);
            return int2(
                _buffer.GetInt(0, LittleEndian),
                _buffer.GetInt(4, LittleEndian));
        }

        public int3 ReadInt3()
        {
            FillBuffer(12);
            return int3(
                _buffer.GetInt(00, LittleEndian),
                _buffer.GetInt(04, LittleEndian),
                _buffer.GetInt(08, LittleEndian));
        }

        public int4 ReadInt4()
        {
            FillBuffer(16);
            return int4(
                _buffer.GetInt(00, LittleEndian),
                _buffer.GetInt(04, LittleEndian),
                _buffer.GetInt(08, LittleEndian),
                _buffer.GetInt(12, LittleEndian));
        }

        public float2 ReadFloat2()
        {
            FillBuffer(8);
            return float2(
                _buffer.GetFloat(0, LittleEndian),
                _buffer.GetFloat(4, LittleEndian));
        }

        public float3 ReadFloat3()
        {
            FillBuffer(12);
            return float3(
                _buffer.GetFloat(0, LittleEndian),
                _buffer.GetFloat(4, LittleEndian),
                _buffer.GetFloat(8, LittleEndian));
        }

        public float4 ReadFloat4()
        {
            FillBuffer(16);
            return float4(
                _buffer.GetFloat(00, LittleEndian),
                _buffer.GetFloat(04, LittleEndian),
                _buffer.GetFloat(08, LittleEndian),
                _buffer.GetFloat(12, LittleEndian));
        }

        public float3x3 ReadFloat3x3()
        {
            FillBuffer(36);
            return float3x3(
                _buffer.GetFloat(00, LittleEndian),
                _buffer.GetFloat(04, LittleEndian),
                _buffer.GetFloat(08, LittleEndian),
                _buffer.GetFloat(12, LittleEndian),
                _buffer.GetFloat(16, LittleEndian),
                _buffer.GetFloat(20, LittleEndian),
                _buffer.GetFloat(24, LittleEndian),
                _buffer.GetFloat(28, LittleEndian),
                _buffer.GetFloat(32, LittleEndian));
        }

        public float4x4 ReadFloat4x4()
        {
            FillBuffer(64);
            return float4x4(
                _buffer.GetFloat(00, LittleEndian),
                _buffer.GetFloat(04, LittleEndian),
                _buffer.GetFloat(08, LittleEndian),
                _buffer.GetFloat(12, LittleEndian),
                _buffer.GetFloat(16, LittleEndian),
                _buffer.GetFloat(20, LittleEndian),
                _buffer.GetFloat(24, LittleEndian),
                _buffer.GetFloat(28, LittleEndian),
                _buffer.GetFloat(32, LittleEndian),
                _buffer.GetFloat(36, LittleEndian),
                _buffer.GetFloat(40, LittleEndian),
                _buffer.GetFloat(44, LittleEndian),
                _buffer.GetFloat(48, LittleEndian),
                _buffer.GetFloat(52, LittleEndian),
                _buffer.GetFloat(56, LittleEndian),
                _buffer.GetFloat(60, LittleEndian));
        }

        protected internal int Read7BitEncodedInt()
        {
            int count = 0;
            int shift = 0;

            while (shift != 35)
            {
                var b = ReadByte();
                count |= (int)(b & 127) << shift;
                shift += 7;

                if ((b & 128) == 0)
                    return count;
            }

            throw new FormatException("Invalid 7 bit encoded int");
        }

        public string ReadString()
        {
            return Utf8.GetString(ReadBytes(Read7BitEncodedInt()));
        }
    }
}
