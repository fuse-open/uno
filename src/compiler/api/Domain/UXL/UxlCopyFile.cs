using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.Serialization;

namespace Uno.Compiler.API.Domain.UXL
{
    static class UxlCopyFile
    {
        public static void Write(this CacheWriter f, CopyFile e)
        {
            var flags = (UxlCopyFileFlags)e.Flags & UxlCopyFileFlags.FlagsMask;

            if (e.TargetName != null)
                flags |= UxlCopyFileFlags.HasTargetName;
            if (e.Condition != null)
                flags |= UxlCopyFileFlags.HasCondition;
            if (e.Type != null)
                flags |= UxlCopyFileFlags.HasType;

            f.Write((byte)flags);
            f.WriteGlobal(e.SourceName);

            if (flags.HasFlag(UxlCopyFileFlags.HasTargetName))
                f.WriteGlobal(e.TargetName.Value);
            if (flags.HasFlag(UxlCopyFileFlags.HasCondition))
                f.WriteGlobal(e.Condition.Value);
            if (flags.HasFlag(UxlCopyFileFlags.HasType))
                f.WriteGlobal(e.Type.Value);
        }

        public static CopyFile ReadCopyFile(this CacheReader f)
        {
            var flags = (UxlCopyFileFlags)f.ReadByte();
            var srcName = f.ReadGlobalValue();

            SourceValue? dstName = null
                , cond = null
                , type = null;
            if (flags.HasFlag(UxlCopyFileFlags.HasTargetName))
                dstName = f.ReadGlobalValue();
            if (flags.HasFlag(UxlCopyFileFlags.HasCondition))
                cond = f.ReadGlobalValue();
            if (flags.HasFlag(UxlCopyFileFlags.HasType))
                type = f.ReadGlobalValue();

            return new CopyFile(srcName, (CopyFileFlags)(flags & UxlCopyFileFlags.FlagsMask), dstName, cond, type);
        }
    }
}