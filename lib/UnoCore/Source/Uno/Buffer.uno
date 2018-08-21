using Uno.Compiler.ExportTargetInterop;
using Uno.Runtime.InteropServices;
using Uno.IO;

namespace Uno
{
    [Obsolete("Use extension methods on Uno.ByteArrayExtensions instead")]
    public sealed class Buffer
    {
        public static Buffer Load(BundleFile file)
        {
            return new Buffer(file.ReadAllBytes());
        }

        public static Buffer Load(string filename)
        {
            return new Buffer(IO.File.ReadAllBytes(filename));
        }

        int _offset;
        int _sizeInBytes;
        byte[] _data;

        internal Buffer(byte[] data, int offset, int sizeInBytes)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (sizeInBytes < 0 || offset + sizeInBytes > data.Length)
                throw new ArgumentOutOfRangeException(nameof(sizeInBytes));

            _data = data;
            _offset = offset;
            _sizeInBytes = sizeInBytes;
        }

        [Obsolete("Buffers can no longer be read-only - use one of the other constructors instead")]
        public Buffer(byte[] data, bool isReadOnly)
            : this(data, 0, data.Length)
        {
        }

        public Buffer(byte[] data)
            : this(data, 0, data.Length)
        {
        }

        public Buffer(int sizeInBytes)
            : this(new byte[sizeInBytes], 0, sizeInBytes)
        {
        }

        public byte[] GetBytes()
        {
            return _data;
        }

        public int ByteOffset
        {
            get { return _offset; }
        }

        public int SizeInBytes
        {
            get { return _sizeInBytes; }
        }

        [Obsolete("Buffers can no longer be read-only - use CreateSubBuffer() instead")]
        public Buffer CreateReadOnlySubBuffer(int offset, int sizeInBytes)
        {
            return CreateSubBuffer(offset, sizeInBytes);
        }

        public Buffer CreateSubBuffer(int offset, int sizeInBytes)
        {
            if (offset < 0 ||
                offset > _sizeInBytes)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (sizeInBytes < 0 ||
                offset + sizeInBytes > _sizeInBytes)
                throw new ArgumentOutOfRangeException(nameof(sizeInBytes));

            return new Buffer(_data, _offset + offset, sizeInBytes);
        }

        public byte this[int offset]
        {
            get { return _data.GetByte(_offset + offset); }
            set { _data.Set(_offset + offset, value); }
        }

        public sbyte GetSByte(int offset)
        {
            return _data.GetSByte(_offset + offset);
        }

        public void Set(int offset, sbyte value)
        {
            _data.Set(_offset + offset, value);
        }

        public sbyte2 GetSByte2(int offset)
        {
            return _data.GetSByte2(_offset + offset);
        }

        public void Set(int offset, sbyte2 value)
        {
            _data.Set(_offset + offset, value);
        }

        public sbyte4 GetSByte4(int offset)
        {
            return _data.GetSByte4(_offset + offset);
        }

        public void Set(int offset, sbyte4 value)
        {
            _data.Set(_offset + offset, value);
        }

        public byte GetByte(int offset)
        {
            return _data.GetByte(_offset + offset);
        }

        public void Set(int offset, byte value)
        {
            _data.Set(_offset + offset, value);
        }

        public byte2 GetByte2(int offset)
        {
            return _data.GetByte2(_offset + offset);
        }

        public void Set(int offset, byte2 value)
        {
            _data.Set(_offset + offset, value);
        }

        public byte4 GetByte4(int offset)
        {
            return _data.GetByte4(_offset + offset);
        }

        public void Set(int offset, byte4 value)
        {
            _data.Set(_offset + offset, value);
        }

        public short GetShort(int offset, bool littleEndian = true)
        {
            return _data.GetShort(_offset + offset, littleEndian);
        }

        public void Set(int offset, short value, bool littleEndian = true)
        {
            _data.Set(_offset + offset, value, littleEndian);
        }

        public short2 GetShort2(int offset, bool littleEndian = true)
        {
            return _data.GetShort2(_offset + offset, littleEndian);
        }

        public void Set(int offset, short2 value, bool littleEndian = true)
        {
            _data.Set(_offset + offset, value, littleEndian);
        }

        public short4 GetShort4(int offset, bool littleEndian = true)
        {
            return _data.GetShort4(_offset + offset, littleEndian);
        }

        public void Set(int offset, short4 value, bool littleEndian = true)
        {
            _data.Set(_offset + offset, value, littleEndian);
        }

        public ushort GetUShort(int offset, bool littleEndian = true)
        {
            return _data.GetUShort(_offset + offset, littleEndian);
        }

        public void Set(int offset, ushort value, bool littleEndian = true)
        {
            _data.Set(_offset + offset, value, littleEndian);
        }

