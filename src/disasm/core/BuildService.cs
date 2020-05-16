using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Uno.Build;
using Uno.CLI.Projects;
using Uno.Disasm.ILView;
using Uno.Logging;

namespace Uno.Disasm
{
    public class BuildService : IDisposable
    {
        readonly Socket _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        readonly IILView _view;
        readonly IBuildLog _buildLog;
        readonly Log _log;
        BuildArguments _args;
        BuildItem _item;
        Thread _thread;

        public bool IsBuilding => _thread != null;

        public BuildService(IILView view, IBuildLog buildLog)
        {
            _view = view;
            _buildLog = buildLog;
            _log = new Log(buildLog.Writer);
            _log.Level++;
            Listen();
        }

        public void Dispose()
        {
            _listener.Dispose();
        }

        public void StartBuild(IEnumerable<string> args)
        {
            if (!SetBuildArgs(args))
                return;

            StartBuild();
        }

        public void StartBuild(BuildTarget target)
        {
            if (!CheckBuildArgs())
                return;

            _args.Target = target;
            StartBuild();
        }

        public void StartBuild()
        {
            if (!CheckBuildArgs())
                return;

            StartAsync(
                _args.Target.Identifier,
                build => new ILItemBuilder(build)
                    .Build(BuildCommand.Build(_args)));
        }

        public void StartClean()
        {
            if (!CheckBuildArgs())
                return;

            StartAsync(
                "Clean Project", 
                build => new ProjectCleaner(_log).Clean(_args.ProjectFile));
        }

        bool CheckBuildArgs()
        {
            if (string.IsNullOrEmpty(_args.ProjectFile))
            {
                string filename;
                return _view.TryOpenProject(out filename) && SetBuildArgs(new[] { filename });
            }

            return true;
        }

        bool SetBuildArgs(IEnumerable<string> args)
        {
            Cancel();

            try
            {
                _args = BuildCommand.Parse(args, _log);
                _args.Options.Native = false;
                _args.Options.PrintInternals = true;
                _view.OnProjectChanged(_args.ProjectFile);
                return true;
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
                return false;
            }
        }

        void StartAsync(string name, Action<BuildItem> action)
        {
            Cancel();
            _item = new BuildItem(name, _args.Options.Configuration.ToString());
            _view.OnBuildStarting(_item);

            lock (this)
                (_thread = new Thread(
                    () =>
                    {
                        try
                        {
                            action(_item);
                        }
                        catch (Exception e)
                        {
                            _log.Error(e.Message);
                        }
                        finally
                        {
                            lock (this)
                                _thread = null;

                            _view.BeginInvoke(_view.OnBuildFinished, _item);
                            _item = null;
                        }
                    }))
                    .Start();
        }

        public void Cancel()
        {
            if (_thread != null)
            {
                lock (this)
                    _thread.Abort();

                while (_thread != null)
                    Thread.Sleep(100);

                if (_item != null)
                    _view.OnBuildFinished(_item);

                _item = null;
            }

            _buildLog.Clear();
            _log.Flush();
        }

        void Listen()
        {
            new Thread(
                () =>
                {
                    try
                    {
                        _listener.Bind(new IPEndPoint(IPAddress.Loopback, 12345));
                        _listener.Listen(10);

                        while (true)
                        {
                            using (var socket = _listener.Accept())
                            {
                                try
                                {
                                    var byteBuffer = new byte[4096];
                                    var byteCount = socket.Receive(byteBuffer);

                                    if (byteCount > 0)
                                    {
                                        using (var ms = new MemoryStream(byteBuffer, 0, byteCount))
                                        {
                                            var r = new BinaryReader(ms);
                                            var magic = r.ReadUInt32();

                                            if (magic != 0x12345678)
                                                throw new Exception("Invalid magic number");

                                            var argCount = r.ReadInt32();
                                            var args = new string[argCount];

                                            for (int i = 0; i < argCount; i++)
                                                args[i] = r.ReadString();

                                            _view.BeginInvoke(StartBuild, args);
                                        }
                                    }

                                    socket.Send(new byte[] { 0 });
                                }
                                catch (Exception e)
                                {
                                    _log.Error("Socket failed: " + e.Message);
                                    socket.Send(new byte[] { 1 });
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _log.Error("Server failed: " + e.Message);
                    }
                }).Start();
        }
    }
}
