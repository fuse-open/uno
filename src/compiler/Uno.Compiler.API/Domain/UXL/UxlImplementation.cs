using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.Serialization;

namespace Uno.Compiler.API.Domain.UXL
{
    public class UxlImplementation
    {
        public readonly Source Source;
        public readonly ImplementationType Type;
        public readonly SourceValue Body;
        public readonly SourceValue? Condition;
        public readonly bool IsDefault;

        public UxlImplementation(Source src, ImplementationType type, SourceValue body, SourceValue? cond, bool isDefault)
        {
            Source = src;
            Type = type;
            Body = body;
            Condition = cond;
            IsDefault = isDefault;
        }

        public void Write(CacheWriter f)
        {
            var flags = (UxlImplementationFlags)Type & UxlImplementationFlags.TypeMask;
            if (Condition != null)
                flags |= UxlImplementationFlags.HasCondition;
            if (IsDefault)
                flags |= UxlImplementationFlags.IsDefault;

            f.Write((byte) flags);
            f.Write(Source);
            f.WriteGlobal(Body);

            if (flags.HasFlag(UxlImplementationFlags.HasCondition))
                f.WriteGlobal(Condition.Value);
        }

        public static UxlImplementation Read(CacheReader r)
        {
            var flags = (UxlImplementationFlags)r.ReadByte();
            var src = r.ReadSource();
            var body = r.ReadGlobalValue();
            SourceValue? cond = null;

            if (flags.HasFlag(UxlImplementationFlags.HasCondition))
                cond = r.ReadGlobalValue();

            return new UxlImplementation(src, (ImplementationType)(flags & UxlImplementationFlags.TypeMask), body, cond, flags.HasFlag(UxlImplementationFlags.IsDefault));
        }
    }
}