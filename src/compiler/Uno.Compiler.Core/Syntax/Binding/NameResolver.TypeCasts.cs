using System;
using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public partial class NameResolver
    {
        void FindTypeCastsRecursive(DataType dt, DataType targetType, DataType operandType, List<Cast> result, HashSet<DataType> castTypes = null)
        {
            dt.PopulateMembers();

            foreach (var cast in dt.Casts)
            {
                if (cast.Parameters.Length == 1 && operandType.IsSubclassOfOrEqual(cast.Parameters[0].Type))
                {
                    if (cast.ReturnType.Equals(targetType))
                    {
                        var hidden = false;

                        foreach (var foundCast in result)
                        {
                            if (foundCast == cast || cast.CompareParameters(foundCast) && dt.IsSubclassOf(foundCast.DeclaringType))
                            {
                                hidden = true;
                                break;
                            }
                        }

                        if (!hidden)
                            result.Add(cast);
                    }
                    else if (cast.IsImplicitCast)
                    {
                        if (castTypes == null)
                            castTypes = new HashSet<DataType>() { dt };

                        if (castTypes.Contains(cast.ReturnType))
                            continue;

                        castTypes.Add(cast.ReturnType);
                        FindTypeCastsRecursive(cast.ReturnType, targetType, cast.ReturnType, result, castTypes);
                        FindTypeCastsRecursive(targetType, targetType, cast.ReturnType, result, castTypes);
                    }
                }
            }

            if (dt.Base != null)
                FindTypeCastsRecursive(dt.Base, targetType, operandType, result, castTypes);
        }

        public IReadOnlyList<Cast> GetTypeCasts(DataType targetType, DataType operandType)
        {
            var tuple = Tuple.Create(targetType, operandType);

            IReadOnlyList<Cast> result;
            if (!_casts.TryGetValue(tuple, out result))
            {
                var allCasts = new List<Cast>();
                if (!targetType.IsEnum && !operandType.IsEnum)
                {
                    FindTypeCastsRecursive(targetType, targetType, operandType, allCasts);
                    FindTypeCastsRecursive(operandType, targetType, operandType, allCasts);
                }

                _casts[tuple] = result = allCasts;
            }

            return result;
        }
    }
}
