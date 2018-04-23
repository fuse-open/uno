using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.IL.Members
{
    public sealed class Operator : Function
    {
        public readonly OperatorType Type;

        public override MemberType MemberType => MemberType.Operator;

        public Operator(Source src, DataType owner, OperatorType type, string comment, Modifiers modifiers, DataType returnType, Parameter[] parameters, Scope optionalBody = null)
            : base(src, owner, "op_" + type, returnType, parameters, optionalBody, comment, modifiers)
        {
            Type = type;
        }

        public string Symbol
        {
            get
            {
                switch (Type)
                {
                    case OperatorType.UnaryPlus:
                        return "+";
                    case OperatorType.UnaryNegation:
                        return "-";
                    case OperatorType.OnesComplement:
                        return "~";
                    case OperatorType.LogicalNot:
                        return "!";
                    case OperatorType.Addition:
                        return "+";
                    case OperatorType.Subtraction:
                        return "-";
                    case OperatorType.Multiply:
                        return "*";
                    case OperatorType.Division:
                        return "/";
                    case OperatorType.Modulus:
                        return "%";
                    case OperatorType.BitwiseAnd:
                        return "&";
                    case OperatorType.BitwiseOr:
                        return "|";
                    case OperatorType.ExclusiveOr:
                        return "^";
                    case OperatorType.GreaterThan:
                        return ">";
                    case OperatorType.GreaterThanOrEqual:
                        return ">=";
                    case OperatorType.LessThan:
                        return "<";
                    case OperatorType.LessThanOrEqual:
                        return "<=";
                    case OperatorType.Equality:
                        return "==";
                    case OperatorType.Inequality:
                        return "!=";
                    case OperatorType.LeftShift:
                        return "<<";
                    case OperatorType.RightShift:
                        return ">>";
                    default:
                        return "(unknown)";
                }
            }
        }

        public static OperatorType Parse(int args, string symbol)
        {
            switch (args)
            {
                case 1:
                    return ParseUnary(symbol);
                case 2:
                    return ParseBinary(symbol);
                default:
                    return 0;
            }
        }

        public static OperatorType ParseBinary(string symbol)
        {
            switch (symbol)
            {
                case "+":
                    return OperatorType.Addition;
                case "-":
                    return OperatorType.Subtraction;
                case "*":
                    return OperatorType.Multiply;
                case "/":
                    return OperatorType.Division;
                case "%":
                    return OperatorType.Modulus;
                case "&":
                    return OperatorType.BitwiseAnd;
                case "|":
                    return OperatorType.BitwiseOr;
                case "^":
                    return OperatorType.ExclusiveOr;
                case ">":
                    return OperatorType.GreaterThan;
                case ">=":
                    return OperatorType.GreaterThanOrEqual;
                case "<":
                    return OperatorType.LessThan;
                case "<=":
                    return OperatorType.LessThanOrEqual;
                case "==":
                    return OperatorType.Equality;
                case "!=":
                    return OperatorType.Inequality;
                case "<<":
                    return OperatorType.LeftShift;
                case ">>":
                    return OperatorType.RightShift;
                default:
                    return 0;
            }
        }

        public static OperatorType ParseUnary(string symbol)
        {
            switch (symbol)
            {
                case "+":
                    return OperatorType.UnaryPlus;
                case "-":
                    return OperatorType.UnaryNegation;
                case "~":
                    return OperatorType.OnesComplement;
                case "!":
                    return OperatorType.LogicalNot;
                default:
                    return 0;
            }
        }
    }
}