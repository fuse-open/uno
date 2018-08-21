using System;
using System.Runtime.InteropServices;
using OpenGL;

namespace Uno.Runtime.Implementation
{
    public abstract class GraphicsContextHandle
    {
        public abstract GLFramebufferHandle GetBackbufferGLHandle();
        public abstract Int2 GetBackbufferSize();
        public abstract Int2 GetBackbufferOffset();
        public abstract Recti GetBackbufferScissor();
        public abstract int GetRealBackbufferHeight();
    }
}
