using System.Collections.Generic;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.Serialization;

namespace Uno.Compiler.API.Domain.UXL
{
    public class UxlType : UxlEntity
    {
        public readonly SourceValue Name;
        public readonly SourceValue? Condition;
        public readonly bool IsDefault;
        public readonly List<UxlMethod> Methods = new List<UxlMethod>();

        public UxlType(SourceValue name, SourceValue? cond, bool isDefault, List<UxlElement> elements = null)
            : base(elements)
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

        public void Write(CacheWriter f)
        {
            var flags = (UxlTypeFlags)EntityFlags;
            if (Condition.HasValue)
                flags |= UxlTypeFlags.HasCondition;
            if (IsDefault)
                flags |= UxlTypeFlags.IsDefault;
            if (Methods.Count > 0)
                flags |= UxlTypeFlags.Methods;

            f.Write((byte)flags);
            f.WriteGlobal(Name);

            if (flags.HasFlag(UxlTypeFlags.HasCondition))
                f.WriteGlobal(Condition.Value);
            if (flags.HasFlag(UxlTypeFlags.Methods))
                f.WriteList(Methods, (w, x) => x.Write(w));

            WriteEntity(f, (UxlEntityFlags)flags);
        }

        public static UxlType Read(CacheReader f)
        {
            var flags = (UxlTypeFlags)f.ReadByte();
            var name = f.ReadGlobalValue();

            SourceValue? cond = null;
            if (flags.HasFlag(UxlTypeFlags.HasCondition))
                cond = f.ReadGlobalValue();

            var result = new UxlType(name, cond, flags.HasFlag(UxlTypeFlags.IsDefault));

            if (flags.HasFlag(UxlTypeFlags.Methods))
                f.ReadList(result.Methods, UxlMethod.Read);

            result.ReadEntity(f, (UxlEntityFlags)flags);
            return result;
        }

        public override string ToString()
        {
            return Name.String;
        }
    }
}