using System;
using System.Text;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.Backends.UnoDoc.Builders.Extensions;

namespace Uno.Compiler.Backends.UnoDoc.Builders.Syntax
{
    internal class MemberSyntaxGenerator : SyntaxGenerator, ISyntaxGenerator
    {
        public MemberSyntaxGenerator(IExportableCheck exportableCheck, IEntityNaming entityNaming)
                : base(exportableCheck, entityNaming) {}

        public string BuildUnoSyntax(IEntity entity, IEntity context = null)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context), "context is required for members");
            }
            var dataType = context as DataType;
            if (dataType == null)
            {
                throw new ArgumentException("context must be a DataType for members", nameof(context));
            }

            if (entity is ShaderFunction) return null;

            var member = (Member) entity;
            var constructorMember = member as Constructor;
            var propertyMember = member as Property;
            var memberIsIndexer = propertyMember != null && propertyMember.Parameters.Length > 0;

            var result = new StringBuilder();
            result.Append(BuildAttributes(member.Attributes, dataType));
            result.Append(BuildModifiers(member.GetModifierNames()));

            if (constructorMember == null && member.ReturnType != null)
            {
                result.Append(BuildReturnType(member.ReturnType, dataType));
            }

            if (constructorMember != null)
            {
                result.Append(dataType.Name);
            }
            else if (memberIsIndexer)
            {
                result.Append("this");
            }
            else
            {
                result.Append(BuildExplicitImplementationPrefix(member));
                result.Append(member.Name);
            }

            result.Append(BuildGenericParameters(member.GetGenericParameterNamesOrNull()));
            if (!(propertyMember != null && !memberIsIndexer)) // Don't add parameters if it's a property that is not an indexer (skips an empty "()" clause)
            {
                result.Append(BuildParameters(member.GetParametersOrNull(), dataType, propertyMember != null && memberIsIndexer));
            }

            if (member.MemberType == MemberType.Field || member.MemberType == MemberType.Literal)
            {
                result.Append(";");
            }
            else
            {
                result.Append(" {");
                if (propertyMember != null)
                {
                    if (propertyMember.GetMethod != null)
                    {
                        result.Append(" get;");
                    }
                    if (propertyMember.SetMethod != null)
                    {
                        result.Append(" get;");
                    }
                    if (propertyMember.GetMethod != null || propertyMember.SetMethod != null)
                    {
                        result.Append(" ");
                    }
                }
                result.Append("}");
            }

            return result.ToString();
        }

        public string BuildUxSyntax(IEntity entity)
        {
            return null;
        }

        private string BuildExplicitImplementationPrefix(Member member)
        {
            var propertyMember = member as Property;
            var methodMember = member as Method;
            var eventMember = member as Event;

            if (propertyMember?.ImplementedProperty != null)
            {
                return EntityNaming.GetFullIndexTitle(propertyMember.ImplementedProperty.DeclaringType) + ".";
            }
            if (methodMember?.ImplementedMethod != null)
            {
                return EntityNaming.GetFullIndexTitle(methodMember.ImplementedMethod.DeclaringType) + ".";
            }
            if (eventMember?.ImplementedEvent != null)
            {
                return EntityNaming.GetFullIndexTitle(eventMember.ImplementedEvent.DeclaringType) + ".";
            }

            return null;
        }
    }
}