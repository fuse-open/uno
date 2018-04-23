using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public abstract class FieldExpression : Expression, IMemberReference
    {
        public Expression Object;
        public Field Field;

        public Member ReferencedMember => Field;
        public DataType ReferencedType => Field.DeclaringType;
        public override DataType ReturnType => Field.ReturnType;

        protected FieldExpression(Source src, Expression obj, Field field)
            : base(src)
        {
            Object = obj;
            Field = field;
        }
    }
}