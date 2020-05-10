using System;

namespace Uno.TestRunner.Loggers
{
    public class ConsoleWriter : IWriter
    {
        public void Write(string format = "", params object[] args)
        {
            Console.WriteLine(format, args);
        }
    }
}
