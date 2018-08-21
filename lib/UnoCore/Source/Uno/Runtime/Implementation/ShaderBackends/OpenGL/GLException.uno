using Uno.Compiler.ExportTargetInterop;

namespace Uno.Runtime.Implementation.ShaderBackends.OpenGL
{
    public extern(OPENGL) class GLException : Exception
    {
        public GLException(string message)
            : base(message)
        {
        }
    }
}
