using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Statements;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        public Expression CompileCondition(AstExpression e)
        {
            return CompileImplicitCast(e.Source, Essentials.Bool, CompileExpression(e));
        }

        public Scope CompileScope(AstScope scope)
        {
            return CompileScope(scope.Source, scope);
        }

        public Scope CompileScope(Source src, AstScope s)
        {
            var vscope = new VariableScope();
            var scope = new Scope(src);

            VariableScopeStack.Add(vscope);

            foreach (var e in s.Statements)
                scope.Statements.Add(CompileStatement(e));

            if (s.IsClosed)
            {
                VariableScopeStack.Remove(vscope);
                CurrentVariableScope.Scopes.Add(vscope);
            }

            return scope;
        }
    }
}
