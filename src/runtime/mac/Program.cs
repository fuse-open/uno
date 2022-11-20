using System;
using System.Globalization;
using System.Threading;
using AppKit;
using ObjCRuntime;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Uno.AppLoader
{
    [SupportedOSPlatform("macOS10.14")]
    public static class Program
    {
        static Program()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        }

        [STAThread]
        static int Main()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            try
            {
                NSApplication.Init();
                var application = NSApplication.SharedApplication;
                
                var app = new AppDelegate();
                application.Delegate = app;
                application.Run();
                return 0;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }
        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Console.Error.WriteLine("The application has crashed because of an unhandled exception:\n\n" + e.ExceptionObject);
            Environment.Exit(1);
        }

        internal static void UnoGenerated()
        {
            // Uno compiler will replace this.
            new DummyUnoApp();
        }
    }

    [SupportedOSPlatform("macOS10.14")]
    public static class ObjCExtensions
    {
        [DllImport ("/usr/lib/libobjc.dylib")]
        static extern IntPtr class_getClassMethod (IntPtr cls, IntPtr sel);

        public static IntPtr GetMethod (this Class cls, IntPtr selector)
        {
            return class_getClassMethod (cls.Handle, selector);
        }

        [DllImport ("/usr/lib/libobjc.dylib")]
        static extern bool class_addMethod (IntPtr cls, IntPtr sel, Delegate method, string argTypes);

        public static bool AddMethod (this Class cls, IntPtr selector, Delegate method, string arguments)
        {
            return class_addMethod (cls.Handle, selector, method, arguments);
        }

        [DllImport ("/usr/lib/libobjc.dylib")]
        static extern bool method_exchangeImplementations (IntPtr method1, IntPtr method2);

        public static void ExchangeMethod (this Class cls, IntPtr selMethod1, IntPtr selMethod2)
        {
            var method1 = class_getClassMethod (cls.Handle, selMethod1);
            var method2 = GetMethod (cls, selMethod2);
            method_exchangeImplementations (method1, method2);
        }

        [DllImport ("/usr/lib/libobjc.dylib")]
        static extern IntPtr objc_getMetaClass (string metaClassName);

        public static Class GetMetaClass (string metaClassName)
        {
            return new Class (objc_getMetaClass (metaClassName));
        }
    }
}
