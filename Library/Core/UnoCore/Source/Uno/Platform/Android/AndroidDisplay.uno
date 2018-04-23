using Uno;
using Uno.Graphics;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;
using Uno.Platform.iOS;

namespace Uno.Platform
{
    extern(android)
    public class AndroidDisplay : Display
    {
        protected override float GetDensity() { return _getDensity(); }

        [Foreign(Language.Java)]
        static float _getDensity()
        @{
            android.util.DisplayMetrics metrics = new android.util.DisplayMetrics();
            if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.JELLY_BEAN_MR1) {
                com.fuse.Activity.getRootActivity().getWindowManager().getDefaultDisplay().getRealMetrics(metrics);
            } else {
                com.fuse.Activity.getRootActivity().getWindowManager().getDefaultDisplay().getMetrics(metrics);
            }
            return metrics.density;
        @}

        static bool _initialized = false;

        protected override void DisableTicks()
        {
            _initialized = false;
        }

        protected override void EnableTicks()
        {
            JavaEnableTicks();
        }

        [Foreign(Language.Java)]
        void JavaEnableTicks()
        @{
            final UnoObject display = _this;
            if (@{_initialized}) return;
            @{_initialized:Set(true)};

            if (android.os.Build.VERSION.SDK_INT >= 16)
            {
                android.view.Choreographer.getInstance().postFrameCallback(new android.view.Choreographer.FrameCallback()
                {
                    android.view.Choreographer _choreographer = android.view.Choreographer.getInstance();
                    long _previousTimeNanos = 0;
                    public void doFrame(long frameTimeNanos)
                    {
                        if (@{_initialized})
                            _choreographer.postFrameCallback(this);
                        double currentTime = frameTimeNanos * 1e-9;
                        double deltaTime = (frameTimeNanos - _previousTimeNanos) * 1e-9;
                        @{AndroidDisplay:Of(display).OnFrameCallback(double,double):Call(currentTime,deltaTime)};
                        _previousTimeNanos = frameTimeNanos;
                    }
                });
            }
            else
            {
                android.animation.TimeAnimator timeAnimator = new android.animation.TimeAnimator();
                timeAnimator.setDuration(Long.MAX_VALUE);
                timeAnimator.setTimeListener(new android.animation.TimeAnimator.TimeListener()
                {
                    public void onTimeUpdate(android.animation.TimeAnimator animation, long totalTime, long deltaTime)
                    {
                        @{AndroidDisplay:Of(display).OnFrameCallback(double,double):Call(totalTime / 1000.0, deltaTime / 1000.0)};
                    }
                });
                timeAnimator.start();
            }
        @}

        void OnFrameCallback(double currentTime, double deltaTime)
        {
            OnTick(new TimerEventArgs(0, deltaTime, currentTime));
        }
    }
}
