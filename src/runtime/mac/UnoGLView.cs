using System;
using System.Drawing;
using System.Linq;
using Foundation;
using AppKit;
using CoreGraphics;
using OpenTK.Graphics.OpenGL;
using Uno.Diagnostics;
using Uno.Platform;
using Uno.Platform.Internal;
using System.Runtime.Versioning;

namespace Uno.AppLoader.MonoMac
{
    [SupportedOSPlatform("macOS10.14")]
    public class UnoGLView : NSView
    {
        readonly MonoMacPlatformWindow _unoWindow;
        readonly MonoMacGraphicsContext _unoGC;

        internal bool EnableText;
        internal uint PressedMouseButtons;
        internal NSEventModifierMask ModifierFlags;

        bool _isInitializedHack;

        public event Action PreUpdate;
        public event Action PostDraw;

        bool _disposed;
        NSOpenGLContext _openGLContext;
        NSObject _notificationProxy;
        NSTimer _animationTimer;
        bool _animating;

        public UnoGLView(CGRect rect)
            : base(rect)
        {
            var attribs = new object[] {
                NSOpenGLPixelFormatAttribute.NoRecovery,
                NSOpenGLPixelFormatAttribute.DoubleBuffer,
                NSOpenGLPixelFormatAttribute.ColorSize, 24,
                NSOpenGLPixelFormatAttribute.DepthSize, 16
            };

            var pixelFormat = new NSOpenGLPixelFormat(attribs);

            _openGLContext = new NSOpenGLContext(pixelFormat, null);
            _openGLContext.MakeCurrentContext();

            _notificationProxy = NSNotificationCenter.DefaultCenter.AddObserver(GlobalFrameChangedNotification, HandleReshape);

            WantsBestResolutionOpenGLSurface = true;

            _unoWindow = new MonoMacPlatformWindow(this);
            _unoGC = new MonoMacGraphicsContext(this);
        }

        bool _hasDown;
        int _lastX;
        int _lastY;
        MouseButton _lastButton;

        public void RaiseMouseUp()
        {
            if (_hasDown)
                Bootstrapper.OnMouseUp (_unoWindow, _lastX, _lastY, _lastButton);
            _hasDown = false;
        }

        void OnUpdateFrame()
        {
            if(!_isInitializedHack)
            {
                Window.SetContentSize(new CGSize(800, 601));
                _isInitializedHack = true;
            }

            OnPreUpdate();
            if (Application.Current != null)
                Bootstrapper.OnUpdate();
        }

        void OnRenderFrame()
        {
            if (Window == null || !Window.IsVisible)
                return;

            // This fixes rendering with Xcode 10 (macOS Mojave) and newer.
            // https://github.com/xamarin/xamarin-macios/issues/4959#issuecomment-621914507
            if (_openGLContext.View == null)
                _openGLContext.View = this;

            if (Application.Current != null)
            {
                Bootstrapper.OnDraw();
                OnPostDraw();
            }
            else
            {
                GL.ClearColor(0, 0, 0, 1);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            }
        }

