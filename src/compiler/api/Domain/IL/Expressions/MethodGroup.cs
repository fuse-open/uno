using System.Collections.Generic;
using System.Text;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class MethodGroup : Expression
    {
        public Expression Object;
        public readonly bool IsQualified;
        public readonly IReadOnlyList<Method> Candidates;

        public string CandidatesBaseName
        {
            get
            {
                if (Candidates.Count > 0)
                    return Candidates[0].Name;
                else
                    return "<method group>";
            }
        }

        public override DataType ReturnType => DataType.MethodGroup;

        public MethodGroup(Source src, Expression obj, bool qualified, IReadOnlyList<Method> candidates)
            : base(src)
        {
            Object = obj;
            IsQualified = qualified;
            Candidates = candidates;
        }

        public override ExpressionType ExpressionType => ExpressionType.MethodGroup;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("<method group>");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            if (Object != null)
            {
                p.Begin(ref Object, ExpressionUsage.Object);
                Object.Visit(p, ExpressionUsage.Object);
                p.End(ref Object, ExpressionUsage.Object);
            }
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new MethodGroup(Source, Object.CopyNullable(state), IsQualified, Candidates.Copy(state));
        }
    }
}