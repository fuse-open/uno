using Uno.Compiler.ExportTargetInterop;

namespace Uno.Graphics.OpenGL
{
    public extern(OPENGL) class GLException : Exception
    {
        public GLException(string message)
            : base(message)
        {
        }
    }
}