        public void Initialize()
        {
            if (Environment.GetEnvironmentVariable("UNO_WINDOW_HIDDEN") == "1")
                Window.IsVisible = false;

            _unoWindow.Initialize ();
            OpenGL.GL.Initialize(new MonoMacGL(), !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DEBUG_GL")));
            GraphicsContextBackend.SetInstance(_unoGC);
            WindowBackend.SetInstance(_unoWindow);

            Window.WillClose += (sender, args) => Bootstrapper.OnAppTerminating(_unoWindow);

            Window.AcceptsMouseMovedEvents = true;
            Window.MakeFirstResponder(this);
        }

        void OnPreUpdate()
        {
            var handler = PreUpdate;
            if (handler != null) handler();
        }

        void OnPostDraw()
        {
            var handler = PostDraw;
            if (handler != null) handler();
        }

        public Int2 DrawableSize
        {
            get {
                var bounds = ConvertRectToBacking(Bounds);
                return new Int2((int)bounds.Width, (int)bounds.Height);
            }
        }

        public override bool AcceptsFirstResponder()
        {
            return true;
        }

        public override void KeyDown(NSEvent theEvent)
        {
            ModifierFlags = theEvent.ModifierFlags;

            Key key;
            if (MonoMacEnums.TryGetUnoKey((NSKey)theEvent.KeyCode, out key))
                Bootstrapper.OnKeyDown(_unoWindow, key);
            else
                Log.Error("Unsupported key code: " + theEvent.KeyCode);

            if (EnableText && IsTextInputEvent (ModifierFlags))
            {
                var characters = theEvent.Characters.Where (CharacterIsNotSpecial).ToArray();
                if (characters.Length == 0)
                    return;

                Bootstrapper.OnTextInput (_unoWindow, new string (characters));
            }
        }

        static bool IsTextInputEvent(NSEventModifierMask modifierFlags)
        {
            return (modifierFlags & NSEventModifierMask.CommandKeyMask) == 0;
        }

        static bool CharacterIsNotSpecial(char character)
        {
            return char.IsLetterOrDigit (character) || char.IsSymbol (character) || char.IsWhiteSpace (character) || char.IsPunctuation (character) || char.IsNumber (character) || char.IsSeparator (character);
        }

        public override void KeyUp(NSEvent theEvent)
        {
            ModifierFlags = theEvent.ModifierFlags;

            Key key;
            if (MonoMacEnums.TryGetUnoKey((NSKey)theEvent.KeyCode, out key))
                Bootstrapper.OnKeyUp(_unoWindow, key);
            else
                Log.Error("Unsupported key code: " + theEvent.KeyCode);
        }

        public override void MouseMoved(NSEvent theEvent)
        {
            var p = Remap (theEvent.LocationInWindow);
            Bootstrapper.OnMouseMove(_unoWindow, p.X, p.Y);
        }

        public override void MouseDown(NSEvent theEvent)
        {
            MouseButton b;
            if (MonoMacEnums.TryGetUnoMouseButton(theEvent.ButtonNumber, out b))
            {
                PressedMouseButtons |= 1U << (int)b;
                var p = Remap (theEvent.LocationInWindow);
                _lastX = p.X;
                _lastY = p.Y;
                _lastButton = b;
                _hasDown = true;
                Bootstrapper.OnMouseDown(_unoWindow, p.X, p.Y, b);
            }
            else
            {
                Log.Error("Unsupported mouse button: " + theEvent.ButtonNumber);
            }
        }

        public override void MouseUp(NSEvent theEvent)
        {
            MouseButton b;
            if (MonoMacEnums.TryGetUnoMouseButton(theEvent.ButtonNumber, out b))
            {
                PressedMouseButtons &= ~(1U << (int)b);
                var p = Remap (theEvent.LocationInWindow);
                Bootstrapper.OnMouseUp(_unoWindow, p.X, p.Y, b);
                _hasDown = false;
            }
            else
            {
                Log.Error("Unsupported mouse button: " + theEvent.ButtonNumber);
            }
        }

        public override void MouseDragged(NSEvent theEvent)
        {
            MouseMoved(theEvent);
        }

        public override void RightMouseDown(NSEvent theEvent)
        {
            MouseDown(theEvent);
        }

        public override void RightMouseUp(NSEvent theEvent)
        {
            MouseUp(theEvent);
        }

        public override void RightMouseDragged(NSEvent theEvent)
        {
            MouseMoved(theEvent);
        }

        public override void OtherMouseDown(NSEvent theEvent)
        {
            MouseDown(theEvent);
        }

        public override void OtherMouseUp(NSEvent theEvent)
        {
            MouseUp(theEvent);
        }

        public override void OtherMouseDragged(NSEvent theEvent)
        {
            MouseMoved(theEvent);
        }

        public override void ScrollWheel(NSEvent theEvent)
        {
            var deltaMode = (int)(theEvent.HasPreciseScrollingDeltas ? WheelDeltaMode.DeltaPixel : WheelDeltaMode.DeltaLine);
            Bootstrapper.OnMouseWheel(_unoWindow, (float)theEvent.ScrollingDeltaX, (float)theEvent.ScrollingDeltaY, deltaMode);
        }

        NSCursor _cursor;
        public override void ResetCursorRects ()
        {
            if (_cursor != null)
            {
                AddCursorRect (Bounds, _cursor);
            }
            else
            {
                base.ResetCursorRects ();
            }
        }

        public void SetCursor(NSCursor cursor)
        {
            _cursor = cursor;
            Window.InvalidateCursorRectsForView (this);
        }

        Int2 Remap(CGPoint p)
        {
            return new Int2 ((int)(p.X * _unoWindow.GetDensity()), (int)((Size.Height - p.Y) * _unoWindow.GetDensity()));
        }
        [Preserve(Conditional=true)]
        public override void LockFocus()
        {
            base.LockFocus();
            if (_openGLContext.View != this)
                _openGLContext.View = this;
        }

        protected void UpdateView()
        {
            Size = new Size((int)Bounds.Width, (int)Bounds.Height);
        }

        private void HandleReshape(NSNotification note)
        {
            UpdateView();
        }

        Size _size;
        public Size Size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                {
                    _size = value;
                    OnResize(EventArgs.Empty);
                    _openGLContext.Update();
                }
            }
        }

        protected virtual void OnResize(EventArgs e)
        {
            var h = Resize;
            if (h != null)
                h(this, e);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                if (_animating)
                {
                    _animationTimer.Invalidate();
                    _animationTimer = null;
                }
                _animating = false;
                NSNotificationCenter.DefaultCenter.RemoveObserver(_notificationProxy);
            }
            base.Dispose(disposing);
            _disposed = true;
        }

        public void Run(double updatesPerSecond)
        {
            _openGLContext.SwapInterval = false;

            if (!_animating)
            {
                var timeout = new TimeSpan((long)(((1.0 * TimeSpan.TicksPerSecond) / updatesPerSecond) + 0.5));
                _animationTimer = NSTimer.CreateRepeatingScheduledTimer(timeout,
                    delegate {
                        _openGLContext.MakeCurrentContext();
                        OnUpdateFrame();
                        OnRenderFrame();
                        _openGLContext.FlushBuffer();
                    });

                NSRunLoop.Current.AddTimer(_animationTimer, NSRunLoopMode.Default);
                NSRunLoop.Current.AddTimer(_animationTimer, NSRunLoopMode.EventTracking);
            }
            _animating = true;
        }

        public event EventHandler<EventArgs> Resize;
    }
}
