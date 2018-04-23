using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL
{
    public interface IParametersEntity : IEntity
    {
        Parameter[] Parameters { get; }
    }
}