using System.Text;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.API.Domain.IL
{
    public abstract class Expression : Statement
    {
        public static readonly InvalidExpression Invalid = new InvalidExpression();

        public abstract ExpressionType ExpressionType { get; }
        public abstract DataType ReturnType { get; }

        public bool IsInvalid => this is InvalidExpression || ReturnType is InvalidType;
        public Expression Address => !ReturnType.IsReferenceType ? this as AddressOf ?? new AddressOf(this) : ActualValue;
        public virtual Expression ActualValue => this;
        public virtual object ConstantValue => null;
        public string ConstantString => ConstantValue as string ?? ConstantValue?.ToString();
        public override StatementType StatementType => StatementType.Expression;

        protected Expression(Source src)
            : base(src)
        {
        }

        public abstract Expression CopyExpression(CopyState state);
        public abstract void Disassemble(StringBuilder sb, ExpressionUsage u = ExpressionUsage.Argument);

        public override void Visit(Pass p, ExpressionUsage u = ExpressionUsage.Argument)
        {
        }

        public override Statement CopyStatement(CopyState state)
        {
            return CopyExpression(state);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            Disassemble(sb, ExpressionUsage.Statement);
            return sb.ToString();
        }
    }
}
