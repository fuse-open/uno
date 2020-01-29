using Uno;
using Uno.Diagnostics;
using Uno.Platform;
using Uno.Testing;
using Uno.Threading;

namespace Uno.Testing
{
    public sealed class TestSetup
    {
        readonly AbstractRunner _runner;

        public TestSetup(Registry registry)
        {
            if defined(Android)
            {
                // Increase the chance for uno to connect before we're done
                // This is required because `uno build --run` on Android have
                // have problems outputting log of a short-running process.
                //
                // The 2500ms duration was chosen as the problem occured once
                // during testing at 1500ms, but never after increasing to this value.
                Thread.Sleep(2500);
            }

            _runner = new RemoteRunner(registry);

            Uno.Platform.Displays.MainDisplay.Tick += OnTick;
        }

        void OnTick(object sender, TimerEventArgs args)
        {
            _runner.Update();
        }
    }
}
