using System;

namespace Uno.Logging
{
    class LogProfiler : IDisposable
    {
        readonly Log _log;
        readonly double _startTime;
        readonly object _typeObject;

        public LogProfiler(Log log, object typeObject)
        {
            _log = log;
            _typeObject = typeObject;
            _startTime = log.Time;
        }

        public void Dispose()
        {
            if (!_log.IsVeryVerbose)
                return;

            lock (_log.Lock)
            {
                _log.Flush();
                _log.Write($"{(_log.Time - _startTime) * 1000.0,10:0.00} ms  ", ConsoleColor.Magenta);
                _log.WriteLine(_typeObject, ConsoleColor.DarkMagenta);
            }
        }
    }
}