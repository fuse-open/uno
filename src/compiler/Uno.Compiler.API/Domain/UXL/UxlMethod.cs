using System.Collections.Generic;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.Serialization;

namespace Uno.Compiler.API.Domain.UXL
{
    public class UxlMethod : UxlEntity
    {
        public readonly SourceValue Signature;
        public readonly SourceValue? Condition;
        public readonly bool IsDefault;
        public readonly List<UxlImplementation> Implementations = new List<UxlImplementation>();

        public UxlMethod(SourceValue sig, SourceValue? cond, bool isDefault, List<UxlElement> elements = null)
            : base(elements)
        {
            Signature = sig;
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
            var flags = (UxlMethodFlags)EntityFlags;
            if (Condition != null)
                flags |= UxlMethodFlags.HasCondition;
            if (IsDefault)
                flags |= UxlMethodFlags.IsDefault;
            if (Implementations.Count > 0)
                flags |= UxlMethodFlags.Implementations;

            f.Write((byte) flags);
            f.WriteGlobal(Signature);

            if (flags.HasFlag(UxlMethodFlags.HasCondition))
                f.WriteGlobal(Condition.Value);
            if (flags.HasFlag(UxlMethodFlags.Implementations))
                f.WriteList(Implementations, (w, x) => x.Write(w));

            WriteEntity(f, (UxlEntityFlags)flags);
        }

        public static UxlMethod Read(CacheReader f)
        {
            var flags = (UxlMethodFlags)f.ReadByte();
            var sig = f.ReadGlobalValue();

            SourceValue? cond = null;
            if (flags.HasFlag(UxlMethodFlags.HasCondition))
                cond = f.ReadGlobalValue();

            var result = new UxlMethod(sig, cond, flags.HasFlag(UxlMethodFlags.IsDefault));

            if (flags.HasFlag(UxlMethodFlags.Implementations))
                f.ReadList(result.Implementations, UxlImplementation.Read);

            result.ReadEntity(f, (UxlEntityFlags)flags);
            return result;
        }

        public override string ToString()
        {
            return Signature.String;
        }
    }
}