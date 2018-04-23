using System;

namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public static class AstOperators
    {
        public static AstUnaryType ParseUnary(string s)
        {
            switch (s)
            {
                case "!": return AstUnaryType.LogNot;
                case "~": return AstUnaryType.BitwiseNot;
                case "-": return AstUnaryType.Negate;
                default: throw new Exception("invalid unop: " + s);
            }
        }

        public static AstBinaryType ParseBinary(string s)
        {
            switch (s)
            {
                case "=": return AstBinaryType.Assign;
                case "+=": return AstBinaryType.AddAssign;
                case "-=": return AstBinaryType.SubAssign;
                case "*=": return AstBinaryType.MulAssign;
                case "/=": return AstBinaryType.DivAssign;
                case "%=": return AstBinaryType.ModAssign;
                case "&=": return AstBinaryType.BitwiseAndAssign;
                case "^=": return AstBinaryType.BitwiseXorAssign;
                case "|=": return AstBinaryType.BitwiseOrAssign;
                case "<<=": return AstBinaryType.ShiftLeftAssign;
                case ">>=": return AstBinaryType.ShiftRightAssign;
                case "||": return AstBinaryType.LogOr;
                case "&&": return AstBinaryType.LogAnd;
                case "|": return AstBinaryType.BitwiseOr;
                case "^": return AstBinaryType.BitwiseXor;
                case "&": return AstBinaryType.BitwiseAnd;
                case "==": return AstBinaryType.Equal;
                case "!=": return AstBinaryType.NotEqual;
                case "<": return AstBinaryType.LessThan;
                case ">": return AstBinaryType.GreaterThan;
                case "<=": return AstBinaryType.LessThanOrEqual;
                case ">=": return AstBinaryType.GreaterThanOrEqual;
                case "<<": return AstBinaryType.ShiftLeft;
                case ">>": return AstBinaryType.ShiftRight;
                case "+": return AstBinaryType.Add;
                case "-": return AstBinaryType.Sub;
                case "*": return AstBinaryType.Mul;
                case "/": return AstBinaryType.Div;
                case "%": return AstBinaryType.Mod;
                case "??": return AstBinaryType.Null;
                default: throw new Exception("invalid binop: " + s);
            }
        }

        public static string ToSymbol(this AstBinaryType s)
        {
            switch (s)
            {
                case AstBinaryType.Assign: return "=";
                case AstBinaryType.AddAssign: return "+=";
                case AstBinaryType.SubAssign: return "-=";
                case AstBinaryType.MulAssign: return "*=";
                case AstBinaryType.DivAssign: return "/=";
                case AstBinaryType.ModAssign: return "%=";
                case AstBinaryType.BitwiseAndAssign: return "&=";
                case AstBinaryType.BitwiseXorAssign: return "^=";
                case AstBinaryType.BitwiseOrAssign: return "|=";
                case AstBinaryType.ShiftLeftAssign: return "<<=";
                case AstBinaryType.ShiftRightAssign: return ">>=";
                case AstBinaryType.LogOr: return "||";
                case AstBinaryType.LogAnd: return "&&";
                case AstBinaryType.BitwiseOr: return "|";
                case AstBinaryType.BitwiseXor: return "^";
                case AstBinaryType.BitwiseAnd: return "&";
                case AstBinaryType.Equal: return "==";
                case AstBinaryType.NotEqual: return "!=";
                case AstBinaryType.LessThan: return "<";
                case AstBinaryType.GreaterThan: return ">";
                case AstBinaryType.LessThanOrEqual: return "<=";
                case AstBinaryType.GreaterThanOrEqual: return ">=";
                case AstBinaryType.ShiftLeft: return "<<";
                case AstBinaryType.ShiftRight: return ">>";
                case AstBinaryType.Add: return "+";
                case AstBinaryType.Sub: return "-";
                case AstBinaryType.Mul: return "*";
                case AstBinaryType.Div: return "/";
                case AstBinaryType.Mod: return "%";
                case AstBinaryType.Null: return "??";
                case AstBinaryType.Sequence: return ",";
                default: throw new Exception("invalid binop: " + s);
            }
        }

        public static string ToSymbol(this AstUnaryType op)
        {
            switch (op)
            {
                case AstUnaryType.DecreasePrefix: return "$prefix--";
                case AstUnaryType.DecreasePostfix: return "$postfix--";
                case AstUnaryType.IncreasePrefix: return "$prefix++";
                case AstUnaryType.IncreasePostfix: return "$postfix++";
                case AstUnaryType.Negate: return "-";
                case AstUnaryType.LogNot: return "!";
                case AstUnaryType.BitwiseNot: return "~";
                default: throw new Exception("invalid unop: " + op);
            }
        }
    }
}
