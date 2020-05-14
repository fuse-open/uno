using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.Syntax.Compilers;

namespace Uno.Compiler.Core.Syntax.Builders
{
    public partial class TypeBuilder
    {
        static object TypedEnumValue(DataType dt, dynamic v)
        {
            switch (dt.BuiltinType)
            {
                case BuiltinType.Byte: return (byte)v;
                case BuiltinType.SByte: return (sbyte)v;
                case BuiltinType.Short: return (short)v;
                case BuiltinType.UShort: return (ushort)v;
                case BuiltinType.UInt: return (uint)v;
                case BuiltinType.Int: return (int)v;
                case BuiltinType.ULong: return (ulong)v;
                case BuiltinType.Long: return (long)v;
            }

            return null; // should not happen, the above are the only allowed enum types
        }

        public void CreateEnum(AstEnum ast, Namescope parent)
        {
            var result = new EnumType(ast.Name.Source, parent, ast.DocComment, GetTypeModifiers(parent, ast.Modifiers), ast.Name.Symbol);

            if (parent is DataType)
                (parent as DataType).NestedTypes.Add(result);
            else if (parent is Namespace)
                (parent as Namespace).Types.Add(result);
            else
                Log.Error(result.Source, ErrorCode.I3037, "'enum' is not allowed in this context");

            if (ast.Attributes.Count > 0)
                EnqueueAttributes(result,
                    x => result.SetAttributes(_compiler.CompileAttributes(result.Parent, ast.Attributes)));

            EnqueueType(result,
                assignBaseType: x =>
                {
                    x.SetBase(ast.OptionalBaseType != null
                        ? _resolver.GetType(x.Parent, ast.OptionalBaseType)
                        : _ilf.Essentials.Int);
                },
                populate: x =>
                {
                    var parameterizedType = Parameterize(x);
                    parameterizedType.AssignBaseType();

                    x.Methods.Add(
                        CreateMethod(parameterizedType, "HasFlag", _ilf.Essentials.Bool,
                            CreateParameter(ast.Name, parameterizedType, "flag")));
                    x.Operators.Add(
                        CreateOperator(parameterizedType, OperatorType.Equality, _ilf.Essentials.Bool,
                            CreateParameter(ast.Name, parameterizedType, "left"),
                            CreateParameter(ast.Name, parameterizedType, "right")));
                    x.Operators.Add(
                        CreateOperator(parameterizedType, OperatorType.Inequality, _ilf.Essentials.Bool,
                            CreateParameter(ast.Name, parameterizedType, "left"),
                            CreateParameter(ast.Name, parameterizedType, "right")));
                    x.Operators.Add(
                        CreateOperator(parameterizedType, OperatorType.BitwiseOr, parameterizedType,
                            CreateParameter(ast.Name, parameterizedType, "left"),
                            CreateParameter(ast.Name, parameterizedType, "right")));
                    x.Operators.Add(
                        CreateOperator(parameterizedType, OperatorType.BitwiseAnd, parameterizedType,
                            CreateParameter(ast.Name, parameterizedType, "left"),
                            CreateParameter(ast.Name, parameterizedType, "right")));
                    x.Operators.Add(
                        CreateOperator(parameterizedType, OperatorType.ExclusiveOr, parameterizedType,
                            CreateParameter(ast.Name, parameterizedType, "left"),
                            CreateParameter(ast.Name, parameterizedType, "right")));
                    x.Operators.Add(
                        CreateOperator(parameterizedType, OperatorType.Subtraction, parameterizedType,
                            CreateParameter(ast.Name, parameterizedType, "left"),
                            CreateParameter(ast.Name, parameterizedType, "right")));
                    x.Operators.Add(
                        CreateOperator(parameterizedType, OperatorType.OnesComplement, parameterizedType,
                            CreateParameter(ast.Name, parameterizedType, "operand")));

                    dynamic previousValue = -1;
                    foreach (var s in ast.Literals)
                    {
                        dynamic value;
                        if (s.OptionalValue != null)
                        {
                            var c = _compiler.CompileConstant(s.OptionalValue, parameterizedType, parameterizedType.Base);
                            previousValue = c.Value;
                            value = previousValue;
                        }
                        else
                            value = ++previousValue;

                        var literal = new Literal(s.Name.Source, parameterizedType, s.Name.Symbol, 
                            s.DocComment, Modifiers.Public, parameterizedType, TypedEnumValue(parameterizedType.Base, value));

                        if (s.Attributes.Count > 0)
                            EnqueueAttributes(literal,
                                _ => literal.SetAttributes(_compiler.CompileAttributes(result, s.Attributes)));

                        result.Literals.Add(literal);
                    }

                    if (ast.Members != null)
                        Log.Error(ast.Name.Source, ErrorCode.E3021, "'enum' cannot contain members");
                });
        }
    }
}
