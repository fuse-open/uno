using System;
using System.Net;
using Uno.TestRunner.BasicTypes;

namespace Uno.TestRunner
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                var options = CommandLineOptions.From(args);
                return options == null ? -1 : UnoTest.DiscoverAndRun(options) ? 1 : 0;
            }
            catch (HttpListenerException e)
            {
                Console.Error.WriteLine("Failed to open network socket for test communication: " + e.Message);
                Console.Error.WriteLine(
                    "If you're trying to run your tests on an external device, remember to run UnoTest as Administrator.");
                return 1;
            }
            catch (ArgumentException e)
            {
                Console.Error.WriteLine("Invalid argument: " + e.Message);
                return 1;
            }
            catch (AggregateException)
            {
                Console.Error.WriteLine("Internal error(s) occured.");
                return 1;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Exited with exception: " + e.Message);
                return 1;
            }
        }
    }
}
