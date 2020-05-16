using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public class PartialMethodGroup : PartialMember
    {
        public readonly IReadOnlyList<Method> Methods;
        public readonly bool IsQualified;

        public override PartialExpressionType ExpressionType => PartialExpressionType.MethodGroup;

        public PartialMethodGroup(Source src, Expression instance, bool qualified, IReadOnlyList<Method> methods)
            : base(src, instance)
        {
            IsQualified = qualified;
            Methods = methods;
        }

        public override string ToString()
        {
            return "(methods)";
        }
    }
}