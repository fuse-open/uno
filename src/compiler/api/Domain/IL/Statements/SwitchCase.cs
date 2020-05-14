using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.API.Domain.IL.Statements
{
    public class SwitchCase
    {
        public Constant[] Values;
        public bool HasDefault;
        public Scope Scope;

        public SwitchCase(Constant[] values, bool hasDefault, Scope scope)
        {
            Values = values;
            HasDefault = hasDefault;
            Scope = scope;
        }

        public SwitchCase Copy(CopyState state)
        {
            return new SwitchCase(Values.Copy(state), HasDefault, (Scope)Scope.CopyStatement(state));
        }
    }
}