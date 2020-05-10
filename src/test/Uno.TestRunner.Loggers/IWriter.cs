namespace Uno.TestRunner.Loggers
{
    public interface IWriter
    {
        void Write(string format = "", params object[] args);
    }
}