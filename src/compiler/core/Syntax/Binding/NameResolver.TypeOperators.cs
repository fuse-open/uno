using System;
using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public partial class NameResolver
    {
        void FindTypeOperatorsRecursive(DataType dt, string opOp, int paramCount, List<Operator> result, HashSet<DataType> castTypes = null)
        {
            dt.PopulateMembers();

            foreach (var op in dt.Operators)
            {
                if (op.Symbol == opOp &&
                    op.Parameters.Length == paramCount)
                {
                    var hidden = false;
                    foreach (var foundOp in result)
                    {
                        if (foundOp == op || op.CompareParameters(foundOp) && dt.IsSubclassOf(foundOp.DeclaringType))
                        {
                            hidden = true;
                            break;
                        }
                    }

                    if (!hidden)
                        result.Add(op);
                }
            }

            foreach (var cast in dt.Casts)
            {
                if (cast.IsImplicitCast &&
                    cast.Parameters.Length == 1 &&
                    dt.IsSubclassOfOrEqual(cast.Parameters[0].Type))
                {
                    if (castTypes == null)
                        castTypes = new HashSet<DataType>() { dt };

                    if (castTypes.Contains(cast.ReturnType))
                        continue;

                    castTypes.Add(cast.ReturnType);
                    FindTypeOperatorsRecursive(cast.ReturnType, opOp, paramCount, result, castTypes);
                }
            }

            if (dt.Base != null && dt.IsReferenceType)
                FindTypeOperatorsRecursive(dt.Base, opOp, paramCount, result, castTypes);
        }

        public IReadOnlyList<Operator> GetTypeOperators(DataType leftType, DataType rightType, string opOp)
        {
            var tuple = Tuple.Create(leftType, rightType, opOp);

            IReadOnlyList<Operator> result;
            if (!_binOps.TryGetValue(tuple, out result))
            {
                var allOperators = new List<Operator>();
                FindTypeOperatorsRecursive(leftType, opOp, 2, allOperators);
                FindTypeOperatorsRecursive(rightType, opOp, 2, allOperators);
                _binOps[tuple] = result = allOperators;
            }

            return result;
        }

        public IReadOnlyList<Operator> GetTypeOperators(DataType operandType, string opOp)
        {
            var tuple = Tuple.Create(operandType, opOp);

            IReadOnlyList<Operator> result;
            if (!_unOps.TryGetValue(tuple, out result))
            {
                var allOperators = new List<Operator>();
                FindTypeOperatorsRecursive(operandType, opOp, 1, allOperators);
                _unOps[tuple] = result = allOperators;;
            }

            return result;
        }
    }
}
