using System;

namespace Uno
{
    public class SourceException : Exception
    {
        public new readonly Source Source;

        public SourceException(Source src, string msg)
            : base(msg)
        {
            Source = src;
        }

        public SourceException(Source src, string msg, Exception innerException)
            : base(msg, innerException)
        {
            Source = src;
        }

        public override string ToString()
        {
            return $"{Source}: {base.ToString()}";
        }
    }
}