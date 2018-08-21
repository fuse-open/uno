using Uno.Compiler.ExportTargetInterop;

namespace Uno.Graphics
{
    public static class FormatHelpers
    {
        public static int GetStrideInBytes(Format format)
        {
            switch (format)
            {
                case Format.L8: return 1;
                case Format.LA88: return 2;
                case Format.RGBA8888: return 4;
                case Format.RGBA4444: return 2;
                case Format.RGBA5551: return 2;
                case Format.RGB565: return 2;
            }

            throw new FormatException("Invalid format <" + format + ">");
        }
    }
}
