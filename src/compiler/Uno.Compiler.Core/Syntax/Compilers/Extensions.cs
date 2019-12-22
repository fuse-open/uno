using System;
using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.IO;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    static class Extensions
    {
        public static Expression CompileExpression(this Compiler compiler, AstExpression expression, Namescope scope, DataType expectedType = null)
        {
            var fc = new FunctionCompiler(compiler, scope);
            var result = fc.CompileExpression(expression);

            if (expectedType != null)
                result = fc.CompileImplicitCast(expression.Source, expectedType, result);

            return result;
        }

        public static Constant CompileConstant(this Compiler compiler, AstExpression expression, Namescope scope, DataType expectedType = null)
        {
            var value = CompileExpression(compiler, expression, scope, expectedType);
            var result = compiler.ConstantFolder.TryMakeConstant(value);

            if (result == null)
            {
                if (value != Expression.Invalid)
                    compiler.Log.Error(expression.Source, ErrorCode.E0059, "Expression must be constant");

                return new Constant(expression.Source, DataType.Invalid, 0);
            }

            return result;
        }

        public static Parameter[] CompileParameterList(this Compiler compiler, DataType type, IReadOnlyList<AstParameter> parameters, List<Action> deferredActions)
        {
            var result = new Parameter[parameters.Count];

            for (int i = 0; i < parameters.Count; i++)
            {
                var pd = parameters[i];
                var dt = compiler.NameResolver.GetType(type, pd.OptionalType);
                var attributes = CompileAttributes(compiler, type, pd.Attributes);
                var p = new Parameter(pd.Name.Source, attributes, pd.Modifier, dt, pd.Name.Symbol, null);

                if (pd.OptionalValue != null)
                    deferredActions.Add(() =>
                    {
                        p.OptionalDefault = compiler.CompileExpression(pd.OptionalValue, type, dt);
                        compiler.ConstantFolder.TryMakeConstant(ref p.OptionalDefault);
                    });

                result[i] = p;
            }

            return result;
        }

        static AstIdentifier GetSuffixedTypeIdentifier(AstIdentifier id, string suffix)
        {
            return id.Symbol.EndsWith(suffix) 
                ? id 
                : new AstIdentifier(id.Source, id.Symbol + suffix);
        }

        static AstExpression GetSuffixedTypeExpression(AstExpression type, string suffix)
        {
            if (type is AstIdentifier)
                return GetSuffixedTypeIdentifier(type as AstIdentifier, suffix);
            if (type is AstMember)
                return new AstMember((type as AstMember).Base, GetSuffixedTypeIdentifier((type as AstMember).Name, suffix));
            if (type is AstParameterizer)
                return new AstParameterizer(GetSuffixedTypeExpression((type as AstParameterizer).Base, suffix), (type as AstParameterizer).Arguments);

            return type;
        }

        public static NewObject TryCompileSuffixedObject(this Compiler compiler, Namescope scope, AstExpression type, string suffix, IReadOnlyList<AstArgument> args)
        {
            var sym = CompileExpression(compiler, new AstNew(type.Source, GetSuffixedTypeExpression(type, suffix), args), scope);
            var result = sym as NewObject;

            if (result == null)
            {
                if (!sym.IsInvalid)
                    compiler.Log.Error(type.Source, ErrorCode.I0045, "Compiled expression was not a 'NewObject' node");

                return null;
            }

            for (int i = 0; i < result.Arguments.Length; i++)
            {
                if (result.Arguments[i].ExpressionType != ExpressionType.Constant)
                {
                    var c = compiler.ConstantFolder.TryMakeConstant(result.Arguments[i]);

                    if (c == null)
                    {
                        compiler.Log.Error(result.Arguments[i].Source, ErrorCode.E0000, "Argument must be a constant value");
                        continue;
                    }

                    result.Arguments[i] = c;
                }
            }

            return result;
        }

        public static NewObject[] CompileAttributes(this Compiler compiler, Namescope scope, IReadOnlyList<AstAttribute> attributes)
        {
            if (attributes.Count == 0)
                return AttributeList.Empty;

            var result = new List<NewObject>();

            foreach (var ast in attributes)
            {
                if (!compiler.Environment.Test(ast.Attribute.Source, ast.OptionalCondition))
                    continue;

                if (ast.Modifier != 0)
                    compiler.Log.Error(ast.Attribute.Source, ErrorCode.E0000, "Attribute modifier " + ast.Modifier.ToString().ToLower().Quote() + " is not supported");

                var attr = TryCompileSuffixedObject(compiler, scope, ast.Attribute, "Attribute", ast.Arguments);

                if (attr != null)
                    result.Add(attr);
            }

            return result.ToArray();
        }
    }
}
