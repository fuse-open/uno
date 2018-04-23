namespace Uno.Compiler.API.Domain.IL.Types
{
    public abstract class ArrayType : DataType
    {
        public new readonly DataType ElementType;

        protected ArrayType(Source src, string name, DataType elementType, DataType objectType)
            : base(src, null, null, Modifiers.Public, name)
        {
            ElementType = elementType;
            SetBase(objectType);
        }
    }
}