// This file was generated based on lib/UnoCore/Source/OpenGL/GLShaderHandle.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace OpenGL
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public struct GLShaderHandle
    {
        public static readonly GLShaderHandle Zero;
        public readonly int _id;

        public GLShaderHandle(int id)
        {
            this._id = id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public static bool operator ==(GLShaderHandle left, GLShaderHandle right)
        {
            return left._id == right._id;
        }

        public static bool operator !=(GLShaderHandle left, GLShaderHandle right)
        {
            return left._id != right._id;
        }

        public static explicit operator int(GLShaderHandle handle)
        {
            return handle._id;
        }

        public static explicit operator GLShaderHandle(int handle)
        {
            return new GLShaderHandle(handle);
        }
    }
}
