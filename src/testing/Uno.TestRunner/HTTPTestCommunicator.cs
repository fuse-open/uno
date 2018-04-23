using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Uno.TestRunner.Loggers;

namespace Uno.TestRunner
{
    public class HttpTestCommunicator
    {
        public string Prefix { get; private set; }
        private readonly HttpListener _httpListener;
        private readonly ITestResultLogger _logger;
        private readonly TestRun _testRun;
        private readonly bool _needsPublicIP;
        private readonly ManualResetEvent _idle = new ManualResetEvent(true);
        private readonly ManualResetEvent _terminate = new ManualResetEvent(false);
        private Thread _workerThread;

        public HttpTestCommunicator(ITestResultLogger logger, TestRun testRun, bool needsPublicIP = false)
        {
            _httpListener = new HttpListener();
            _logger = logger;
            _testRun = testRun;
            _needsPublicIP = needsPublicIP;
        }

        public void Start()
        {
            var prefix = $@"http://{GetIP()}:{HttpHelpers.GetRandomUnusedPort()}/";
            _httpListener.Prefixes.Add(prefix);
            _httpListener.Start();

            _workerThread = new Thread(WorkerThread);
            _workerThread.Start();

            var task = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        var context = _httpListener.GetContext();
                        try
                        {
                            _pendingRequests.Enqueue(context.Request.QueryString);
                        }
                        finally
                        {
                            context.Response.Close();
                        }
                    }
                    catch (HttpListenerException e)
                    {
                        if (e.ErrorCode == 995)
                            break;

                        throw e;
                    }
                }
            });

            Prefix = prefix;
            _logger.Log("Started test runner on " + Prefix);
        }

        private string GetIP()
        {
            if (!_needsPublicIP)
                return "localhost";

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // The network cards of virtual machines doesn't have a default gateway, so let's use
                // that as a reasonable heuristic to find the right card. Not iron-clad, but seems to
                // eliminate at least one class of problems.
                var addr = ni.GetIPProperties().GatewayAddresses.FirstOrDefault();
                if (addr != null)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            return ip.Address.ToString();
                    }
                }
            }

            throw new Exception("Could not get local IP address.");
        }

        public void Stop()
        {
            if (_httpListener.IsListening)
                _httpListener.Stop();

            _terminate.Set();
            _workerThread.Join();
        }

        ConcurrentQueue<NameValueCollection> _pendingRequests = new ConcurrentQueue<NameValueCollection>();
        public void HandleRequest(HttpListenerContext context)
        {
        }

        void WorkerThread()
        {
            try
            {
                var sortedRequests = new SortedList<int, NameValueCollection>();
                int expectedSequenceId = 0;

                while (!_testRun.IsFinished() && !_terminate.WaitOne(0))
                {
                    NameValueCollection request;
                    while (_pendingRequests.TryDequeue(out request))
                    {
                        int sequenceId = int.Parse(request.Get("sequenceId"));
                        sortedRequests.Add(sequenceId, request);
                    }

                    while (sortedRequests.TryGetValue(expectedSequenceId, out request))
                    {
                        _testRun.EventOccured(request);

                        sortedRequests.Remove(expectedSequenceId++);
                    }
                }

                _idle.Set();

                if (!_pendingRequests.IsEmpty || sortedRequests.Count > 0)
                    throw new Exception("TestRun finished, but there's still pending requests!");
            }
            catch (Exception e)
            {
                _testRun.ErrorOccured(e);
            }
        }

        public void WaitUntilIdle()
        {
            _idle.WaitOne();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _idle.Dispose();
        }
    }

}
