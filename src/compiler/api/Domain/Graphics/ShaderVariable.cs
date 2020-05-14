using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.API.Domain.Graphics
{
    public class ShaderVariable
    {
        public string Name;
        public Expression Value;
        public DataType Type;

        public bool IsArray => Type is FixedArrayType;
        public DataType ElementType => (Type as FixedArrayType)?.ElementType;
        public Expression ArraySize => (Type as FixedArrayType)?.OptionalSize;

        public ShaderVariable(DataType dt, string name, Expression value)
        {
            Type = dt;
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}