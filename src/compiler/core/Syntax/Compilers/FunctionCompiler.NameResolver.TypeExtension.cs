using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.Core.Syntax.Binding;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        public PartialExpression TryResolveTypeExtension(Expression obj, AstIdentifier id, int? typeParamCount)
        {
            var name = id.Symbol;
            var usings = NameResolver.TryGetUsings(Namescope, id.Source);
            var staticClasses = new HashSet<DataType>();

            for (var scope = Namescope; scope != null; scope = scope.Parent)
            {
                var ns = scope as Namespace;

                if (ns != null)
                    foreach (var dt in ns.Types)
                        if (dt.IsStatic && !dt.IsGenericDefinition &&
                            dt.IsAccessibleFrom(id.Source))
                            staticClasses.Add(dt);
            }

            if (usings != null)
            {
                foreach (var ns in usings.Namespaces)
                    foreach (var dt in ns.Types)
                        if (dt.IsStatic && !dt.IsGenericDefinition &&
                            dt.IsAccessibleFrom(id.Source))
                            staticClasses.Add(dt);

                foreach (var dt in usings.Types)
                    if (dt.IsStatic && !dt.IsGenericDefinition &&
                        dt.IsAccessibleFrom(id.Source))
                        staticClasses.Add(dt);
            }

            if (staticClasses.Count == 0)
                return null;

            var extensionMethods = new List<Method>();

            foreach (var dt in staticClasses)
                foreach (var m in dt.Methods)
                    if (m.IsStatic &&
                        m.Parameters.Length > 0 && m.Parameters[0].Modifier == ParameterModifier.This &&
                        (typeParamCount == null || m.IsGenericDefinition && m.GenericParameters.Length == typeParamCount) &&
                        m.Name == name)
                        extensionMethods.Add(m);

            if (extensionMethods.Count == 0)
                return null;

            return new PartialExtensionGroup(id.Source, obj.ActualValue, extensionMethods);
        }
    }
}
