using Uno.Compiler.ExportTargetInterop;
using Uno;

namespace OpenGL
{
    [extern(!DOTNET) TargetSpecificType]
    [extern(CPLUSPLUS) Set("TypeName", "uint32_t")]
    [extern(CPLUSPLUS) Set("DefaultValue", "0")]
    public extern(OPENGL) struct GLShaderHandle
    {
        public static readonly GLShaderHandle Zero;
        extern(DOTNET) readonly int _id;

        extern(DOTNET)
        public GLShaderHandle(int id)
        {
            _id = id;
        }

        extern(CPLUSPLUS || DOTNET)
        public static explicit operator int(GLShaderHandle handle)
        {
            return defined(DOTNET)
                ? handle._id
                : extern<int> "$0";
        }

        extern(CPLUSPLUS || DOTNET)
        public static explicit operator GLShaderHandle(int handle)
        {
            return defined(DOTNET)
                ? new GLShaderHandle(handle)
                : extern<GLShaderHandle> "$0";
        }

        public static bool operator == (GLShaderHandle left, GLShaderHandle right)
        {
            return defined(DOTNET)
                ? left._id == right._id
                : extern<bool> "$0 == $1";
        }

        public static bool operator != (GLShaderHandle left, GLShaderHandle right)
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
