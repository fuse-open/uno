using System.Collections.Generic;
using Uno.Compiler.API.Domain;

namespace Uno.Compiler.Backends.UnoDoc.Builders.Extensions
{
    public static class ModifiersExtensions
    {
        public static List<string> GetModifierNames(this Modifiers modifiers)
        {
            var modifierNames = new List<string>();

            if (modifiers.HasFlag(Modifiers.Private)) modifierNames.Add("private");
            if (modifiers.HasFlag(Modifiers.Protected)) modifierNames.Add("protected");
            if (modifiers.HasFlag(Modifiers.Public)) modifierNames.Add("public");
            if (modifiers.HasFlag(Modifiers.Internal)) modifierNames.Add("internal");

            // If there are no visibility flags, assume private.
            if (!modifiers.HasFlag(Modifiers.Private) && !modifiers.HasFlag(Modifiers.Protected) && !modifiers.HasFlag(Modifiers.Public) && !modifiers.HasFlag(Modifiers.Internal))
            {
                modifierNames.Add("private");
            }

            if (modifiers.HasFlag(Modifiers.Intrinsic)) modifierNames.Add("intrinsic");

            if (modifiers.HasFlag(Modifiers.Static)) modifierNames.Add("static");

            if (modifiers.HasFlag(Modifiers.Virtual)) modifierNames.Add("virtual");
            if (modifiers.HasFlag(Modifiers.Override)) modifierNames.Add("override");
            if (modifiers.HasFlag(Modifiers.Abstract)) modifierNames.Add("abstract");
            if (modifiers.HasFlag(Modifiers.Sealed)) modifierNames.Add("sealed");

            if (modifiers.HasFlag(Modifiers.Explicit)) modifierNames.Add("explicit");
            if (modifiers.HasFlag(Modifiers.Implicit)) modifierNames.Add("implicit");

            if (modifiers.HasFlag(Modifiers.Partial)) modifierNames.Add("partial");

            if (modifiers.HasFlag(Modifiers.Generated)) modifierNames.Add("generated");

            return modifierNames;
        }
    }
}