using System.Collections.Generic;
using System.Text;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        string PrintableArgumentList(IReadOnlyList<AstArgument> arguments)
        {
            var sb = new StringBuilder("(");
            var compiledList = CompileArgumentList(arguments);
            for (int i = 0; i < compiledList.Length; i++)
            {
                var compiledArg = compiledList[i];
                var mode = (compiledArg is AddressOf ? (compiledArg as AddressOf).AddressType + " " : "").ToLower();
                var type = compiledArg.ReturnType;
                sb.Append((i > 0 ? "," : "") + mode + type);
            }
            sb.Append(")");
            return sb.ToString();
        }

        string PrintableParameterList(Parameter[] parameters)
        {
            var sb = new StringBuilder("(");
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var modifier = parameter.Modifier != 0 ? parameter.Modifier.ToString().ToLower() + " " : "";
                var type = parameter.Type;
                sb.Append((i > 0 ? "," : "") + modifier + type);
            }
            sb.Append(")");
            return sb.ToString();
        }

        bool VerifyLValue(Expression e)
        {
            if (e == null ||
                e.ReturnType.IsReferenceType)
                return true;

            switch (e.ExpressionType)
            {
                case ExpressionType.LoadLocal:
                case ExpressionType.LoadArgument:
                case ExpressionType.LoadElement:
                case ExpressionType.This:
                case ExpressionType.Base:
                    return true;

                case ExpressionType.AddressOf:
                    return VerifyLValue((e as AddressOf).Operand);
                case ExpressionType.LoadField:
                    return VerifyLValue((e as LoadField).Object);

                case ExpressionType.CallMethod:
                    Log.Error(e.Source, ErrorCode.E2017, "Cannot modify the return value of a method call");
                    return false;
                case ExpressionType.CastOp:
                    Log.Error(e.Source, ErrorCode.E2088, "Cannot modify the return value of a cast");
                    return false;
                case ExpressionType.GetProperty:
                case ExpressionType.SetProperty:
                    Log.Error(e.Source, ErrorCode.E2018, "Cannot modify the return value of a property or indexer accessor call");
                    return false;
                default:
                    Log.Error(e.Source, ErrorCode.E2022, "Cannot modify the return value of an expression of type <" + e.ExpressionType + ">");
                    return false;
            }
        }
    }
}
