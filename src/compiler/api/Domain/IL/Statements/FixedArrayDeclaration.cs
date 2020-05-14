using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL.Statements
{
    public class FixedArrayDeclaration : Statement
    {
        public readonly Variable Variable;
        public readonly Expression[] OptionalInitializer;

        public override StatementType StatementType => StatementType.FixedArrayDeclaration;

        public FixedArrayDeclaration(Variable var, Expression[] optionalInitializer)
            : base(var.Source)
        {
            Variable = var;
            OptionalInitializer = optionalInitializer;
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            if (OptionalInitializer != null)
            {
                for (int i = 0; i < OptionalInitializer.Length; i++)
                {
                    p.Next(this);
                    p.VisitNullable(ref OptionalInitializer[i]);
                }
            }
        }

        public override Statement CopyStatement(CopyState state)
        {
            return new FixedArrayDeclaration(
                Variable.Copy(state), 
                OptionalInitializer.Copy(state));
        }
    }
}