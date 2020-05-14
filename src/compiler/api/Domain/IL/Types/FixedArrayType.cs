using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Logging;

namespace Uno.Compiler.API.Domain.IL.Types
{
    public sealed class FixedArrayType : ArrayType
    {
        public Expression OptionalSize { get; private set; }

        public FixedArrayType(Source src, DataType elementType, Expression optionalSize, DataType intType)
            : base(src, "fixed_array", elementType, null)
        {
            OptionalSize = optionalSize;

            var length = new Property(src, null, Modifiers.Public | Modifiers.Extern | Modifiers.Generated, "Length", this, intType);
            length.CreateGetMethod(src, length.Modifiers);

            Properties.Add(length);
        }

        // Used by the compiler to infer the size of the type after its construction
        // Only allowed if size was null during construction
        public void InferSize(Constant size)
        {
            if (OptionalSize == null)
                OptionalSize = size;
            else
                throw new FatalException(size.Source, ErrorCode.I0002, "Cannot infer fixed array size");
        }

        public override TypeType TypeType => TypeType.FixedArray;

        public override string FullName => "fixed " + ElementType + "[" + (OptionalSize?.ToString() ?? "") + "]";

        public override int GetHashCode()
        {
            return ElementType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var fat = obj as FixedArrayType;

            if (fat == null || !ElementType.Equals(fat.ElementType))
                return false;

            return fat.OptionalSize != null && OptionalSize != null ?
                fat.OptionalSize.Equals(OptionalSize) :
                fat.OptionalSize == OptionalSize ||
                fat.OptionalSize == null ||
                OptionalSize == null;
        }
    }
}
