using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.Core.IL.Building.Functions;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        public Expression CompileExpression(AstExpression e)
        {
            switch (e.ExpressionType)
            {
                case AstExpressionType.Initializer:
                {
                    var s = (AstInitializer) e;
                    var root = CompileExpression(s.Expressions[0]);
                    for (var i = 1; i < s.Expressions.Count; i++)
                        root = new SequenceOp(root, CompileExpression(s.Expressions[i]));
                    return root;
                }

                case AstExpressionType.Identifier:
                    return CompilePartial(ResolveIdentifier(e as AstIdentifier, null));
                case AstExpressionType.Macro:
                    return new ExternString(e.Source, Essentials.String, ((AstMacro) e).Value, GetUsings(e.Source));

                case AstExpressionType.Void:
                case AstExpressionType.Global:
                case AstExpressionType.Generic:
                case AstExpressionType.BuiltinType:
                case AstExpressionType.Array:
                case AstExpressionType.FixedArray:
                case AstExpressionType.Parameterizer:
                    return CompilePartial(NameResolver.ResolveExpression(Namescope, e, null));

                case AstExpressionType.Add:
                case AstExpressionType.Sub:
                case AstExpressionType.Mul:
                case AstExpressionType.Div:
                case AstExpressionType.Mod:
                case AstExpressionType.NullOp:
                case AstExpressionType.LogAnd:
                case AstExpressionType.LogOr:
                case AstExpressionType.Equal:
                case AstExpressionType.NotEqual:
                case AstExpressionType.LessThan:
                case AstExpressionType.LessThanOrEqual:
                case AstExpressionType.GreaterThan:
                case AstExpressionType.GreaterThanOrEqual:
                case AstExpressionType.BitwiseAnd:
                case AstExpressionType.BitwiseOr:
                case AstExpressionType.BitwiseXor:
                case AstExpressionType.ShiftLeft:
                case AstExpressionType.ShiftRight:
                case AstExpressionType.Assign:
                case AstExpressionType.AddAssign:
                case AstExpressionType.SubAssign:
                case AstExpressionType.MulAssign:
                case AstExpressionType.DivAssign:
                case AstExpressionType.ModAssign:
                case AstExpressionType.BitwiseAndAssign:
                case AstExpressionType.BitwiseOrAssign:
                case AstExpressionType.BitwiseXorAssign:
                case AstExpressionType.ShiftLeftAssign:
                case AstExpressionType.ShiftRightAssign:
                case AstExpressionType.LogAndAssign:
                case AstExpressionType.LogOrAssign:
                case AstExpressionType.Sequence:
                    return CompileBinOp(e as AstBinary);
                case AstExpressionType.DecreasePrefix:
                case AstExpressionType.DecreasePostfix:
                case AstExpressionType.IncreasePrefix:
                case AstExpressionType.IncreasePostfix:
                case AstExpressionType.Negate:
                case AstExpressionType.LogNot:
                case AstExpressionType.BitwiseNot:
                    return CompileUnOp(e as AstUnary);
                case AstExpressionType.Cast:
                    return CompileCast(e as AstCast);
                case AstExpressionType.Call:
                    return CompileCall(e as AstCall);
                case AstExpressionType.LookUp:
                    return CompilePartial(ResolveLookUp(e as AstCall));
                case AstExpressionType.Member:
                    return CompilePartial(ResolveMember(e as AstMember, null));
                case AstExpressionType.New:
                    return CompileNewExpression(e as AstNew);
                case AstExpressionType.This:
                    return CompileThis(e.Source);
                case AstExpressionType.Base:
                    return CompileBase(e.Source);
                case AstExpressionType.Import:
                    return CompileImport(e as AstImport);
                case AstExpressionType.VertexAttribImplicit:
                    return CompileVertexAttribImplicit(e as AstVertexAttribImplicit);
                case AstExpressionType.VertexAttribExplicit:
                    return CompileVertexAttribExplicit(e as AstVertexAttribExplicit);

                case AstExpressionType.Null:
                    return new Constant(e.Source, DataType.Null, null);
                case AstExpressionType.Defined:
                    return new Constant(e.Source, Essentials.Bool, Environment.Test(e.Source, (e as AstDefined).Condition));
                case AstExpressionType.Default:
                    return new Default(e.Source, NameResolver.GetType(Namescope, ((AstUnary) e).Operand));
                case AstExpressionType.True:
                    return new Constant(e.Source, Essentials.Bool, true);
                case AstExpressionType.False:
                    return new Constant(e.Source, Essentials.Bool, false);
                case AstExpressionType.Zero:
                    return new Constant(e.Source, Essentials.Int, 0);
                case AstExpressionType.Int:
                    return new Constant(e.Source, Essentials.Int, ((AstInt) e).Value);
                case AstExpressionType.UInt:
                    return new Constant(e.Source, Essentials.UInt, ((AstUInt) e).Value);
                case AstExpressionType.Long:
                    return new Constant(e.Source, Essentials.Long, ((AstLong) e).Value);
                case AstExpressionType.ULong:
                    return new Constant(e.Source, Essentials.ULong, ((AstULong) e).Value);
                case AstExpressionType.Float:
                    return new Constant(e.Source, Essentials.Float, ((AstFloat) e).Value);
                case AstExpressionType.Double:
                    return new Constant(e.Source, Essentials.Double, ((AstDouble) e).Value);
                case AstExpressionType.Char:
                    return new Constant(e.Source, Essentials.Char, ((AstChar) e).Value);
                case AstExpressionType.String:
                    return new Constant(e.Source, Essentials.String, ((AstString) e).Value);
                case AstExpressionType.Unchecked:
                {
                    CheckCastStack.Add(false);
                    var result = CompileExpression(((AstUnary) e).Operand);
                    CheckCastStack.RemoveLast();
                    return result;
                }
                case AstExpressionType.ReadOnly:
                {
                    var s = e as AstUnary;
                    return new StageOp(s.Source, MetaStage.ReadOnly, CompileExpression(s.Operand));
                }
                case AstExpressionType.Volatile:
                {
                    var s = e as AstUnary;
                    return new StageOp(s.Source, MetaStage.Volatile, CompileExpression(s.Operand));
                }
                case AstExpressionType.Vertex:
                {
                    var s = e as AstUnary;
                    return new StageOp(s.Source, MetaStage.Vertex, CompileExpression(s.Operand));
                }
                case AstExpressionType.Pixel:
                {
                    var s = e as AstUnary;
                    return new StageOp(s.Source, MetaStage.Pixel, CompileExpression(s.Operand));
                }
                case AstExpressionType.Extern:
                {
                    var s = (AstExtern) e;
                    return new ExternOp(s.Value.Source, 
                        Compiler.CompileAttributes(Namescope, s.Attributes), 
                        s.OptionalType != null
                            ? NameResolver.GetType(Namescope, s.OptionalType)
                            : DataType.Void, 
                        s.Value.String, 
                        ExtensionTransform.CreateObject(s.Source, Function, TypeBuilder.Parameterize(Function.DeclaringType)), 
                        s.OptionalArguments != null
                            ? CompileArgumentList(s.OptionalArguments)
                            : ExtensionTransform.CreateArgumentList(s.Source, Function),
                        GetUsings(s.Source));
                }
                case AstExpressionType.Ternary:
                {
                    var s = e as AstTernary;

                    if (s.Condition is AstDefined)
                    {
                        var def = s.Condition as AstDefined;
                        var result = Environment.Test(def.Source, def.Condition)
                            ? CompileExpression(s.True)
                            : CompileExpression(s.False);
                        return result.ReturnType.IsNull
                            ? Error(s.Source, ErrorCode.E0000, "Cannot resolve type of <null> in constant folded conditional expression")
                            : result;
                    }

                    var cond = CompileImplicitCast(s.Condition.Source, Essentials.Bool, CompileExpression(s.Condition));
                    var left = CompileExpression(s.True);
                    var right = CompileExpression(s.False);

                    if (!left.ReturnType.IsNull)
                        right = CompileImplicitCast(s.False.Source, left.ReturnType, right);
                    else if (!right.ReturnType.IsNull)
                        left = CompileImplicitCast(s.True.Source, right.ReturnType, left);
                    else
                        return Error(s.Source, ErrorCode.E2086, "The type of the conditional expression could not be resolved because there is no implicit conversion between <null> and <null>");

                    return new ConditionalOp(s.Source, cond, left, right);
                }
                case AstExpressionType.Local:
                {
                    var s = e as AstLocal;
                    var p = TryResolveLocalIdentifier(s.Name);

                    if (p == null && Namescope is BlockBase)
                        p = TryResolveCapturedLocalIdentifier(Namescope as BlockBase, s.Name);

                    return p != null
                        ? CompilePartial(p)
                        : Error(e.Source, ErrorCode.E0000, s.Name.Quote() + " is not a local variable or method parameter");
                }
                case AstExpressionType.Prev:
                {
                    var s = e as AstPrev;

                    if (s.OptionalName != null)
                    {
                        var b = CompileExpression(s.OptionalName);
                        if (b.IsInvalid)
                            return b;
                        var mp = b as GetMetaProperty;
                        return mp == null
                            ? Error(s.OptionalName.Source, ErrorCode.E2035, b.Quote() + " is not a meta property")
                            : new GetMetaProperty(s.Source, mp.ReturnType, mp.Name, mp.Offset + s.Offset);
                    }

                    return MetaProperty != null
                        ? new GetMetaProperty(s.Source, MetaProperty.ReturnType, MetaProperty.Name, 1)
                        : Error(s.Source, ErrorCode.E2036, "Current scope is not a meta property scope");
                }
                case AstExpressionType.PixelSampler:
                {
                    var s = e as AstPixelSampler;
                    var texture = CompileExpression(s.Texture);

                    Expression samplerState = null;
                    if (s.OptionalState != null)
                    {
                        var samplerStateType = ILFactory.GetType(s.OptionalState.Source, "Uno.Graphics.SamplerState");
                        samplerState = CompileImplicitCast(s.OptionalState.Source, samplerStateType, CompileExpression(s.OptionalState));
                    }

                    switch (texture.ReturnType.BuiltinType)
                    {
                        case BuiltinType.Texture2D:
                            return new NewPixelSampler(s.Source, Essentials.Sampler2D, texture, samplerState);
                        case BuiltinType.TextureCube:
                            return new NewPixelSampler(s.Source, Essentials.SamplerCube, texture, samplerState);
                        case BuiltinType.VideoTexture:
                            return new NewPixelSampler(s.Source, Essentials.VideoSampler, texture, samplerState);
                    }

                    return !texture.IsInvalid
                        ? Error(s.Texture.Source, ErrorCode.E0000, "First argument to 'pixel_sampler' must be an object of type 'texture2D' or 'textureCube'")
                        : Expression.Invalid;
                }
                case AstExpressionType.Is:
                {
                    var s = (AstBinary) e;
                    var obj = CompileExpression(s.Left);
                    var type = NameResolver.GetType(Namescope, s.Right);
                    return new IsOp(e.Source, obj, type, Essentials.Bool);
                }
                case AstExpressionType.As:
                {
                    var s = (AstBinary) e;
                    var obj = CompileExpression(s.Left);
                    var type = NameResolver.GetType(Namescope, s.Right);
                    return new AsOp(e.Source, obj, type);
                }
                case AstExpressionType.SizeOf:
                {
                    var s = (AstUnary) e;
                    var dt = NameResolver.GetType(Namescope, s.Operand);
                    return new Constant(s.Source, Essentials.Int, NameResolver.GetSizeOf(s.Source, dt));
                }
                case AstExpressionType.TypeOf:
                {
                    var s = (AstUnary) e;
                    return new TypeOf(s.Source, Essentials.Type, NameResolver.GetType(Namescope, s.Operand));
                }
                case AstExpressionType.NameOf:
                {
                    // TODO: Missing verification
                    var s = (AstUnary) e;
                    return new Constant(s.Source, Essentials.String, ResolveExpression(s.Operand, null).ToString());
                }
                case AstExpressionType.Lambda:
                    return CompileLambda((AstLambda) e);

                case AstExpressionType.ArrayInitializer:
                    return Error(e.Source, ErrorCode.E2075, "Array initializers are not supported in this context");
            }

            return e is AstIL
                ? (e as AstIL).Expression
                : Error(e.Source, ErrorCode.I2042, "Unhandled expression type in CompileExpression: " + e.ExpressionType);
        }

        public Expression CompileThis(Source src)
        {
            var dt = (Namescope as BlockBase)?.TryFindTypeParent();
            return dt != null
                    ? new GetMetaObject(src, dt) :
                !IsFunctionScope
                    ? Error(src, ErrorCode.E2030, "'this' not allowed in current context")
                    : new This(src, TypeBuilder.Parameterize(Function.DeclaringType));
        }

        public Expression CompileBase(Source src)
        {
            return !IsFunctionScope
                    ? Error(src, ErrorCode.E2032, "'base' not allowed in current context") :
                Function.DeclaringType.Base == null
                    ? Error(src, ErrorCode.E2033, "'base' not allowed in current context - " + Function.DeclaringType.Quote() + " does not have a base class")
                    : new Base(src, Function.DeclaringType.Base);
        }
    }
}
