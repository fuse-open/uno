using System;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Backends.UnoDoc.Builders.Extensions;

namespace Uno.Compiler.Backends.UnoDoc.Builders.Naming
{
    internal class DataTypeNaming : Naming, IEntityNaming
    {
        public string GetPageTitle(IEntity entity)
        {
            var dataType = (DataType) entity;
            var genericArguments = dataType.GetGenericParameterNamesOrNull(false);
            var typeName = GetTypeName(dataType);
            var parameterTypes = dataType.GetParameterTypesOrNull();
            var dataTypeName = GetDataTypeName(dataType);
            var title = (dataTypeName + FormatGenerics(genericArguments) + " " + typeName + " " + FormatParameters(parameterTypes, false)).Trim();
            return title;
        }

        public string GetIndexTitle(IEntity entity)
        {
            return GetIndexTitleInternal(entity, false);
        }

        public string GetFullIndexTitle(IEntity entity)
        {
            var dataType = (DataType) entity;
            var ns = dataType.FindNamespace();

            var title = TypeAliases.HasAlias(dataType.QualifiedName)
                                ? GetIndexTitleInternal(entity, true)
                                : ns.FullName + "." + GetIndexTitleInternal(entity, true);
            return title;
        }

        private string GetIndexTitleInternal(IEntity entity, bool fullyQualified)
        {
            var dataType = (DataType) entity;
            var genericArguments = dataType.GetGenericParameterNamesOrNull(fullyQualified);
            var parameterTypes = dataType.GetParameterTypesOrNull();
            var dataTypeName = GetDataTypeName(dataType);
            var title = (dataTypeName + FormatGenerics(genericArguments) + " " + FormatParameters(parameterTypes, fullyQualified)).Trim();
            return title;
        }

        public string GetNavigationTitle(IEntity entity)
        {
            return GetPageTitle(entity);
        }

        private string GetDataTypeName(DataType dataType)
        {
            string name;
            return TypeAliases.TryGetAliasFromType(dataType.QualifiedName, out name) ? name : dataType.Name;
        }

        private static string GetTypeName(DataType dataType)
        {
            switch (dataType.TypeType)
            {
                case TypeType.Class:
                    return "Class";

                case TypeType.Enum:
                    return "Enum";

                case TypeType.Struct:
                    return "Struct";

                case TypeType.Interface:
                    return "Interface";

                case TypeType.Delegate:
                    return "Delegate";

                default:
                    throw new ArgumentException("Unsupported data type for type name generation: " + dataType.TypeType.ToString("G"));
            }
        }
    }
}