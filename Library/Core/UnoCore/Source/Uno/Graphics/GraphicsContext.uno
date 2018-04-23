using OpenGL;
using Uno.Runtime.Implementation;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Graphics
{
    [extern(DOTNET) DotNetType]
    public class GraphicsContext
    {
        internal GraphicsContextHandle _handle;

        internal GraphicsContext(GraphicsContextHandle handle)
        {
            _handle = handle;
        }
    }
}
