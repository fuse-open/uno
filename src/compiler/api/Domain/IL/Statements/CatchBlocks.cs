namespace Uno.Compiler.API.Domain.IL.Statements
{
    public static class CatchBlocks
    {
        public static CatchBlock[] Copy(this CatchBlock[] c, CopyState state)
        {
            var r = new CatchBlock[c.Length];

            for (var i = 0; i < c.Length; i++)
                r[i] = c[i].Copy(state);

            return r;
        }
    }
}