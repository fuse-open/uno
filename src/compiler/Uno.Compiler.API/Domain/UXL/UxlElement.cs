using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.Serialization;

namespace Uno.Compiler.API.Domain.UXL
{
    public class UxlElement
    {
        public readonly UxlElementType Type;
        public readonly SourceValue Key;
        public readonly SourceValue Value;
        public readonly SourceValue? Condition;
        public readonly bool IsDefault;

        public UxlElement(UxlElementType type, SourceValue key, SourceValue value, SourceValue? cond, bool isDefault)
        {
            Type = type;
            Key = key;
            Value = value;
            Condition = cond;
            IsDefault = isDefault;
        }

        public Source Source => Key.Source;

        public Disambiguation Disambiguation
        {
            get
            {
                if (IsDefault)
                    return Disambiguation.Default;
                if (Condition.HasValue)
                    return Disambiguation.Condition;
                return 0;
            }
        }

        public void Write(CacheWriter f)
        {
            var flags = (UxlElementFlags)Type & UxlElementFlags.TypeMask;
            if (Condition != null)
                flags |= UxlElementFlags.HasCondition;
            if (IsDefault)
                flags |= UxlElementFlags.IsDefault;

            f.Write((byte)flags);
            f.WriteGlobal(Key);
            f.WriteGlobal(Value);

            if (flags.HasFlag(UxlElementFlags.HasCondition))
                f.WriteGlobal(Condition.Value);
        }

        public static UxlElement Read(CacheReader f)
        {
            var flags = (UxlElementFlags)f.ReadByte();
            var key = f.ReadGlobalValue();
            var value = f.ReadGlobalValue();
            SourceValue? cond = null;

            if (flags.HasFlag(UxlElementFlags.HasCondition))
                cond = f.ReadGlobalValue();

            return new UxlElement((UxlElementType)(flags & UxlElementFlags.TypeMask), key, value, cond, flags.HasFlag(UxlElementFlags.IsDefault));
        }
    }
}