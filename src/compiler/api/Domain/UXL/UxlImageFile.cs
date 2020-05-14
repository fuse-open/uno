using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.Serialization;

namespace Uno.Compiler.API.Domain.UXL
{
    static class UxlImageFile
    {
        public static void Write(this CacheWriter f, ImageFile e)
        {
            UxlImageFileFlags flags = 0;

            if (e.Condition != null)
                flags |= UxlImageFileFlags.HasCondition;
            if (e.TargetName != null)
                flags |= UxlImageFileFlags.HasTargetName;
            if (e.TargetWidth != null)
                flags |= UxlImageFileFlags.HasTargetWidth;
            if (e.TargetHeight != null)
                flags |= UxlImageFileFlags.HasTargetHeight;

            f.Write((byte) flags);
            f.WriteGlobal(e.SourceName);

            if (flags.HasFlag(UxlImageFileFlags.HasCondition))
                f.WriteGlobal(e.Condition.Value);
            if (flags.HasFlag(UxlImageFileFlags.HasTargetName))
                f.WriteGlobal(e.TargetName.Value);
            if (flags.HasFlag(UxlImageFileFlags.HasTargetWidth))
                f.WriteCompressed(e.TargetWidth.Value);
            if (flags.HasFlag(UxlImageFileFlags.HasTargetHeight))
                f.WriteCompressed(e.TargetHeight.Value);
        }

        public static ImageFile ReadImageFile(this CacheReader f)
        {
            var flags = (UxlImageFileFlags)f.ReadByte();
            var srcName = f.ReadGlobalValue();

            SourceValue? dstName = null
                , cond = null;
            if (flags.HasFlag(UxlImageFileFlags.HasCondition))
                cond = f.ReadGlobalValue();
            if (flags.HasFlag(UxlImageFileFlags.HasTargetName))
                dstName = f.ReadGlobalValue();

            int? w = null, h = null;
            if (flags.HasFlag(UxlImageFileFlags.HasTargetWidth))
                w = f.ReadCompressedInt();
            if (flags.HasFlag(UxlImageFileFlags.HasTargetHeight))
                h = f.ReadCompressedInt();

            return new ImageFile(srcName, cond, dstName, w, h);
        }
    }
}