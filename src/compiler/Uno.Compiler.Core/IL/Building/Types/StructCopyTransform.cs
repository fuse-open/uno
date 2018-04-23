using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.IL.Utilities.Analyzing;

namespace Uno.Compiler.Core.IL.Building.Types
{
    class StructCopyTransform : CompilerPass
    {
        readonly Dictionary<Function, bool> FunctionsWritingToThis = new Dictionary<Function, bool>();
        readonly Dictionary<Function, bool> FunctionsNotReturningNewObject = new Dictionary<Function, bool>();
        readonly Dictionary<DataType, Method> AssignMethods = new Dictionary<DataType, Method>();
        readonly DataType DontCopyAttribute;

        Expression CurrentExpression;
        Expression ParentExpression;
        Variable[] ParameterVariables;

        public StructCopyTransform(CompilerPass parent)
            : base(parent)
        {
            DontCopyAttribute = ILFactory.GetType("Uno.Compiler.ExportTargetInterop.DontCopyStructAttribute");
        }

        void CopyField(DataType pt, Method result, Field f)
        {
            if (!f.IsStatic)
            {
                if (MustCopyType(f.ReturnType))
                    result.Body.Statements.Add(
                        Assign(new LoadField(f.Source, new This(pt.Source, pt).Address, f),
                               new LoadField(f.Source, new LoadArgument(f.Source, result, 0).Address, f)));
                else
                    result.Body.Statements.Add(
                        new StoreField(f.Source, new This(pt.Source, pt).Address, f,
                                       new LoadField(f.Source, new LoadArgument(f.Source, result, 0).Address, f)));
            }
        }

        Method GetAssignMethod(DataType dt)
        {
            Method result;
            if (AssignMethods.TryGetValue(dt, out result))
                return result;

            var pt = TypeBuilder.Parameterize(dt);
            result = new Method(dt.Source, dt, null,
                Modifiers.Public | Modifiers.Generated, "op_Assign", DataType.Void,
                new[] { new Parameter(dt.Source, AttributeList.Empty, 0, pt, "value", null) });

            if (!dt.IsMasterDefinition)
            {
                var master = GetAssignMethod(dt.MasterDefinition);
                result.SetBody(master.Body);
                result.SetMasterDefinition(master);
            }
            else
            {
                result.SetBody(new Scope(dt.Source));

                var oldParentExpression = ParentExpression;
                ParentExpression = null;

                foreach (var f in pt.Fields)
                    CopyField(pt, result, f);

                foreach (var p in pt.Properties)
                    if (p.ImplicitField != null)
                        CopyField(pt, result, p.ImplicitField);

                foreach (var p in pt.Events)
                    if (p.ImplicitField != null)
                        CopyField(pt, result, p.ImplicitField);

                ParentExpression = oldParentExpression;
            }

            dt.Methods.Add(result);
            AssignMethods.Add(dt, result);
            return result;
        }

        Expression Assign(Expression obj, Expression value)
        {
            var result = new CallMethod(obj.Source, obj.Address, GetAssignMethod(obj.ReturnType), value);
            var pseq = ParentExpression as SequenceOp;

            return ParentExpression == null ||
                    pseq != null && pseq.Left == CurrentExpression
                ? (Expression) result
                :              new SequenceOp(result, obj);
        }

        bool MustCopyType(DataType dt)
        {
            if (dt.HasAttribute(DontCopyAttribute))
                return false;

            if (dt is StructType)
                foreach (var f in dt.Fields)
                    if (!f.IsStatic)
                        return true;

            return false;
        }

        bool MustCopyBeforeCall(Function f)
        {
            bool result;
            if (FunctionsWritingToThis.TryGetValue(f, out result))
                return result;

            result = !f.IsStatic && MustCopyType(f.DeclaringType) && ThisFinder.IsWritingToThis(this, f);
            FunctionsWritingToThis.Add(f, result);
            return result;
        }

