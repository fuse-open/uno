using System.Collections.Generic;
using System.Text;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class ExtensionGroup : Expression
    {
        public Expression Object;
        public readonly IReadOnlyList<Method> Candidates;

        public string CandidatesBaseName
        {
            get
            {
                if (Candidates.Count > 0)
                    return Candidates[0].Name;

                return "<extension group>";
            }
        }

        public override DataType ReturnType => DataType.MethodGroup;

        public ExtensionGroup(Source src, Expression obj, IReadOnlyList<Method> candidates)
            : base(src)
        {
            Object = obj;
            Candidates = candidates;
        }

        public override ExpressionType ExpressionType => ExpressionType.ExtensionGroup;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("<extension group>");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            if (Object != null)
            {
                p.Begin(ref Object);
                Object.Visit(p);
                p.End(ref Object);
            }
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new ExtensionGroup(Source, Object.CopyNullable(state), Candidates.Copy(state));
        }
    }
}