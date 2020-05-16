using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.IL;
using Uno.Compiler.Core.IL.Utilities;
using Uno.Compiler.Core.Syntax.Compilers;

namespace Uno.Compiler.Core.Syntax.Generators.Passes
{
    class ShaderProcessor : CompilerPass
    {
        readonly Dictionary<Function, ShaderFunction> ProcessedFunctions = new Dictionary<Function, ShaderFunction>();
        readonly ShaderGenerator Generator;

        ShaderProcessor(ShaderGenerator generator, Shader shader)
            : base(generator.Compiler.Pass)
        {
            Generator = generator;
            Shader = shader;
        }

        class ThisSwapper : Pass
        {
            readonly Expression _replace;

            public ThisSwapper(Pass parent, Expression replace)
                : base(parent)
            {
                _replace = replace;
            }

            public override void Begin(ref Expression e, ExpressionUsage u)
            {
                if (e is This) e = _replace;
            }
        }

        Expression[] ProcessArguments(Expression[] args, Expression optionalValueOrObject = null, Expression optionalObject = null)
        {
            var resultIndex = 0;
            var resultLength = args.Length;

            if (optionalValueOrObject != null)
                resultLength++;
            if (optionalObject != null)
                resultLength++;

            var result = new Expression[resultLength];

            while (resultIndex < args.Length)
            {
                result[resultIndex] = args[resultIndex];
                resultIndex++;
            }

            if (optionalValueOrObject != null)
                result[resultIndex++] = optionalValueOrObject;
            if (optionalObject != null)
                result[resultIndex++] = optionalObject;

            return result;
        }

        Parameter[] TryProcessParameterList(Function f, Expression[] args, CopyState state)
        {
            var copy = f.Parameters.Copy(state);

            for (int i = 0; i < copy.Length; i++)
            {
                if (copy[i].Type.IsRefArray)
                {
                    if (copy[i].IsReference)
                    {
                        Log.Error(copy[i].Source, ErrorCode.E0000, "Reference to array parameter is not supported by current shader backend (in " + Generator.Path.Quote() + ")");
                        return null;
                    }

                    var rat = copy[i].Type as RefArrayType;
                    var fat = args[i].ReturnType as FixedArrayType;

                    if (fat == null || rat.ElementType != fat.ElementType)
                    {
                        Log.Error(copy[i].Source, ErrorCode.E0000, copy[i].Type.Quote() + " parameter is not supported by current shader backend (in " + Generator.Path.Quote() + ")");
                        return null;
                    }

                    copy[i].Type = fat;

                    var address = args[i] as AddressOf;

                    if (address != null)
                    {
                        switch (address.AddressType)
                        {
                            case 0:
                            case AddressType.Ref:
                                copy[i].Modifier = ParameterModifier.Ref;
                                address.AddressType = AddressType.Ref;
                                break;

                            case AddressType.Out:
                                copy[i].Modifier = ParameterModifier.Out;
                                break;
                            case AddressType.Const:
                                copy[i].Modifier = ParameterModifier.Const;
                                break;
                        }
                    }
                }
            }

            if (f.IsStatic || f.IsConstructor)
                return copy;

            var result = new Parameter[copy.Length + 1];
            for (int i = 0; i < copy.Length; i++)
                result[i] = copy[i];

            result[copy.Length] = new Parameter(f.Source, AttributeList.Empty, ParameterModifier.Ref, f.DeclaringType, "__this", null);

            return result;
        }

