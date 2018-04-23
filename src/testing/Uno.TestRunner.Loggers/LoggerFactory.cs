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
            switch (options.Reporter.ToLower())
            {
                case "teamcity":
                    return new TeamCityResultLogger(writer, options.Trace);
                case "console":
                case "":
                    return new ConsoleLogger(writer, options.Trace);
                default:
                    throw new ArgumentException("Unknown reporter type {0}", options.Reporter);
            }
        }
    }
}
