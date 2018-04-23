// This file was generated based on Library/Core/UnoCore/Source/OpenGL/GLProgramHandle.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace OpenGL
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public struct GLProgramHandle
    {
        public static readonly GLProgramHandle Zero;
        public readonly int _id;

        public GLProgramHandle(int id)
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

        public static bool operator ==(GLProgramHandle left, GLProgramHandle right)
        {
            return left._id == right._id;
        }

        public static bool operator !=(GLProgramHandle left, GLProgramHandle right)
        {
            return left._id != right._id;
        }

        public static explicit operator int(GLProgramHandle handle)
        {
            return handle._id;
        }

        public static explicit operator GLProgramHandle(int handle)
        {
            return new GLProgramHandle(handle);
        }
    }
}
