// This file was generated based on lib/UnoCore/Source/Uno/Platform/Displays.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Platform
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public abstract class Display
    {
        public uint _ticksPerSecond;

        public Display()
        {
            this._ticksPerSecond = 0;
        }

        protected abstract float GetDensity();

        protected virtual void SetTicksPerSecond(uint value)
        {
        }

        public void OnTick(TimerEventArgs args)
        {
            global::System.EventHandler<TimerEventArgs> handler = this._tick;

            if (handler != null)
                handler(null, args);
        }

        protected virtual void EnableTicks()
        {
        }

        protected virtual void DisableTicks()
        {
        }

        public float Density
        {
            get { return this.GetDensity(); }
        }

        public uint TicksPerSecond
        {
            get { return this._ticksPerSecond; }
            set
            {
                if (this._ticksPerSecond == value)
                    return;

                this._ticksPerSecond = value;

                if (this._tick != null)
                    this.SetTicksPerSecond(value);
            }
        }

        public event global::System.EventHandler<TimerEventArgs> _tick;
        public event global::System.EventHandler<TimerEventArgs> Tick
        {
            add
            {
                if (this._tick == null)
                    this.EnableTicks();

                this._tick += value;
            }
            remove
            {
                if (this._tick == null)
                    return;

                this._tick -= value;

                if (this._tick == null)
                    this.DisableTicks();
            }
        }
    }
}
