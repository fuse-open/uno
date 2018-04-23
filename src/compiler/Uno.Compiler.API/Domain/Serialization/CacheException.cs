using System;

namespace Uno.Compiler.API.Domain.Serialization
{
    public class CacheException : Exception
    {
        public CacheException(string msg)
            : base("Corrupt cache: " + msg)
        {
        }
    }
}