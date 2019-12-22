// This file was generated based on lib/UnoCore/Source/Uno/Graphics/GraphicsController.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Graphics
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public sealed class GraphicsController : GraphicsContext
    {
        public global::Uno.Recti _viewport;
        public global::Uno.Recti _scissor;
        public RenderTarget _backbuffer;
        public RenderTarget _renderTarget;
        public bool _scissorEnabled;

        public GraphicsController()
        {
            this.ClearColor = new global::Uno.Float4(0.0f, 0.0f, 0.0f, 1.0f);
            this.ClearDepth = 1.0f;
            this._renderTarget = this._backbuffer = new RenderTarget();
            this.UpdateBackbuffer();
        }

        public void UpdateBackbuffer()
        {
            this._backbuffer.GLFramebufferHandle = this._backend.GetBackbufferGLHandle();
            this._backbuffer.Size = this._backend.GetBackbufferSize();
            this._backbuffer.HasDepth = true;
        }

        public void SetRenderTarget(RenderTarget renderTarget)
        {
            if (renderTarget == null)
                throw new global::System.ArgumentNullException("renderTarget");

            global::Uno.Recti full = new global::Uno.Recti(new global::Uno.Int2(0), renderTarget.Size);
            this.SetRenderTarget(renderTarget, full, full);
        }

        public void SetRenderTarget(RenderTarget renderTarget, global::Uno.Recti viewport, global::Uno.Recti scissor)
        {
            if (renderTarget == null)
                throw new global::System.ArgumentNullException("renderTarget");

            this._renderTarget = renderTarget;
            global::OpenGL.GL.BindFramebuffer(global::OpenGL.GLFramebufferTarget.Framebuffer, this._renderTarget.GLFramebufferHandle);
            this.Viewport = viewport;
            this.Scissor = scissor;
        }

        public void Clear(global::Uno.Float4 color, float depth)
        {
            global::OpenGL.GL.ClearDepth(depth);
            global::OpenGL.GL.ClearColor(color.X, color.Y, color.Z, color.W);
            global::OpenGL.GL.Clear((global::OpenGL.GLClearBufferMask)17664);
        }

        public RenderTarget Backbuffer
        {
            get { return this._backbuffer; }
        }

        public RenderTarget RenderTarget
        {
            get { return this._renderTarget; }
        }

        public global::Uno.Recti Scissor
        {
            get { return this._scissor; }
            set
            {
                if (!this._scissorEnabled)
                {
                    global::OpenGL.GL.Enable(global::OpenGL.GLEnableCap.ScissorTest);
                    this._scissorEnabled = true;
                }

                this._scissor = value;

                if (this._renderTarget == this._backbuffer)
                {
                    global::Uno.Int2 offset = this._backend.GetBackbufferOffset();
                    int realFbHeight = this._backend.GetRealBackbufferHeight();
                    global::Uno.Recti offsetScissor = new global::Uno.Recti(this._scissor.Position + offset, this._scissor.Size);
                    global::Uno.Recti currentScissor = this._backend.GetBackbufferScissor();
                    global::Uno.Recti clippedScissor = new global::Uno.Recti(global::Uno.Math.Max(offsetScissor.Left, currentScissor.Left), global::Uno.Math.Max(offsetScissor.Top, currentScissor.Top), global::Uno.Math.Min(offsetScissor.Right, currentScissor.Right), global::Uno.Math.Min(offsetScissor.Bottom, currentScissor.Bottom));
                    global::OpenGL.GL.Scissor(clippedScissor.Left, realFbHeight - clippedScissor.Bottom, global::Uno.Math.Max(0, clippedScissor.Size.X), global::Uno.Math.Max(0, clippedScissor.Size.Y));
                }
                else
                    global::OpenGL.GL.Scissor(this._scissor.Left, this._renderTarget.Size.Y - this._scissor.Bottom, global::Uno.Math.Max(0, this._scissor.Size.X), global::Uno.Math.Max(0, this._scissor.Size.Y));
            }
        }

        public global::Uno.Recti Viewport
        {
            get { return this._viewport; }
            set
            {
                this._viewport = value;

                if (this._renderTarget == this._backbuffer)
                {
                    global::Uno.Int2 offset = this._backend.GetBackbufferOffset();
                    int realFbHeight = this._backend.GetRealBackbufferHeight();
                    global::Uno.Recti offsetViewport = new global::Uno.Recti(this._viewport.Position + offset, this._viewport.Size);
                    global::OpenGL.GL.Viewport(offsetViewport.Left, realFbHeight - offsetViewport.Bottom, global::Uno.Math.Max(0, offsetViewport.Size.X), global::Uno.Math.Max(0, offsetViewport.Size.Y));
                }
                else
                    global::OpenGL.GL.Viewport(this._viewport.Left, this._renderTarget.Size.Y - this._viewport.Bottom, global::Uno.Math.Max(0, this._viewport.Size.X), global::Uno.Math.Max(0, this._viewport.Size.Y));
            }
        }

        public global::Uno.Float4 ClearColor
        {
            get;
            set;
        }

        public float ClearDepth
        {
            get;
            set;
        }
    }
}
