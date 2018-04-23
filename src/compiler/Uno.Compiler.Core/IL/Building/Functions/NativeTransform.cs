using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.IL.Building.Functions
{
    class NativeTransform : CompilerPass
    {
        private Variable[] _parameters;
        readonly List<Variable> Variables = new List<Variable>();
        readonly List<HashSet<object>> ValidPointerStack = new List<HashSet<object>>();
        readonly HashSet<object> ValidPointers = new HashSet<object>();

        HashSet<object> CurrentScopeValidPointers => ValidPointerStack[ValidPointerStack.Count - 1];

        public NativeTransform(CompilerPass parent)
            : base(parent)
        {
        }

        public override bool Begin(Function f)
        {
            _parameters = null;
            Variables.Clear();
            return f.HasBody;
        }

        public override void End(Function f)
        {
            for (int i = Variables.Count - 1; i >= 0; i--)
                f.Body.Statements.Insert(0, 
                    new VariableDeclaration(Variables[i]));
        }

        public override void BeginScope(Scope s)
        {
            ValidPointerStack.Add(new HashSet<object>());
        }

        public override void EndScope(Scope s)
        {
            ValidPointerStack.RemoveLast();
        }

        public override void Begin(ref Statement e)
        {
            ValidPointers.Clear();

            switch (e.StatementType)
            {
                case StatementType.VariableDeclaration:
                {
                    var s = e as VariableDeclaration;
                    for (var var = s.Variable; var != null; var = var.Next)
                        VerifyPointer(var, var.OptionalValue);
                    break;
                }
            }
        }

        void OnExpressionStatementEnd(ref Expression e)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.SequenceOp:
                {
                    var s = e as SequenceOp;
                    OnExpressionStatementEnd(ref s.Left);
                    OnExpressionStatementEnd(ref s.Right);
                    break;
                }
            }
        }

        public override void End(ref Statement e)
        {
            switch (e.StatementType)
            {
                case StatementType.Expression:
                case StatementType.VariableDeclaration:
                    foreach (var p in ValidPointers)
                        CurrentScopeValidPointers.Add(p);
                    break;
            }

            switch (e.StatementType)
            {
                case StatementType.Expression:
                {
                    var es = e as Expression;
                    OnExpressionStatementEnd(ref es);
                    e = es;
                    break;
                }
            }
        }

        bool IsNonNullExpression(Expression e)
        {
            if (e.ReturnType.IsValueType)
                return true;

            switch (e.ExpressionType)
            {
                case ExpressionType.LoadLocal:
                {
                    var s = e as LoadLocal;
                    return CurrentScopeValidPointers.Contains(s.Variable);
                }
                case ExpressionType.LoadArgument:
                {
                    var s = e as LoadArgument;
                    return CurrentScopeValidPointers.Contains(s.Parameter) && !s.Parameter.IsReference;
                }
                case ExpressionType.CastOp:
                {
                    var s = e as CastOp;
                    return IsNonNullExpression(s.Operand);
                }
                case ExpressionType.AddressOf:
                {
                    var s = (AddressOf)e;
                    return s.AddressType != 0;
                }

                case ExpressionType.This:
                case ExpressionType.Base:
                case ExpressionType.NewArray:
                case ExpressionType.NewObject:
                case ExpressionType.NewDelegate:
                case ExpressionType.AllocObject:
                case ExpressionType.LoadPtr:
                    return true;

                case ExpressionType.CallMethod:
                    return ((CallMethod) e).Method.Prototype.IsConstructor;

                default:
                    return false;
            }
        }

        void OnObject(ref Expression e)
        {
            if (e == null || IsNonNullExpression(e))
                return;

            switch (e.ExpressionType)
            {
                case ExpressionType.LoadLocal:
                {
                    var s = e as LoadLocal;
                    ValidPointers.Add(s.Variable);
                    break;
                }
                case ExpressionType.LoadArgument:
                {
                    var s = e as LoadArgument;
                    ValidPointers.Add(s.Parameter);
                    break;
                }
            }

            switch (e.ExpressionType)
            {
                case ExpressionType.AddressOf:
                {
                    var s = (AddressOf) e;
                    OnObject(ref s.Operand);
                    break;
                }
                default:
                {
                    e = new LoadPtr(e);
                    break;
                }
            }
        }

        void VerifyPointer(object variable, Expression value)
        {
            if (value != null && IsNonNullExpression(value))
                ValidPointers.Add(variable);
            else
                foreach (var s in ValidPointerStack)
                    s.Remove(variable);
        }

        bool GetIndirection(int index, Parameter p, out Variable v)
        {
            if (!p.IsReference && p.Type.IsValueType &&
                Backend.PassByRef(Function))
            {
                if (_parameters == null)
                    _parameters = new Variable[Function.Parameters.Length];

                if (_parameters[index] == null)
                {
                    _parameters[index] = new Variable(p.Source, Function, p.Name + "_", p.Type,
                        VariableType.Default, new LoadArgument(p.Source, Function, index));
                    Variables.Add(_parameters[index]);
                }

                v = _parameters[index];
                return true;
            }

            v = null;
            return false;
        }

        void GetIndirection(ref Expression e)
        {
            Variable var;
            var s = e as LoadArgument;
            if (s != null && GetIndirection(s.Index, s.Parameter, out var))
                e = new LoadLocal(s.Source, var);
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.FixOp:
                {
                    var s = (FixOp) e;
                    GetIndirection(ref s.Operand);
                    break;
                }
                case ExpressionType.AddressOf:
                {
                    var s = (AddressOf) e;
                    switch (s.AddressType)
                    {
                        case AddressType.Out:
                        case AddressType.Ref:
                            GetIndirection(ref s.Operand);
                            break;
                    }
                    break;
                }
                case ExpressionType.StoreField:
                {
                    var s = e as StoreField;
                    OnObject(ref s.Object);
                    break;
                }
                case ExpressionType.StoreElement:
                {
                    var s = e as StoreElement;
                    OnObject(ref s.Array);
                    break;
                }
                case ExpressionType.StoreArgument:
                {
                    Variable var;
                    var s = e as StoreArgument;
                    if (GetIndirection(s.Index, s.Parameter, out var))
                    {
                        e = new StoreLocal(s.Source, var, s.Value);
                        Begin(ref e, u);
                    }
                    else
                        VerifyPointer(s.Parameter, s.Value);
                    break;
                }
                case ExpressionType.LoadArgument:
                {
                    Variable var;
                    var s = e as LoadArgument;
                    if (GetIndirection(s.Index, s.Parameter, out var))
                    {
                        e = new LoadLocal(s.Source, var);
                        Begin(ref e, u);
                    }
                    break;
                }
                case ExpressionType.StoreLocal:
                {
                    var s = e as StoreLocal;
                    VerifyPointer(s.Variable, s.Value);
                    break;
                }
                case ExpressionType.LoadField:
                {
                    var s = e as LoadField;
                    OnObject(ref s.Object);
                    break;
                }
                case ExpressionType.LoadElement:
                {
                    var s = e as LoadElement;
                    OnObject(ref s.Array);
                    break;
                }
                case ExpressionType.AddListener:
                case ExpressionType.RemoveListener:
                case ExpressionType.GetProperty:
                case ExpressionType.SetProperty:
                case ExpressionType.CallMethod:
                case ExpressionType.CallDelegate:
                {
                    var s = (CallExpression) e;
                    OnObject(ref s.Object);

                    if (!s.Function.ReturnType.IsVoid && (
                            Backend.IsConstrained(s.Function) ||
                            s.Object is Base && s.Function.IsVirtual)
                        )
                    {
                        s.Storage = new Variable(s.Source, Function, Type.GetUniqueIdentifier("ret"), s.Function.ReturnType, VariableType.Indirection);
                        Variables.Add(s.Storage);
                    }
                    break;
                }
                case ExpressionType.NewDelegate:
                {
                    var s = (NewDelegate) e;
                    OnObject(ref s.Object);
                    break;
                }
                case ExpressionType.NewArray:
                {
                    var s = e as NewArray;

                    // Structs with non-trivial copy-ctor cannot be passed through '...' (uInitArray<T>)
                    if (s.Initializers != null && IsNonTrivialStruct(s.ArrayType.ElementType))
                    {
                        var v = new Variable(s.Source, null, Type.GetUniqueIdentifier("array"), s.ReturnType);
                        e = new StoreLocal(s.Source, v, new NewArray(s.Source, s.ArrayType, new Constant(s.Source, Essentials.Int, s.Initializers.Length)));

                        for (int i = 0; i < s.Initializers.Length; i++)
                            e = new SequenceOp(e,
                                               new StoreElement(s.Source,
                                                                new LoadLocal(s.Source, v),
                                                                new Constant(s.Source, Essentials.Int, i),
                                                                s.Initializers[i]));

                        e = new SequenceOp(e, new LoadLocal(s.Source, v));
                    }
                    break;
                }
            }
        }

        public override void End(ref Expression e, ExpressionUsage u)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.SequenceOp:
                {
                    var s = e as SequenceOp;
                    // This fixes bug https://github.com/Outracks/Uno/issues/219
                    if (s.Right is Constant && s.Right.ConstantValue == null)
                        s.Right = new CastOp(s.Right.Source, s.Right.ReturnType, s.Right);
                    break;
                }
                case ExpressionType.ConditionalOp:
                {
                    var s = e as ConditionalOp;
                    // This fixes bug https://github.com/Outracks/Uno/issues/219
                    if (s.True is Constant && s.True.ConstantValue == null)
                        s.True = new CastOp(s.True.Source, s.True.ReturnType, s.True);
                    break;
                }
            }
        }

        bool IsNonTrivialStruct(DataType dt)
        {
            if (dt.IsStruct)
            {
                foreach (var f in dt.EnumerateFields())
                {
                    if (f.IsStatic)
                        continue;

                    switch (f.ReturnType.TypeType)
                    {
                        default:
                            return true;
                        case TypeType.Enum:
                            break;
                        case TypeType.Struct:
                            if (IsNonTrivialStruct(f.ReturnType))
                                return true;
                            break;
                    }
                }
            }

            return false;
        }
    }
}
