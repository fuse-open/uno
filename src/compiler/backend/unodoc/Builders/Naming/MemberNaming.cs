using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.Backends.UnoDoc.Builders.Extensions;

namespace Uno.Compiler.Backends.UnoDoc.Builders.Naming
{
    internal class MemberNaming : Naming, IEntityNaming
    {
        private static readonly IReadOnlyDictionary<string, string> OperatorSymbolMap = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            { "op_UnaryPlus", "+" },
            { "op_UnaryNegation", "-" },
            { "op_OnesComplement", "~" },
            { "op_LogicalNot", "!" },
            { "op_Addition", "+" },
            { "op_Subtraction", "-" },
            { "op_Multiply", "*" },
            { "op_Division", "/" },
            { "op_Modulus", "%" },
            { "op_BitwiseAnd", "&" },
            { "op_BitwiseOr", "|" },
            { "op_ExclusiveOr", "^" },
            { "op_GreaterThan", ">" },
            { "op_GreaterThanOrEqual", ">=" },
            { "op_LessThan", "<" },
            { "op_LessThanOrEqual", "<=" },
            { "op_Equality", "==" },
            { "op_Inequality", "!=" },
            { "op_LeftShift", "<<" },
            { "op_RightShift", ">>" }
        });

        public string GetIndexTitle(IEntity entity)
        {
            return GetIndexTitleInternal((Member) entity, false);
        }

        public string GetFullIndexTitle(IEntity entity)
        {
            var member = (Member) entity;
            var ns = member.DeclaringType.FindNamespace();
            return ns.FullName + "." + GetIndexTitleInternal((Member) entity, true);
        }

        private string GetIndexTitleInternal(Member member, bool fullyQualified)
        {
            var dataType = member.DeclaringType;
            var baseName = GetPageBaseName(member, dataType, true);
            var genericArguments = member.GetGenericParameterNamesOrNull();
            var parameterTypes = member.GetParameterTypesOrNull();
            if (fullyQualified)
            {
                baseName = dataType.Name + "." + baseName;
            }
            return baseName + FormatGenerics(genericArguments) + FormatParameters(parameterTypes, fullyQualified);
        }

        public string GetNavigationTitle(IEntity entity)
        {
            var member = (Member)entity;
            var dataType = member.DeclaringType;
            var baseName = GetPageBaseName(member, dataType, true);
            var genericArguments = member.GetGenericParameterNamesOrNull();
            var typeName = GetTypeName(member);
            var parameterTypes = member.GetParameterTypesOrNull();
            return (baseName + FormatGenerics(genericArguments) + " " + typeName + " " + FormatParameters(parameterTypes, false)).Trim();
        }

        public string GetPageTitle(IEntity entity)
        {
            var member = (Member) entity;
            var dataType = member.DeclaringType;
            var baseName = GetPageBaseName(member, dataType, false);
            var genericArguments = member.GetGenericParameterNamesOrNull();
            var typeName = GetTypeName(member);
            var parameterTypes = member.GetParameterTypesOrNull();
            return (baseName + FormatGenerics(genericArguments) + " " + typeName + " " + FormatParameters(parameterTypes, false)).Trim();
        }

        private string GetPageBaseName(Member member, DataType dataType, bool isTocTitle)
        {
            var constructorMember = member as Constructor;
            var castMember = member as Cast;
            var operatorMember = member as Operator;
            var propertyMember = member as Property;
            var propertyIsIndexer = propertyMember != null && propertyMember.Parameters.Length > 0;

            if (constructorMember != null)
            {
                return GetPageBaseNameForConstructor(dataType);
            }

            if (castMember != null)
            {
                return GetPageBaseNameForCast(castMember, isTocTitle);
            }

            if (operatorMember != null)
            {
                return GetPageBaseNameForOperator(operatorMember);
            }

            if (propertyMember != null && propertyIsIndexer)
            {
                return GetPageBaseNameForIndexer(propertyMember, dataType, !isTocTitle);
            }

            return isTocTitle
                           ? member.Name
                           : dataType.Name + "." + member.Name;
        }

        private string GetPageBaseNameForConstructor(DataType context)
        {
            string name;
            return TypeAliases.TryGetAliasFromType(context.QualifiedName, out name) ? name : context.Name;
        }

        private string GetPageBaseNameForCast(Cast cast, bool shortVersion)
        {
            if (cast.Parameters.Length < 1)
            {
                throw new ArgumentException("Expected cast " + cast.FullName + " to have at least one parameter", nameof(cast));
            }
            if (cast.ReturnType == null)
            {
                throw new ArgumentException("Expected cast " + cast.FullName + " to have a return type", nameof(cast));
            }

            var implicitSuffix = shortVersion ? "(implicit)" : "Implicit";
            var explicitSuffix = shortVersion ? "(explicit)" : "Explicit";

            var from = new EntityNaming().GetIndexTitle(cast.Parameters[0].Type);
            var to = new EntityNaming().GetIndexTitle(cast.ReturnType);
            var explicitOrImplicit = cast.Name == "op_Implicit" ? implicitSuffix : explicitSuffix;
            return shortVersion
                           ? to + " " + explicitOrImplicit
                           : from + " to " + to + " " + explicitOrImplicit;
        }

        private string GetPageBaseNameForOperator(Operator op)
        {
            if (op.Parameters.Length < 1)
            {
                throw new ArgumentException("Expected operator " + op.FullName + " to have at least one parameter", nameof(op));
            }
            if (!OperatorSymbolMap.ContainsKey(op.Name))
            {
                throw new ArgumentException("Unknown operator type " + op.Name, nameof(op));
            }

            var leftSide = new EntityNaming().GetIndexTitle(op.Parameters[0].Type);
            var rightSide = op.Parameters.Length > 1
                                    ? new EntityNaming().GetIndexTitle(op.Parameters[1].Type)
                                    : null;
            var operatorName = OperatorSymbolMap[op.Name];

            return rightSide == null
                           ? operatorName + " " + leftSide
                           : leftSide + " " + operatorName + " " + rightSide;
        }

        private string GetPageBaseNameForIndexer(Property property, DataType dataType, bool addDataTypeName)
        {
            if (property.Parameters.Length < 1)
            {
                throw new ArgumentException("Expected indexer to have at least one parameter", nameof(property));
            }

            var type = new EntityNaming().GetIndexTitle(property.Parameters[0].Type);
            return addDataTypeName
                           ? dataType.Name + "[" + type + "]"
                           : "[" + type + "]";
        }

        private string GetTypeName(Member member)
        {
            switch (member.MemberType)
            {
                case MemberType.Literal:
                    return "Literal";
                case MemberType.Field:
                    return "Field";
                case MemberType.Event:
                    return "Event";
                case MemberType.Property:
                    return "Property";
                case MemberType.Method:
                    return "Method";
                case MemberType.Operator:
                    return "Operator";
                case MemberType.Cast:
                    return "Cast";
                case MemberType.Constructor:
                    return "Constructor";
                case MemberType.ShaderFunction:
                    return "Shader Function";
                default:
                    throw new ArgumentOutOfRangeException("Unsupported member type for page title: " +  member.MemberType.ToString("G"));
            }
        }
    }
}