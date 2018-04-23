using Uno.Runtime.Implementation;
using Uno.Text;

namespace Uno.IO
{
    public class BinaryWriter : IDisposable
    {
        Stream _stream;
        byte[] _buffer;

        const int BufferSize = 64;

        public BinaryWriter(Stream stream)
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

        public void Write(bool value)
        {
            _buffer[0] = (byte)(value ? 1 : 0);
            _stream.Write(_buffer, 0, 1);
        }

        public void Write(byte[] value)
        {
            _stream.Write(value, 0, value.Length);
        }

        public void Write(sbyte value)
        {
            _buffer[0] = (byte)value;
            _stream.Write(_buffer, 0, 1);
        }

        public void Write(byte value)
        {
            _buffer[0] = value;
            _stream.Write(_buffer, 0, 1);
        }

        public void Write(char value)
        {
            Write(Utf8.GetBytes(value.ToString()));
        }

        public void Write(short value)
        {
            BufferImpl.SetShort(_buffer, 0, value, LittleEndian);
            _stream.Write(_buffer, 0, 2);
        }

        public void Write(ushort value)
        {
            BufferImpl.SetUShort(_buffer, 0, value, LittleEndian);
            _stream.Write(_buffer, 0, 2);
        }

        public void Write(int value)
        {
            BufferImpl.SetInt(_buffer, 0, value, LittleEndian);
            _stream.Write(_buffer, 0, 4);
        }

        public void Write(uint value)
        {
            BufferImpl.SetUInt(_buffer, 0, value, LittleEndian);
            _stream.Write(_buffer, 0, 4);
        }

        public void Write(long value)
        {
            BufferImpl.SetLong(_buffer, 0, value, LittleEndian);
            _stream.Write(_buffer, 0, 8);
        }

        public void Write(ulong value)
        {
            BufferImpl.SetULong(_buffer, 0, value, LittleEndian);
            _stream.Write(_buffer, 0, 8);
        }

        public void Write(float value)
        {
            BufferImpl.SetFloat(_buffer, 0, value, LittleEndian);
            _stream.Write(_buffer, 0, 4);
        }

        public void Write(double value)
        {
            BufferImpl.SetDouble(_buffer, 0, value, LittleEndian);
            _stream.Write(_buffer, 0, 8);
        }

        public void Write(sbyte2 value)
        {
            for (int i = 0; i < 2; ++i)
                _buffer[i] = (byte)value[i];
            _stream.Write(_buffer, 0, 2);
        }

        public void Write(sbyte4 value)
        {
            for (int i = 0; i < 4; ++i)
                _buffer[i] = (byte)value[i];
            _stream.Write(_buffer, 0, 4);
        }

        public void Write(byte2 value)
        {
            for (int i = 0; i < 2; ++i)
                _buffer[i] = value[i];
            _stream.Write(_buffer, 0, 2);
        }

        public void Write(byte4 value)
        {
            for (int i = 0; i < 4; ++i)
                _buffer[i] = value[i];
            _stream.Write(_buffer, 0, 4);
        }

        public void Write(short2 value)
        {
            for (int i = 0; i < 2; ++i)
                BufferImpl.SetShort(_buffer, i*2, value[i], LittleEndian);
            _stream.Write(_buffer, 0, 4);
        }

        public void Write(short4 value)
        {
            for (int i = 0; i < 4; ++i)
                BufferImpl.SetShort(_buffer, i*2, value[i], LittleEndian);
            _stream.Write(_buffer, 0, 8);
        }

        public void Write(ushort2 value)
        {
            for (int i = 0; i < 2; ++i)
                BufferImpl.SetUShort(_buffer, i*2, value[i], LittleEndian);
            _stream.Write(_buffer, 0, 4);
        }

        public void Write(ushort4 value)
        {
            for (int i = 0; i < 4; ++i)
                BufferImpl.SetUShort(_buffer, i*2, value[i], LittleEndian);
            _stream.Write(_buffer, 0, 8);
        }

        public void Write(int2 value)
        {
            for (int i = 0; i < 2; ++i)
                BufferImpl.SetInt(_buffer, i*4, value[i], LittleEndian);
            _stream.Write(_buffer, 0, 8);
        }

        public void Write(int3 value)
        {
            for (int i = 0; i < 3; ++i)
                BufferImpl.SetInt(_buffer, i*4, value[i], LittleEndian);
            _stream.Write(_buffer, 0, 12);
        }

        public void Write(int4 value)
        {
            for (int i = 0; i < 4; ++i)
                BufferImpl.SetInt(_buffer, i*4, value[i], LittleEndian);
            _stream.Write(_buffer, 0, 16);
        }

        public void Write(float2 value)
        {
            for (int i = 0; i < 2; ++i)
                BufferImpl.SetFloat(_buffer, i*4, value[i], LittleEndian);
            _stream.Write(_buffer, 0, 8);
        }

        public void Write(float3 value)
        {
            for (int i = 0; i < 3; ++i)
                BufferImpl.SetFloat(_buffer, i*4, value[i], LittleEndian);
            _stream.Write(_buffer, 0, 12);
        }

        public void Write(float4 value)
        {
            for (int i = 0; i < 4; ++i)
                BufferImpl.SetFloat(_buffer, i*4, value[i], LittleEndian);
            _stream.Write(_buffer, 0, 16);
        }

        public void Write(float3x3 value)
        {
            for (int i = 0; i < 3; ++i)
                Write(value[i]);
        }

        public void Write(float4x4 value)
        {
            for (int i = 0; i < 4; ++i)
                Write(value[i]);
        }

        protected internal void Write7BitEncodedInt(int value)
        {
            uint v = (uint)value;
            while (v >= 0x80)
            {
                Write((byte)(v | 0x80));
                v >>= 7;
            }
            Write((byte)v);
        }

        public void Write(string value)
        {
            var bytes = Utf8.GetBytes(value);
            Write7BitEncodedInt(bytes.Length);
            Write(bytes);
        }
    }
}
