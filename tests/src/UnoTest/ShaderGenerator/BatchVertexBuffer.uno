using Uno;
using Uno.Collections;
using Uno.Graphics;

namespace UnoTest
{
    public class BatchVertexBuffer
    {
        VertexAttributeType dataType;
        public VertexAttributeType DataType
        {
            get { return dataType; }
            set
            {
                if (buf != null) throw new Exception("Vertex attribute type cannot be changed after Bufferfer is written to");
                dataType = value;
            }
        }

        public int StrideInBytes { get { return VertexAttributeTypeHelpers.GetStrideInBytes(DataType); } }

        int maxVertices;
        BufferUsage usage;

        public BatchVertexBuffer(VertexAttributeType type, int maxVertices, bool staticBatch)
        {
            this.DataType = type;
            this.maxVertices = maxVertices;
            this.usage = staticBatch ? BufferUsage.Immutable : BufferUsage.Dynamic;
        }

        public BatchVertexBuffer(VertexAttributeType type, byte[] data)
        {
            this.DataType = type;
            this.usage = BufferUsage.Immutable;

            this.buf = new byte[data.Length];
            for (int i =0; i<buf.Length; i++)
                this.buf[i] = data[i];
        }


        byte[] buf;
        public byte[] Buffer
        {
            get
            {
                if (buf == null) buf = new byte[maxVertices * StrideInBytes];
                return buf;
            }
        }

        VertexBuffer vbo;
        public VertexBuffer VertexBuffer
        {
            get
            {
                if (Buffer == null)
                    return null;
                Flush();
                return vbo;
            }
        }

        int _position;
        public int Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public void Write(float2 value)
        {
            Buffer.Set(_position, value);
            _position += 8;
        }

        public void Write(float3 value)
        {
            Buffer.Set(_position, value);
            _position += 12;
        }

        public void Write(float4 value)
        {
            Buffer.Set(_position, value);
            _position += 16;
        }

        public void Write(byte value)
        {
            Buffer.Set(_position, value);
            _position += 1;
        }

        public void Write(byte2 value)
        {
            Buffer.Set(_position, value);
            _position += 2;
        }

        public void Write(byte4 value)
        {
            Buffer.Set(_position, value);
            _position += 4;
        }

        public void Write(sbyte value)
        {
            Buffer.Set(_position, value);
            _position += 1;
        }

        public void Write(sbyte2 value)
        {
            Buffer.Set(_position, value);
            _position += 2;
        }

        public void Write(sbyte4 value)
        {
            Buffer.Set(_position, value);
            _position += 4;
        }

        public void Write(ushort value)
        {
            Buffer.Set(_position, value);
            _position += 2;
        }

        public void Write(ushort2 value)
        {
            Buffer.Set(_position, value);
            _position += 4;
        }

        public void Write(ushort4 value)
        {
            Buffer.Set(_position, value);
            _position += 8;
        }

        public void Write(short value)
        {
            Buffer.Set(_position, value);
            _position += 2;
        }

        public void Write(short2 value)
        {
            Buffer.Set(_position, value);
            _position += 4;
        }

        public void Write(short4 value)
        {
            Buffer.Set(_position, value);
            _position += 8;
        }

        public void Write(uint value)
        {
            Buffer.Set(_position, value);
            _position += 4;
        }

        public void Write(int value)
        {
            Buffer.Set(_position, value);
            _position += 4;
        }

        public void Write(int2 value)
        {
            Buffer.Set(_position, value);
            _position += 8;
        }

        public void Write(int3 value)
        {
            Buffer.Set(_position, value);
            _position += 12;
        }

        public void Write(int4 value)
        {
            Buffer.Set(_position, value);
            _position += 16;
        }

        bool isDirty = true;
        public void Flush()
        {
            if (buf != null && isDirty)
            {
                if (vbo == null)
                    this.vbo = new VertexBuffer(Buffer.Length, usage);

                vbo.Update(buf);
                isDirty = false;
            }
        }

        public void Invalidate()
        {
            isDirty = true;
        }
    }
}
