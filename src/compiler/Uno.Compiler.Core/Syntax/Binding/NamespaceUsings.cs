using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public class NamespaceUsings
    {
        public readonly int LineNumber;
        public readonly HashSet<Namespace> Namespaces;
        public readonly HashSet<DataType> Types;

        public NamespaceUsings(int line, IEnumerable<Namespace> parentNamespaces, IEnumerable<DataType> parentTypes)
        {
            LineNumber = line;
            Namespaces = new HashSet<Namespace>(parentNamespaces);
            Types = new HashSet<DataType>(parentTypes);
        }
    }
}