using Uno;
using Uno.Graphics;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;
using Uno.Platform.iOS;

namespace Uno.Platform
{
    public static class Displays
    {
        public static List<Display> All { public get; private set; }
        public static Display MainDisplay { get { return GetMainDisplay(); } }

        static Displays()
        {
            All = new List<Display>();
            PopulateDisplaysList();
        }

        extern(android)
        static void PopulateDisplaysList()
        {
            All.Add(new AndroidDisplay());
        }

        extern(iOS)
        static void PopulateDisplaysList()
        {
            // TODO: Base this on something real
            All.Add(iOSDisplay.WrapMainDisplay());
        }

        extern(!mobile)
        static void PopulateDisplaysList()
        {
            All.Add(new DesktopDisplay());
        }

        static Display GetMainDisplay()
        {
            return All[0];
        }

        static internal void TickAll(TimerEventArgs args)
        {
            foreach (var d in All)
                d.OnTick(args);
        }
    }


    public abstract class Display
    {
        // Density

        public float Density { get { return GetDensity(); } }

        protected abstract float GetDensity();

        // Tick Rate

        protected uint _ticksPerSecond = 0; // Zero for platform default
        public uint TicksPerSecond
        {
            get { return _ticksPerSecond; }
            set
            {
                if (_ticksPerSecond == value)
                    return;

                _ticksPerSecond = value;
                if (_tick != null)
                    SetTicksPerSecond(value);
            }
        }

        protected virtual void SetTicksPerSecond(uint value) {}

        // Ticks

        event EventHandler<TimerEventArgs> _tick;
        public event EventHandler<TimerEventArgs> Tick
        {
            add
            {
                if (_tick == null)
                    EnableTicks();
                _tick += value;
            }

            remove
            {
                if (_tick == null)
                    return;

                _tick -= value;

                if (_tick == null)
                    DisableTicks();
            }
        }

        internal void OnTick(TimerEventArgs args)
        {
            EventHandler<TimerEventArgs> handler = _tick;
            if (handler != null)
                handler(null, args);
        }

        protected virtual void EnableTicks() {}
        protected virtual void DisableTicks() {}
    }

    extern(!mobile)
    public class DesktopDisplay : Display
    {
        protected override float GetDensity()
        {
            return WindowBackend.Instance.GetDensity();
        }
    }
}
