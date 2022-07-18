namespace Uno.Time.Text
{
    public sealed class ParseResult<T>
    {
        private readonly T _value;
        private readonly Exception _exception;

        private ParseResult(Exception exception)
        {
            _exception = exception;
        }

        private ParseResult(T value)
        {
            _value = value;
        }

        public T Value { get { return GetValueOrThrow(); } }

        public Exception Exception
        {
            get
            {
                if (_exception == null)
                {
                    throw new InvalidOperationException("Parse operation succeeded, so no exception is available");
                }
                return _exception;
            }
        }

        public bool Success { get { return _exception == null; } }

        public T GetValueOrThrow()
        {
            if (_exception == null)
            {
                return _value;
            }
            throw _exception;
        }

        public bool TryGetValue(T failureValue, out T result)
        {
            bool success = _exception == null;
            result = success ? _value : failureValue;
            return success;
        }

        internal static ParseResult<T> ForValue(T value)
        {
            return new ParseResult<T>(value);
        }

        internal static ParseResult<T> ForException(Exception exception)
        {
            return new ParseResult<T>(exception);
        }
    }
}
