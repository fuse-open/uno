using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Cache;

namespace MyHttpLib
{
    public class HttpRequest
    {
        readonly HttpWebRequest _request;
        readonly IDictionary<string, string> _headers;
        public string Url { get; private set; }
        public string Method { get; set; }
        public string Referer { get; private set; }
        public string UserAgent { get; private set; }
        public string ContentType { get; set; }
        public Action<byte[]> OnDoneCallback { private get; set; }
        public Action<int, int> OnProgress { private get; set; }
        public Action<Exception> OnExeptionCallback { private get; set; }

        public Action<HttpWebResponse> OnHeadersReceived { private get; set; }

        public HttpRequest(string url)
        {
            _request = (HttpWebRequest)HttpWebRequest.Create(url);
            _request.AllowAutoRedirect = true;

            Url = url;
            _headers = new Dictionary<string, string>();
        }

        public void Send(byte[] data)
        {
            _request.Method = Method;
            _request.ContentType = ContentType;
            AttachHeadersToRequest();

            HttpHelper.Send(_request, data, OnHeadersReceived, OnDoneCallback, OnProgress, OnExeptionCallback);
        }

        public void SetCacheEnabled(bool isCacheEnabled)
        {
            _request.CachePolicy = isCacheEnabled
                ? HttpWebRequest.DefaultCachePolicy
                : new HttpRequestCachePolicy(HttpRequestCacheLevel.BypassCache);
        }

        public void SetHeader(string name, string value)
        {
            _headers.Add(name, value);
        }

        public string GetHeaders()
        {
            return _request.Headers.ToString();
        }

        public void Abort()
        {
            _request.Abort();
        }

        private void AttachHeadersToRequest()
        {
            foreach (var header in _headers)
            {
                switch (header.Key.ToLower())
                {
                    case "connection" :
                        AttachConnectionHeader(header.Value);
                        break;
                    case "content-length":
                        _request.ContentLength = long.Parse(header.Value);
                        break;
                    case "content-type":
                        _request.ContentType = header.Value;
                        break;
                    case "date":
                        _request.Date = DateTime.Parse(header.Value);
                        break;
                    case "accept":
                        _request.Accept = header.Value;
                        break;
                    case "expect":
                        AttachExpectHeader(header.Value);
                        break;
                    case "host":
                        _request.Host = header.Value;
                        break;
                    case "if-modified-since":
                        _request.IfModifiedSince = DateTime.Parse(header.Value);
                        break;
                    case "referer":
                        _request.Referer = header.Value;
                        break;
                    case "user-agent":
                        _request.UserAgent = header.Value;
                        break;
                    case "range":
                        AttachRangeHeader(header.Value);
                        break;
                    default:
                        _request.Headers.Add(header.Key, header.Value);
                        break;
                }
            }
        }

        private void AttachConnectionHeader(string headerValue)
        {
            if (headerValue.ToLower() == "keep-alive")
                _request.KeepAlive = true;
            else if (headerValue.ToLower() == "close")
                _request.KeepAlive = false;
            else
                _request.Connection = headerValue;
        }

        private void AttachRangeHeader(string headerValue)
        {
            var units = "bytes";
            var rangeString = headerValue;
            if (headerValue.Contains("="))
            {
                var arr = headerValue.Split('=');
                units = arr[0];
                rangeString = arr[1];
            }

            foreach(var rangeValue in rangeString.Split(','))
            {
                var lastDashPosition = rangeValue.LastIndexOf("-");
                if (lastDashPosition == rangeValue.Length - 1)
                {
                    var rangeStart = rangeValue.Substring(0, rangeValue.Length - 1);
                    _request.AddRange(units, long.Parse(rangeStart));
                }
                else if (lastDashPosition == 0)
                {
                    _request.AddRange(units, long.Parse(rangeValue));
                }
                else
                {
                    var rangeStart = rangeValue.Substring(0, lastDashPosition);
                    var rangeEnd = rangeValue.Substring(lastDashPosition + 1);
                    _request.AddRange(units, long.Parse(rangeStart), long.Parse(rangeEnd));
                }
            }
        }

        private void AttachExpectHeader(string headerValue)
        {
            if (headerValue.ToLower().Contains("100-continue"))
                _request.ServicePoint.Expect100Continue = true;
            else
            {
                _request.ServicePoint.Expect100Continue = false;
                _request.Expect = headerValue;
            }
        }

        public void SetTimeout(int timeoutInMilliseconds)
        {
            _request.Timeout = timeoutInMilliseconds;
        }
    }
}