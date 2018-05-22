using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.Exception")]
    public class Exception
    {
        readonly string _message;
        public virtual string Message
        {
            get { return _message; }
        }

        readonly Exception _inner;
        public Exception InnerException
        {
            get { return _inner; }
        }

        string _trace;
        extern(CPLUSPLUS) public readonly IntPtr[] NativeStackTrace;

        public virtual string StackTrace
        {
            get { return _trace ?? "Uno.Exception.StackTrace is not supported in this build configuration"; }
        }

        public Exception()
            : this("")
        {
        }

        public Exception(string message)
            : this(message, null)
        {
        }

        public Exception(string message, Exception inner)
        {
            _message = message;
            _inner = inner;

            if defined(CPLUSPLUS)
            {
                _trace = extern<string> "uGetStackTrace()";
                NativeStackTrace = extern<IntPtr[]> "uGetNativeStackTrace(0)";
            }
        }

        public override string ToString()
        {
            var temp = GetType() + ": " + Message;

            if (_inner != null)
                temp = temp + " ---> " + _inner.ToString() + "\n    --- End of inner exception stack trace ---";

            if (string.IsNullOrEmpty(_trace))
                return temp;

            return temp + "\n" + _trace;
        }
    }
}
