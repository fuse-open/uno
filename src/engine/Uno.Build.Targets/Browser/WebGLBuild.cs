using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.Backends.JavaScript;
using Uno.Compiler.Backends.OpenGL;
using Uno.Diagnostics;

namespace Uno.Build.Targets.Browser
{
    public class WebGLBuild : BuildTarget
    {
        public override string Identifier => "WebGL";
        public override string Description => "JavaScript/WebGL code and HTML page. Opens in default browser.";
        public override bool IsExperimental => true;
        public override bool IsObsolete => true;

        public override Backend CreateBackend()
        {
            return new JsBackend(new GLBackend());
        }

        public override void Configure(ICompiler compiler)
        {
            compiler.Environment.Set("Html.ScriptElements", GetScriptElements((JsBackend)compiler.Backend));
        }

        string GetScriptElements(JsBackend backend)
        {
            if (backend.Minify)
                return "<script type=\"text/javascript\" src=\"app.js\" defer=\"defer\"></script>";

            var sb = new StringBuilder();

            foreach (var f in backend.SourceFiles)
                sb.AppendLine("    <script type=\"text/javascript\" src=\"src/" + f.NativeToUnix() + "\"></script>");

            return sb.ToString().Trim();
        }

        public override bool CanRun(BuildFile file)
        {
            return true;
        }

        public static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Any, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        public override Task<bool> Run(Shell shell, BuildFile file, string args, CancellationToken ct)
        {
            var prefix = string.Format("http://localhost:{0}/", GetRandomUnusedPort());
            var webServer = new WebServer(prefix, file.RootDirectory);

            shell.Open(Path.Combine(prefix, "index.html"), args);

            Thread.Sleep(TimeSpan.FromSeconds(30)); // TODO: make configurable

            return Task.FromResult(true);
        }
    }
}
