using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Members;
using Uno.Compiler.API.Domain.AST.Statements;

namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public class AstLambda : AstExpression
    {
        public readonly AstParameterList ParameterList;
        public readonly AstStatement Body;

        public override AstExpressionType ExpressionType => AstExpressionType.Lambda;

        public AstLambda(Source src, AstParameterList parameterList, AstStatement body)
            : base(src)
        {
            ParameterList = parameterList;
            Body = body;
        }
    }
}