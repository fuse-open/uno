using Uno.Compiler;
using Uno.Logging;

namespace Uno.UX.Markup.CompilerReflection
{
    public class MarkupErrorLog : Common.IMarkupErrorLog
    {
        readonly Log _log;
        readonly SourceBundle _bundle;
        public MarkupErrorLog(Log log, SourceBundle bundle)
        {
            _log = log;
            _bundle = bundle;
        }

        public void ReportError(string message)
        {
            _log.Error(Source.Unknown, ErrorCode.E8001, message);
        }

        public void ReportError(string path, int line, string message)
        {
            _log.Error(new Source(_bundle, path, line), ErrorCode.E8001, message);
        }

        public void ReportWarning(string message)
        {
            _log.Warning(Source.Unknown, ErrorCode.W8002, message);
        }

        public void ReportWarning(string path, int line, string message)
        {
            _log.Warning(new Source(_bundle, path, line), ErrorCode.W8002, message);
        }
    }
}
