using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public abstract class CallExpression : Expression, IMemberReference
    {
        public Expression Object;
        public Variable Storage;
        public override DataType ReturnType => DataType.Void;
        public DataType ReferencedType => Function.DeclaringType;
        public Member ReferencedMember => Function;
        public abstract Function Function { get; }

        protected CallExpression(Source src)
            : base(src)
        {
        }
    }
}