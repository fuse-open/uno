using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.API.Domain.IL.Statements
{
    public class ExternScope : Statement
    {
        public NewObject[] Attributes;
        public string String;
        public Expression Object;
        public Expression[] Arguments;
        public Namescope[] Scopes;

        public override StatementType StatementType => StatementType.ExternScope;

        public ExternScope(Source src, NewObject[] attributes, string str, Expression obj, Expression[] args, Namescope[] scopes)
            : base(src)
        {
            Attributes = attributes;
            String = str;
            Object = obj;
            Arguments = args;
            Scopes = scopes;
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            Arguments.Visit(p);
        }

        public override Statement CopyStatement(CopyState state)
        {
            return new ExternScope(Source, Attributes.Copy(state), String, Object.CopyNullable(state), Arguments.Copy(state), Scopes);
        }
    }
}
