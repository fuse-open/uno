using System;

namespace Uno.AppLoader
{
    class Program
    {
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            LoadApplication();
            
            Platform2.Internal.Application.Start();
            
            // Double check this behaviour, not satisfied with it
            while (Platform.CoreApp.State > Platform.ApplicationState.Uninitialized &&
                   Platform.CoreApp.State > Platform.ApplicationState.Terminating)
                Platform.Displays.TickAll(new Platform.TimerEventArgs(0,0,0));    
        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("Fatal Error: The application has crashed because of an unhandled exception:\n\n" + e.ExceptionObject);
            Environment.Exit(1);
        }

        static void LoadApplication()
        {
            // The Uno compiler will replace this
        }
    }
}
