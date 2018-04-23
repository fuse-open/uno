// This file was generated based on Library/Core/UnoCore/Source/Uno/Application.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public abstract class Application : Platform.CoreApp
    {
        public Application()
        {
            this.Window = new Platform.Window();
            this.GraphicsController = new Graphics.GraphicsController();
        }

        public virtual void Update()
        {
        }

        public virtual void Draw()
        {
        }

        public static Application Current
        {
            get { return (Application)Platform.CoreApp.Current; }
        }

        public Graphics.GraphicsController GraphicsController
        {
            get;
            set;
        }

        public Platform.Window Window
        {
            get;
            set;
        }

        public virtual bool NeedsRedraw
        {
            get { return true; }
        }
    }
}
