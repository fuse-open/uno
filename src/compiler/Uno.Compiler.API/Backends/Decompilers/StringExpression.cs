using System;
using System.Text;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.API.Backends.Decompilers
{
    public class StringExpression : Expression
    {
        public readonly string String;
        public readonly DataType Type;

        public override ExpressionType ExpressionType => ExpressionType.Other;
        public override DataType ReturnType => Type;

        public StringExpression(Source src, DataType type, string expression)
            : base(src)
        {
            Type = type;
            String = expression;
        }

        public override Expression CopyExpression(CopyState state)
        {
            throw new NotImplementedException();
        }

        public override void Disassemble(StringBuilder sb, ExpressionUsage u = ExpressionUsage.Argument)
        {
            throw new NotImplementedException();
        }
    }
}