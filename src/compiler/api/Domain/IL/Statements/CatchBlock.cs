using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL.Statements
{
    public class CatchBlock
    {
        public Source Source;
        public Variable Exception;
        public Scope Body;

        public CatchBlock(Source src, Variable exception, Scope body)
        {
            Source = src;
            Exception = exception;
            Body = body;
        }

        public CatchBlock Copy(CopyState state)
        {
            return new CatchBlock(Source, Exception.Copy(state), (Scope)Body.CopyStatement(state));
        }
    }
}