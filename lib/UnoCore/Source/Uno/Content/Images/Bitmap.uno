using Uno.Compiler.ExportTargetInterop;
using Uno.Graphics;

namespace Uno.Content.Images
{
    [Obsolete]
    public class Bitmap
    {
        public int2 Size
        {
            get;
            private set;
        }

        public Format Format
        {
            get;
            private set;
        }

        public byte[] Data
        {
            get;
            private set;
        }

        public Bitmap(int2 size, Format format, byte[] optionalData = null)
        {
            Size = size;
            Format = format;

            int bpp = FormatHelpers.GetStrideInBytes(Format);
            int byteCount = Size.X * Size.Y * bpp;

            if (byteCount < 0)
                throw new Exception("size: Cannot be negative");

            if (optionalData != null)
            {
                if (optionalData.Length != byteCount)
                    throw new ArgumentException("optionalData", "Invalid buffer size");

                Data = optionalData;
            }
            else
            {
                Data = new byte[byteCount];
            }
        }

        [Obsolete]
        public Buffer Buffer
        {
            get { return new Buffer(Data); }
        }

        [Obsolete]
        public Bitmap(int2 size, Format format, Buffer optionalBuffer)
            : this(size, format, optionalBuffer != null ? optionalBuffer.GetBytes() : null)
        {
        }
    }
}
