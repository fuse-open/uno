using System.Collections.Generic;
using Uno.Collections;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API
{
    public interface IUtilities
    {
        HashSet<DataType> FindAllTypes();
        HashSet<IEntity> FindDependencies(Statement s);
        HashSet<IEntity> FindDependencies(Function f);
    }
}