        bool MustCopyAfterCall(Function f)
        {
            bool result;
            if (FunctionsNotReturningNewObject.TryGetValue(f, out result))
                return result;

            result = !ReturnFinder.IsAlwaysReturningNewObject(this, f);
            FunctionsNotReturningNewObject.Add(f, result);
            return result;
        }

        bool MustCopyValue(Expression e)
        {
            switch (e.ExpressionType)
            {
                default:
                    return true;

                case ExpressionType.Default:
                    return false;
                case ExpressionType.NewObject:
                    return false;

                case ExpressionType.CallMethod:
                    return MustCopyAfterCall((e as CallMethod).Method);
                case ExpressionType.GetProperty:
                    return MustCopyAfterCall((e as GetProperty).Property.GetMethod);
                case ExpressionType.SetProperty:
                    return MustCopyAfterCall((e as SetProperty).Property.SetMethod);
                case ExpressionType.CallBinOp:
                    return MustCopyAfterCall((e as CallBinOp).Operator);
                case ExpressionType.CallUnOp:
                    return MustCopyAfterCall((e as CallUnOp).Operator);
                case ExpressionType.CallCast:
                    return MustCopyAfterCall((e as CallCast).Cast);
            }
        }

        public override bool Begin(DataType dt)
        {
            if (MustCopyType(dt))
                GetAssignMethod(dt);

            return true;
        }

        public override bool Begin(Function f)
        {
            if (f.IsGenerated && f.UnoName == "op_Assign" ||
                f is ShaderFunction ||
                !f.HasBody)
                return false;

            ParameterVariables = new Variable[f.Parameters.Length];
            bool foundCopyParams = false;

            for (int i = 0; i < f.Parameters.Length; i++)
            {
                if (MustCopyType(f.Parameters[i].Type) && !f.Parameters[i].IsReference)
                {
                    ParameterVariables[i] =
                        new Variable(f.Parameters[i].Source, f, f.DeclaringType.GetUniqueIdentifier(f.Parameters[i].Name),
                            f.Parameters[i].Type, VariableType.Default, new Default(f.Parameters[i].Source, f.Parameters[i].Type));

                    foundCopyParams = true;
                }
            }

            if (foundCopyParams)
            {
                var unsafeParams = ParameterFinder.FindUnsafeParameters(this, f);

                for (int i = 0; i < ParameterVariables.Length; i++)
                    if (ParameterVariables[i] != null && !unsafeParams.Contains(i))
                        ParameterVariables[i] = null;
            }

            return true;
        }

        public override void End(Function f)
        {
            ParentExpression = null;

            for (int i = ParameterVariables.Length - 1; i >= 0; i--)
                if (ParameterVariables[i] != null)
                    f.Body.Statements.Insert(0,
                        Assign(new LoadLocal(f.Parameters[i].Source, ParameterVariables[i]),
                               new LoadArgument(f.Parameters[i].Source, f, i)));
        }

        public override void Begin(ref Statement s)
        {
            ParentExpression = null;

            switch (s.StatementType)
            {
                case StatementType.VariableDeclaration:
                {
                    var d = s as VariableDeclaration;
                    for (var var = d.Variable; var != null; var = var.Next)
                    {
                        if (MustCopyType(var.ValueType))
                        {
                            if (var.OptionalValue == null)
                                var.OptionalValue = new Default(var.Source, var.ValueType);
                            else if (MustCopyValue(var.OptionalValue))
                            {
                                s = new StoreLocal(var.Source, var, var.OptionalValue);
                                var.OptionalValue = new Default(var.Source, var.ValueType);
                                var.SetName(Type.GetUniqueIdentifier(var.Name));
                            }
                        }                        
                    }
                    break;
                }
            }
        }

