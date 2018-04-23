namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public abstract class VoidExpression : Expression
    {
        public override DataType ReturnType => DataType.Void;

        protected VoidExpression(Source src)
            : base(src)
        {
        }
    }
}