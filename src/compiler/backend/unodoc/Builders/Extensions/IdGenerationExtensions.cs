using System;
using System.Linq;
using System.Text;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Backends.UnoDoc.Builders.Extensions
{
    public static class IdGenerationExtensions
    {
        public static string GetId(this SourceObject entity)
        {
            if (entity is Namespace)
                return GetId((Namespace) entity);
            if (entity is Block)
                return GetId((Block) entity);
            if (entity is MetaProperty)
                return GetId((MetaProperty) entity);
            if (entity is DataType)
                return GetId((DataType) entity);

            throw new ArgumentException("Unknown entity type: " + entity.GetType().Name);
        }

        public static string GetId(this Namespace ns)
        {
            if (ns.Parent.FullName == ExportConstants.RootNamespaceName)
                return ns.UnoName;
            return GetId(ns.Parent) + "." + ns.UnoName;
        }

        public static string GetId(this Block block)
        {
            return GetId(block.Parent) + "." + block.UnoName;
        }

        public static string GetId(this MetaProperty metaProperty)
        {
            return GetId(metaProperty.Parent) + "." + metaProperty.Name;
        }

        public static string GetId(this DataType dataType)
        {
            if (dataType.IsArray)
                return GetId(dataType.ElementType);

            if (dataType.IsGenericParameter)
                return dataType.UnoName;

            var id = new StringBuilder();
            if (dataType.Parent != null)
                id.AppendFormat("{0}.", GetId(dataType.Parent));
            id.Append(dataType.UnoName);

            if (dataType.IsGenericDefinition)
                id.AppendFormat("`{0}", dataType.GenericParameters.Length);
            else if (dataType.IsGenericParameterization)
                id.AppendFormat("`{0}", dataType.GenericArguments.Length);

            return id.ToString();
        }

        public static string GetId(this Member member)
        {
            var id = new StringBuilder();
            id.AppendFormat("{0}.", GetId(member.DeclaringType));
            id.Append(member.GetSignature());
            return id.ToString();
        }

        public static string GetSignature(this Member member)
        {
            var signature = new StringBuilder();

            if (member is Constructor)
            {
                signature.Append(".ctor");
            }
            else
            {
                signature.Append(member.UnoName);
            }

            if (member is Method)
            {
                var method = (Method) member;
                if (method.IsGenericDefinition && method.GenericParameters.Length > 0)
                {
                    signature.AppendFormat("`{0}", method.GenericParameters.Length);
                }
            }

            var parameterList = member is ParametersMember ? ((ParametersMember)member).Parameters : null;
            if (parameterList != null && parameterList.Any())
            {
                signature.Append("(");
                foreach (var param in parameterList)
                {
                    signature.AppendWhen(param != parameterList.First(), ",");
                    signature.Append(param.Type.ToStringDontAlias());
                }
                signature.Append(")");
            }

            // Casts might have the same method signature, but varying return types; Only using the signature itself
            // as an ID is not good enough for casts as they probably won't be unique. Append the return type to make them unique.
            if (member is Cast)
            {
                signature.AppendFormat("~{0}", member.ReturnType.ToStringDontAlias());
            }

            return signature.ToString();
        }

        public static string GetId(this Parameter parameter, ParametersMember member)
        {
            var id = new StringBuilder();
            id.AppendFormat("{0}#", member.GetId());
            id.Append(Array.IndexOf(member.Parameters, parameter));
            return id.ToString();
        }
    }
}
