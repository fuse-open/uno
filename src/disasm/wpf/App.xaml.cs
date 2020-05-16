using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;

namespace Uno.Disasm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        static App()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("The application has crashed because of an unhandled exception:\n\n" + e.ExceptionObject, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(1);
        }

        App(string[] args)
        {
            InitializeComponent();

            if (args.Length > 0)
                Dispatcher.BeginInvoke((Action<IEnumerable<string>>)(
                    x => ((MainWindow)MainWindow).BuildService.StartBuild(x)),
                    new object[] {args});
        }

        [STAThread]
        public static int Main(string[] args)
        {
            return new App(args).Run();
        }
    }
}
