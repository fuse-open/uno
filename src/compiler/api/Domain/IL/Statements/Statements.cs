namespace Uno.Compiler.API.Domain.IL.Statements
{
    public static class Statements
    {
        public static Statement[] CopyStatements(this Statement[] list, CopyState s)
        {
            var result = new Statement[list.Length];

            for (var i = 0; i < list.Length; i++)
                result[i] = list[i].CopyStatement(s);

            return result;
        }
    }
}