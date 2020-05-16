namespace Uno.CompilerTestRunner
{
    public class ErrorItem
    {
        public readonly string ErrorCode;
        public readonly Source Source;
        public readonly string Message;
        public readonly string Expression;
        public readonly ErrorType Type;

        public ErrorItem(ErrorType type, string message, Source src = null, string code = "E0000", string expression = null)
            : this(message, src, code, expression)
        {
            Type = type;
        }

        public ErrorItem(string message, Source src = null, string code = "E0000", string expression = null)
        {
            Message = message;
            Source = src ?? Source.Unknown;
            ErrorCode = code;
            Expression = expression;
        }

        public override string ToString()
        {
            return string.Format("{0}. {1}", ErrorCode, Message);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return Equals(obj as ErrorItem);
        }

        public bool Equals(ErrorItem other)
        {
            if (other == null)
            {
                return false;
            }
            return ErrorCode == other.ErrorCode &&
                (Message == other.Message || Message == null || other.Message == null) &&
                (Source == other.Source || Source == null || other.Source == null);
        }

        public override int GetHashCode()
        {
            return ErrorCode.GetHashCode();
        }
    }
}