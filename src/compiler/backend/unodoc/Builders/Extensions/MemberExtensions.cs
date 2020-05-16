using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Backends.UnoDoc.Builders.Extensions
{
    public static class MemberExtensions
    {
        public static List<Parameter> GetParametersOrNull(this Member member)
        {
            var paramMember = member as ParametersMember;
            return paramMember?.Parameters.ToList();
        }

        public static List<string> GetGenericParameterNamesOrNull(this Member member)
        {
            var methodMember = member as Method;
            if (methodMember == null || !methodMember.IsGenericDefinition)
            {
                return null;
            }
            return methodMember.GenericParameters.Select(e => e.Name).ToList();
        }


        public static List<DataType> GetParameterTypesOrNull(this Member member)
        {
            var methodMember = member as Method;
            var constructorMember = member as Constructor;

            if (methodMember != null)
            {
                return methodMember.Parameters.Select(e => e.Type).ToList();
            }

            return constructorMember?.Parameters.Select(e => e.Type).ToList();
        }

        public static List<string> GetModifierNames(this Member member)
        {
            return member.Modifiers.GetModifierNames();
        }
    }
}