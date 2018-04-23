using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public static class Expressions
    {
        public static readonly Expression[] Empty = new Expression[0];

        public static Expression[] Concat(this Expression[] e, Expression value)
        {
            var r = new Expression[e.Length + 1];

            for (int i = 0; i < e.Length; i++)
                r[i] = e[i];

            r[e.Length] = value;
            return r;
        }

        public static Expression[] Copy(this Expression[] e, CopyState state)
        {
            if (e == null)
                return null;
            if (e.Length == 0)
                return e;

            var r = new Expression[e.Length];

            for (int i = 0; i < e.Length; i++)
                r[i] = e[i].CopyExpression(state);

            return r;
        }

        public static NewObject[] Copy(this NewObject[] e, CopyState state)
        {
            if (e.Length == 0)
                return e;

            var r = new NewObject[e.Length];

            for (int i = 0; i < e.Length; i++)
                r[i] = (NewObject) e[i].CopyExpression(state);

            return r;
        }

        public static Constant[] Copy(this Constant[] c, CopyState state)
        {
            if (c.Length == 0)
                return c;

            var r = new Constant[c.Length];

            for (var i = 0; i < c.Length; i++)
                r[i] = (Constant)c[i].CopyExpression(state);

            return r;
        }

        public static void Visit(this Expression[] e, Pass p)
        {
            for (int i = 0; i < e.Length; i++)
                p.VisitNullable(ref e[i]);
        }

        public static void Disassemble(this Expression[] args, StringBuilder sb, string begin = "(", string end = ")", bool beginWithComma = false)
        {
            sb.Append(begin);

            for (int i = 0; i < args.Length; i++)
            {
                sb.CommaWhen(i > 0 || beginWithComma);
                args[i].Disassemble(sb);
            }

            sb.Append(end);
        }

        public static string Disassemble(this Expression[] args, string begin = "(", string end = ")", bool beginWithComma = false)
        {
            var sb = new StringBuilder();
            Disassemble(args, sb, begin, end, beginWithComma);
            return sb.ToString();
        }
    }
}