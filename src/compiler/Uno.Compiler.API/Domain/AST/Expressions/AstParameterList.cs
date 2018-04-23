using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Members;

namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public class AstParameterList : AstExpression
    {
        public readonly IReadOnlyList<AstParameter> Parameters;

        public override AstExpressionType ExpressionType => AstExpressionType.ParameterList;

        public AstParameterList(Source src, IReadOnlyList<AstParameter> parameters)
            : base(src)
        {
            Parameters = parameters;
        }
    }
}