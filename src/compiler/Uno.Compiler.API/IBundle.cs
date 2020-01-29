using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API
{
    public interface IBundle
    {
        Expression AddFile(Source src, string filename);
        Expression AddProgram(DrawBlock block, Expression program);
    }
}
