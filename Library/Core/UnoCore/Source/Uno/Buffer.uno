using Uno.Compiler.ExportTargetInterop;
using Uno.Runtime.Implementation;
using Uno.Runtime.InteropServices;
using Uno.IO;

namespace Uno
{
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
            get { return GetByte(offset); }
            set { Set(offset, value); }
        }

        public sbyte GetSByte(int offset)
        {
            if (offset < 0 || _sizeInBytes < offset + sizeof(sbyte))
                throw new ArgumentOutOfRangeException(nameof(offset));

            return (sbyte)_data[_offset + offset];
        }

        public void Set(int offset, sbyte value)
        {
            if (offset < 0 || _sizeInBytes < offset + sizeof(sbyte))
                throw new ArgumentOutOfRangeException(nameof(offset));

            _data[_offset + offset] = (byte)value;
        }

        public sbyte2 GetSByte2(int offset)
        {
            return sbyte2(GetSByte(offset + 0), GetSByte(offset + 1));
        }

        public void Set(int offset, sbyte2 value)
        {
            Set(offset + 0, value.X);
            Set(offset + 1, value.Y);
        }

        public sbyte4 GetSByte4(int offset)
        {
            return sbyte4(GetSByte(offset + 0), GetSByte(offset + 1), GetSByte(offset + 2), GetSByte(offset + 3));
        }

        public void Set(int offset, sbyte4 value)
        {
            Set(offset + 0, value.X);
            Set(offset + 1, value.Y);
            Set(offset + 2, value.Z);
            Set(offset + 3, value.W);
        }

        public byte GetByte(int offset)
        {
            if (offset < 0 || _sizeInBytes < offset + sizeof(byte))
                throw new ArgumentOutOfRangeException(nameof(offset));

            return _data[_offset + offset];
        }

        public void Set(int offset, byte value)
        {
            if (offset < 0 || _sizeInBytes < offset + sizeof(byte))
                throw new ArgumentOutOfRangeException(nameof(offset));

            _data[_offset + offset] = value;
        }

        public byte2 GetByte2(int offset)
        {
            return byte2(GetByte(offset + 0), GetByte(offset + 1));
        }

        public void Set(int offset, byte2 value)
        {
            Set(offset + 0, value.X);
            Set(offset + 1, value.Y);
        }

        public byte4 GetByte4(int offset)
        {
            return byte4(GetByte(offset + 0), GetByte(offset + 1), GetByte(offset + 2), GetByte(offset + 3));
        }

        public void Set(int offset, byte4 value)
        {
            Set(offset + 0, value.X);
            Set(offset + 1, value.Y);
            Set(offset + 2, value.Z);
            Set(offset + 3, value.W);
        }

        public short GetShort(int offset, bool littleEndian = true)
        {
            if (offset < 0 || _sizeInBytes < offset + sizeof(short))
                throw new ArgumentOutOfRangeException(nameof(offset));

            return BufferImpl.GetShort(_data, _offset + offset, littleEndian);
        }

        public void Set(int offset, short value, bool littleEndian = true)
        {
            if (offset < 0 || _sizeInBytes < offset + sizeof(short))
                throw new ArgumentOutOfRangeException(nameof(offset));

            BufferImpl.SetShort(_data, _offset + offset, value, littleEndian);
        }

        public short2 GetShort2(int offset, bool littleEndian = true)
        {
            return short2(GetShort(offset + 0, littleEndian), GetShort(offset + 2, littleEndian));
        }

        public void Set(int offset, short2 value, bool littleEndian = true)
        {
            Set(offset + 0, value.X, littleEndian);
            Set(offset + 2, value.Y, littleEndian);
        }

        public short4 GetShort4(int offset, bool littleEndian = true)
        {
            return short4(GetShort(offset + 0, littleEndian), GetShort(offset + 2, littleEndian), GetShort(offset + 4, littleEndian), GetShort(offset + 6, littleEndian));
        }

        public void Set(int offset, short4 value, bool littleEndian = true)
        {
            Set(offset + 0, value.X, littleEndian);
            Set(offset + 2, value.Y, littleEndian);
            Set(offset + 4, value.Z, littleEndian);
            Set(offset + 6, value.W, littleEndian);
        }

        public ushort GetUShort(int offset, bool littleEndian = true)
        {
            if (offset < 0 || _sizeInBytes < offset + sizeof(ushort))
                throw new ArgumentOutOfRangeException(nameof(offset));

            return BufferImpl.GetUShort(_data, _offset + offset, littleEndian);
        }

        public void Set(int offset, ushort value, bool littleEndian = true)
        {
            if (offset < 0 || _sizeInBytes < offset + sizeof(ushort))
                throw new ArgumentOutOfRangeException(nameof(offset));

            BufferImpl.SetUShort(_data, _offset + offset, value, littleEndian);
        }

        public ushort2 GetUShort2(int offset, bool littleEndian = true)
        {
            return ushort2(GetUShort(offset + 0, littleEndian), GetUShort(offset + 2, littleEndian));
        }

        public void Set(int offset, ushort2 value, bool littleEndian = true)
        {
            Set(offset + 0, value.X, littleEndian);
            Set(offset + 2, value.Y, littleEndian);
        }

        public ushort4 GetUShort4(int offset, bool littleEndian = true)
        {
            return ushort4(GetUShort(offset + 0, littleEndian), GetUShort(offset + 2, littleEndian), GetUShort(offset + 4, littleEndian), GetUShort(offset + 6, littleEndian));
        }

        public void Set(int offset, ushort4 value, bool littleEndian = true)
        {
            Set(offset + 0, value.X, littleEndian);
            Set(offset + 2, value.Y, littleEndian);
            Set(offset + 4, value.Z, littleEndian);
            Set(offset + 6, value.W, littleEndian);
        }

        public int GetInt(int offset, bool littleEndian = true)
        {
            if (offset < 0 || _sizeInBytes < offset + sizeof(int))
                throw new ArgumentOutOfRangeException(nameof(offset));

            return BufferImpl.GetInt(_data, _offset + offset, littleEndian);
        }

        public void Set(int offset, int value, bool littleEndian = true)
        {
            if (offset < 0 || _sizeInBytes < offset + sizeof(int))
                throw new ArgumentOutOfRangeException(nameof(offset));

            BufferImpl.SetInt(_data, _offset + offset, value, littleEndian);
        }

        public int2 GetInt2(int offset, bool littleEndian = true)
        {
            return int2(GetInt(offset + 0, littleEndian), GetInt(offset + 4, littleEndian));
        }

        public void Set(int offset, int2 value, bool littleEndian = true)
        {
            Set(offset + 0, value.X, littleEndian);
            Set(offset + 4, value.Y, littleEndian);
        }

        public int3 GetInt3(int offset, bool littleEndian = true)
        {
            return int3(GetInt(offset + 0, littleEndian), GetInt(offset + 4, littleEndian), GetInt(offset + 8, littleEndian));
        }

        public void Set(int offset, int3 value, bool littleEndian = true)
        {
            Set(offset + 0, value.X, littleEndian);
            Set(offset + 4, value.Y, littleEndian);
            Set(offset + 8, value.Z, littleEndian);
        }

        public int4 GetInt4(int offset, bool littleEndian = true)
        {
            return int4(GetInt(offset + 0, littleEndian), GetInt(offset + 4, littleEndian), GetInt(offset + 8, littleEndian), GetInt(offset + 12, littleEndian));
        }

        public void Set(int offset, int4 value, bool littleEndian = true)
        {
            Set(offset + 00, value.X, littleEndian);
            Set(offset + 04, value.Y, littleEndian);
            Set(offset + 08, value.Z, littleEndian);
            Set(offset + 12, value.W, littleEndian);
        }

        public uint GetUInt(int offset, bool littleEndian = true)
        {
            if (offset < 0 || _sizeInBytes < offset + sizeof(uint))
                throw new ArgumentOutOfRangeException(nameof(offset));

            return BufferImpl.GetUInt(_data, _offset + offset, littleEndian);
        }

        public void Set(int offset, uint value, bool littleEndian = true)
        {
            if (offset < 0 || _sizeInBytes < offset + sizeof(uint))
                throw new ArgumentOutOfRangeException(nameof(offset));

            BufferImpl.SetUInt(_data, _offset + offset, value, littleEndian);
        }

        public long GetLong(int offset, bool littleEndian = true)
        {
            if (offset < 0 || _sizeInBytes < offset + sizeof(long))
                throw new ArgumentOutOfRangeException(nameof(offset));

            return BufferImpl.GetLong(_data, _offset + offset, littleEndian);
        }

        public void Set(int offset, long value, bool littleEndian = true)
        {
            if (offset < 0 || _sizeInBytes < offset + sizeof(long))
                throw new ArgumentOutOfRangeException(nameof(offset));

            BufferImpl.SetLong(_data, _offset + offset, value, littleEndian);
        }

        public ulong GetULong(int offset, bool littleEndian = true)
        {
            if (offset < 0 || _sizeInBytes < offset + sizeof(ulong))
                throw new ArgumentOutOfRangeException(nameof(offset));

            return BufferImpl.GetULong(_data, _offset + offset, littleEndian);
        }

        public void Set(int offset, ulong value, bool littleEndian = true)
        {
            if (offset < 0 || _sizeInBytes < offset + sizeof(ulong))
                throw new ArgumentOutOfRangeException(nameof(offset));

            BufferImpl.SetULong(_data, _offset + offset, value, littleEndian);
        }

        public float GetFloat(int offset, bool littleEndian = true)
        {
            if (offset < 0 || _sizeInBytes < offset + sizeof(float))
                throw new ArgumentOutOfRangeException(nameof(offset));

            return BufferImpl.GetFloat(_data, _offset + offset, littleEndian);
        }

        public void Set(int offset, float value, bool littleEndian = true)
        {
            if (offset < 0 || _sizeInBytes < offset + sizeof(float))
                throw new ArgumentOutOfRangeException(nameof(offset));

            BufferImpl.SetFloat(_data, _offset + offset, value, littleEndian);
        }

        public float2 GetFloat2(int offset, bool littleEndian = true)
        {
            return float2(GetFloat(offset + 0, littleEndian), GetFloat(offset + 4, littleEndian));
        }

        public void Set(int offset, float2 value, bool littleEndian = true)
        {
            Set(offset + 0, value.X, littleEndian);
            Set(offset + 4, value.Y, littleEndian);
        }

        public float3 GetFloat3(int offset, bool littleEndian = true)
        {
            return float3(GetFloat(offset + 0, littleEndian), GetFloat(offset + 4, littleEndian), GetFloat(offset + 8, littleEndian));
        }

        public void Set(int offset, float3 value, bool littleEndian = true)
        {
            Set(offset + 0, value.X, littleEndian);
            Set(offset + 4, value.Y, littleEndian);
            Set(offset + 8, value.Z, littleEndian);
        }

        public float4 GetFloat4(int offset, bool littleEndian = true)
        {
            return float4(GetFloat(offset + 0, littleEndian), GetFloat(offset + 4, littleEndian), GetFloat(offset + 8, littleEndian), GetFloat(offset + 12, littleEndian));
        }

        public void Set(int offset, float4 value, bool littleEndian = true)
        {
            Set(offset + 00, value.X, littleEndian);
            Set(offset + 04, value.Y, littleEndian);
            Set(offset + 08, value.Z, littleEndian);
            Set(offset + 12, value.W, littleEndian);
        }

        public float3x3 GetFloat3x3(int offset, bool littleEndian = true)
        {
            return float3x3(
                GetFloat(offset + 00, littleEndian), GetFloat(offset + 04, littleEndian), GetFloat(offset + 08, littleEndian),
                GetFloat(offset + 12, littleEndian), GetFloat(offset + 16, littleEndian), GetFloat(offset + 20, littleEndian),
                GetFloat(offset + 24, littleEndian), GetFloat(offset + 28, littleEndian), GetFloat(offset + 32, littleEndian));
        }

        public void Set(int offset, float3x3 value, bool littleEndian = true)
        {
            Set(offset + 00, value.M11, littleEndian);
            Set(offset + 04, value.M12, littleEndian);
            Set(offset + 08, value.M13, littleEndian);
            Set(offset + 12, value.M21, littleEndian);
            Set(offset + 16, value.M22, littleEndian);
            Set(offset + 20, value.M23, littleEndian);
            Set(offset + 24, value.M31, littleEndian);
            Set(offset + 28, value.M32, littleEndian);
            Set(offset + 32, value.M33, littleEndian);
        }

        public float4x4 GetFloat4x4(int offset, bool littleEndian = true)
        {
            return float4x4(
                GetFloat(offset + 00, littleEndian), GetFloat(offset + 04, littleEndian), GetFloat(offset + 08, littleEndian), GetFloat(offset + 12, littleEndian),
                GetFloat(offset + 16, littleEndian), GetFloat(offset + 20, littleEndian), GetFloat(offset + 24, littleEndian), GetFloat(offset + 28, littleEndian),
                GetFloat(offset + 32, littleEndian), GetFloat(offset + 36, littleEndian), GetFloat(offset + 40, littleEndian), GetFloat(offset + 44, littleEndian),
                GetFloat(offset + 48, littleEndian), GetFloat(offset + 52, littleEndian), GetFloat(offset + 56, littleEndian), GetFloat(offset + 60, littleEndian));
        }

        public void Set(int offset, float4x4 value, bool littleEndian = true)
        {
            Set(offset + 00, value.M11, littleEndian);
            Set(offset + 04, value.M12, littleEndian);
            Set(offset + 08, value.M13, littleEndian);
            Set(offset + 12, value.M14, littleEndian);
            Set(offset + 16, value.M21, littleEndian);
            Set(offset + 20, value.M22, littleEndian);
            Set(offset + 24, value.M23, littleEndian);
            Set(offset + 28, value.M24, littleEndian);
            Set(offset + 32, value.M31, littleEndian);
            Set(offset + 36, value.M32, littleEndian);
            Set(offset + 40, value.M33, littleEndian);
            Set(offset + 44, value.M34, littleEndian);
            Set(offset + 48, value.M41, littleEndian);
            Set(offset + 52, value.M42, littleEndian);
            Set(offset + 56, value.M43, littleEndian);
            Set(offset + 60, value.M44, littleEndian);
        }

        public double GetDouble(int offset, bool littleEndian = true)
        {
            if (offset < 0 || _sizeInBytes < offset + sizeof(double))
                throw new ArgumentOutOfRangeException(nameof(offset));

            return BufferImpl.GetDouble(_data, _offset + offset, littleEndian);
        }

        public void Set(int offset, double value, bool littleEndian = true)
        {
            if (offset < 0 || _sizeInBytes < offset + sizeof(double))
                throw new ArgumentOutOfRangeException(nameof(offset));

            BufferImpl.SetDouble(_data, _offset + offset, value, littleEndian);
        }

        public IntPtr PinPtr(out GCHandle pin)
        {
            pin = GCHandle.Alloc(_data, GCHandleType.Pinned);
            return pin.AddrOfPinnedObject() + ByteOffset;
        }
    }
}
