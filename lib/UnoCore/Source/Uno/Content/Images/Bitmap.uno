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

        public Buffer Buffer
        {
            get;
            private set;
        }

        public Bitmap(int2 size, Format format, Buffer optionalBuffer = null)
        {
            Size = size;
            Format = format;

            int bpp = FormatHelpers.GetStrideInBytes(Format);
            int byteCount = Size.X * Size.Y * bpp;

            if (byteCount < 0)
                throw new Exception("size: Cannot be negative");

            if (optionalBuffer != null)
            {
                if (optionalBuffer.SizeInBytes != byteCount)
                    throw new Exception("optionalBuffer: Invalid buffer size");

                Buffer = optionalBuffer;
            }
            else
            {
                Buffer = new Buffer(byteCount);
            }
        }
    }
}
