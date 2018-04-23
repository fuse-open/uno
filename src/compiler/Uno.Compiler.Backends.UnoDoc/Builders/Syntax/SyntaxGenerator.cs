using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.Backends.UnoDoc.Builders.Extensions;

namespace Uno.Compiler.Backends.UnoDoc.Builders.Syntax
{
    internal abstract class SyntaxGenerator
    {
        protected IExportableCheck ExportableCheck { get; set; }
        protected IEntityNaming EntityNaming { get; }

        protected SyntaxGenerator(IExportableCheck exportableCheck, IEntityNaming entityNaming)
        {
            if (exportableCheck == null)
            {
                throw new ArgumentNullException(nameof(exportableCheck));
            }
            if (entityNaming == null)
            {
                throw new ArgumentNullException(nameof(entityNaming));
            }

            ExportableCheck = exportableCheck;
            EntityNaming = entityNaming;
        }

        protected string BuildAttributes(NewObject[] attributes, Namescope context)
        {
            var result = new List<string>();

            foreach (var attribute in attributes)
            {
                var sb = new StringBuilder("[");
                var attributeName = new EntityNaming().GetFullIndexTitle(attribute.ReturnType);
                if (attributeName.EndsWith("Attribute"))
                {
                    attributeName = attributeName.Substring(0, attributeName.Length - "Attribute".Length);
                }
                sb.Append(attributeName);

                if (attribute.Arguments.Length > 0)
                {
                    sb.Append("(");

                    for (var i = 0; i < attribute.Arguments.Length; i++)
                    {
                        var arg = (Constant)attribute.Arguments[i];
                        var param = attribute.Constructor.Parameters[i];

                        if (i > 0) sb.Append(", ");
                        sb.Append(param.Name);
                        sb.Append(" = ");
                        if (arg.Value is string)
                        {
                            sb.Append("\"" + arg.Value + "\"");
                        }
                        else
                        {
                            sb.Append(arg.Value);
                        }
                    }

                    sb.Append(")");
                }

                sb.AppendLine("]");
                result.Add(sb.ToString());
            }

            return result.Any()
                           ? string.Join("\n", result)
                           : null;
        }

        protected string BuildModifiers(List<string> modifiers)
        {
            return modifiers.Any()
                           ? string.Join(" ", modifiers) + " "
                           : null;
        }

        protected string BuildReturnType(DataType returnType, DataType context)
        {
            if (returnType == null || returnType.IsVoid) return ExportConstants.VoidTypeName;

            if (returnType.IsArray)
            {
                return BuildReturnType(returnType.ElementType, context).Trim() + "[] ";
            }

            return returnType.IsGenericParameter
                           ? returnType.Name + " "
                           : new EntityNaming().GetFullIndexTitle(returnType) + " ";
        }

        protected string BuildGenericParameters(List<string> parameterNames)
        {
            if (parameterNames == null || parameterNames.Count == 0) return null;
            return "<" + string.Join(", ", parameterNames) + ">";
        }

        protected string BuildParameters(List<Parameter> parameters, DataType context, bool useSquareBrackets = false)
        {
            if (parameters == null) return null;
            var result = new List<string>();

            for (var i = 0; i < parameters.Count; i++)
            {
                var param = parameters[i];
                var sb = new StringBuilder();
                sb.Append(string.Join(" ", param.GetModifierNames()));
                sb.Append(" ");

                if (param.Type.IsArray)
                {
                    if (param.Type.IsGenericParameter)
                    {
                        sb.Append(param.Type.Name + "[]");
                    }
                    else
                    {
                        sb.Append(new EntityNaming().GetFullIndexTitle(param.Type.ElementType) + "[]");
                    }
                }
                else
                {
                    sb.Append(param.Type.IsGenericParameter
                                      ? param.Type.Name
                                      : new EntityNaming().GetFullIndexTitle(param.Type));
                }

                sb.Append(" " + param.Name);

                if (param.OptionalDefault != null)
                {
                    sb.Append(" = ");

                    var def = param.OptionalDefault;
                    var constantDef = def as Constant;
                    if (constantDef?.Value is string)
                    {
                        sb.Append("\"");
                    }

                    sb.Append(def.ActualValue);

                    if (constantDef?.Value is string)
                    {
                        sb.Append("\"");
                    }
                }

                result.Add(sb.ToString().Trim());
            }

            var opening = useSquareBrackets ? "[" : "(";
            var closing = useSquareBrackets ? "]" : ")";
            return opening + string.Join(", ", result) + closing;
        }
    }
}