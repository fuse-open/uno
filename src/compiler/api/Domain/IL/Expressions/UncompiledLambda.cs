using System;
using System.Text;
using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class UncompiledLambda : Expression
    {
        public AstLambda AstLambda;

        public UncompiledLambda(Source src, AstLambda astLambda) : base(src)
        {
            AstLambda = astLambda;
        }

        public override ExpressionType ExpressionType => ExpressionType.UncompiledLambda;
        public override DataType ReturnType => DataType.MethodGroup;
        public override Expression CopyExpression(CopyState state)
        {
            return new UncompiledLambda(Source, AstLambda);
        }

        public override void Disassemble(StringBuilder sb, ExpressionUsage u = ExpressionUsage.Argument)
        {
            throw new NotImplementedException("Can't disassemble an uncompiled lambda");
        }
    }
}
