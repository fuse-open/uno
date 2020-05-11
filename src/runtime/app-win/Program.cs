using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Uno.AppLoader
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            DpiAwareness.SetDpiAware(DpiAwareness.ProcessDpiAwareness.SystemAware);
            new MainForm(UnoGenerated).MainLoop();
        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("The application has crashed because of an unhandled exception:\n\n" + e.ExceptionObject, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        static void UnoGenerated()
        {
            // Uno compiler will replace this.
            new DummyApp();
        }
    }
}
