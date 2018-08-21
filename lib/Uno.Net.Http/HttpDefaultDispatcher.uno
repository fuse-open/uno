using Uno;
using Uno.Threading;

namespace Uno.Net.Http
{
    internal class HttpDefaultDispatcher : IDispatcher
    {
        public HttpDefaultDispatcher()
        {

        }

        public void Invoke(Action action)
        {
            action();
        }
    }
}