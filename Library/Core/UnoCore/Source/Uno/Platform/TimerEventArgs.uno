using Uno.Compiler.ExportTargetInterop;

namespace Uno.Platform
{
    [extern(DOTNET) DotNetType]
    public class TimerEventArgs : EventArgs
    {
        public TimerEventArgs(double lastTickTimestamp, double tickDuration, double currentTime)
        {
            LastTickTimestamp = lastTickTimestamp;
            TickDuration = tickDuration;
            CurrentTime = currentTime;
        }

        public double LastTickTimestamp { get; private set; }
        public double TickDuration { get; private set; }
        public double CurrentTime { get; private set; }
    }
}
