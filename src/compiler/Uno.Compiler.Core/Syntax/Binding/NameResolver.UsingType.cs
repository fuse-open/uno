using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public partial class NameResolver
    {
        public PartialExpression TryResolveUsingType(Namescope namescope, AstIdentifier id, int? typeParamCount)
        {
            var usings = TryGetUsings(namescope, id.Source);

            if (usings == null || usings.Types.Count == 0)
                return null;

            var methods = new List<Method>();
            var properties = new List<Property>();
            var fields = new List<Field>();
            var events = new List<Event>();
            var literals = new List<Literal>();

            foreach (var dt in usings.Types)
            {
                dt.PopulateMembers();

                for (int i = 0, l = dt.Methods.Count; i < l; i++)
                {
                    var m = dt.Methods[i];
                    if (m.UnoName == id.Symbol && m.IsPublic && m.IsStatic)
                    {
                        if (typeParamCount != null && (!m.IsGenericDefinition || m.GenericParameters.Length != typeParamCount.Value))
                            continue;

                        methods.Add(m);
                    }
                }

                if (typeParamCount == null)
                {
                    for (int i = 0, l = dt.Properties.Count; i < l; i++)
                    {
                        var m = dt.Properties[i];
                        if (m.UnoName == id.Symbol && m.IsPublic && m.IsStatic && m.Parameters.Length == 0)
                            properties.Add(m);
                    }

                    for (int i = 0, l = dt.Fields.Count; i < l; i++)
                    {
                        var m = dt.Fields[i];
                        if (m.UnoName == id.Symbol && m.IsPublic && m.IsStatic)
                            fields.Add(m);
                    }

                    for (int i = 0, l = dt.Events.Count; i < l; i++)
                    {
                        var m = dt.Events[i];
                        if (m.UnoName == id.Symbol && m.IsPublic && m.IsStatic)
                            events.Add(m);
                    }

                    for (int i = 0, l = dt.Literals.Count; i < l; i++)
                    {
                        var m = dt.Literals[i];
                        if (m.UnoName == id.Symbol && m.IsPublic)
                            literals.Add(m);
                    }
                }
            }

            if (methods.Count == 0 && properties.Count == 0 && fields.Count == 0 && literals.Count == 0)
                return null;
            if (methods.Count > 0 && properties.Count == 0 && fields.Count == 0 && literals.Count == 0)
                return new PartialMethodGroup(id.Source, null, false, methods);
            if (properties.Count == 1 && methods.Count == 0 && fields.Count == 0 && literals.Count == 0)
                return new PartialProperty(id.Source, properties[0], null);
            if (fields.Count == 1 && methods.Count == 0 && properties.Count == 0 && literals.Count == 0)
                return new PartialField(id.Source, fields[0], null);
            if (events.Count == 1 && methods.Count == 0 && properties.Count == 0 && literals.Count == 0)
                return new PartialEvent(id.Source, events[0], null);
            if (literals.Count == 1 && fields.Count == 0 && methods.Count == 0 && properties.Count == 0)
                return new PartialValue(id.Source, new Constant(literals[0].Source, literals[0].ReturnType, literals[0].Value));

            var all = new List<object>();
            all.AddRange(methods);
            all.AddRange(properties);
            all.AddRange(fields);
            all.AddRange(events);
            all.AddRange(literals);

            return PartialError(id.Source, ErrorCode.E3110, id.GetParameterizedSymbol(typeParamCount).Quote() + " is ambiguous" + SuggestCandidates(all));
        }
    }
}
