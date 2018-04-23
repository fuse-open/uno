using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Backends.UnoDoc.Builders.Extensions;

namespace Uno.Compiler.Backends.UnoDoc.Builders.Syntax
{
    internal class DataTypeSyntaxGenerator : SyntaxGenerator, ISyntaxGenerator
    {
        public DataTypeSyntaxGenerator(IExportableCheck exportableCheck, IEntityNaming entityNaming)
                : base(exportableCheck, entityNaming) {}

        public string BuildUnoSyntax(IEntity entity, IEntity context = null)
        {
            var dataType = (DataType) entity;
            var delegateType = dataType as DelegateType;

            var result = new StringBuilder();
            result.Append(BuildAttributes(dataType.Attributes, dataType));

            result.Append(BuildModifiers(dataType.GetModifierNames()));
            result.Append(dataType.TypeType.ToString("G").ToLowerInvariant());
            result.Append(" ");
            if (delegateType != null)
            {
                result.Append(BuildReturnType(delegateType.ReturnType, dataType));
            }
            result.Append(dataType.Name);
            result.Append(BuildGenericParameters(dataType.GetGenericParameterNamesOrNull(true)));
            result.Append(BuildParameters(dataType.GetParametersOrNull(), dataType));

            var inheritanceTypes = FindInheritanceTypes(dataType);
            if (inheritanceTypes.Count > 0)
            {
                result.Append(" : ");
                result.Append(string.Join(", ", inheritanceTypes.Select(type => new EntityNaming().GetFullIndexTitle(type))));
            }

            result.Append(" {}");
            return result.ToString();
        }

        public string BuildUxSyntax(IEntity entity)
        {
            var dataType = (DataType) entity;
            var props = dataType.GetUxClassProperties();
            return props == null ? null : "<" + props.Name + " />";
        }

        private List<DataType> FindInheritanceTypes(DataType dataType)
        {
            var result = new List<DataType>();

            if (dataType.Base != null && ExportableCheck.IsExportableAndVisible(dataType.Base) && dataType.Base.GetId() != ExportConstants.RootDataTypeId)
            {
                result.Add(dataType.Base);
            }

            if (dataType.Interfaces != null && dataType.Interfaces.Length > 0)
            {
                result.AddRange(dataType.Interfaces.Where(ExportableCheck.IsExportableAndVisible));
            }

            return result;
        }
    }
}