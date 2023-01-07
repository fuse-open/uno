using Foundation;
using AppKit;
using System.Linq;
using CoreGraphics;
using Uno.AppLoader.MonoMac;
using Uno.Platform.Internal;
using System.Runtime.Versioning;

namespace Uno.AppLoader
{
    [Register("UnoAppDelegate")]
    [SupportedOSPlatform("macOS10.14")]
    public class AppDelegate : NSApplicationDelegate
    {
        NSWindow _window;
        UnoGLView _control;

        public override void DidFinishLaunching(NSNotification notification)
        {
            var initialWindowSize = new CGRect(0, 0, 375, 667);
            var windowStyle = NSWindowStyle.Titled | NSWindowStyle.Resizable | NSWindowStyle.Miniaturizable | NSWindowStyle.Closable;

            _control = new UnoGLView(initialWindowSize);
            _window = new NSWindow (
                initialWindowSize,
                windowStyle,
                NSBackingStore.Buffered,
                false) {
                Title = GetAssemblyTitle(),
                ContentView = _control
            };

            _window.MakeKeyAndOrderFront (_control);
            _control.Initialize();

            Program.UnoGenerated();
            DotNetApplication.Start();
            _control.Run(60.0);
        }

        string GetAssemblyTitle()
        {
            return (string) typeof(AppDelegate).Assembly.CustomAttributes
                .First(a => a.AttributeType.Name == "AssemblyTitleAttribute")
                .ConstructorArguments.First().Value;
        }
    }
}
