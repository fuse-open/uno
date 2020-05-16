using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;

namespace Uno.Support.OpenTK
{
    public static class GLHelpers
    {
        public static GraphicsContext CreateContext(IWindowInfo window)
        {
            var color = new ColorFormat(8, 8, 8, 8);
            var accum = new ColorFormat();
            var depthBufferBits = 24;
            var stencilBufferBits = 8;

            try
            {
                var mode = new GraphicsMode(color, depthBufferBits, stencilBufferBits, 1, accum);
                var result = new GraphicsContext(mode, window, 2, 0, GraphicsContextFlags.Embedded);

                result.MakeCurrent(window);
                result.LoadAll();

                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to create OpenGL context", e);
            }
        }
    }
}
