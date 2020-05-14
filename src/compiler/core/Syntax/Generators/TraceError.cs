using Uno.Compiler.API.Domain.Graphics;

namespace Uno.Compiler.Core.Syntax.Generators
{
    class TraceError
    {
        public readonly Source Source;
        public readonly ErrorCode ErrorCode;
        public readonly string Message;
        public readonly ReqStatement ReqStatement;
        public readonly MetaLocation[] Stack;

        public TraceError(Source src, ErrorCode code, string msg, ReqStatement req, MetaLocation[] stack)
        {
            Source = src;
            ErrorCode = code;
            Message = msg;
            ReqStatement = req;
            Stack = stack;
        }
    }
}