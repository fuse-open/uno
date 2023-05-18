using OpenGL;
using System;
using System.Globalization;
using System.Threading;
using Uno.AppLoader.Dummy;
using Uno.Diagnostics;
using Uno.Platform;
using Uno.Platform.Internal;

namespace Uno.AppLoader
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            GL.Initialize(new DummyGL());
            GraphicsContextBackend.SetInstance(new DummyGC());
            WindowBackend.SetInstance(new DummyWindow());

            UnoGenerated();
            DotNetApplication.Start();

            const double targetTime = 1.0 / 60;

            while (Application.Current != null)
            {
                var startTime = Clock.GetSeconds();

                Bootstrapper.OnUpdate();
                Bootstrapper.OnDraw();

                var renderTime = Clock.GetSeconds() - startTime;
                var msTimeout = (int)((targetTime - renderTime) * 1000.0 + 0.5);

                if (msTimeout > 0)
                    Thread.Sleep(msTimeout);
            }
        }

        static void UnoGenerated()
        {
            // The Uno compiler will replace this.
        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Console.Error.WriteLine("The application has crashed because of an unhandled exception:\n\n" + e.ExceptionObject);
            Environment.Exit(1);
        }

        static Program()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
        }
    }
}
