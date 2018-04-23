using Uno.Compiler.API.Backends;
using Uno.Compiler.Backends.OpenGL;

namespace Uno.UX.Markup.CompilerReflection
{
    class ILCacheBackend : Backend
    {
        public override string Name => "ILCache";

        public ILCacheBackend()
            : base(new GLBackend())
        {
            
        }
    }
}
