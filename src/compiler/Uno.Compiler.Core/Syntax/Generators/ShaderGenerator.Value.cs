using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.Syntax.Generators
{
    partial class ShaderGenerator
    {
        void AdjustInstanceStageRange(ref StageValue obj, DataType dt, bool isFieldOrPropertyAccess)
        {
            // Default volatile on static members and current class members
            if (obj.MaxStage >= MetaStage.Volatile &&
                (obj.Value == null || obj.Value.ActualValue is This || obj.Value.ActualValue is Base) &&
                (obj.Value != null || isFieldOrPropertyAccess))
                obj.MinStage = MetaStage.Volatile;

            if (isFieldOrPropertyAccess &&
                !Backend.ShaderBackend.IsIntrinsic(dt))
                obj.MaxStage = MetaStage.Volatile;

            var shaderStageObj = dt.TryGetAttribute(Essentials.RequireShaderStageAttribute);

            if (shaderStageObj != null)
                obj.MinStage = obj.MaxStage = (MetaStage)(int)shaderStageObj;
        }

        void AdjustFunctionStageRange(ref StageValue obj, Function func)
        {
            // TODO: Implement code analyzer to make sure func will work in shader or not
            if (obj.MaxStage >= MetaStage.Volatile &&
                !Backend.ShaderBackend.IsIntrinsic(func) &&
                func.Body == null)
                obj.MaxStage = MetaStage.Volatile;

            var shaderStageObj = func.TryGetAttribute(Essentials.RequireShaderStageAttribute);

            if (shaderStageObj != null)
                obj.MinStage = obj.MaxStage = (MetaStage)(int)shaderStageObj;
        }

        void ProcessValues(ref StageValue obj, out MetaStage minStage, out MetaStage maxStage, Expression inArg, out Expression outArg)
        {
            minStage = obj.MinStage;
            maxStage = obj.MaxStage;

            var stageArg = ProcessValue(inArg);

            if (stageArg.MinStage > minStage)
                minStage = stageArg.MinStage;

            if (stageArg.MaxStage < maxStage)
                maxStage = stageArg.MaxStage;

            if (maxStage < minStage)
                maxStage = minStage;

            outArg = ProcessStage(stageArg, minStage, maxStage).Value;
            obj = ProcessStage(obj, minStage, maxStage);
        }

        void ProcessValues(ref StageValue obj, out MetaStage minStage, out MetaStage maxStage, Expression inArg0, out Expression outArg0, Expression inArg1, out Expression outArg1)
        {
            minStage = obj.MinStage;
            maxStage = obj.MaxStage;

            var stageArg0 = ProcessValue(inArg0);
            var stageArg1 = ProcessValue(inArg1);

            if (stageArg0.MinStage > minStage)
                minStage = stageArg0.MinStage;

            if (stageArg1.MinStage > minStage)
                minStage = stageArg1.MinStage;

            if (stageArg0.MaxStage < maxStage)
                maxStage = stageArg0.MaxStage;

            if (stageArg1.MaxStage < maxStage)
                maxStage = stageArg1.MaxStage;

            if (maxStage < minStage)
                maxStage = minStage;

            outArg0 = ProcessStage(stageArg0, minStage, maxStage).Value;
            outArg1 = ProcessStage(stageArg1, minStage, maxStage).Value;
            obj = ProcessStage(obj, minStage, maxStage);
        }

        void ProcessValues(out MetaStage minStage, out MetaStage maxStage, Expression inArg0, out Expression outArg0, Expression inArg1, out Expression outArg1)
        {
            var stageArg0 = ProcessValue(inArg0);
            var stageArg1 = ProcessValue(inArg1);

            minStage = stageArg0.MinStage;
            maxStage = stageArg0.MaxStage;

            if (stageArg1.MinStage > minStage)
                minStage = stageArg1.MinStage;

            if (stageArg1.MaxStage < maxStage)
                maxStage = stageArg1.MaxStage;

            if (maxStage < minStage)
                maxStage = minStage;

            outArg0 = ProcessStage(stageArg0, minStage, maxStage).Value;
            outArg1 = ProcessStage(stageArg1, minStage, maxStage).Value;
        }

        Expression[] ProcessValues(ref StageValue obj, out MetaStage minStage, out MetaStage maxStage, params Expression[] args)
        {
            switch (args.Length)
            {
                case 0:
                    minStage = obj.MinStage;
                    maxStage = obj.MaxStage;
                    return new Expression[0];

                case 1:
                    {
                        Expression arg;
                        ProcessValues(ref obj, out minStage, out maxStage, args[0], out arg);
                        return new[] { arg };
                    }

                case 2:
                    {
                        Expression arg0, arg1;
                        ProcessValues(ref obj, out minStage, out maxStage, args[0], out arg0, args[1], out arg1);
                        return new[] { arg0, arg1 };
                    }

                default:
                    {
                        minStage = obj.MinStage;
                        maxStage = obj.MaxStage;

                        var stageArgs = new StageValue[args.Length];

                        for (int i = 0; i < args.Length; i++)
                        {
                            stageArgs[i] = ProcessValue(args[i]);

                            if (stageArgs[i].MinStage > minStage)
                                minStage = stageArgs[i].MinStage;

                            if (stageArgs[i].MaxStage < maxStage)
                                maxStage = stageArgs[i].MaxStage;
                        }

                        if (maxStage < minStage)
                            maxStage = minStage;

                        var result = new Expression[args.Length];

                        for (int i = 0; i < args.Length; i++)
                            result[i] = ProcessStage(stageArgs[i], minStage, maxStage).Value;

                        obj = ProcessStage(obj, minStage, maxStage);

                        return result;
                    }
            }
        }

        Expression ProcessValuesToStage(MetaStage stage, Expression e)
        {
            return ProcessStage(ProcessValue(e), stage, stage).Value;
        }

        internal StageValue ProcessValue(Expression e)
        {
            if (e == null)
                return new StageValue(null, MetaStage.Const);

            switch (e.ExpressionType)
            {
                case ExpressionType.StageOp:
                    {
                        var s = e as StageOp;
                        var op = ProcessValue(s.Operand);

                        if (s.Stage > MetaStage.Volatile)
                        {
                            op = ProcessStage(op, s.Stage, s.Stage);
                        }
                        else if (op.MinStage > MetaStage.Volatile)
                        {
                            Log.Error(s.Operand.Source, ErrorCode.E0000, "Invalid " + s.Stage.ToLiteral() + " expression in " + Path.Quote());
                            return new StageValue(Expression.Invalid, s.Stage, s.Stage);
                        }

                        op.MinStage = s.Stage;
                        return op;
                    }

                case ExpressionType.NewVertexAttrib:
                    return ProcessVertexAttrib(e as NewVertexAttrib);

                case ExpressionType.NewPixelSampler:
                    return ProcessPixelSampler(e as NewPixelSampler);

                case ExpressionType.GetMetaProperty:
                    {
                        var s = e as GetMetaProperty;
                        var loc = TryGetLocation(LocationStack.Last(), s.Name, s.Offset);

                        if (loc == null)
                        {
                            if (s.Offset == 0)
                                Log.Error(s.Source, ErrorCode.E5019, s.Name.Quote() + " was not found in " + Path.Quote());
                            else
                                Log.Error(s.Source, ErrorCode.E5020, "Previous declaration of " + s.Name.Quote() + " was not found in " + Path.Quote());

                            return new StageValue(Expression.Invalid, MetaStage.Const);
                        }

                        return ProcessMetaProperty(loc.Value);
                    }

                case ExpressionType.GetMetaObject:
                    {
                        var s = e as GetMetaObject;
                        var obj = TryGetObject(LocationStack.Last());

                        if (obj == null || !obj.ReturnType.IsSubclassOfOrEqual(s.ReturnType))
                            Log.Error(s.Source, ErrorCode.E0000, "Unable to resolve instance of " + s.ReturnType.Quote() + " in " + Path.Quote());

                        var objStage = ProcessValue(obj);
                        return new StageValue(objStage.Value, objStage.MinStage, objStage.MaxStage);
                    }

                case ExpressionType.Invalid:
                case ExpressionType.Constant:
                case ExpressionType.Default:
                case ExpressionType.ExternOp:
                case ExpressionType.ExternString:
                    return new StageValue(e, MetaStage.Const);

                case ExpressionType.This:
                case ExpressionType.Base:
                case ExpressionType.TypeOf:
                    return new StageValue(e, MetaStage.ReadOnly, MetaStage.Volatile);

                case ExpressionType.CapturedArgument:
                case ExpressionType.CapturedLocal:
                case ExpressionType.LoadArgument:
                    return new StageValue(e, MetaStage.Volatile, MetaStage.Volatile);

                case ExpressionType.RuntimeConst:
                    return new StageValue(e, MetaStage.Vertex, MetaStage.Pixel);

                case ExpressionType.LoadUniform:
                    return new StageValue(e, MetaStage.Vertex, MetaStage.Pixel);

                case ExpressionType.LoadVarying:
                    return new StageValue(e, MetaStage.Pixel, MetaStage.Pixel);

                case ExpressionType.LoadVertexAttrib:
                    return new StageValue(e, MetaStage.Vertex, MetaStage.Vertex);

                case ExpressionType.FixOp:
                    {
                        var s = e as FixOp;
                        var ps = ProcessValue(s.Operand);
                        return new StageValue(new FixOp(s.Source, s.Operator, ps.Value), ps.MinStage, ps.MaxStage);
                    }

                case ExpressionType.AddressOf:
                    {
                        var s = e as AddressOf;
                        var ps = ProcessValue(s.Operand);
                        return new StageValue(new AddressOf(ps.Value, s.AddressType), ps.MinStage, ps.MaxStage);
                    }

                case ExpressionType.BranchOp:
                    {
                        var s = e as BranchOp;

                        Expression left, right;
                        MetaStage minStage, maxStage;
                        ProcessValues(out minStage, out maxStage, s.Left, out left, s.Right, out right);

                        return new StageValue(new BranchOp(s.Source, s.ReturnType, s.BranchType, left, right), minStage, maxStage);
                    }

                case ExpressionType.ReferenceOp:
                    {
                        var s = e as ReferenceOp;

                        Expression left, right;
                        MetaStage minStage, maxStage;
                        ProcessValues(out minStage, out maxStage, s.Left, out left, s.Right, out right);

                        return new StageValue(new ReferenceOp(s.Source, s.ReturnType, s.EqualityType, left, right), minStage, maxStage);
                    }

                case ExpressionType.SequenceOp:
                    {
                        var s = e as SequenceOp;

                        Expression left, right;
                        MetaStage minStage, maxStage;
                        ProcessValues(out minStage, out maxStage, s.Left, out left, s.Right, out right);

                        return new StageValue(new SequenceOp(left, right), minStage, maxStage);
                    }

                case ExpressionType.ConditionalOp:
                    {
                        var s = e as ConditionalOp;
                        var ps = ProcessValue(s.Condition);

                        Expression a, b;
                        MetaStage minStage, maxStage;
                        ProcessValues(ref ps, out minStage, out maxStage, s.True, out a, s.False, out b);

                        return new StageValue(new ConditionalOp(s.Source, ps.Value, a, b), minStage, maxStage);
                    }

                case ExpressionType.NullOp:
                    {
                        var s = e as NullOp;
                        var ps = StageValue.Default;

                        if (ps.MaxStage > MetaStage.Volatile)
                            ps.MaxStage = MetaStage.Volatile;

                        Expression left, right;
                        MetaStage minStage, maxStage;
                        ProcessValues(ref ps, out minStage, out maxStage, s.Left, out left, s.Right, out right);

                        return new StageValue(new NullOp(s.Source, left, right), minStage, maxStage);
                    }

                case ExpressionType.IsOp:
                    {
                        var s = e as IsOp;
                        var ps = ProcessValue(s.Operand);

                        if (ps.MaxStage > MetaStage.Volatile)
                            ps.MaxStage = MetaStage.Volatile;

                        return new StageValue(new IsOp(s.Source, ps.Value, s.TestType, s.ReturnType), ps.MinStage, ps.MaxStage);
                    }

                case ExpressionType.AsOp:
                    {
                        var s = e as AsOp;
                        var ps = ProcessValue(s.Operand);

                        if (ps.MaxStage > MetaStage.Volatile)
                            ps.MaxStage = MetaStage.Volatile;

                        return new StageValue(new AsOp(s.Source, ps.Value, s.ReturnType), ps.MinStage, ps.MaxStage);
                    }

                case ExpressionType.LoadField:
                    {
                        var s = e as LoadField;
                        var ps = ProcessValue(s.Object);
                        AdjustInstanceStageRange(ref ps, s.Field.DeclaringType, true);

                        return new StageValue(new LoadField(s.Source, ps.Value, s.Field), ps.MinStage, ps.MaxStage);
                    }

                case ExpressionType.LoadElement:
                    {
                        var s = e as LoadElement;
                        var ps = ProcessValue(s.Array);

                        Expression index;
                        MetaStage minStage, maxStage;
                        ProcessValues(ref ps, out minStage, out maxStage, s.Index, out index);

                        return new StageValue(new LoadElement(s.Source, ps.Value, index), minStage, maxStage);
                    }

                case ExpressionType.Swizzle:
                    {
                        var s = e as Swizzle;
                        var ps = ProcessValue(s.Object);
                        AdjustInstanceStageRange(ref ps, s.Object.ReturnType, true);

                        return new StageValue(new Swizzle(s.Source, s.Constructor, ps.Value, s.Fields), ps.MinStage, ps.MaxStage);
                    }

                case ExpressionType.GetProperty:
                    {
                        var s = e as GetProperty;
                        var ps = ProcessValue(s.Object);
                        AdjustInstanceStageRange(ref ps, s.Property.DeclaringType, true);
                        AdjustFunctionStageRange(ref ps, s.Property.GetMethod);

                        MetaStage minStage, maxStage;
                        var args = ProcessValues(ref ps, out minStage, out maxStage, s.Arguments);

                        return new StageValue(new GetProperty(s.Source, ps.Value, s.Property, args), minStage, maxStage);
                    }

                case ExpressionType.NewObject:
                    {
                        var s = e as NewObject;

                        var ps = new StageValue(null, MetaStage.ReadOnly,
                            s.ReturnType.IsReferenceType ? MetaStage.Volatile : MetaStage.Max);

                        AdjustFunctionStageRange(ref ps, s.Constructor);

                        MetaStage minStage, maxStage;
                        var args = ProcessValues(ref ps, out minStage, out maxStage, s.Arguments);

                        return new StageValue(new NewObject(s.Source, s.Constructor, args), minStage, maxStage);
                    }

                case ExpressionType.NewArray:
                    {
                        var s = e as NewArray;

                        var ps = new StageValue(null, MetaStage.ReadOnly,
                            s.ArrayType.ElementType.IsReferenceType ? MetaStage.Volatile : MetaStage.Max);

                        if (s.Size == null)
                        {
                            MetaStage minStage, maxStage;
                            var initializers = ProcessValues(ref ps, out minStage, out maxStage, s.Initializers);
                            return new StageValue(new NewArray(s.Source, (RefArrayType)s.ReturnType, initializers), minStage, maxStage);
                        }
                        else
                        {
                            Expression size;
                            MetaStage minStage, maxStage;
                            ProcessValues(ref ps, out minStage, out maxStage, s.Size, out size);
                            return new StageValue(new NewArray(s.Source, (RefArrayType)s.ReturnType, size), minStage, maxStage);
                        }
                    }

                case ExpressionType.CallMethod:
                    {
                        var s = e as CallMethod;
                        var ps = ProcessValue(s.Object);
                        AdjustInstanceStageRange(ref ps, s.Method.DeclaringType, false);
                        AdjustFunctionStageRange(ref ps, s.Method);

                        MetaStage minStage, maxStage;
                        var args = ProcessValues(ref ps, out minStage, out maxStage, s.Arguments);

                        return new StageValue(new CallMethod(s.Source, ps.Value, s.Method, args), minStage, maxStage);
                    }

                case ExpressionType.CallCast:
                    {
                        var s = e as CallCast;
                        var ps = ProcessValue(s.Operand);
                        AdjustFunctionStageRange(ref ps, s.Cast);

                        return new StageValue(new CallCast(s.Source, s.Cast, ps.Value), ps.MinStage, ps.MaxStage);
                    }

                case ExpressionType.CallBinOp:
                    {
                        var s = e as CallBinOp;
                        var ps = StageValue.Default;
                        AdjustFunctionStageRange(ref ps, s.Operator);

                        Expression left, right;
                        MetaStage minStage, maxStage;
                        ProcessValues(ref ps, out minStage, out maxStage, s.Left, out left, s.Right, out right);

                        return new StageValue(new CallBinOp(s.Source, s.Operator, left, right), minStage, maxStage);
                    }

                case ExpressionType.CallUnOp:
                    {
                        var s = e as CallUnOp;
                        var ps = ProcessValue(s.Operand);
                        AdjustFunctionStageRange(ref ps, s.Operator);

                        return new StageValue(new CallUnOp(s.Source, s.Operator, ps.Value), ps.MinStage, ps.MaxStage);
                    }

                case ExpressionType.CastOp:
                    {
                        var s = e as CastOp;
                        var ps = ProcessValue(s.Operand);

                        if (ps.MaxStage > MetaStage.Volatile)
                            ps.MaxStage = MetaStage.Volatile;

                        return new StageValue(new CastOp(s.Source, s.ReturnType, ps.Value), ps.MinStage, ps.MaxStage);
                    }

                case ExpressionType.StoreElement:
                case ExpressionType.StoreField:
                case ExpressionType.StoreArgument:
                case ExpressionType.StoreLocal:
                case ExpressionType.SetProperty:
                    Log.Error(e.Source, ErrorCode.E5021, "Not allowed to use assign operators inside meta property definitions");
                    return new StageValue(Expression.Invalid, MetaStage.Const);
            }

            Log.Error(e.Source, ErrorCode.I5022, "<" + e.ExpressionType + "> is not supported by ShaderGenerator");
            return new StageValue(Expression.Invalid, MetaStage.Const);
        }
    }
}
