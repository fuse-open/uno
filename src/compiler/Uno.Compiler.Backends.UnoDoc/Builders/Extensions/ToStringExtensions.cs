using System.Text;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Backends.UnoDoc.Builders.Extensions
{
    public static class ToStringExtensions
    {
        public static string ToStringDontAlias(this DataType dataType)
        {
            if (dataType is RefArrayType)
                return dataType.ElementType.ToStringDontAlias() + "[]";
            if (dataType is FixedArrayType)
                return "fixed " + dataType.ElementType.ToStringDontAlias() + "[]";

            var sb = new StringBuilder();
            sb.Append(dataType.QualifiedName);
            sb.Append(dataType.GetGenericSuffixDontAlias());
            return sb.ToString();
        }

        public static string GetGenericSuffixDontAlias(this DataType dataType)
        {
            if (dataType.IsGenericDefinition)
            {
                return "<" + new string(',', dataType.GenericParameters.Length - 1) + ">";
            }

            if (dataType.IsGenericParameterization)
            {
                var sb = new StringBuilder();

                sb.Append("<");

                for (var i = 0; i < dataType.GenericArguments.Length; i++)
                {
                    sb.AppendWhen(i > 0, ",");
                    sb.Append(dataType.GenericArguments[i].ToStringDontAlias());
                }

                sb.Append(">");
                return sb.ToString();
            }

            return "";
        }

        public static string ToStringDontAlias(this Member member)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}.", member.DeclaringType.ToStringDontAlias());
            sb.Append(member.UnoName);
            return sb.ToString();
        }
    }
}
