using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Core.Syntax.Builders
{
    public partial class BlockBuilder
    {
        internal readonly Dictionary<DataType, HashSet<DataType>> FlattenedTypes = new Dictionary<DataType, HashSet<DataType>>();

        void FlattenTypes()
        {
            FlattenTypes(_il);

            foreach (var e in FlattenedTypes)
                foreach (var st in e.Value.ToArray())
                    FlattenTypeRecursive(e.Value, st);
        }

        void FlattenTypes(Namespace ns)
        {
            foreach (var e in ns.Namespaces)
                FlattenTypes(e);

            foreach (var dt in ns.Types)
                FlattenType(dt);
        }

        void FlattenType(DataType dt)
        {
            if (dt.IsGenericDefinition)
            {
                foreach (var pt in dt.GenericParameterizations)
                    FlattenType(pt);

                return;
            }

            if (dt.IsGenericType)
                return;

            foreach (var it in dt.NestedTypes)
                FlattenType(it);

            var bt = dt.Base;

            if (bt != null)
            {
                HashSet<DataType> list;
                if (!FlattenedTypes.TryGetValue(bt, out list))
                {
                    list = new HashSet<DataType>();
                    FlattenedTypes.Add(bt, list);
                }

                list.Add(dt);
            }
        }

        void FlattenTypeRecursive(HashSet<DataType> parentSubclasses, DataType dt)
        {
            HashSet<DataType> subs;
            if (FlattenedTypes.TryGetValue(dt, out subs))
            {
                foreach (var st in subs.ToArray())
                {
                    if (!parentSubclasses.Contains(st))
                    {
                        parentSubclasses.Add(st);
                        FlattenTypeRecursive(parentSubclasses, st);
                    }
                }
            }
        }
    }
}
