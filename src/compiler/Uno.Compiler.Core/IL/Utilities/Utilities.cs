using System.Collections.Generic;
using Uno.Collections;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.IL.Utilities.Analyzing;

namespace Uno.Compiler.Core.IL.Utilities
{
    public class Utilities : IUtilities
    {
        readonly Namespace _il;
        readonly Pass _parent;

        public Utilities(Namespace il, Pass parent)
        {
            _il = il;
            _parent = parent;
        }

        public HashSet<IEntity> FindDependencies(Statement s)
        {
            return DependencyFinder.FindDependencies(_parent, s);
        }

        public HashSet<IEntity> FindDependencies(Function f)
        {
            return DependencyFinder.FindDependencies(_parent, f);
        }

        public HashSet<DataType> FindAllTypes()
        {
            var result = new HashSet<DataType>();
            FindAllTypes(_il, result);
            return result;
        }

        void FindAllTypes(DataType dt, HashSet<DataType> result)
        {
            result.Add(dt);

            if (dt.RefArray != null)
                result.Add(dt.RefArray);

            foreach (var it in dt.NestedTypes)
                FindAllTypes(it, result);

            if (dt.IsGenericDefinition)
                foreach (var pt in dt.GenericParameterizations)
                    FindAllTypes(pt, result);
        }

        void FindAllTypes(Namespace root, HashSet<DataType> result)
        {
            foreach (var c in root.Namespaces)
                FindAllTypes(c, result);

            foreach (var dt in root.Types)
                FindAllTypes(dt, result);
        }
    }
}
