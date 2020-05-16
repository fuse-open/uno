using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Backends.UnoDoc.Builders.Naming
{
    internal abstract class Naming
    {
        protected string FormatGenerics(List<string> genericParameterNames)
        {
            if (genericParameterNames == null || genericParameterNames.Count == 0) return null;
            return "<" + string.Join(", ", genericParameterNames) + ">";
        }

        protected string FormatParameters(List<DataType> parameterTypes, bool fullyQualified)
        {
            if (parameterTypes == null || parameterTypes.Count == 0) return null;
            var typeNames = parameterTypes.Select(type =>
            {
                var suffix = "";
                while (type.IsArray)
                {
                    type = type.ElementType;
                    suffix += "[]";
                }
                var name = fullyQualified
                                   ? new EntityNaming().GetFullIndexTitle(type)
                                   : new EntityNaming().GetIndexTitle(type);
                return type.IsGenericParameter
                               ? type.Name + suffix
                               : name + suffix;
            });
            return "(" + string.Join(", ", typeNames) + ")";
        }
    }
}