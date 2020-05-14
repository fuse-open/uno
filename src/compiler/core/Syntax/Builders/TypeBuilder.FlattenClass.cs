using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;

namespace Uno.Compiler.Core.Syntax.Builders
{
    public partial class TypeBuilder
    {
        AstClass FlattenClass(AstClass astClass, List<string> parentSources, IEnumerable<AstBlockMember> parentItems)
        {
            AstClass result;
            if (_flattenedClasses.TryGetValue(astClass, out result))
                return result;

            var partials = new List<AstClass>();

            foreach (var item in parentItems)
            {
                var cd = item as AstClass;
                if (cd != null && cd.Modifiers.HasFlag(Modifiers.Partial) && cd.Name.Symbol == astClass.Name.Symbol)
                {
                    if (!_env.Test(cd.Name.Source, cd.OptionalCondition))
                        continue;

                    if (astClass.OptionalGeneric != null && cd.OptionalGeneric == null ||
                        cd.OptionalGeneric != null && astClass.OptionalGeneric == null)
                        continue;

                    if (astClass.OptionalGeneric != null &&
                        astClass.OptionalGeneric.Parameters.Count != cd.OptionalGeneric.Parameters.Count)
                        continue;

                    partials.Add(cd);
                }
            }

            Modifiers modifiers = 0;
            string comment = null;
            var attributes = new List<AstAttribute>();
            var swizzlerTypes = new List<AstExpression>();
            var baseTypes = new List<AstExpression>();
            var items = new List<AstBlockMember>();

            foreach (var part in partials)
            {
                modifiers |= part.Modifiers;
                comment = comment ?? part.DocComment;
                attributes.AddRange(part.Attributes);
                swizzlerTypes.AddRange(part.Swizzlers);
                baseTypes.AddRange(part.Bases);
                items.AddRange(part.Members);
            }

            IReadOnlyList<AstIdentifier> genericParameters = null;
            IReadOnlyList<AstConstraint> genericConstraints = null;

            if (astClass.OptionalGeneric != null)
            {
                genericParameters = astClass.OptionalGeneric.Parameters;
                genericConstraints = astClass.OptionalGeneric.Constraints;
            }

            foreach (var part in partials)
            {
                if (part.Type != astClass.Type)
                    Log.Error(part.Name.Source, ErrorCode.E0000, "All partials must be either 'class', 'struct' or 'interface'");

                var partProtection = part.Modifiers & Modifiers.ProtectionModifiers;
                if (partProtection != 0 && partProtection != (modifiers & Modifiers.ProtectionModifiers))
                    Log.Error(part.Name.Source, ErrorCode.E3025, "Partial class cannot have conflicting protection modifier");

                if (part.Modifiers.HasFlag(Modifiers.Sealed) && modifiers.HasFlag(Modifiers.Abstract))
                    Log.Error(part.Name.Source, ErrorCode.E0000, "Partial class cannot specify 'sealed', because it is declared 'abstract' somewhere else");
                else if (part.Modifiers.HasFlag(Modifiers.Abstract) && modifiers.HasFlag(Modifiers.Sealed))
                    Log.Error(part.Name.Source, ErrorCode.E0000, "Partial class cannot specify 'abstract', because it is declared 'sealed' somewhere else");

                if (part.OptionalGeneric != null && part != astClass)
                {
                    for (int i = 0; i < genericParameters.Count; i++)
                        if (part.OptionalGeneric.Parameters[i].Symbol != genericParameters[i].Symbol)
                            Log.Error(part.Name.Source, ErrorCode.E3027, "Partial generic class must use the same type parameter names in the same order");

                    if (part.OptionalGeneric.Constraints.Count != 0)
                    {
                        // TODO: C# does not have this limitation
                        if (genericConstraints.Count == 0)
                            genericConstraints = part.OptionalGeneric.Constraints;
                        else
                            Log.Error(part.Name.Source, ErrorCode.E3027, "Only one of the partials may specify generic constraints");
                    }
                }
            }

            result = new AstClass(
                comment, attributes, modifiers, null, 
                astClass.Type, astClass.Name, baseTypes,
                astClass.OptionalGeneric != null 
                    ? new AstGenericSignature(genericParameters, genericConstraints) 
                    : null,
                items, swizzlerTypes);

            foreach (var part in partials)
            {
                parentSources.Add(part.Name.Source.FullPath);

                if (_flattenedClasses.ContainsKey(part))
                {
                    Log.Error(part.Name.Source, ErrorCode.E0000, "Found more than one partial master definition for: " + part.Name.Symbol);
                    continue;
                }

                _flattenedClasses.Add(part, result);
            }

            return result;
        }
    }
}
