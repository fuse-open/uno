namespace Uno.Net.Http
{
    public enum HttpRequestState
    {
        Uninitialized = 0,
        Opened,
        Sent,
        HeadersReceived,
        Loading,

        Done,

        Aborted,
        Errored,
        TimedOut,

        // Legacy
        Unsent = Uninitialized
    }
}
