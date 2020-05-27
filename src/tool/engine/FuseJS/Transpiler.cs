using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Uno.Configuration;
using Uno.Diagnostics;
using Uno.Logging;
using System.Text;
using System.Text.RegularExpressions;

namespace Uno.Build.FuseJS
{
    public class Transpiler : LogObject, IDisposable
    {
        readonly Task<int> _task;
        readonly StringBuilder _output = new StringBuilder();
        readonly Regex _portFinder = new Regex("port:([0-9]+)");
        string _url;
        bool _disposed;
        bool _isFaulted;

        public Transpiler(Log log, UnoConfig config)
            : base(log)
        {
            var script = Path.Combine(
                config.GetNodeModuleDirectory("@fuse-open/transpiler"),
                "server.min.js");

            _task = new Shell(log).Start(
                "node",
                script.QuoteSpace(),
                outputReceived: (sender, args) => {
                    string port;
                    if (TryMatchPort(args.Data, out port))
                        _url = "http://127.0.0.1:" + port;

                    _output.AppendLine(args.Data);
                },
                errorReceived: (sender, args) => {
                    _output.AppendLine(args.Data);
                });
        }

        bool TryMatchPort(string data, out string port)
        {
            var match = _portFinder.Match(data ?? "");
            port = match.Success ? match.Groups[1].Value : "";
            return string.IsNullOrEmpty(port) == false;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            Log.VeryVerbose("Stopping transpiler server...");
            if (_isFaulted == false && _url != null)
                PostRequest(new {quit = true});
            _disposed = true;
        }

        public bool TryTranspile(string filename, string code, out string output)
        {
            output = "";
            if (_isFaulted)
                return false;

            if (CheckIfTranspilerServerIsDown())
            {
                _isFaulted = true;
                return false;
            }

            if (WaitForServer() == false)
                return false;

            var json = PostRequest(new {filename, code});

            if (json.TryGetValue("code", out output))
                return true;

            if (json.TryGetValue("message", out output))
                Log.Error(output);

            return false;
        }

        bool WaitForServer()
        {
            var i = 0;
            while (_url == null && i < 14)
            {
                Thread.Sleep(1 << i);
                ++i;
            }

            if (_url == null)
            {
                Log.Error("Transpiler doesn't run for unknown reasons.\n" + _output.ToString());
                return false;
            }

            return true;
        }

        bool CheckIfTranspilerServerIsDown()
        {
            if (_task.IsFaulted)
            {
                if (_task.Exception != null)
                    Log.Error("Failed to start transpiler server. " + (_task.Exception.InnerException != null ? _task.Exception.InnerException.Message : _task.Exception.Message) + ".");
                else
                    Log.Error("Failed to start transpiler server for unknown reasons.");

                Log.Message("Make sure 'node' is installed and in your PATH.");
                return true;
            }

            if (_task.IsCanceled)
            {
                Log.Error("Starting the transpiler server was canceled!");
                return true;
            }

            if (_task.IsCompleted)
            {
                Log.Error("Transpiler server is down and exit code was '" + _task.Result + "'");
                return true;
            }

            return false;
        }

        Dictionary<string, string> PostRequest(object json)
        {
            for (int i = 0;; i++)
            {
                if (_disposed)
                    throw new ObjectDisposedException("FuseJS server was stopped");

                try
                {
                    var httpWebRequest = (HttpWebRequest) WebRequest.Create(_url);
                    httpWebRequest.Timeout = (int) TimeSpan.FromSeconds(30).TotalMilliseconds;
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(JsonConvert.SerializeObject(json));
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                    using (var streamReader = new StreamReader(httpWebRequest.GetResponse().GetResponseStream()))
                    {
                        var response = streamReader.ReadToEnd();
                        return !string.IsNullOrEmpty(response)
                            ? JsonConvert.DeserializeObject<Dictionary<string, string>>(response)
                            : new Dictionary<string, string>();
                    }
                }
                catch (WebException e)
                {
                    if (i == 9)
                        throw;

                    Log.VeryVerbose("Failed to connect to transpiler server, retrying... (" + e.Message + ")");
                    Thread.Sleep(200);
                }
            }
        }
    }
}
