using Uno.Compiler.ExportTargetInterop;
using Uno;

namespace OpenGL
{
    [extern(!DOTNET) TargetSpecificType]
    [extern(CPLUSPLUS) Set("typeName", "uint32_t")]
    [extern(CPLUSPLUS) Set("defaultValue", "0")]
    public extern(OPENGL) struct GLTextureHandle
    {
        public static readonly GLTextureHandle Zero;
        extern(DOTNET) readonly int _id;

        extern(DOTNET)
        public GLTextureHandle(int id)
        {
            _id = id;
        }

        extern(CPLUSPLUS || DOTNET)
        public static explicit operator int(GLTextureHandle handle)
        {
            return defined(DOTNET)
                ? handle._id
                : extern<int> "$0";
        }

        extern(CPLUSPLUS || DOTNET)
        public static explicit operator GLTextureHandle(int handle)
        {
            return defined(DOTNET)
                ? new GLTextureHandle(handle)
                : extern<GLTextureHandle> "$0";
        }

        public static bool operator == (GLTextureHandle left, GLTextureHandle right)
        {
            return defined(DOTNET)
                ? left._id == right._id
                : extern<bool> "$0 == $1";
        }

        public static bool operator != (GLTextureHandle left, GLTextureHandle right)
        {
            return defined(DOTNET)
                ? left._id != right._id
                : extern<bool> "$0 != $1";
        }

        // Silence warning
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // Silence warning
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}
