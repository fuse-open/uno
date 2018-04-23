using System;
using System.IO;
using System.Linq;
using Uno.TestRunner.Loggers;

namespace Uno.CompilerTestRunner
{
    public class Program
    {
        static CompilerTestsRunner _compilerTestsRunner;

        static int Main(string[] args)
        {
            try
            {
                var testsPath = string.Format("{0}..{1}..{1}..{1}..{1}..{1}Tests{1}Compiler", AppDomain.CurrentDomain.BaseDirectory, Path.DirectorySeparatorChar);
                if (!Directory.Exists(testsPath))
                    testsPath = string.Format("{0}..{1}Tests{1}Compiler", AppDomain.CurrentDomain.BaseDirectory, Path.DirectorySeparatorChar);

                _compilerTestsRunner = new CompilerTestsRunner(testsPath, Filter(args));
                _compilerTestsRunner.Run(GetLogger(args));
                return 0;
            }
            catch (ArgumentException e)
            {
                Console.Error.WriteLine("Invalid argument: " + e.Message);
                return 1;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Exited with exception: " + e.Message);
                return 1;
            }
        }

        private static ITestResultLogger GetLogger(string[] args)
        {
            if (args.Contains("teamcity"))
                return new TeamCityResultLogger(new ConsoleWriter());

            return new ConsoleLogger(new ConsoleWriter());
        }

        private static string Filter(string[] args)
        {
            foreach (var arg in args)
            {
                if (arg.StartsWith("filter"))
                {
                    return arg.Split('=')[1];
                }
            }
            return null;
        }
    }
}