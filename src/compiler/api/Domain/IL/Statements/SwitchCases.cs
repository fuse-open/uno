namespace Uno.Compiler.API.Domain.IL.Statements
{
    public static class SwitchCases
    {
        public static SwitchCase[] Copy(this SwitchCase[] c, CopyState state)
        {
            var r = new SwitchCase[c.Length];

            for (var i = 0; i < c.Length; i++)
                r[i] = c[i].Copy(state);

            return r;
        }
    }
}