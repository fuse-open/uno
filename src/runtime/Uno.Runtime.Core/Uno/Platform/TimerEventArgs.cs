// This file was generated based on Library/Core/UnoCore/Source/Uno/Platform/TimerEventArgs.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Platform
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public class TimerEventArgs : global::System.EventArgs
    {
        public TimerEventArgs(double lastTickTimestamp, double tickDuration, double currentTime)
        {
            this.LastTickTimestamp = lastTickTimestamp;
            this.TickDuration = tickDuration;
            this.CurrentTime = currentTime;
        }

        public double LastTickTimestamp
        {
            get;
            set;
        }

        public double TickDuration
        {
            get;
            set;
        }

        public double CurrentTime
        {
            get;
            set;
        }
    }
}
