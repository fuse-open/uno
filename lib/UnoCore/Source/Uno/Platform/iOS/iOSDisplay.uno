using Uno;
using Uno.Graphics;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;
using Uno.Platform.iOS;

namespace Uno.Platform
{
    [Require("Source.Include", "Uno-iOS/AppDelegate.h")]
    [Require("Source.Include", "Window.h")]
    [Require("Source.Include", "Uno-iOS/Context.h")]
    [Require("Source.Include", "@{Uno.Platform.iOSDisplay:Include}")]
    public extern(iOS) class iOSDisplay : Uno.Platform.Display
    {
        ObjC.Object _displayLink = null;
        ObjC.Object _handle = null;

        public Rect Frame
        {
            get { return GetFrame(); }
        }

        public event EventHandler FrameChanged;

        void OnFrameChanged()
        {
            EventHandler handler = FrameChanged;
            if (handler != null)
                handler(null, EventArgs.Empty);
        }

        iOSDisplay(ObjC.Object handle)
        {
            _handle = handle;
            assert _handle != null;
            if defined(!LIBRARY)
                extern(_handle.Handle, this) "[((UIWindow*)$0) uSetDisplay:$1]";
        }

        ~iOSDisplay()
        {
            Destroy(_handle, _displayLink);
        }

        Rect GetFrame()
        {
            return Support.CGRectToUnoRect(
            Support.Pre_iOS8_HandleDeviceOrientation_Rect(
                extern<Uno.Platform.iOS.uCGRect>(_handle.Handle)"((UIWindow*)$0).frame", null),
                Density);
        }

        protected override float GetDensity()
        {
            return (float)GetDensityOfScreenFromWindow(_handle);
        }


        [Foreign(Language.ObjC)]
        static void Destroy(ObjC.Object handle, ObjC.Object displayLink)
        @{
            [displayLink invalidate];
        @}

        [Foreign(Language.ObjC)]
        extern(!LIBRARY)
        static internal Display WrapMainDisplay()
        @{
            uWindow* window = (uWindow*)[[uContext sharedContext] window];
            window.rootViewController = (UIViewController*)[[UIApplication sharedApplication] delegate];
            [window makeKeyAndVisible];
            return @{iOSDisplay(ObjC.Object):New(window)};
        @}

        [Foreign(Language.ObjC)]
        extern(LIBRARY)
        static internal Display WrapMainDisplay()
        @{
            CGRect screenBounds = [UIScreen mainScreen].bounds;
            UIWindow* window = [[uContext sharedContext] window];
            if (window == nil)
            {
                NSException* windowNilException = [NSException
                    exceptionWithName:@"Failed to initialize main Display"
                    reason:@"uContext.window is nil"
                    userInfo: nil];
                @throw windowNilException;
            }
            return @{iOSDisplay(ObjC.Object):New(window)};
        @}

        [Foreign(Language.ObjC)]
        static double GetDensityOfScreenFromWindow(ObjC.Object window)
        @{
            UIWindow* _window = (UIWindow*)window;
            UIScreen* screen = _window.screen;
            return @{GetDensityOfScreen(ObjC.Object):Call(screen)};
        @}

        [Foreign(Language.ObjC)]
        static double GetDensityOfScreen(ObjC.Object screen)
        @{
            UIScreen *uiScreen = (UIScreen*)screen;
            return (@{double})[uiScreen respondsToSelector:@selector(nativeScale)] ?
                [uiScreen nativeScale] :
                [uiScreen scale];
        @}

        //------------------------------------------------------------

        [Foreign(Language.ObjC)]
        protected override void SetTicksPerSecond(uint fps)
        @{
            CADisplayLink* displayLink = @{iOSDisplay:Of(_this)._displayLink:Get()};

            CFTimeInterval duration = displayLink.duration;
            if (duration == 0)
            {
                // Assume 60 fps
                duration = 1./60.;
            }

            unsigned actualFps;
            NSInteger frameInterval;

            if (fps == 0)
            {
                // Platform default
                actualFps = 0;
                frameInterval = 1;
            }
            else if (duration * fps > 0.5)
            {
                actualFps = 1. / duration;
                frameInterval = 1;
            }
            else
            {
                frameInterval = 1. / (duration * fps);
                actualFps = 1. / (duration * frameInterval);
            }

            displayLink.frameInterval = frameInterval;
            @{iOSDisplay:Of(_this)._ticksPerSecond:Set(actualFps)};
        @}

        [Foreign(Language.ObjC)]
        protected override void EnableTicks()
        @{
            CADisplayLink* displayLink = @{iOSDisplay:Of(_this)._displayLink};
            UIWindow* window = (UIWindow*)@{iOSDisplay:Of(_this)._handle};

            if (!displayLink)
            {
                uDisplayTickNotifier *notifier = [[uDisplayTickNotifier alloc] init];

                displayLink = [window.screen displayLinkWithTarget:notifier selector:@selector(uOnDisplayTick:) ];

                [displayLink addToRunLoop:[NSRunLoop mainRunLoop] forMode:NSDefaultRunLoopMode];
                [displayLink addToRunLoop:[NSRunLoop mainRunLoop] forMode:UITrackingRunLoopMode];

                @{iOSDisplay:Of(_this)._displayLink:Set(displayLink)};

                @{iOSDisplay:Of(_this).SetTicksPerSecond(uint):Call(@{iOSDisplay:Of(_this)._ticksPerSecond})};
            }

            displayLink.paused = NO;
        @}

        [Foreign(Language.ObjC)]
        protected override void DisableTicks()
        @{
            CADisplayLink* displayLink = @{iOSDisplay:Of(_this)._displayLink};
            displayLink.paused = YES;
        @}
    }
}
