using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST.Statements
{
    public sealed class AstVariableDeclaration : AstStatement
    {
        public readonly AstVariableModifier Modifier;
        public readonly AstExpression Type;
        public readonly IReadOnlyList<AstVariable> Variables;

        public override AstStatementType StatementType => AstStatementType.VariableDeclaration;

        public AstVariableDeclaration(AstVariableModifier modifier, AstExpression type, AstIdentifier name, AstExpression value = null)
            : base(name.Source)
        {
            Modifier = modifier;
            Type = type;
            Variables = new[] {new AstVariable(name, value)};
        }

        public AstVariableDeclaration(AstVariableModifier modifier, AstExpression type, IReadOnlyList<AstVariable> variables)
            : base(variables[0].Name.Source)
        {
            Modifier = modifier;
            Type = type;
            Variables = variables;
        }
    }
}