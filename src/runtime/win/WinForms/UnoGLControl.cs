using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using OpenGL;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform;
using Uno.Platform;
using Uno.Platform.Internal;
using Uno.Support.OpenTK;
using GL = Uno.Support.OpenTK.GL;

namespace Uno.Support.WinForms
{
    public class UnoGLControl : UserControl
    {
        readonly GL GL;
        readonly IWindowInfo _glWindow;
        readonly GraphicsContext _glContext;

        public IUnoWindow Window;
        public readonly WinFormsPlatformWindow CoreWindow;
        public readonly WinFormsGraphicsContext CoreGC;
        
        public IntPtr WindowHandle => _glWindow.Handle;
        public event Action PreUpdate;
        public event Action PostDraw;

        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_VREDRAW = 0x1;
                const int CS_HREDRAW = 0x2;
                const int CS_OWNDC = 0x20;

                var cp = base.CreateParams;
                // Setup necessary class style for OpenGL on windows
                cp.ClassStyle |= CS_VREDRAW | CS_HREDRAW | CS_OWNDC;
                return cp;
            }
        }

        bool _hasDown;
        int _lastX, _lastY;
        MouseButton _lastbutton; 

        public void RaiseMouseUp()
        {
            if (_hasDown)
            {
                Bootstrapper.OnMouseUp(CoreWindow, _lastX, _lastY, _lastbutton);
                _hasDown = false;
            }
        }

        public UnoGLControl()
        {
            Toolkit.Init(new ToolkitOptions
            {
                Backend = PlatformBackend.PreferNative
            });

            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            DoubleBuffered = false;
            Dock = DockStyle.Fill;

            _glWindow = Utilities.CreateWindowsWindowInfo(Handle);
            _glContext = GLHelpers.CreateContext(_glWindow);
            _glContext.SwapInterval = 0; // We do vsync in the form

            CoreWindow = new WinFormsPlatformWindow(this);
            CoreGC = new WinFormsGraphicsContext(this);
            GL = new GL();
        }

        public void SetDensity(float density)
        {
            CoreWindow.SetDensity(density);
        }

        public void Initialize(IUnoWindow parent)
        {
            Window = parent;
            OpenGL.GL.Initialize(GL, !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DEBUG_GL")));
            GraphicsContextBackend.SetInstance(CoreGC);
            WindowBackend.SetInstance(CoreWindow);

            SizeChanged += (sender, e) => Bootstrapper.OnWindowSizeChanged(CoreWindow);

            MouseDown += (sender, e) =>
            {
                MouseButton button;
                if (!WinFormsHelper.TryGetUnoMouseButton(e.Button, out button))
                    return;

                _hasDown = true;
                _lastX = e.X;
                _lastY = e.Y;
                _lastbutton = button;
                Bootstrapper.OnMouseDown(CoreWindow, e.X, e.Y, button);
            };

            MouseUp += (sender, e) =>
            {
                MouseButton button;
                if (!WinFormsHelper.TryGetUnoMouseButton(e.Button, out button))
                    return;

                Bootstrapper.OnMouseUp(CoreWindow, e.X, e.Y, button);
                _hasDown = false;
            };
            
            MouseLeave += (sender, e) => Bootstrapper.OnMouseOut(CoreWindow);
            MouseMove += (sender, e) => Bootstrapper.OnMouseMove(CoreWindow, e.X, e.Y);

            MouseWheel += (sender, e) =>
            {
                var numLinesPerScroll = SystemInformation.MouseWheelScrollLines;
                var deltaMode = numLinesPerScroll > 0
                    ? WheelDeltaMode.DeltaLine
                    : WheelDeltaMode.DeltaPage;

                var delta = deltaMode == WheelDeltaMode.DeltaLine ? (e.Delta / 120.0f) * numLinesPerScroll : e.Delta / 120.0f;
                Bootstrapper.OnMouseWheel(CoreWindow, 0, delta, (int)deltaMode);
            };

            PreviewKeyDown += (sender, e) =>
            {
                // TODO: By doing this the tab key will not be sent to wpf at all, it should be treated as IsInputKey only when not handled by Uno. A better solution could be done by reading http://msdn.microsoft.com/en-us/library/ms742474%28v=vs.110%29.aspx and http://blogs.msdn.com/b/nickkramer/archive/2006/06/09/623203.aspx
                switch (e.KeyData)
                {
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Tab:
                        e.IsInputKey = true;
                        break;

                    case Keys.Control | Keys.Left:
                    case Keys.Control | Keys.Right:
                    case Keys.Control | Keys.Up:
                    case Keys.Control | Keys.Down:
                    case Keys.Control | Keys.Tab:
                        e.IsInputKey = true;
                        break;

                    case Keys.Shift | Keys.Left:
                    case Keys.Shift | Keys.Right:
                    case Keys.Shift | Keys.Up:
                    case Keys.Shift | Keys.Down:
                    case Keys.Shift | Keys.Tab:
                        e.IsInputKey = true;
                        break;

                    case Keys.Control | Keys.Shift | Keys.Left:
                    case Keys.Control | Keys.Shift | Keys.Right:
                    case Keys.Control | Keys.Shift | Keys.Up:
                    case Keys.Control | Keys.Shift | Keys.Down:
                    case Keys.Control | Keys.Shift | Keys.Tab:
                        e.IsInputKey = true;
                        break;
                }
            };

            KeyDown += (sender, e) =>
            {
                Key key;
                if (!WinFormsHelper.TryGetUnoKey(e.KeyCode, out key))
                    return;

                e.Handled = Bootstrapper.OnKeyDown(CoreWindow, key);

                if (!e.Handled && key == Key.F11)
                    Window.IsFullscreen = !Window.IsFullscreen;
            };

            KeyUp += (sender, e) =>
            {
                Key key;
                if (!WinFormsHelper.TryGetUnoKey(e.KeyCode, out key))
                    return;

                e.Handled = Bootstrapper.OnKeyUp(CoreWindow, key);
            };

            KeyPress += (sender, e) =>
            {
                if (CoreWindow.IsTextInputActive() && (ModifierKeys & (Keys.Control | Keys.Alt)) != Keys.Control)
                    e.Handled = Bootstrapper.OnTextInput(CoreWindow, e.KeyChar.ToString());
            };

            GotFocus += (sender, e) => DotNetApplication.EnterInteractive();
            LostFocus += (senders, e) => DotNetApplication.ExitInteractive();
        }

        public void StartApp(MethodInfo entrypoint)
        {
            entrypoint.Invoke(null, null);
            DotNetApplication.Start();
        }

        Size _oldClientSize;
        bool _wasResized;
        public bool OnRender()
        {
            var redraw = _wasResized;

            /*
             * The size of the backbuffer may not be up to date with the Native window size,
             * before after a swap, according to the EGL Specification at this section '3.10.1.1 - Native Windows Resizing'.
             * So a redraw is neccessary the next frame if the Window size has changed.
             */
            if (_oldClientSize != ClientSize)
            {
                _oldClientSize = ClientSize;
                _wasResized = true;
            }
            else
            {
                _wasResized = false;
            }

            if (Application.Current != null)
            {
                OnPreUpdate();
                Bootstrapper.OnUpdate();
                redraw = redraw || Bootstrapper.DrawNextFrame;
                if (redraw)
                {
                    Bootstrapper.OnDraw();
                    OnPostDraw();
                }
            }
            else
            {
                const float R = 30.0f / 255.0f;
                const float G = 30.0f / 255.0f;
                const float B = 30.0f / 255.0f;

                GL.ClearColor(R, G, B, 1);
                GL.Clear(GLClearBufferMask.ColorBufferBit);
            }

            try
            {
                if (redraw)
                {
                    _glContext.SwapBuffers();
                }

                return true;
            }
            catch
            {
                // Window was closed, usually ...
                return false;
            }
        }

        void OnPreUpdate()
        {
            PreUpdate?.Invoke();
        }

        void OnPostDraw()
        {
            PostDraw?.Invoke();
        }

        public bool OnClosing()
        {
            return Bootstrapper.OnAppTerminating(CoreWindow);
        }

        public void OnClosed()
        {
            //Bootstrapper.OnWindowClosed(CoreWindow);
        }

        public WindowBackend GetWindowBackend()
        {
            return CoreWindow;
        }

        public GraphicsContextBackend GetGraphicsContextBackend()
        {
            return CoreGC;
        }

        public void SetCursor(Cursor cursor)
        {
            Cursor = cursor;
        }
    }
}
