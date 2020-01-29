using OpenGL;
using Uno.Compiler.ExportTargetInterop;
using Uno.Platform;

namespace Uno.Graphics
{
    public class GraphicsContext
    {
        protected readonly GraphicsContextBackend _backend = GraphicsContextBackend.Instance;
    }
}