        void OnObject(ref Expression e)
        {
            if (e == null)
                return;

            switch (e.ExpressionType)
            {
                case ExpressionType.LoadField:
                    OnObject(ref (e as LoadField).Object);
                    return;
                case ExpressionType.AddressOf:
                    OnObject(ref (e as AddressOf).Operand);
                    return;
                case ExpressionType.LoadElement:
                case ExpressionType.LoadArgument:
                case ExpressionType.LoadLocal:
                case ExpressionType.This:
                    return;
            }

            if (MustCopyType(e.ReturnType) && MustCopyValue(e))
            {
                if (e.ExpressionType == ExpressionType.SequenceOp && (e as SequenceOp).Right.ExpressionType == ExpressionType.AddressOf)
                {
                    var s = e as SequenceOp;
                    s.Right = (s.Right as AddressOf).Operand;
                }

                var v = new Variable(e.Source, Function, Type.GetUniqueIdentifier("struct"),
                    e.ReturnType, VariableType.Default, new Default(e.Source, e.ReturnType));
                e = new SequenceOp(
                        new StoreLocal(e.Source, v, e),
                        new LoadLocal(e.Source, v));
            }
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            CurrentExpression = e;

            switch (e.ExpressionType)
            {
                case ExpressionType.LoadArgument:
                {
                    var s = e as LoadArgument;
                    if (ParameterVariables[s.Index] != null)
                        e = new LoadLocal(s.Source, ParameterVariables[s.Index]);
                    break;
                }
                case ExpressionType.StoreArgument:
                {
                    var s = e as StoreArgument;
                    if (MustCopyType(s.Parameter.Type) && MustCopyValue(s.Value))
                        e = Assign(new LoadArgument(s.Source, s.Function, s.Index), s.Value);
                    else if (ParameterVariables[s.Index] != null)
                        e = new StoreLocal(s.Source, ParameterVariables[s.Index], s.Value);
                    break;
                }
                case ExpressionType.StoreField:
                {
                    var s = e as StoreField;
                    OnObject(ref s.Object);

                    if (MustCopyType(s.Field.ReturnType) && MustCopyValue(s.Value))
                        e = Assign(new LoadField(s.Source, s.Object, s.Field), s.Value);
                    break;
                }
                case ExpressionType.StoreThis:
                {
                    var s = e as StoreThis;
                    e = Assign(new This(s.Source, s.ReturnType), s.Value);
                    break;
                }
                case ExpressionType.StoreLocal:
                {
                    var s = e as StoreLocal;
                    if (MustCopyType(s.Variable.ValueType) && MustCopyValue(s.Value))
                        e = Assign(new LoadLocal(s.Source, s.Variable), s.Value);
                    break;
                }
                case ExpressionType.StoreElement:
                {
                    var s = e as StoreElement;
                    if (MustCopyType(s.Array.ReturnType.ElementType) && MustCopyValue(s.Value))
                        e = Assign(new LoadElement(s.Source, s.Array, s.Index), s.Value);
                    break;
                }
                case ExpressionType.CallMethod:
                {
                    var s = e as CallMethod;
                    if (MustCopyBeforeCall(s.Method))
                        OnObject(ref s.Object);
                    break;
                }
                case ExpressionType.GetProperty:
                {
                    var s = e as GetProperty;
                    if (MustCopyBeforeCall(s.Property.GetMethod))
                        OnObject(ref s.Object);
                    break;
                }
                case ExpressionType.SetProperty:
                {
                    var s = e as SetProperty;
                    if (MustCopyBeforeCall(s.Property.SetMethod))
                        OnObject(ref s.Object);
                    break;
                }
                case ExpressionType.AddListener:
                {
                    var s = e as AddListener;
                    if (MustCopyBeforeCall(s.Event.AddMethod))
                        OnObject(ref s.Object);
                    break;
                }
                case ExpressionType.RemoveListener:
                {
                    var s = e as RemoveListener;
                    if (MustCopyBeforeCall(s.Event.RemoveMethod))
                        OnObject(ref s.Object);
                    break;
                }
            }

            ParentExpression = e;
        }
    }
}
