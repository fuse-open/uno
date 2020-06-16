using System;
using Uno.TestRunner.BasicTypes;

namespace Uno.TestRunner.Loggers
{
    public static class LoggerFactory
    {
        public static ITestResultLogger CreateLogger(TestOptions options)
        {
            IWriter writer;
            if (string.IsNullOrEmpty(options.LogFile))
            {
                writer = new ConsoleWriter();
            }
            else
            {
                writer = new LogFileWriter(options.LogFile);
            }

            return new ConsoleLogger(writer, options.Trace);
        }
    }
}
