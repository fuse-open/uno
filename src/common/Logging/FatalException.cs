using System;

namespace Uno.Logging
{
    public class FatalException : SourceException
    {
        public readonly object ErrorCode;

        public FatalException(Source src, object code, string msg, Exception innerException = null)
            : base(src, msg, innerException)
        {
            ErrorCode = code;
        }
    }
}