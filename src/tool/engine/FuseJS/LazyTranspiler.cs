using Uno.Compiler.API;
using Uno.Configuration;
using Uno.Logging;

namespace Uno.Build.FuseJS
{
    public class LazyTranspiler : ITranspiler
    {
        readonly Log _log;
        readonly UnoConfig _config;
        TranspilerServer _server;

        public LazyTranspiler(Log log, UnoConfig config)
        {
            _log = log;
            _config = config;
        }

        public bool TryTranspile(string filename, string code, out string output)
        {
            // Ensure that the server is started
            if (_server == null)
                _server = new TranspilerServer(_log, _config);

            return _server.TryTranspile(filename, code, out output);
        }

        public void Dispose()
        {
            // Stop the server again if necessary
            if (_server != null)
            {
                _server.Dispose();
                _server = null;
            }
        }
    }
}
