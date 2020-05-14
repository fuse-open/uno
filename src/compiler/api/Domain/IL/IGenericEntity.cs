using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.API.Domain.IL
{
    public interface IGenericEntity : IEntity
    {
        string GenericSuffix { get; }
        IGenericEntity GenericDefinition { get; }
        IEnumerable<IGenericEntity> GenericParameterizations { get; }
        bool IsGenericDefinition { get; }
        GenericParameterType[] GenericParameters { get; }
        bool IsGenericParameterization { get; }
        DataType[] GenericArguments { get; }
    }
}