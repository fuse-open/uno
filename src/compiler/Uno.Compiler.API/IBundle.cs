using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API
{
    public interface IBundle
    {
        string Directory { get; }

        Expression AddBundleFile(Source src, string filename);
        Expression AddProgram(DrawBlock block, Expression program);
        Expression Add(Expression expression);
    }
}
