using OpenGL;
using Uno.Compiler.ExportTargetInterop;
using Uno.Math;

namespace Uno.Graphics
{
    public extern(!MOBILE) sealed class GraphicsController : GraphicsContext
    {
        internal Recti _viewport;
        internal Recti _scissor;
        internal RenderTarget _backbuffer;
        internal RenderTarget _renderTarget;

        bool _scissorEnabled = false;

        internal GraphicsController()
        {
            ClearColor = float4(0,0,0,1);
            ClearDepth = 1;
            _renderTarget = _backbuffer = new RenderTarget();
            UpdateBackbuffer();
        }

        public void UpdateBackbuffer()
        {
            if defined(OPENGL)
                _backbuffer.GLFramebufferHandle = _backend.GetBackbufferGLHandle();

            _backbuffer.Size = _backend.GetBackbufferSize();
            _backbuffer.HasDepth = true;
        }

        public RenderTarget Backbuffer
        {
            get { return _backbuffer; }
        }

        public RenderTarget RenderTarget
        {
            get
            {
                return _renderTarget;
            }
        }

        public Recti Scissor
        {
            get { return _scissor;}
            set
            {
                if defined(OPENGL)
                {
                    if (!_scissorEnabled) // because i don't know where to turn this on
                    {
                        GL.Enable(GLEnableCap.ScissorTest);
                        _scissorEnabled = true;
                    }

                    _scissor = value;
                    if (_renderTarget == _backbuffer)
                    {
                        var offset = _backend.GetBackbufferOffset();
                        var realFbHeight = _backend.GetRealBackbufferHeight();
                        var offsetScissor = new Recti(_scissor.Position + offset, _scissor.Size);
                        var currentScissor = _backend.GetBackbufferScissor();
                        var clippedScissor = new Recti(
                            Max(offsetScissor.Left, currentScissor.Left),
                            Max(offsetScissor.Top, currentScissor.Top),
                            Min(offsetScissor.Right, currentScissor.Right),
                            Min(offsetScissor.Bottom, currentScissor.Bottom)); // TODO: Some kind of Recti.Intersect[ion] would be better here

                        GL.Scissor(clippedScissor.Left,
                            realFbHeight - clippedScissor.Bottom,
                            Max(0, clippedScissor.Size.X),
                            Max(0, clippedScissor.Size.Y));

                    }
                    else
                    {
                        GL.Scissor(_scissor.Left, _renderTarget.Size.Y - _scissor.Bottom, Max(0, _scissor.Size.X), Max(0, _scissor.Size.Y));
                    }
                }
                else
                {
                    build_error;
                }
            }
        }

        public Recti Viewport
        {
            get { return _viewport; }
            set
            {
                if defined(OPENGL)
                {
                    _viewport = value;
                    if (_renderTarget == _backbuffer)
                    {
                        var offset = _backend.GetBackbufferOffset();
                        var realFbHeight = _backend.GetRealBackbufferHeight();
                        var offsetViewport = new Recti(_viewport.Position + offset, _viewport.Size);

                        GL.Viewport(offsetViewport.Left,
                            realFbHeight - offsetViewport.Bottom,
                            Max(0, offsetViewport.Size.X),
                            Max(0, offsetViewport.Size.Y));

                    }
                    else
                    {
                        GL.Viewport(_viewport.Left, _renderTarget.Size.Y - _viewport.Bottom, Max(0, _viewport.Size.X), Max(0, _viewport.Size.Y));
                    }
                }
                else
                {
                    build_error;
                }
            }
        }

        public void SetRenderTarget(RenderTarget renderTarget)
        {
            if (renderTarget == null)
                throw new ArgumentNullException(nameof(renderTarget));

            var full = new Recti(int2(0), renderTarget.Size);
            SetRenderTarget(renderTarget, full, full);
        }

        public void SetRenderTarget(RenderTarget renderTarget, Recti viewport, Recti scissor)
        {
            if (renderTarget == null)
                throw new ArgumentNullException(nameof(renderTarget));

            _renderTarget = renderTarget;

            if defined(OPENGL)
                GL.BindFramebuffer(GLFramebufferTarget.Framebuffer, _renderTarget.GLFramebufferHandle);

            Viewport = viewport;
            Scissor = scissor;
        }

        public float4 ClearColor
        {
            get;
            set;
        }

        public float ClearDepth
        {
            get;
            protected set;
        }

        public void Clear(float4 color, float depth)
        {
            if defined(OPENGL)
            {
                GL.ClearDepth(depth);
                GL.ClearColor(color.X, color.Y, color.Z, color.W);
                GL.Clear(GLClearBufferMask.ColorBufferBit | GLClearBufferMask.DepthBufferBit | GLClearBufferMask.StencilBufferBit);
            }
        }
    }
}
