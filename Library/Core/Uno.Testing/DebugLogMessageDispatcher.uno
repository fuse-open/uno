using Uno.Diagnostics;

namespace Uno.Testing
{
    class DebugLogMessageDispatcher : ITestRunnerMessageDispatcher
    {
        public void Start()
        {
        }

        public void Stop()
        {
            Uno.Runtime.Implementation.Internal.Unsafe.Quit();
        }

        public void Get(string uri)
        {
            var maxChunkLen = 120;

            for (int i = 0; i < uri.Length; i += maxChunkLen)
            {
                // We have to chunk up the message, as there is a max length on Android
                // for each log line.
                var chunkLen = Math.Min(uri.Length - i, maxChunkLen);
                var chunk = uri.Substring(i, chunkLen);
                var isLast = i + chunkLen >= uri.Length;
                var output = "{" + chunk.Length + "|" + chunk + "}" + (isLast ? ";" : "\\");

                Debug.Log(output);

                if defined(iOS)
                {
                    // HACK:
                    //
                    // When running on iOS with ios-deploy the debug output stream
                    // sometimes gets interrupted by lldb, causing errors.
                    //
                    // To remedy this problem we send the line twice, and ignore
                    // duplicates on the receiving side.
                    //
                    // This won't FIX the problem completely, at least not in theory.
                    // Ideally we should fix this permanently in ios-deploy, but hopefully
                    // this workaround will be sufficient in the meantime.
                    Debug.Log(output + " (retransmit)");
                }
            }
        }
    }
}
