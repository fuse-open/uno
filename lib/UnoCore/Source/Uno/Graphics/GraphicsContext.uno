using OpenGL;
using Uno.Compiler.ExportTargetInterop;
using Uno.Platform;

namespace Uno.Graphics
{
    [extern(DOTNET) DotNetType]
    public class GraphicsContext
    {
        protected readonly GraphicsContextBackend _backend = GraphicsContextBackend.Instance;
    }
}
