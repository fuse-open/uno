using System;
using Uno.TestRunner.BasicTypes;

namespace Uno.TestRunner.Loggers
{
    public static class LoggerFactory
    {
        public static ITestResultLogger CreateLogger(CommandLineOptions options)
        {
            IWriter writer;
            if (string.IsNullOrEmpty(options.LogFileName))
            {
                writer = new ConsoleWriter();
            }
            else
            {
                writer = new LogFileWriter(options.LogFileName);
            }

            return new ConsoleLogger(writer, options.Trace);
        }
    }
}
