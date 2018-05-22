// This file was generated based on lib/UnoCore/Source/OpenGL/GLFramebufferHandle.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace OpenGL
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public struct GLFramebufferHandle
    {
        public static readonly GLFramebufferHandle Zero;
        public readonly int _id;

        public GLFramebufferHandle(int id)
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

        public static bool operator ==(GLFramebufferHandle left, GLFramebufferHandle right)
        {
            return left._id == right._id;
        }

        public static bool operator !=(GLFramebufferHandle left, GLFramebufferHandle right)
        {
            return left._id != right._id;
        }

        public static explicit operator int(GLFramebufferHandle handle)
        {
            return handle._id;
        }

        public static explicit operator GLFramebufferHandle(int handle)
        {
            return new GLFramebufferHandle(handle);
        }
    }
}
