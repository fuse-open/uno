using System;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;
using Uno.Compiler.API.Domain.AST.Statements;

namespace Uno.Compiler.Frontend.Analysis
{
    public class TokenAttribute : Attribute
    {
        public readonly string Value;
        public readonly AstUnaryType UnaryType;
        public readonly AstUnaryType UnaryPostfix;
        public readonly AstBinaryType BinaryType;
        public readonly Associativity Associativity = Associativity.LeftToRight;
        public readonly Precedence Precedence = Precedence.Invalid;
        public readonly ParameterModifier ParameterModifier;
        public readonly Modifiers Modifier;
        public readonly AstValueStatementType ValueStatementType;
        public readonly AstSymbolType SymbolType;
        public readonly AstEmptyStatementType EmptyStatementType;
        public readonly AstStatementModifier StatementModifier;
        public readonly AstClassType ClassType;
        public readonly OperatorType UnaryOperator;
        public readonly OperatorType BinaryOperator;
        public readonly AstConstraintType ConstraintClassType;
        public readonly BuiltinType BuiltinType;
        public readonly AstConstructorCallType ConstructorCallType;

        public TokenAttribute(string value)
        {
            Value = value;
        }

        public TokenAttribute(string value, BuiltinType builtinType)
        {
            Value = value;
            BuiltinType = builtinType;
        }

        public TokenAttribute(string value, AstClassType classType, AstConstraintType constraintClassType = 0)
        {
            Value = value;
            ClassType = classType;
            ConstraintClassType = constraintClassType;
        }

        public TokenAttribute(string value, Precedence prec, AstBinaryType binary = 0, Associativity assoc = Associativity.LeftToRight)
        {
            Value = value;
            Precedence = prec;
            BinaryType = binary;
            Associativity = assoc;
        }

        public TokenAttribute(string value, Precedence prec, AstUnaryType unary, AstBinaryType binary)
        {
            Value = value;
            Precedence = prec;
            UnaryType = unary;
            BinaryType = binary;
        }

        public TokenAttribute(string value, Precedence prec, OperatorType unaryOp, OperatorType binOp, AstUnaryType unary, AstBinaryType binary)
        {
            Value = value;
            Precedence = prec;
            UnaryType = unary;
            BinaryType = binary;
            UnaryOperator = unaryOp;
            BinaryOperator = binOp;
        }

        public TokenAttribute(string value, Precedence prec, OperatorType binOp, AstBinaryType binary)
        {
            Value = value;
            Precedence = prec;
            BinaryType = binary;
            BinaryOperator = binOp;
        }

        public TokenAttribute(string value, OperatorType unaryOp, AstUnaryType unaryType, AstUnaryType unaryPostfix = 0)
        {
            Value = value;
            UnaryOperator = unaryOp;
            UnaryType = unaryType;
            UnaryPostfix = unaryPostfix;
        }

        public TokenAttribute(string value, AstUnaryType unaryType)
        {
            Value = value;
            UnaryType = unaryType;
        }

        public TokenAttribute(string value, ParameterModifier pm)
        {
            Value = value;
            ParameterModifier = pm;
        }

        public TokenAttribute(string value, Modifiers modifier)
        {
            Value = value;
            Modifier = modifier;
        }

        public TokenAttribute(string value, AstUnaryType unaryType, AstStatementModifier statementModifier)
        {
            Value = value;
            UnaryType = unaryType;
            StatementModifier = statementModifier;
        }

        public TokenAttribute(string value, AstEmptyStatementType emptyStatement, AstValueStatementType valueStatement = 0)
        {
            Value = value;
            EmptyStatementType = emptyStatement;
            ValueStatementType = valueStatement;
        }

        public TokenAttribute(string value, AstSymbolType symbol, AstConstructorCallType callType = 0, ParameterModifier pm = 0)
        {
            Value = value;
            SymbolType = symbol;
            ConstructorCallType = callType;
            ParameterModifier = pm;
        }
    }
}