using Uno.Compiler.API;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Core.Syntax.Macros
{
    class MacroContext
    {
        public readonly Function Function;
        public readonly Namescope[] Usings;
        public readonly ExpandInterceptor Interceptor;

        public MacroContext(
            Function func,
            Namescope[] usings,
            ExpandInterceptor interceptor = null)
        {
            Function = func;
            Usings = usings;
            Interceptor = interceptor;
        }
    }
}
