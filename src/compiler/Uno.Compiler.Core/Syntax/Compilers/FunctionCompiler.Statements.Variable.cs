using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Statements;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        void ReportVariableAlreadyDeclared(Source src, string name, int line)
        {
            Log.Error(src, ErrorCode.E3412, name.Quote() + " is already declared in this scope, at line " + line);
        }

        bool VerifyVariableNameRecursive(VariableScope scope, Source src, string name)
        {
            if (scope.Variables.ContainsKey(name))
            {
                ReportVariableAlreadyDeclared(src, name, scope.Variables[name].Source.Line);
                return false;
            }

            for (int i = scope.Scopes.Count - 1; i >= 0; i--)
                if (!VerifyVariableNameRecursive(scope.Scopes[i], src, name))
                    return false;

            return true;
        }

        bool VerifyVariableName(Source src, string name)
        {
            if (!VerifyVariableNameRecursive(CurrentVariableScope, src, name))
                return false;

            for (int i = VariableScopeStack.Count - 2; i >= 0; i--)
            {
                var vs = VariableScopeStack[i];

                if (vs.Variables.ContainsKey(name))
                {
                    ReportVariableAlreadyDeclared(src, name, vs.Variables[name].Source.Line);
                    return false;
                }
            }

            foreach (var p in Function.Parameters)
            {
                if (p.Name == name)
                {
                    ReportVariableAlreadyDeclared(src, name, p.Source.Line);
                    return false;
                }
            }

            var db = MetaProperty?.Parent.TryFindDrawBlockParent();

            if (db != null)
            {
                // check captured locals
                if (db.CapturedLocals.ContainsKey(name))
                {
                    ReportVariableAlreadyDeclared(src, name, db.CapturedLocals[name].Source.Line);
                    return false;
                }

                // check captured parameters
                foreach (var p in db.Method.Parameters)
                {
                    if (p.Name == name)
                    {
                        ReportVariableAlreadyDeclared(src, name, p.Source.Line);
                        return false;
                    }
                }
            }

            return true;
        }

        public FixedArrayDeclaration CompileFixedArrayDeclaration(Source src, DataType dt, string name, AstFixedArrayInitializer s)
        {
            DataType et = null;
            Expression size = null;
            Expression[] values = null;

            if (s.OptionalElementType != null)
                et = NameResolver.GetType(Namescope, s.OptionalElementType);

            if (s.OptionalSize != null)
                size = CompileImplicitCast(s.OptionalSize.Source, Essentials.Int, CompileExpression(s.OptionalSize));

            if (s.OptionalValues != null)
            {
                values = new Expression[s.OptionalValues.Count];

                for (int i = 0; i < values.Length; i++)
                    values[i] = CompileExpression(s.OptionalValues[i]);

                if (et != null)
                {
                    for (int i = 0; i < values.Length; i++)
                        values[i] = CompileImplicitCast(values[i].Source, et, values[i]);
                }
                else
                {
                    var it = TryGetImplicitElementType(values);

                    if (it == null)
                    {
                        Log.Error(s.Source, ErrorCode.E0000, "No best type found for implicitly typed fixed array");
                        it = DataType.Invalid;
                    }

                    et = it;
                }

                if (size != null)
                {
                    Compiler.ConstantFolder.TryMakeConstant(ref size);

                    if (size.ExpressionType != ExpressionType.Constant || (int)size.ConstantValue != values.Length)
                        Log.Error(size.Source, ErrorCode.E0000, "Fixed array size does not match initializer count");
                }
                else
                {
                    size = new Constant(s.Source, Essentials.Int, values.Length);
                }
            }
            else if (et == null)
            {
                Log.Error(s.Source, ErrorCode.E0000, "Must provide non-empty initializer list for implicitly typed fixed arrays");
                et = DataType.Invalid;
            }
            else if (size == null)
            {
                Log.Error(s.Source, ErrorCode.E0000, "Must provide non-empty initializer list for implicitly sized fixed arrays");
                size = new Constant(s.Source, DataType.Invalid, -1);
            }

            if (dt != null)
            {
                var fat = dt as FixedArrayType;

                if (fat == null ||
                    !fat.ElementType.Equals(et) ||
                    fat.OptionalSize != null && !fat.OptionalSize.Equals(size))
                    Log.Error(src, ErrorCode.E0000, "Declaring type must be 'fixed " + et + "[" + size + "]' or 'fixed " + et + "[]'");
            }

            var var = new Variable(s.Source, Function, name, new FixedArrayType(s.Source, et, size, Essentials.Int));
            CurrentVariableScope.Variables[name] = var;
            return new FixedArrayDeclaration(var, values);
        }

        Statement CompileVariableDeclaration(AstVariableDeclaration e)
        {
            DataType dt = null;
            switch (e.Type.ExpressionType)
            {
                case AstExpressionType.Var:
                {
                    if (e.Variables.Count != 1)
                    {
                        Log.Error(e.Source, ErrorCode.E0000, "Implicitly-typed variables cannot have multiple declarators");
                        dt = DataType.Invalid;
                    }
                    else if (e.Variables[0].OptionalValue == null)
                    {
                        Log.Error(e.Source, ErrorCode.E3400, "Variable declarations using 'var' must provide an initial value");
                        dt = DataType.Invalid;
                    }

                    if (e.Modifier == AstVariableModifier.Const)
                    {
                        Log.Error(e.Source, ErrorCode.E0000, "Implicitly-typed variables cannot be constant");
                        dt = DataType.Invalid;
                    }
                    break;
                }
                default:
                {
                    dt = NameResolver.GetType(Namescope, e.Type);
                    break;
                }
            }

            Variable root = null;
            Variable next = null;
            foreach (var v in e.Variables)
            {
                VerifyVariableName(v.Name.Source, v.Name.Symbol);
                Expression initValue = null;

                if (v.OptionalValue != null)
                {
                    initValue = CompileExpression(v.OptionalValue);
                    if (e.Modifier == 0 &&
                        initValue is NewObject &&
                        e.Type.ExpressionType != AstExpressionType.Var &&
                        initValue.ReturnType.Equals(dt) &&
                        dt.BuiltinType == 0)
                        Log.Warning3(e.Source, ErrorCode.W3401, "Variable can be implicitly typed using 'var'");

                    if (dt != null)
                        initValue = CompileImplicitCast(e.Source, dt, initValue);
                    else
                        dt = initValue.ReturnType;

                    if (e.Modifier == AstVariableModifier.Const)
                        Compiler.ConstantFolder.TryMakeConstant(ref initValue);
                }

                var var = new Variable(v.Name.Source, Function, v.Name.Symbol, dt, 
                    (VariableType) e.Modifier,
                    initValue);
                CurrentVariableScope.Variables[v.Name.Symbol] = var;
                next = root == null
                    ? root = var
                    : next.Next = var;
            }

            return new VariableDeclaration(root);
        }

        Statement CompileFixedArrayDeclaration(AstFixedArrayDeclaration e)
        {
            var s = e.Type;
            var size = s.OptionalSize != null
                ? CompileImplicitCast(s.OptionalSize.Source, Essentials.Int, 
                    CompileExpression(s.OptionalSize))
                : null;
            var elementType = NameResolver.GetType(Namescope, s.ElementType);
            var dt = new FixedArrayType(s.Source, elementType, size, Essentials.Int);

            VerifyVariableName(e.Source, e.Name.Symbol);

            var fixedInit = e.OptionalValue as AstFixedArrayInitializer;
            if (fixedInit != null)
                return CompileFixedArrayDeclaration(e.Source, dt, e.Name.Symbol, fixedInit);

            var var = new Variable(e.Source, Function, e.Name.Symbol, dt, 0, 
                e.OptionalValue != null
                ? CompileImplicitCast(e.Source, dt, CompileExpression(e.OptionalValue))
                : null);
            CurrentVariableScope.Variables[e.Name.Symbol] = var;
            return new FixedArrayDeclaration(var, null);
        }
    }
}
