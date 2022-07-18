using OpenGL;
using Uno.Graphics;
using Uno.Platform;
using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    // this is only here so I dont have to hack the UX compiler yet
    public extern(MOBILE) abstract class Application : CoreApp
    {
        [Obsolete("Application.Current class is deprecated on mobile. Please use CoreApp.Current")]
        public static new Application Current
        {
            get { return CoreApp.Current as Application; }
        }

        [Obsolete("Deprecated on mobile. Please interact with UI through fuselibs")]
        public Window Window
        {
            get;
            private set;
        }

        [Obsolete("Deprecated on mobile")]
        public virtual bool NeedsRedraw
        {
            get { return true; }
        }

        protected Application() : base()
        {
            Window = new Window();
        }

        [Obsolete("Deprecated on mobile: Please use Uno.Platform.Displays.MainDisplay.Tick")]
        public virtual void Update()
        {
        }

        [Obsolete("Deprecated on mobile: Please use Uno.Platform.Displays.MainDisplay.Tick")]
        public virtual void Draw()
        {
        }
    }

    public extern(!MOBILE) abstract class Application : CoreApp
    {
        public static new Application Current
        {
            get { return (Application)CoreApp.Current; }
        }

        public GraphicsController GraphicsController
        {
            get;
            internal set;
        }

        public Window Window
        {
            get;
            private set;
        }

        public virtual bool NeedsRedraw
        {
            get { return true; }
        }

        protected Application() : base()
        {
            Window = new Window();
            GraphicsController = new GraphicsController();
        }

        public virtual void Update()
        {
        }

        public virtual void Draw()
        {
        }
    }
}