        bool TryProcessFunctionCall(ref Expression e, Function f, Expression[] args, Expression optionalValueOrObject = null, Expression optionalObject = null)
        {
            // Special case for T[].get_Length()
            if (f.DeclaringType.IsRefArray &&
                optionalValueOrObject != null && optionalValueOrObject.ReturnType.IsFixedArray &&
                f.Name == "get_Length" && f.Parameters.Length == 0)
            {
                var fat = (FixedArrayType)optionalValueOrObject.ReturnType;
                e = fat.OptionalSize ?? Expression.Invalid;
                return true;
            }

            if (Backend.ShaderBackend.IsIntrinsic(f))
                return false;

            var processedArgs = ProcessArguments(args, optionalValueOrObject, optionalObject);

            ShaderFunction result;
            if (!ProcessedFunctions.TryGetValue(f, out result))
            {
                // TODO: This code doesn't handle:
                // - Recursion
                // - Multiple uses of function with array in parameterlist

                if (!f.HasBody)
                {
                    // Attempt lazy compile.
                    var fc = f.MasterDefinition.Tag as FunctionCompiler;
                    fc?.Compile(true);

                    if (!f.HasBody)
                    {
                        Log.Error(e.Source, ErrorCode.E5010, f.Quote() + " is pure intrinsic and is not supported by current shader backend (in " + Generator.Path.Quote() + ")");
                        return false;
                    }
                }

                result = new ShaderFunction(f.Source, Shader, f.UnoName.ToIdentifier() + "_" + Generator.ShaderGlobalCounter++, f.ReturnType, null);

                var state = new CopyState(result);
                var parameterList = TryProcessParameterList(f, processedArgs, state);

                if (parameterList == null)
                    return false;

                result.SetParameters(parameterList);
                result.SetBody(f.Body.CopyNullable(state));

                if (!f.IsStatic)
                {
                    if (f.IsConstructor)
                    {
                        result.ReturnType = f.DeclaringType;
                        var var = new Variable(f.Source, f, "__this", f.DeclaringType);
                        result.Body.Statements.Insert(0, new VariableDeclaration(var));
                        result.Body.Statements.Add(new Return(f.Source, new This(f.Source, f.DeclaringType)));
                        result.Body.Visit(new ThisSwapper(this, new LoadLocal(f.Source, var)));
                    }
                    else
                        result.Body.Visit(new ThisSwapper(this, new LoadArgument(f.Source, result, f.Parameters.Length)));
                }

                ProcessedFunctions.Add(f, result);
                result.Visit(this);
                Shader.Functions.Add(result); // add after process
            }

            e = new CallShader(e.Source, result, processedArgs);
            return true;
        }

        void ProcessType(Source src, DataType dt)
        {
            if (Backend.ShaderBackend.IsIntrinsic(dt))
                return;

            switch (dt.TypeType)
            {
                case TypeType.Void:
                case TypeType.Invalid:
                    return;

                case TypeType.FixedArray:
                    ProcessType(src, (dt as FixedArrayType).ElementType);
                    return;

                case TypeType.Struct:
                    {
                        var st = dt as StructType;

                        int index;
                        if (!Generator.ShaderStructMap.TryGetValue(st, out index))
                        {
                            index = Generator.ShaderStructQueue.Count;
                            Generator.ShaderStructMap.Add(st, index);

                            foreach (var f in st.Fields)
                                ProcessType(f.Source, f.ReturnType);

                            Generator.ShaderStructQueue.Add(st);
                        }

                        Shader.ReferencedStructs.Add(index);
                        return;
                    }
            }

            Log.Error(src, ErrorCode.E5011, dt.Quote() + " is not supported by current shader backend (in " + Generator.Path.Quote() + ")");
        }

        Statement Parent;

        public override void Begin(ref Statement s)
        {
            switch (s.StatementType)
            {
                case StatementType.VariableDeclaration:
                    {
                        var e = s as VariableDeclaration;
                        if (e.Variable.IsConstant)
                            s = new NoOp(e.Source);
                    }
                    break;

                case StatementType.TryCatchFinally:
                case StatementType.Throw:
                case StatementType.Switch:
                case StatementType.Draw:
                case StatementType.DrawDispose:
                case StatementType.ExternScope:
                    Log.Error(s.Source, ErrorCode.E5014, "<" + s.StatementType + "> is not supported in shader (in " + Generator.Path.Quote() + ")");
                    break;
            }

            Parent = s is Expression ? null : s;
        }

