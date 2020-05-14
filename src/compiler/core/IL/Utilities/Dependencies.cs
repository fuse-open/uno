using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.IL.Utilities
{
    static class Dependencies
    {
        public static bool FindDependencies(this DataType dt, HashSet<DataType> fieldSet)
        {
            switch (dt.TypeType)
            {
                case TypeType.RefArray:
                case TypeType.FixedArray:
                    if (!fieldSet.Contains(dt))
                        return FindDependencies(dt.ElementType, fieldSet);

                    break;

                case TypeType.Enum:
                case TypeType.Class:
                case TypeType.Struct:
                case TypeType.Delegate:
                case TypeType.Interface:
                    if (!fieldSet.Contains(dt))
                    {
                        if (dt.IsGenericParameterization)
                            foreach (var a in dt.GenericArguments)
                                FindDependencies(a, fieldSet);

                        if (dt.MasterDefinition != dt)
                            FindDependencies(dt.MasterDefinition, fieldSet);

                        fieldSet.Add(dt);
                        return true;
                    }

                    break;
            }

            return false;
        }
    }
}