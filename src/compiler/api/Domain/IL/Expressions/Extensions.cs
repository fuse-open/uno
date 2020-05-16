namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public static class Extensions
    {
        public static void VisitNullable(this Pass p, ref Expression e, ExpressionUsage u = ExpressionUsage.Argument)
        {
            if (e != null)
            {
                p.Begin(ref e, u);
                e.Visit(p, u);
                p.End(ref e, u);
            }
        }

        public static bool IsObject(this ExpressionUsage u)
        {
            return u == ExpressionUsage.Object;
        }

        public static bool IsOperand(this ExpressionUsage u)
        {
            return u == ExpressionUsage.Operand;
        }
    }
}
