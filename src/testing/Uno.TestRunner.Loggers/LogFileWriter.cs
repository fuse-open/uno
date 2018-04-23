using System;

namespace Uno.TestRunner.Loggers
{
    class LogFileWriter : IWriter
    {
        private readonly string _logName;

        public LogFileWriter(string logName)
        {
            _logName = logName;
            FileHelpers.WriteAllTextToSharedFile(_logName, "");
        }

        public void Write(string format = "", params object[] args)
        {
            var line = DateTime.Now.ToString("hh:mm:ss:ffff") + " "
                + string.Format(format, args)
                + Environment.NewLine;
            FileHelpers.AppendAllTextToSharedFile(_logName, line);
        }
    }
}
