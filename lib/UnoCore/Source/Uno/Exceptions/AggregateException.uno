using Uno.Compiler.ExportTargetInterop;
using Uno.Collections;
using Uno.Text;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.AggregateException")]
    public class AggregateException : Exception
    {
        Exception[] _innerExceptions;
        public ReadOnlyCollection<Exception> InnerExceptions
        {
            get
            {
                var wrapper = new Uno.Internal.ArrayList<Exception>(_innerExceptions);
                return new ReadOnlyCollection<Exception>(wrapper);
            }
        }

        public AggregateException()
            : this("One or more errors occurred.")
        {
        }

        public AggregateException(Exception[] innerExceptions)
            : this("One or more errors occurred.", innerExceptions)
        {
        }

        public AggregateException(string message)
            : base(message)
        {
        }

        public AggregateException(string message, Exception[] innerExceptions)
            : base(message)
        {
            _innerExceptions = innerExceptions;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < _innerExceptions.Length; ++i)
            {
                sb.Append(string.Format("---> (Inner Exception #{0}) ", i));
                sb.Append(_innerExceptions[i].ToString());
                sb.Append("<---\n");

                if (i != _innerExceptions.Length - 1)
                    sb.Append("\n");
            }

            return base.ToString() + "\n" + sb.ToString();
        }
    }
}
