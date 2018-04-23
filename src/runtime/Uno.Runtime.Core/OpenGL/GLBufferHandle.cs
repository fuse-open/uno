// This file was generated based on Library/Core/UnoCore/Source/OpenGL/GLBufferHandle.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace OpenGL
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public struct GLBufferHandle
    {
        public static readonly GLBufferHandle Zero;
        public readonly int _id;

        public GLBufferHandle(int id)
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

        public static bool operator ==(GLBufferHandle left, GLBufferHandle right)
        {
            return left._id == right._id;
        }

        public static bool operator !=(GLBufferHandle left, GLBufferHandle right)
        {
            return left._id != right._id;
        }

        public static explicit operator int(GLBufferHandle handle)
        {
            return handle._id;
        }

        public static explicit operator GLBufferHandle(int handle)
        {
            return new GLBufferHandle(handle);
        }
    }
}
