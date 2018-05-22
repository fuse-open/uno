using Uno;
using Uno.Graphics;
using Uno.Platform;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Platform.iOS
{
    [TargetSpecificImplementation]
    [Require("Source.Include", "Uno-iOS/AppDelegate.h")]
    [Set("FileExtension", "mm")]
    public extern(IOS) static class Application
    {
        public static bool IsIdleTimerEnabled
        {
            get
            @{
                return ![UIApplication sharedApplication].idleTimerDisabled;
            @}
            set
            @{
                [UIApplication sharedApplication].idleTimerDisabled = !$0;
            @}
        }

        public static bool IsNetworkActivityIndicatorVisible
        {
            get
            @{
                return [UIApplication sharedApplication].networkActivityIndicatorVisible;
            @}
            set
            @{
                [UIApplication sharedApplication].networkActivityIndicatorVisible = $0;
            @}
        }

        public static int IconBadgeNumber
        {
            get
            @{
                return [UIApplication sharedApplication].applicationIconBadgeNumber;
            @}
            set
            @{
                [UIApplication sharedApplication].applicationIconBadgeNumber = $0;
            @}
        }

        public static ObjC.Object LaunchOptions { public get; private set; }

        [Foreign(Language.ObjC)]
        static public ObjC.Object GetRootView()
        @{
            return (UIView*)((uAppDelegate*)[UIApplication sharedApplication].delegate).view;
        @}

        [Foreign(Language.ObjC)]
        static public void SetRootView(ObjC.Object view)
        @{
            ((uAppDelegate*)[UIApplication sharedApplication].delegate).view = (UIView*)view;
        @}

        [Foreign(Language.ObjC)]
        public static bool IsRootView(ObjC.Object view)
        @{
            return view == ((uAppDelegate*)[UIApplication sharedApplication].delegate).view;
        @}

        [Foreign(Language.ObjC)]
        public static bool IsLandscape()
        @{
            return UIInterfaceOrientationIsLandscape([UIApplication sharedApplication].statusBarOrientation);
        @}
    }
}
