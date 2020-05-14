using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.IL.Validation
{
    public partial class ILVerifier
    {
        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            _parent = _current;
            _current = e;

            switch (e.ReturnType.TypeType)
            {
                case TypeType.Void:
                case TypeType.Enum:
                case TypeType.GenericParameter:
                case TypeType.RefArray:
                    break;

                case TypeType.FixedArray:
                    switch (e.ExpressionType)
                    {
                        case ExpressionType.AddressOf:
                        case ExpressionType.GetMetaProperty:
                            break;

                        default:
                            if (!(_parent is AddressOf))
                                Log.Error(e.Source, ErrorCode.I0000, "'fixed' arrays can only be loaded by address");
                            break;
                    }

                    break;

                case TypeType.Class:
                case TypeType.Struct:
                case TypeType.Delegate:
                case TypeType.Interface:
                    if (Environment.IsGeneratingCode)
                        break;
                    if (e.ReturnType.IsGenericDefinition || e.ReturnType.IsFlattenedDefinition)
                        Log.Error(e.Source, ErrorCode.I4124, "Invalid expression returning unparameterized type " + e.ReturnType.Quote());
                    break;

                default:
                    Log.Error(e.Source, ErrorCode.I4125, "Invalid expression returning " + e.ReturnType.Quote());
                    break;
            }

            switch (e.ExpressionType)
            {
                case ExpressionType.Constant:
                {
                    var s = e as Constant;
                    if (s.Value == null)
                        break;

                    var dt = s.ReturnType;
                    if (dt.IsEnum)
                        dt = dt.Base;

                    switch (dt.BuiltinType)
                    {
                        case BuiltinType.Bool:
                            if (s.Value is bool) return;
                            break;
                        case BuiltinType.Byte:
                            if (s.Value is byte) return;
                            break;
                        case BuiltinType.Char:
                            if (s.Value is char) return;
                            break;
                        case BuiltinType.Double:
                            if (s.Value is double) return;
                            break;
                        case BuiltinType.Int:
                            if (s.Value is int) return;
                            break;
                        case BuiltinType.Float:
                            if (s.Value is float) return;
                            break;
                        case BuiltinType.Long:
                            if (s.Value is long) return;
                            break;
                        case BuiltinType.SByte:
                            if (s.Value is sbyte) return;
                            break;
                        case BuiltinType.Short:
                            if (s.Value is short) return;
                            break;
                        case BuiltinType.String:
                            if (s.Value is string) return;
                            break;
                        case BuiltinType.UInt:
                            if (s.Value is uint) return;
                            break;
                        case BuiltinType.ULong:
                            if (s.Value is ulong) return;
                            break;
                        case BuiltinType.UShort:
                            if (s.Value is ushort) return;
                            break;
                    }

                    Log.Error(s.Source, ErrorCode.I4113, "Constant was not of expected type " + dt.Quote() + ", but .NET:" + s.Value.GetType().Quote());
                    break;
                }
                case ExpressionType.StoreLocal:
                {
                    var s = e as StoreLocal;

                    if (s.Variable.IsConstant)
                        Log.Error(s.Source, ErrorCode.E0000, "Cannot write to constant " + s.Variable.Name.Quote());
                    else if (s.Variable.IsIterator)
                        Log.Error(s.Source, ErrorCode.E0000, "Cannot write to foreach iterator variable " + s.Variable.Name.Quote());
                    else if (s.Variable.ValueType is FixedArrayType)
                        Log.Error(s.Source, ErrorCode.E0000, "Cannot write to 'fixed' array variable " + s.Variable.Name.Quote());
                    break;
                }
                case ExpressionType.LoadField:
                {
                    var s = e as LoadField;
                    VerifyMemberAccess(s.Source, s.Object, s.Field);

                    break;
                }
                case ExpressionType.StoreField:
                {
                    var s = e as StoreField;
                    VerifyWrite(s.Source, s.Field);
                    VerifyLValue(s.Source, s.Object);
                    VerifyMemberAccess(s.Source, s.Object, s.Field);

                    if (s.Field.ReturnType is FixedArrayType)
                        Log.Error(s.Source, ErrorCode.E0000, "Cannot write to 'fixed' array field " + s.Field.Quote());
                    break;
                }
                case ExpressionType.StoreElement:
                {
                    var s = e as StoreElement;
                    VerifyLValue(s.Source, s.Array);

                    var a = s.Array as AddressOf;

                    if (a != null && a.AddressType == AddressType.Const)
                        Log.Error(s.Source, ErrorCode.E0000, "Cannot write to array because reference is 'const'");
                    break;
                }
                case ExpressionType.StoreThis:
                {
                    var s = e as StoreThis;

                    if (s.ReturnType.TypeType != TypeType.Struct)
                        Log.Error(s.Source, ErrorCode.E0000, "Invalid write to 'this' because current type is not a struct");
                    break;
                }
                case ExpressionType.CallConstructor:
                {
                    var s = e as CallConstructor;
                    VerifyMemberAccess(s.Source, s.Constructor);
                    VerifyArguments(s.Source, s.Constructor, s.Arguments);

                    // Verify that base constructor is callable from current context
                    if (Function.MemberType != MemberType.Constructor || Function.IsStatic)
                        Log.Error(s.Source, ErrorCode.E4026, "Base contructor can only be called from instance constructor");
                    else if (s.Constructor.DeclaringType.MasterDefinition != Type.MasterDefinition &&
                        s.Constructor.DeclaringType != Type.Base)
                        Log.Error(s.Source, ErrorCode.I0000, s.Constructor.Quote() + " cannot be called by " + Function.Quote());
                    else if (Type.IsValueType && s.Constructor.DeclaringType.IsReferenceType)
                        Log.Error(s.Source, ErrorCode.I0000, s.Constructor.Quote() + " cannot be called by " + Function.Quote() + " because " + Type.Quote() + " is a struct");
                    else if (s.Constructor.MasterDefinition == Function.MasterDefinition)
                        Log.Error(s.Source, ErrorCode.E0000, s.Constructor.Quote() + " cannot call itself (circular reference)");

                    // Verify that 'this' or 'base' is not used as an argument
                    var p = new ThisVerifier(this);

                    for (int i = 0; i < s.Arguments.Length; i++)
                    {
                        p.Begin(ref s.Arguments[i], ExpressionUsage.Argument);
                        s.Arguments[i].Visit(p);
                        p.End(ref s.Arguments[i]);
                    }

                    break;
                }
                case ExpressionType.CallMethod:
                {
                    var s = e as CallMethod;
                    VerifyMemberAccess(s.Source, s.Object, s.Method);
                    VerifyArguments(s.Source, s.Method, s.Arguments);
                    break;
                }
                case ExpressionType.CallBinOp:
                {
                    var s = e as CallBinOp;
                    VerifyMemberAccess(s.Source, s.Operator);
                    VerifyArguments(s.Source, s.Operator, s.Left, s.Right);
                    break;
                }
                case ExpressionType.CallUnOp:
                {
                    var s = e as CallUnOp;
                    VerifyMemberAccess(s.Source, s.Operator);
                    VerifyArguments(s.Source, s.Operator, s.Operand);
                    break;
                }
                case ExpressionType.CallCast:
                {
                    var s = e as CallCast;
                    VerifyMemberAccess(s.Source, s.Cast);
                    VerifyArguments(s.Source, s.Cast, s.Operand);
                    break;
                }
                case ExpressionType.SetProperty:
                {
                    var s = e as SetProperty;

                    if (s.Property.SetMethod == null)
                        Log.Error(s.Source, ErrorCode.I0000, s.Property.Quote() + " does not define a set accessor");
                    else
                    {
                        VerifyMemberAccess(s.Source, s.Object, s.Property.SetMethod);
                        VerifyArguments(s.Source, s.Property.SetMethod, s.Arguments.Concat(s.Value));
                    }
                    break;
                }
                case ExpressionType.GetProperty:
                {
                    var s = e as GetProperty;

                    if (s.Property.GetMethod == null)
                        Log.Error(s.Source, ErrorCode.I0000, s.Property.Quote() + " does not define a get accessor");
                    else
                    {
                        VerifyMemberAccess(s.Source, s.Object, s.Property.GetMethod);
                        VerifyArguments(s.Source, s.Property.GetMethod, s.Arguments);
                    }
                    break;
                }
                case ExpressionType.AddListener:
                {
                    var s = e as AddListener;

                    if (s.Event.AddMethod == null)
                        Log.Error(s.Source, ErrorCode.I0000, s.Event.Quote() + " does not define a set accessor");
                    else
                    {
                        VerifyMemberAccess(s.Source, s.Object, s.Event.AddMethod);
                        VerifyArguments(s.Source, s.Event.AddMethod, s.Listener);
                    }
                    break;
                }
                case ExpressionType.RemoveListener:
                {
                    var s = e as RemoveListener;

                    if (s.Event.RemoveMethod == null)
                        Log.Error(s.Source, ErrorCode.I0000, s.Event.Quote() + " does not define a get accessor");
                    else
                    {
                        VerifyMemberAccess(s.Source, s.Object, s.Event.RemoveMethod);
                        VerifyArguments(s.Source, s.Event.RemoveMethod, s.Listener);
                    }
                    break;
                }
                case ExpressionType.NewObject:
                {
                    var s = e as NewObject;
                    VerifyMemberAccess(s.Source, s.Constructor);
                    VerifyArguments(s.Source, s.Constructor, s.Arguments);
                    VerifyObsolete(s.Source, s.Constructor.DeclaringType);

                    if (s.ReturnType.IsAbstract)
                        Log.Error(s.Source, ErrorCode.E4075, "Cannot instantiate abstract class " + s.ReturnType.Quote());
                    else if (s.ReturnType.IsStatic)
                        Log.Error(s.Source, ErrorCode.E4076, "Cannot instantiate static class " + s.ReturnType.Quote());
                    break;
                }
                case ExpressionType.NewArray:
                {
                    var s = e as NewArray;
                    VerifyDataTypeAccess(s.Source, s.ReturnType.ElementType);

                    if (s.Size == null && s.Initializers == null)
                        Log.Error(s.Source, ErrorCode.I0000, "Expected array size or initializer for " + s.ArrayType.Quote());
                    break;
                }
                case ExpressionType.NewDelegate:
                {
                    var s = e as NewDelegate;
                    VerifyDataTypeAccess(s.Source, s.ReturnType);
                    VerifyMemberAccess(s.Source, s.Method);

                    if (!s.Method.IsStatic && s.Object == null)
                        Log.Error(s.Source, ErrorCode.E0000, "Object is required for non-static method " + s.Method.Quote());
                    else if (s.Method.IsStatic && s.Object != null)
                        Log.Error(s.Source, ErrorCode.E0000, "Invalid object specified for static method " + s.Method.Quote());

                    if (!(
                            s.Method.ReturnType.Equals(s.DelegateType.ReturnType) ||
                            s.Method.ReturnType.IsReferenceType && s.Method.ReturnType.IsSubclassOfOrEqual(s.DelegateType.ReturnType)) ||
                            !s.DelegateType.CompareParametersEqualOrSubclassOf(s.Method)
                        )
                        Log.Error(s.Source, ErrorCode.I0000, "Invalid return type or parameter list on method " + s.Method.Quote() + " bound to delegate " + s.DelegateType.Quote());

                    if (s.Object != null && !s.Object.ReturnType.IsReferenceType)
                        Log.Error(s.Source, ErrorCode.I0000, "Delegate instance field must be a reference type (was " + s.Object.ReturnType.Quote() + ")");
                    break;
                }
                case ExpressionType.This:
                {
                    var s = e as This;

                    if (Function == null)
                        Log.Error(s.Source, ErrorCode.E0000, "'this' not allowed in current context");
                    else if (Function.IsStatic)
                        Log.Error(s.Source, ErrorCode.E4081, "'this' not allowed in static context");
                    else if (s.ReturnType.MasterDefinition != Function.DeclaringType.MasterDefinition)
                        Log.Error(s.Source, ErrorCode.I4082, "'this' has an invalid data type");
                    break;
                }
                case ExpressionType.Base:
                {
                    var s = e as Base;

                    if (Function == null)
                        Log.Error(s.Source, ErrorCode.E0000, "'base' not allowed in current context");
                    else if (Function.IsStatic)
                        Log.Error(s.Source, ErrorCode.E4083, "'base' not allowed in static context");
                    else if (s.ReturnType != Function.DeclaringType.Base)
                        Log.Error(s.Source, ErrorCode.I4084, "'base' has an invalid data type");
                    else
                    {
                        // Verify that 'base' is not used illegally
                        if (_parent != null)
                        {
                            switch (_parent.ExpressionType)
                            {
                                case ExpressionType.LoadField:
                                case ExpressionType.StoreField:
                                case ExpressionType.GetProperty:
                                case ExpressionType.SetProperty:
                                case ExpressionType.CallMethod:
                                case ExpressionType.AddListener:
                                case ExpressionType.RemoveListener:
                                    return;
                            }
                        }

                        Log.Error(s.Source, ErrorCode.E4085, "'base' not allowed in current context");
                    }
                    break;
                }
                case ExpressionType.FixOp:
                {
                    var s = e as FixOp;

                    switch (s.Operand.ExpressionType)
                    {
                        case ExpressionType.LoadArgument:
                        case ExpressionType.LoadLocal:
                        case ExpressionType.LoadElement:
                        case ExpressionType.LoadField:
                            VerifyLValue(s.Source, s.Operand);
                            break;

                        default:
                            Log.Error(s.Source, ErrorCode.E4114, "'++'/'--' is not valid because " + s.Operand.Quote() + " is not a variable");
                            break;
                    }

                    break;
                }
                case ExpressionType.TypeOf:
                {
                    var s = e as TypeOf;

                    if (s.Type.HasAttribute(Essentials.TargetSpecificTypeAttribute))
                        Log.Error(s.Source, ErrorCode.E0000, "Invalid 'typeof' on target specific type");

                    break;
                }

                // TODO: Check flattened generic type definitions, figure out how to handle generic parameter types.

                case ExpressionType.IsOp:
                {
                    var s = e as IsOp;
                    VerifyDataTypeAccess(s.Source, s.TestType);

                    if (s.Operand.ReturnType.HasAttribute(Essentials.TargetSpecificTypeAttribute) || s.ReturnType.HasAttribute(Essentials.TargetSpecificTypeAttribute))
                        Log.Error(s.Source, ErrorCode.E0000, "Invalid 'is' on target specific type");
                    break;
                }
                case ExpressionType.AsOp:
                {
                    var s = e as AsOp;
                    VerifyDataTypeAccess(s.Source, s.TestType);

                    if (!s.Operand.ReturnType.IsReferenceType)
                        Log.Error(s.Source, ErrorCode.E4116, "'as' is not valid because " + s.Operand.ReturnType.Quote() + " is not a reference type");
                    else if (!s.ReturnType.IsReferenceType)
                        Log.Error(s.Source, ErrorCode.E4117, "'as' is not valid because " + s.ReturnType.Quote() + " is not a reference type");
                    else if (!s.ReturnType.IsRelatedTo(s.Operand.ReturnType) && 
                             !s.ReturnType.IsInterface && !s.Operand.ReturnType.IsInterface &&
                             !s.ReturnType.IsGenericType)
                        Log.Error(s.Source, ErrorCode.E4118, s.Operand.ReturnType.Quote() + " cannot be converted to " + s.ReturnType.Quote() + " because the types are not related");
                    else if (s.Operand.ReturnType.HasAttribute(Essentials.TargetSpecificTypeAttribute) || s.ReturnType.HasAttribute(Essentials.TargetSpecificTypeAttribute))
                        Log.Error(s.Source, ErrorCode.E0000, "Invalid 'as' on target specific type");
                    break;
                }
                case ExpressionType.ReferenceOp:
                {
                    var s = e as ReferenceOp;

                    if (!s.Left.ReturnType.IsReferenceType ||
                        !s.Right.ReturnType.IsReferenceType ||
                        !s.Left.ReturnType.IsRelatedTo(s.Right.ReturnType) && !s.Left.ReturnType.IsInterface && !s.Right.ReturnType.IsInterface)
                        Log.Error(s.Source, ErrorCode.E0000, "Operator '==' cannot be applied to operands of type " + s.Left.ReturnType.Quote() + " and " + s.Right.ReturnType.Quote());
                    break;
                }
                case ExpressionType.CastOp:
                {
                    var s = e as CastOp;
                    VerifyDataTypeAccess(s.Source, s.ReturnType);

                    if (!s.ReturnType.IsInterface &&
                        !s.Operand.ReturnType.IsInterface &&
                        !s.ReturnType.IsSubclassOf(s.Operand.ReturnType) &&
                        !s.ReturnType.IsImplementingInterface(s.Operand.ReturnType) &&
                        !(s.ReturnType.IsGenericParameter && s.Operand.ReturnType.IsReferenceType) &&
                        !s.Operand.ReturnType.IsSubclassOf(s.ReturnType) &&
                        !s.Operand.ReturnType.IsImplementingInterface(s.ReturnType))
                        Log.Error(s.Source, ErrorCode.I4119, "Invalid cast from " + s.Operand.ReturnType.Quote() + " to " + s.ReturnType.Quote());

                    if (s.ReturnType.IsClass && s.ReturnType.HasAttribute(Essentials.TargetSpecificTypeAttribute) ||
                        s.Operand.ReturnType.IsClass && s.Operand.ReturnType.HasAttribute(Essentials.TargetSpecificTypeAttribute))
                        Log.Error(s.Source, ErrorCode.E0000, "Cannot cast [TargetSpecificType] class");
                    break;
                }
                case ExpressionType.AddressOf:
                {
                    var s = e as AddressOf;

                    switch (s.AddressType)
                    {
                        case AddressType.Ref:
                        case AddressType.Out:
                            switch (s.Operand.ExpressionType)
                            {
                                case ExpressionType.LoadLocal:
                                case ExpressionType.LoadArgument:
                                case ExpressionType.LoadField:
                                case ExpressionType.LoadElement:
                                    return;
                            }

                            Log.Error(s.Source, ErrorCode.E4126, "Only local variables, function parameters, fields and array elements may be passed as ref/out-arguments.");
                            break;

                        default:
                            switch (s.Operand.ExpressionType)
                            {
                                case ExpressionType.AddressOf:
                                    break;

                                default:
                                    switch (s.ReturnType.TypeType)
                                    {
                                        case TypeType.FixedArray:
                                            return;

                                        case TypeType.Enum:
                                        case TypeType.Struct:
                                        case TypeType.GenericParameter:
                                            if (!u.IsObject())
                                                Log.Error(s.Source, ErrorCode.I0000, "Address cannot be used as value " + s.Quote());
                                            return;
                                    }
                                    break;
                            }

                            Log.Error(s.Source, ErrorCode.I0000, "Invalid address " + s.Quote());
                            break;
                    }
                    break;
                }
                case ExpressionType.StageOp:
                {
                    var s = e as StageOp;

                    if (MetaProperty != null)
                    {
                        var stage = _stageStack.Last();

                        if (stage != MetaStage.Undefined && stage < s.Stage)
                            Log.Error(s.Source, ErrorCode.E0000, "Invalid " + s.Stage.ToLiteral().Quote() + " operator inside a " + stage.ToLiteral().Quote() + " operator");

                        _stageStack.Add(s.Stage);
                    }
                    else
                        Log.Error(s.Source, ErrorCode.E0000, s.Stage.ToLiteral().Quote() + " operator is not allowed in current context");
                    break;
                }
                case ExpressionType.GetMetaProperty:
                {
                    var s = e as GetMetaProperty;

                    if (MetaProperty != null && s.Offset == 0 && MetaProperty.Name == s.Name)
                        Log.Error(s.Source, ErrorCode.E0000, "Not allowed for meta property to refer to itself");
                    break;
                }
                case ExpressionType.NewVertexAttrib:
                {
                    if (MetaProperty == null)
                        Log.Error(e.Source, ErrorCode.E0000, "'vertex_attrib' is not allowed in current context");
                    break;
                }
                case ExpressionType.NewPixelSampler:
                {
                    if (MetaProperty == null)
                        Log.Error(e.Source, ErrorCode.E0000, "'pixel_sampler' is not allowed in current context");
                    break;
                }
                case ExpressionType.LoadVertexAttrib:
                {
                    var sf = Function as ShaderFunction;

                    if (sf == null || sf.Shader.Type != ShaderType.Vertex)
                        Log.Error(e.Source, ErrorCode.I0000, "Invalid vertex attribute load outside of vertex shader");
                    break;
                }
                case ExpressionType.LoadVarying:
                {
                    var sf = Function as ShaderFunction;

                    if (sf == null || sf.Shader.Type != ShaderType.Pixel)
                        Log.Error(e.Source, ErrorCode.I0000, "Invalid varying load outside of pixel shader");
                    break;
                }
                case ExpressionType.RuntimeConst:
                {
                    var s = e as RuntimeConst;
                    var sf = Function as ShaderFunction;

                    if (sf == null || !sf.Shader.ReferencedConstants.Contains(s.Index))
                        Log.Error(e.Source, ErrorCode.I0000, "Invalid shader constant reference outside of shader declaring the constant");
                    break;
                }
                case ExpressionType.LoadUniform:
                {
                    var s = e as LoadUniform;
                    var sf = Function as ShaderFunction;

                    if (sf == null || !sf.Shader.ReferencedUniforms.Contains(s.Index))
                        Log.Error(e.Source, ErrorCode.I0000, "Invalid uniform reference outside of shader declaring the uniform");
                    break;
                }
                case ExpressionType.LoadPixelSampler:
                {
                    var sf = Function as ShaderFunction;

                    if (sf == null || sf.Shader.Type != ShaderType.Pixel)
                        Log.Error(e.Source, ErrorCode.I0000, "Invalid pixel sampler reference outside of pixel shader declaring the sampler");
                    break;
                }
                case ExpressionType.CallShader:
                {
                    var s = e as CallShader;
                    var sf = Function as ShaderFunction;

                    if (sf == null || !sf.Shader.Functions.Contains(s.Function))
                        Log.Error(e.Source, ErrorCode.I0000, "Invalid function call outside of shader declaring the function");
                    break;
                }
                case ExpressionType.CapturedLocal:
                case ExpressionType.CapturedArgument:
                {
                    if (MetaProperty == null) // TODO: Verify that meta property has draw block as parent (somewhere)
                        Log.Error(e.Source, ErrorCode.I0000, "Invalid variable capture outside of draw block");
                    break;
                }
                case ExpressionType.Lambda:
                {
                    _lambdas.Push((Lambda) e);
                    break;
                }
                case ExpressionType.ExternOp:
                {
                    VerifyExtern(e);
                    break;
                }
            }
        }

        public override void End(ref Expression e, ExpressionUsage u)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.StageOp:
                {
                    if (MetaProperty != null)
                        _stageStack.RemoveLast();
                    break;
                }
                case ExpressionType.Lambda:
                {
                    _lambdas.Pop();
                    break;
                }
            }
        }

        void VerifyWrite(Source src, Field f)
        {
            if (Environment.IsGeneratingCode && Backend.Has(TypeOptions.IgnoreProtection))
                return;

            if (f.FieldModifiers.HasFlag(FieldModifiers.ReadOnly) && !(Function.IsConstructor && Function.DeclaringType.MasterDefinition == f.DeclaringType.MasterDefinition))
                Log.Error(src, ErrorCode.E4087, "The field " + f.Quote() + " is readonly and can only be written to from its constructor");
        }

        void VerifyLValue(Source src, Expression e)
        {
            if (e == null || e.ReturnType.IsReferenceType)
                return;

            switch (e.ExpressionType)
            {
                case ExpressionType.LoadLocal:
                case ExpressionType.LoadArgument:
                case ExpressionType.LoadElement:
                case ExpressionType.This:
                case ExpressionType.Base:
                    break;

                case ExpressionType.LoadPtr:
                    VerifyLValue(src, (e as LoadPtr).Argument);
                    break;
                case ExpressionType.AddressOf:
                    VerifyLValue(src, (e as AddressOf).Operand);
                    break;
                case ExpressionType.LoadField:

                    VerifyLValue(src, (e as LoadField).Object);
                    break;

                default:
                    Log.Error(src, ErrorCode.I0000, "L-value is not an address (" + e.ExpressionType + ")");
                    break;
            }
        }
    }
}
