// This file was generated based on Library/Core/UnoCore/Source/OpenGL/GLRenderbufferHandle.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace OpenGL
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public struct GLRenderbufferHandle
    {
        public static readonly GLRenderbufferHandle Zero;
        public readonly int _id;

        public GLRenderbufferHandle(int id)
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

        public static bool operator ==(GLRenderbufferHandle left, GLRenderbufferHandle right)
        {
            return left._id == right._id;
        }

        public static bool operator !=(GLRenderbufferHandle left, GLRenderbufferHandle right)
        {
            return left._id != right._id;
        }

        public static explicit operator int(GLRenderbufferHandle handle)
        {
            return handle._id;
        }

        public static explicit operator GLRenderbufferHandle(int handle)
        {
            return new GLRenderbufferHandle(handle);
        }
    }
}
