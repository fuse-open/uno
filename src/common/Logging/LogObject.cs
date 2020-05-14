namespace Uno.Logging
{
    public class LogObject
    {
        public readonly Log Log;

        public LogObject(Log log)
        {
            Log = log ?? Log.Default;
        }

        public LogObject(LogObject obj)
        {
            Log = obj?.Log ?? Log.Default;
        }
    }
}
