using System;

namespace Uno.Compiler.API.Domain.IL
{
    public class CopyException : Exception
    {
        public CopyException(string message)
            : base(message)
        {
        }
    }
}