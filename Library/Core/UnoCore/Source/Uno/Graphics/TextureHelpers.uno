using Uno.Compiler.ExportTargetInterop;

namespace Uno.Graphics
{
    public static class TextureHelpers
    {
        public static int2 GetMipSize(texture2D texture, int mip)
        {
            var size = texture.Size;

            if (mip > 0)
            {
                size.X >>= mip;
                size.Y >>= mip;

                if (size.X < 0)
                    size.X = 1;

                if (size.Y < 0)
                    size.Y = 1;
            }

            return size;
        }

        public static int2 GetMipSize(textureCube texture, int mip)
        {
            var size = texture.Size;

            if (mip > 0)
            {
                size >>= mip;

                if (size < 0)
                    size = 1;
            }

            return int2(size, size);
        }

        public static int GetMipCount(int2 size)
        {
            int result = 0;

            do
            {
                result++;
                size.X = size.X >> 1;
                size.Y = size.Y >> 1;
            }
            while (size.X > 0 && size.Y > 0);

            return result;
        }

        public static int GetMipCount(int size)
        {
            int result = 0;

            do
            {
                result++;
                size = size >> 1;
            }
            while (size > 0);

            return result;
        }
    }
}
