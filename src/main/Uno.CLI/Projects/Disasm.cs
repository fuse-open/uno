using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Uno.Configuration;
using Uno.Diagnostics;

namespace Uno.CLI.Projects
{
    public class Disasm : Command
    {
        public static string Exe => UnoConfig.Current.GetFullPath("Assemblies.Disasm");
        public static string App => UnoConfig.Current.GetFullPath("Apps.Disasm");
        public static int Port => UnoConfig.Current.GetInt("DisasmPort", 12700);

        public override string Name => "disasm";
        public override string Description => "Open frontend for inspecting generated code.";
        public override bool IsExperimental => true;

        public override void Help()
        {
            WriteUsage("[options] [project-path]");
            WriteLine("This command uses the same options as build -- see \"uno build --help\".");
        }

        public override void Execute(IEnumerable<string> args)
        {
            var input = GetAllArguments(args, "-f", "--cd=\"" + Directory.GetCurrentDirectory() + "\"");
            // This method verifies args
            BuildCommand.Parse(input);

            if (TryConnectAndSendArgs(input))
                return;

            if (PlatformDetection.IsMac)
                Shell.Open(App, "--args " + input.JoinArguments());
            else if (PlatformDetection.IsWindows)
                Shell.Open(Exe, input.JoinArguments());
            else
                throw new NotSupportedException();
        }

        public bool TryConnectAndSendArgs(IReadOnlyList<string> args)
        {
            try
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {
                    ReceiveTimeout = 200
                })
                {
                    socket.Connect(new IPEndPoint(IPAddress.Loopback, Port));

                    Log.Verbose("Sending arguments to " + socket.RemoteEndPoint.Quote());

                    using (var ms = new MemoryStream())
                    {
                        var w = new BinaryWriter(ms);
                        w.Write(0x12345678);
                        w.Write(args.Count);

                        foreach (var arg in args)
                            w.Write(arg);

                        w.Flush();
                        socket.Send(ms.GetBuffer(), (int) ms.Length, SocketFlags.None);
                    }

                    var byteBuffer = new byte[1024];
                    return socket.Receive(byteBuffer) == 1
                        && byteBuffer[0] == 0;
                }
            }
            catch (SocketException)
            {
                return false;
            }
            catch (Exception e)
            {
                Log.Warning(e.Message);
            }

            return false;
        }
    }
}
