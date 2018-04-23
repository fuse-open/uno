using System;
using System.Diagnostics;
using System.Threading;

namespace Uno.Diagnostics
{
    struct ProcessRunner : IDisposable
    {
        readonly Process _proc;
        readonly ConsoleCancelEventHandler _handler;

        public ProcessRunner(Process proc, string input = null, CancellationToken ct = default(CancellationToken))
        {
            _proc = proc;
            ct.Register(proc.KillTree);
            _handler = (s, e) => proc.KillTree();
            Console.CancelKeyPress += _handler; // TODO: Move handling of cancel key press, and use cancellation token instead

            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            if (input != null)
            {
                proc.StandardInput.Write(input);
                proc.StandardInput.Close();
            }
        }

        public void Dispose()
        {
            try
            {
                if (!_proc.HasExited)
                    _proc.KillTree();
            }
            catch
            {
            }

            Console.CancelKeyPress -= _handler;
        }
    }
}