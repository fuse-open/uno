using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API
{
    public interface IScheduler
    {
        void AddGenerator(Pass pass);
        void AddTransform(Pass pass);
    }
}