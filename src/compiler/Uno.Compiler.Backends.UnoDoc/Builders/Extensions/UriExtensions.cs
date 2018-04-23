using System;
using System.Collections.Generic;
using System.Text;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Backends.UnoDoc.Builders.Extensions
{
    public static class UriExtensions
    {
        public static string GetUri(this SourceObject entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (entity is Namespace)
                return GetUri((Namespace) entity);
            if (entity is Block)
                return GetUri((Block) entity);
            if (entity is MetaProperty)
                return GetUri((MetaProperty) entity);
            if (entity is DataType)
                return GetUri((DataType) entity);
            if (entity is Member)
                return GetUri((Member) entity);

            throw new ArgumentException("Unknown entity type: " + entity.GetType().Name);
        }

        public static string GetUri(this Namespace ns)
        {
            if (ns == null)
            {
                throw new ArgumentNullException(nameof(ns));
            }

            var sb = new StringBuilder(ns.Parent.FullName == ExportConstants.RootNamespaceName ? "" : GetUri(ns.Parent));
            AppendSeparator(sb);
            sb.Append(FormatSegment(ns.UnoName));
            return sb.ToString();
        }

        public static string GetUri(this DataType dataType)
        {
            if (dataType == null)
            {
                throw new ArgumentNullException(nameof(dataType));
            }

            var sb = new StringBuilder(GetUri(dataType.IsArray ? dataType.Base.Parent : dataType.Parent));
            AppendSeparator(sb);
            sb.Append(FormatSegment(dataType.UnoName));

            if (dataType.IsGenericDefinition)
            {
                sb.AppendFormat("_{0}", dataType.GenericParameters.Length);
            } else if (dataType.IsGenericParameterization)
            {
                sb.AppendFormat("_{0}", dataType.GenericArguments.Length);
            }

            return sb.ToString();
        }

        public static string GetUri(this Block block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            var sb = new StringBuilder(GetUri(block.Parent));
            AppendSeparator(sb);
            sb.Append(FormatSegment(block.UnoName));
            return sb.ToString();
        }

        public static string GetUri(this MetaProperty metaProperty)
        {
            if (metaProperty == null)
            {
                throw new ArgumentNullException(nameof(metaProperty));
            }
            var sb = new StringBuilder(GetUri(metaProperty.Parent));
            AppendSeparator(sb);
            sb.Append(FormatSegment(metaProperty.Name));
            return sb.ToString();
        }

        public static string GetUri(this Member member)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            var sb = new StringBuilder(GetUri(member.DeclaringType));
            AppendSeparator(sb);
            sb.Append(member.GetUriSignature());
            return sb.ToString();
        }

        public static string GetUriSignature(this Member member)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            // Try to identify the master definition of this member
            if (!member.IsMasterDefinition)
            {
                do
                {
                    member = member.MasterDefinition;
                } while (!member.IsMasterDefinition);
            }

            var sb = new StringBuilder();

            if (member is Constructor)
            {
                sb.Append("_ctor");
            }
            else if (member is Method)
            {
                var method = (Method) member;

                // If the method is an explicit implementation of another method,
                // don't use the UnoName directly as it will contain something like
                // Uno.Collections.ICollection<T>.Add which is not a valid URI identifier.
                //
                // Instead, we convert those to uno_collections_icollection_1_add to follow
                // the uri patterns used elsewhere.
                if (method.ImplementedMethod != null)
                {
                    var implementedMethodTypeUri = GetUri(method.ImplementedMethod.DeclaringType).Replace("/", "_");
                    var implementedMethodUri = GetUriSignature(method.ImplementedMethod);
                    sb.Append(implementedMethodTypeUri + "_" + implementedMethodUri);
                }
                else
                {
                    sb.Append(method.UnoName.ToLowerInvariant());
                    if (method.IsGenericDefinition && method.GenericParameters.Length > 0)
                    {
                        sb.AppendFormat("_{0}", method.GenericParameters.Length);
                    }
                }
            }
            else
            {
                sb.Append(member.UnoName.ToLowerInvariant());
            }

            if (member is ParametersMember)
            {
                var paramMember = (ParametersMember)member;
                var paramIds = new List<string>();

                // Casts can have the same method signature (name + parameter types), but vary by return type - so in case of
                // casts, we add the return type as the first parameter type to the list in order to make the parameter checksum
                // unique for each cast.
                if (member is Cast)
                {
                    paramIds.Add(member.ReturnType.ToStringDontAlias());
                }

                foreach (var param in paramMember.Parameters)
                {
                    var paramId = param.Type.ToStringDontAlias();
                    if (param.Type.IsGenericParameter)
                    {
                        paramId = param.Type.Name;
                    }

                    paramIds.Add(paramId);
                }

                if (paramIds.Count > 0)
                {
                    var paramChecksum = CRC32.Compute(Encoding.UTF8.GetBytes(string.Join(",", paramIds)));
                    sb.AppendFormat("_{0:x8}", paramChecksum);
                }
            }

            return sb.ToString();
        }

        private static void AppendSeparator(StringBuilder sb)
        {
            sb.AppendWhen(sb.Length > 0 && sb[sb.Length - 1] != '/', "/");
        }

        private static string FormatSegment(string segment)
        {
            return segment.ToLower();
        }
    }
}
