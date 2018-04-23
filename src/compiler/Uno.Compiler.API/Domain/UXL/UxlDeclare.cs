using Uno.Compiler.API.Domain.Serialization;

namespace Uno.Compiler.API.Domain.UXL
{
    public class UxlDeclare
    {
        public readonly Source Source;
        public readonly UxlDeclareType Type;
        public readonly SourceValue Key;
        public readonly SourceValue? Condition;

        public UxlDeclare(Source src, UxlDeclareType type, SourceValue key, SourceValue? cond)
        {
            Source = src;
            Type = type;
            Key = key;
            Condition = cond;
        }

        public void Write(CacheWriter f)
        {
            var flags = (UxlDeclareFlags)Type;

            if (Condition != null)
                flags |= UxlDeclareFlags.HasCondition;

            f.Write((byte)flags);
            f.Write(Source);
            f.WriteGlobal(Key);

            if (flags.HasFlag(UxlDeclareFlags.HasCondition))
                f.WriteGlobal(Condition.Value);
        }

        public static UxlDeclare Read(CacheReader f)
        {
            var flags = (UxlDeclareFlags)f.ReadByte();
            var src = f.ReadSource();
            var key = f.ReadGlobalValue();

            SourceValue? cond = null;
            if (flags.HasFlag(UxlDeclareFlags.HasCondition))
                cond = f.ReadGlobalValue();

            return new UxlDeclare(src, (UxlDeclareType)(flags & UxlDeclareFlags.TypeMask), key, cond);
        }
    }
}