        public ushort2 GetUShort2(int offset, bool littleEndian = true)
        {
            return _data.GetUShort2(_offset + offset, littleEndian);
        }

        public void Set(int offset, ushort2 value, bool littleEndian = true)
        {
            _data.Set(_offset + offset, value, littleEndian);
        }

        public ushort4 GetUShort4(int offset, bool littleEndian = true)
        {
            return _data.GetUShort4(_offset + offset, littleEndian);
        }

        public void Set(int offset, ushort4 value, bool littleEndian = true)
        {
            _data.Set(_offset + offset, value, littleEndian);
        }

        public int GetInt(int offset, bool littleEndian = true)
        {
            return _data.GetInt(_offset + offset, littleEndian);
        }

        public void Set(int offset, int value, bool littleEndian = true)
        {
            _data.Set(_offset + offset, value, littleEndian);
        }

        public int2 GetInt2(int offset, bool littleEndian = true)
        {
            return _data.GetInt2(_offset + offset, littleEndian);
        }

        public void Set(int offset, int2 value, bool littleEndian = true)
        {
            _data.Set(_offset + offset, value, littleEndian);
        }

        public int3 GetInt3(int offset, bool littleEndian = true)
        {
            return _data.GetInt3(_offset + offset, littleEndian);
        }

        public void Set(int offset, int3 value, bool littleEndian = true)
        {
            _data.Set(_offset + offset, value, littleEndian);
        }

        public int4 GetInt4(int offset, bool littleEndian = true)
        {
            return _data.GetInt4(_offset + offset, littleEndian);
        }

        public void Set(int offset, int4 value, bool littleEndian = true)
        {
            _data.Set(_offset + offset, value, littleEndian);
        }

        public uint GetUInt(int offset, bool littleEndian = true)
        {
            return _data.GetUInt(_offset + offset, littleEndian);
        }

        public void Set(int offset, uint value, bool littleEndian = true)
        {
            _data.Set(_offset + offset, value, littleEndian);
        }

        public long GetLong(int offset, bool littleEndian = true)
        {
            return _data.GetLong(_offset + offset, littleEndian);
        }

        public void Set(int offset, long value, bool littleEndian = true)
        {
            _data.Set(_offset + offset, value, littleEndian);
        }

        public ulong GetULong(int offset, bool littleEndian = true)
        {
            return _data.GetULong(_offset + offset, littleEndian);
        }

        public void Set(int offset, ulong value, bool littleEndian = true)
        {
            _data.Set(_offset + offset, value, littleEndian);
        }

        public float GetFloat(int offset, bool littleEndian = true)
        {
            return _data.GetFloat(_offset + offset, littleEndian);
        }

        public void Set(int offset, float value, bool littleEndian = true)
        {
            _data.Set(_offset + offset, value, littleEndian);
        }

        public float2 GetFloat2(int offset, bool littleEndian = true)
        {
            return _data.GetFloat2(_offset + offset, littleEndian);
        }

        public void Set(int offset, float2 value, bool littleEndian = true)
        {
            _data.Set(_offset + offset, value, littleEndian);
        }

        public float3 GetFloat3(int offset, bool littleEndian = true)
        {
            return _data.GetFloat3(_offset + offset, littleEndian);
        }

        public void Set(int offset, float3 value, bool littleEndian = true)
        {
            _data.Set(_offset + offset, value, littleEndian);
        }

        public float4 GetFloat4(int offset, bool littleEndian = true)
        {
            return _data.GetFloat4(_offset + offset, littleEndian);
        }

        public void Set(int offset, float4 value, bool littleEndian = true)
        {
            _data.Set(_offset + offset, value, littleEndian);
        }

        public float3x3 GetFloat3x3(int offset, bool littleEndian = true)
        {
            return _data.GetFloat3x3(_offset + offset, littleEndian);
        }

        public void Set(int offset, float3x3 value, bool littleEndian = true)
        {
            _data.Set(_offset + offset, value, littleEndian);
        }

        public float4x4 GetFloat4x4(int offset, bool littleEndian = true)
        {
            return _data.GetFloat4x4(_offset + offset, littleEndian);
        }

        public void Set(int offset, float4x4 value, bool littleEndian = true)
        {
            _data.Set(_offset + offset, value, littleEndian);
        }

        public double GetDouble(int offset, bool littleEndian = true)
        {
            return _data.GetDouble(_offset + offset, littleEndian);
        }

        public void Set(int offset, double value, bool littleEndian = true)
        {
            _data.Set(_offset + offset, value, littleEndian);
        }

        public IntPtr PinPtr(out GCHandle pin)
        {
            pin = GCHandle.Alloc(_data, GCHandleType.Pinned);
            return pin.AddrOfPinnedObject() + ByteOffset;
        }
    }
}
