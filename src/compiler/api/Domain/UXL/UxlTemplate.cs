using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.Serialization;

namespace Uno.Compiler.API.Domain.UXL
{
    public class UxlTemplate : UxlEntity
    {
        public readonly SourceValue Name;
        public readonly SourceValue? Condition;
        public readonly bool IsDefault;

        public UxlTemplate(SourceValue name, SourceValue? cond, bool isDefault)
        {
            Name = name;
            Condition = cond;
            IsDefault = isDefault;
        }

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

        UxlTemplateFlags Flags
        {
            get
            {
                var flags = (UxlTemplateFlags)EntityFlags;
                if (Condition != null)
                    flags |= UxlTemplateFlags.HasCondition;
                if (IsDefault)
                    flags |= UxlTemplateFlags.IsDefault;

                return flags;
            }
        }

        public void Write(CacheWriter f)
        {
            var flags = Flags;
            f.Write((byte) flags);
            f.WriteGlobal(Name);

            if (flags.HasFlag(UxlTemplateFlags.HasCondition))
                f.WriteGlobal(Condition.Value);

            WriteEntity(f, (UxlEntityFlags)flags);
        }

        public static UxlTemplate Read(CacheReader f)
        {
            var flags = (UxlTemplateFlags)f.ReadByte();

            SourceValue? cond = null;
            if (flags.HasFlag(UxlTemplateFlags.HasCondition))
                cond = f.ReadGlobalValue();

            var result = new UxlTemplate(f.ReadGlobalValue(), cond, flags.HasFlag(UxlTemplateFlags.IsDefault));
            result.ReadEntity(f, (UxlEntityFlags)flags);
            return result;
        }

        public override string ToString()
        {
            return Name.String;
        }
    }
}