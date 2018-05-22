namespace Uno.Testing
{
    internal interface ITestRunnerMessageDispatcher
    {
        void Start();
        void Stop();
        void Get(string uri);
    }
}
