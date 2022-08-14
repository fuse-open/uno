using System;
using System.Collections.Generic;
using System.Text;
using MyHttpLib;

namespace Uno.Net.Http.Implementation
{
    public static class CilHttpMessageHandler
    {
        static readonly List<Action> EventQueue = new List<Action>();

        internal static void AddEvent(Action action)
        {
            lock (EventQueue)
                EventQueue.Add(action);
        }

        public static void ProcessEvents()
        {
            Action[] events = null;
            lock (EventQueue)
            {
                events= EventQueue.ToArray();
                EventQueue.Clear();
            }

            foreach (var action in events)
                action();
        }
    }

    public interface IHttpRequest : IDisposable
    {
        void EnableCache(bool enableCache);

        void SetHeader(string name, string value);
        void SetTimeout(int timeoutInMilliseconds);

        void SendAsync(byte[] data);
        void SendAsync(string data);
        void SendAsync();

        void Abort();

        int GetResponseStatus();

        string GetResponseHeader(string name);
        string GetResponseHeaders();
        string GetResponseContentString();
        byte[] GetResponseContentByteArray();
    }

    public interface IHttpMessageHandlerRequestCallbacks
    {
        void OnAborted();
        void OnError(string errorMessage);
        void OnTimeout();
        void OnDone();
        void OnHeadersReceived();
        void OnProgress(int current, int total, bool hasTotal);
    }

    public class CilHttpRequest : IHttpRequest
    {
        public static CilHttpRequest Create(
            IHttpMessageHandlerRequestCallbacks requestCallbacks,
            string method, string url)
        {
            return new CilHttpRequest(requestCallbacks, method, url);
        }

        HttpRequest Request;
        IHttpMessageHandlerRequestCallbacks RequestCallbacks;

        HttpResponse Response;
        byte[] Content;

        CilHttpRequest(
            IHttpMessageHandlerRequestCallbacks requestCallbacks,
            string method, string url)
        {
            Request = new HttpRequest(url) { Method = method };
            RequestCallbacks = requestCallbacks;

            Request.OnHeadersReceived =
                webResponse => CilHttpMessageHandler.AddEvent(() =>
                {
                    Response = new HttpResponse(webResponse);
                    RequestCallbacks.OnHeadersReceived();
                });

            Request.OnProgress =
                (current, total) => CilHttpMessageHandler.AddEvent(() =>
            {
                RequestCallbacks.OnProgress(current, total, (total > 0));
            });

            Request.OnDoneCallback =
                content => CilHttpMessageHandler.AddEvent(() =>
                {
                    Content = content;
                    RequestCallbacks.OnDone();
                });

            Request.OnExeptionCallback =
                x => CilHttpMessageHandler.AddEvent(() =>
                {
                    RequestCallbacks.OnError(x.Message);
                });
        }

        public void Dispose()
        {
            Request = null;
            RequestCallbacks = null;
            Response = null;
            Content = null;
        }

        public void EnableCache(bool enableCache)
        {
            Request.SetCacheEnabled(enableCache);
        }

        public void SetHeader(string name, string value)
        {
            Request.SetHeader(name, value);
        }

        public void SetTimeout(int timeoutInMilliseconds)
        {
            Request.SetTimeout(timeoutInMilliseconds);
        }

        public void SendAsync(byte[] data)
        {
            Request.Send(data);
        }

        public void SendAsync(string data)
        {
            Request.Send(Encoding.UTF8.GetBytes(data));
        }

        public void SendAsync()
        {
            Request.Send(null);
        }

        public void Abort()
        {
            Request.Abort();
        }

        public int GetResponseStatus()
        {
            return (Response == null) ? 0 : Response.StatusCode;
        }

        public string GetResponseHeader(string name)
        {
            return (Response == null) ? "" : Response.GetHeader(name);
        }

        public string GetResponseHeaders()
        {
            return (Response == null) ? "" : Response.GetHeaders();
        }

        public string GetResponseContentString()
        {
            if(Content != null && Content.Length > 0)
                return Encoding.UTF8.GetString(Content);

            return "";
        }

        public byte[] GetResponseContentByteArray()
        {
            return Content ?? new byte[0];
        }
    }
}
