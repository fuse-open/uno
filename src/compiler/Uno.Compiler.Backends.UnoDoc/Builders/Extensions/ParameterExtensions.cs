using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Backends.UnoDoc.Builders.Extensions
{
    public static class ParameterExtensions
    {
        public static List<string> GetModifierNames(this Parameter parameter)
        {
            var modifier = parameter.Modifier;
            var modifierNames = new List<string>();

            if (modifier.HasFlag(ParameterModifier.Ref)) modifierNames.Add("ref");
            if (modifier.HasFlag(ParameterModifier.Const)) modifierNames.Add("const");
            if (modifier.HasFlag(ParameterModifier.Out)) modifierNames.Add("out");
            if (modifier.HasFlag(ParameterModifier.Params)) modifierNames.Add("params");
            if (modifier.HasFlag(ParameterModifier.This)) modifierNames.Add("this");

            return modifierNames;
        }
    }
}