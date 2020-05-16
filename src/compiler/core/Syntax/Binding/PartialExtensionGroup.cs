using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public class PartialExtensionGroup : PartialMember
    {
        public override PartialExpressionType ExpressionType => PartialExpressionType.ExtensionGroup;

        public readonly IReadOnlyList<Method> Methods;

        public PartialExtensionGroup(Source src, Expression instance, IReadOnlyList<Method> methods)
            : base(src, instance)
        {
            Methods = methods;
        }

        public override string ToString()
        {
            return "(extension methods)";
        }
    }
}