        public override void Next(Statement s)
        {
            Parent = s is Expression ? null : s;
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            // TODO: Could need more testing of this code
            // TODO: Could need more verification here

            ProcessType(e.Source, e.ReturnType);

            switch (e.ExpressionType)
            {
                case ExpressionType.Invalid:
                case ExpressionType.Constant:
                case ExpressionType.LoadLocal:
                case ExpressionType.LoadElement:
                case ExpressionType.LoadVarying:
                case ExpressionType.LoadVertexAttrib:
                case ExpressionType.LoadPixelSampler:
                case ExpressionType.StoreLocal:
                case ExpressionType.StoreElement:
                case ExpressionType.ExternOp:
                case ExpressionType.ConditionalOp:
                case ExpressionType.BranchOp:
                case ExpressionType.ReferenceOp:
                case ExpressionType.SequenceOp:
                case ExpressionType.FixOp:
                case ExpressionType.NoOp:
                    break;

                case ExpressionType.CallShader:
                    {
                        var s = e as CallShader;

                        for (int i = 0; i < s.Arguments.Length; i++)
                        {
                            var pfat = s.Function.Parameters[i].Type as FixedArrayType;

                            // Workaround to avoid fixed array parameters without length which is illegal in GLSL (FIXME: This may not be 100% robust)
                            if (pfat != null && pfat.OptionalSize == null)
                            {
                                var afat = s.Arguments[i].ReturnType as FixedArrayType;

                                if (afat != null)
                                    s.Function.Parameters[i].Type = afat;
                            }
                        }
                    }
                    break;

                case ExpressionType.AddressOf:
                    {
                        var a = e as AddressOf;
                        var b = a.Operand as AddressOf;

                        // This happens when a fixed array is bound to uniform (FIXME: Could be cleaned up for consisency)
                        if (b != null)
                        {
                            e = b;
                            Begin(ref e, u);
                        }
                    }
                    break;

                case ExpressionType.LoadUniform:
                    Shader.ReferencedUniforms.Add((e as LoadUniform).Index);
                    break;

                case ExpressionType.RuntimeConst:
                    Shader.ReferencedConstants.Add((e as RuntimeConst).Index);
                    break;

                case ExpressionType.NullOp:
                    e = (e as NullOp).TransformNullOpToConditionalOp(Essentials, Generator.Path.DrawBlock.Method.DeclaringType);
                    Begin(ref e, u);
                    break;

                case ExpressionType.Swizzle:
                    if (Backend.ShaderBackend.IsIntrinsic((e as Swizzle).Fields[0].DeclaringType))
                        break;

                    e = (e as Swizzle).TransformSwizzleToNewObject(Generator.Path.DrawBlock.Method.DeclaringType);
                    Begin(ref e, u);
                    break;

                case ExpressionType.Default:
                    // TODO: Remove default init from shader code
                    break;

                case ExpressionType.LoadArgument:
                    {
                        var s = e as LoadArgument;

                        if (s.ReturnType.IsFixedArray && !(Parent is AddressOf))
                        {
                            if (!u.IsObject())
                                Log.Error(e.Source, ErrorCode.E0000, "Fixed array argument " + Function.Parameters[s.Index].Quote() + " cannot be used as value in shader (in " + Generator.Path.Quote() + ")");

                            switch (Function.Parameters[s.Index].Modifier)
                            {
                                case ParameterModifier.Const:
                                    if (Parent is StoreElement)
                                        Log.Error(e.Source, ErrorCode.E0000, "Cannot store to fixed array argument " + Function.Parameters[s.Index].Quote() + " in shader (in " + Generator.Path.Quote() + ")");

                                    e = new AddressOf(s, AddressType.Const);
                                    Begin(ref e, u);
                                    break;

                                default:
                                    e = new AddressOf(s);
                                    Begin(ref e, u);
                                    break;
                            }
                        }
                    }
                    break;

                case ExpressionType.StoreArgument:
                    {
                        var s = e as StoreArgument;

                        if (s.ReturnType.IsFixedArray)
                            Log.Error(e.Source, ErrorCode.E0000, "Cannot store to fixed array argument " + Function.Parameters[s.Index].Quote() + " in shader (in " + Generator.Path.Quote() + ")");
                    }
                    break;

                case ExpressionType.LoadField:
                    {
                        var s = e as LoadField;

                        if (s.Object == null)
                            Log.Error(e.Source, ErrorCode.E5012, "Cannot read from static field " + s.Field.Quote() + " in shader (in " + Generator.Path.Quote() + ")");

                        ProcessType(e.Source, (e as LoadField).Field.DeclaringType);
                    }
                    break;

                case ExpressionType.StoreField:
                    {
                        var s = e as StoreField;

                        if (s.Object == null)
                            Log.Error(e.Source, ErrorCode.E5013, "Cannot write to static field " + s.Field.Quote() + " in shader (in " + Generator.Path.Quote() + ")");

                        ProcessType(e.Source, (e as StoreField).Field.DeclaringType);
                    }
                    break;

                case ExpressionType.CallBinOp:
                    {
                        var s = e as CallBinOp;

                        if (s.TryTransformEnumBinOpToIntBinOp(Log, ref e) ||
                            TryProcessFunctionCall(ref e, s.Operator, new[] { s.Left, s.Right }))
                            Begin(ref e, u);
                    }
                    break;

                case ExpressionType.CallUnOp:
                    {
                        var s = e as CallUnOp;

                        if (s.TryTransformEnumUnOpToIntUnOp(Log, ref e) ||
                            TryProcessFunctionCall(ref e, s.Operator, new[] { s.Operand }))
                            Begin(ref e, u);
                    }
                    break;

                case ExpressionType.CallCast:
                    {
                        var s = e as CallCast;

                        if (TryProcessFunctionCall(ref e, s.Cast, new[] { s.Operand }))
                            Begin(ref e, u);
                    }
                    break;

                case ExpressionType.NewObject:
                    {
                        var s = e as NewObject;

                        if (TryProcessFunctionCall(ref e, s.Constructor, s.Arguments))
                            Begin(ref e, u);
                    }
                    break;

                case ExpressionType.GetProperty:
                    {
                        var s = e as GetProperty;

                        if (s.TryTransformGetFixedArrayLength(Log, ref e) ||
                            TryProcessFunctionCall(ref e, s.Property.GetMethod, s.Arguments, s.Object))
                            Begin(ref e, u);
                    }
                    break;

                case ExpressionType.SetProperty:
                    {
                        var s = e as SetProperty;

                        if (s.TryTransformSetPropertyChainToSequence(Generator.Path.DrawBlock.Method.DeclaringType, Parent, ref e) ||
                            TryProcessFunctionCall(ref e, s.Property.SetMethod, s.Arguments, s.Value, s.Object))
                            Begin(ref e, u);
                    }
                    break;

                case ExpressionType.CallMethod:
                    {
                        var s = e as CallMethod;

                        if (s.TryTransformEnumHasFlagToIntOps(Log, ref e) ||
                            TryProcessFunctionCall(ref e, s.Method, s.Arguments, s.Object))
                            Begin(ref e, u);
                    }
                    break;

                default:
                    Log.Error(e.Source, ErrorCode.E5014, "<" + e.ExpressionType + "> is not supported in shader (in " + Generator.Path.Quote() + ")");
                    break;
            }

            Parent = e;
        }

        static void VerifyShaderFunction(ShaderGenerator g, ShaderFunction f)
        {
            for (int i = 0; i < f.Parameters.Length; i++)
            {
                var fat = f.Parameters[i].Type as FixedArrayType;
                if (fat != null && fat.OptionalSize == null)
                    g.Log.Error(f.Parameters[i].Source, ErrorCode.E0000, "Unable to detect length of 'fixed' array (in " + g.Path.Quote() + ")");
            }
        }

        public static void ProcessShader(ShaderGenerator g, Shader shader)
        {
            shader.Entrypoint.Visit(new ShaderProcessor(g, shader));

            foreach (var i in shader.ReferencedUniforms)
            {
                var u = shader.State.Uniforms[i];
                var fat = u.Type as FixedArrayType;
                var shaderConst = fat?.OptionalSize as RuntimeConst;

                if (shaderConst != null)
                    shader.ReferencedConstants.Add(shaderConst.Index);
            }

            foreach (var f in shader.Functions)
                VerifyShaderFunction(g, f);

            VerifyShaderFunction(g, shader.Entrypoint);
        }
    }
}
