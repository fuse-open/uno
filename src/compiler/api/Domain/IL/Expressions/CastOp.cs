using System.Text;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class CastOp : Expression
    {
        public Expression Operand;
        public DataType TargetType;

        public override DataType ReturnType => TargetType;
        public override ExpressionType ExpressionType => ExpressionType.CastOp;

        public CastOp(Source src, DataType targetType, Expression arg)
            : base(src)
        {
            Operand = arg;
            TargetType = targetType;
        }

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.AppendWhen(u.IsObject(), "(");
            sb.Append("(" + TargetType + ")");
            Operand.Disassemble(sb, ExpressionUsage.Operand);
            sb.AppendWhen(u.IsObject(), ")");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.Begin(ref Operand, ExpressionUsage.Operand);
            Operand.Visit(p, ExpressionUsage.Operand);
            p.End(ref Operand, ExpressionUsage.Operand);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new CastOp(Source, state.GetType(TargetType), Operand.CopyExpression(state));
        }

        public CastType CastType
        {
            get
            {
                switch (TargetType.TypeType)
                {
                    case TypeType.Enum:
                    case TypeType.Struct:
                        switch (Operand.ReturnType.TypeType)
                        {
                            case TypeType.Enum:
                            case TypeType.Struct:
                                return CastType.Default;
                            default:
                                return CastType.Unbox;
                        }
                    default:
                        switch (Operand.ReturnType.TypeType)
                        {
                            case TypeType.Enum:
                            case TypeType.Struct:
                                return CastType.Box;
                            default:
                                return Operand.ReturnType.IsGenericParameter && !Operand.ReturnType.IsReferenceType
                                        ? CastType.Box :
                                    ReturnType.IsGenericParameter && !TargetType.IsReferenceType
                                        ? CastType.Unbox :
                                    Operand.ReturnType.IsSubclassOf(TargetType)
                                        ? CastType.Up
                                        : CastType.Down;
                        }
                }
            }
        }
    }
}