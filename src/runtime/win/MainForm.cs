using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using Uno.Diagnostics;
using Uno.Platform.Internal;
using Uno.Support.WinForms;

namespace Uno.AppLoader
{
    public partial class MainForm : Form, IUnoWindow
    {
        FormWindowState _state = FormWindowState.Normal;
        FormBorderStyle _style = FormBorderStyle.Sizable;
        readonly UnoGLControl _control = new UnoGLControl();

        public MainForm(Action initializeApp)
        {
            InitializeComponent();
            Controls.Add(_control);
            _control.Initialize(this);
            var dpi = DpiAwareness.GetDpi(Handle);
            _control.SetDensity((float)dpi);
            ClientSize = new Size((int)(375*dpi), (int)(667*dpi));
            FormClosing += (sender, e) => e.Cancel = _control.OnClosing();
            FormClosed += (sender, e) => _control.OnClosed();
            Title = GetAssemblyTitle();

            initializeApp();
            DotNetApplication.Start();
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(Environment.GetEnvironmentVariable("UNO_WINDOW_HIDDEN") != "1" && value);
        }

        string GetAssemblyTitle()
        {
            return (string) typeof(MainForm).Assembly.CustomAttributes
                .First(a => a.AttributeType.Name == "AssemblyTitleAttribute")
                .ConstructorArguments.First().Value;
        }

        public void MainLoop()
        {
            var context = new System.Windows.Forms.ApplicationContext(this);
            context.MainForm.Visible = true;

            var openAdapter = new D3DKMT_OPENADAPTERFROMHDC();
            var waitForVblankEvent = new D3DKMT_WAITFORVERTICALBLANKEVENT();

            openAdapter.hDc = CreateDC(Screen.PrimaryScreen.DeviceName, null, null, IntPtr.Zero);

            bool useD3DKMT = false;
            double targetTime = 1.0 / 60;
            if (D3DKMTOpenAdapterFromHdc(ref openAdapter) == 0)
            {
                useD3DKMT = true;
                waitForVblankEvent.hAdapter = openAdapter.hAdapter;
                waitForVblankEvent.hDevice = 0;
                waitForVblankEvent.VidPnSourceId = openAdapter.VidPnSourceId;
            }
            else
            {
                var mode = new DEVMODE();
                if (EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref mode) && mode.dmBitsPerPel > 0)
                    targetTime = 1.0 / mode.dmDisplayFrequency;
            }

            while (!IsDisposed)
            {
                var msg = new MSG();
                while (PeekMessage(ref msg, IntPtr.Zero, 0, 0, 0x0001 /*PM_REMOVE*/))
                {
                    TranslateMessage(ref msg);
                    DispatchMessage(ref msg);
                }

                if (useD3DKMT)
                {
                    D3DKMTWaitForVerticalBlankEvent(ref waitForVblankEvent);

                    if (!_control.OnRender())
                        return;
                }
                else
                {
                    var startTime = Clock.GetSeconds();

                    if (!_control.OnRender())
                        return;

                    var renderTime = Clock.GetSeconds() - startTime;
                    var msTimeout = (int)((targetTime - renderTime) * 1000.0 + 0.5);

                    if (msTimeout > 0)
                        Thread.Sleep(msTimeout);
                }
            }
        }

        public string Title
        {
            get { return Text; }
            set { Text = value; }
        }

        public bool IsFullscreen
        {
            get
            {
                return FormBorderStyle == FormBorderStyle.None;
            }
            set
            {
                if (value != IsFullscreen)
                {
                    if (value)
                    {
                        _style = FormBorderStyle;
                        _state = WindowState;
                        WindowState = FormWindowState.Normal;
                        FormBorderStyle = FormBorderStyle.None;
                        WindowState = FormWindowState.Maximized;
                    }
                    else
                    {
                        FormBorderStyle = _style;
                        WindowState = _state;
                    }
                }
            }
        }

        public void SetClientSize(int width, int height)
        {
            ClientSize = new Size(width, height);
        }

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateDC(string strDriver, string strDevice, string strOutput, IntPtr pData);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        public struct D3DKMT_OPENADAPTERFROMHDC
        {
            public IntPtr hDc;
            public uint hAdapter;
            public uint AdapterLuidLowPart;
            public uint AdapterLuidHighPart;
            public uint VidPnSourceId;
        }

        [DllImport("gdi32.dll")]
        public static extern uint D3DKMTOpenAdapterFromHdc(ref D3DKMT_OPENADAPTERFROMHDC pData);

        public struct D3DKMT_WAITFORVERTICALBLANKEVENT
        {
            public uint hAdapter;
            public uint hDevice;
            public uint VidPnSourceId;
        }

        [DllImport("gdi32.dll")]
        public static extern uint D3DKMTWaitForVerticalBlankEvent(ref D3DKMT_WAITFORVERTICALBLANKEVENT pData);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll")]
        public static extern bool PeekMessage(ref MSG msg, IntPtr hWnd, int messageFilterMin, int messageFilterMax, int flags);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll")]
        public static extern bool TranslateMessage(ref MSG msg);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll")]
        public static extern bool DispatchMessage(ref MSG msg);

        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }

        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);
        const int ENUM_CURRENT_SETTINGS = -1;

        public struct MSG
        {
            public IntPtr HWnd;
            public uint Message;
            public IntPtr WParam;
            public IntPtr LParam;
            public uint Time;
            public POINT Point;
        }
    }
}
