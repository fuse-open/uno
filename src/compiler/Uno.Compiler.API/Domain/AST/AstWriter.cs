using System;
using System.Collections.Generic;
using System.IO;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;
using Uno.Compiler.API.Domain.AST.Statements;
using Uno.Compiler.API.Domain.Serialization;
using Uno.IO;

namespace Uno.Compiler.API.Domain.AST
{
    public sealed class AstWriter : CacheWriter
    {
        public static AstWriter Create(SourcePackage upk, string filename, AstSerializationFlags flags)
        {
            if (upk.IsUnknown)
                throw new InvalidOperationException("AstWriter: Unknown source package");

            var w = new AstWriter(upk, filename, flags);
            w.Write(AstSerialization.Magic);
            return w;
        }

        public AstWriter(SourcePackage upk, string filename, AstSerializationFlags flags)
            : base(upk, filename)
        {
            OptimizeSources = flags.HasFlag(AstSerializationFlags.OptimizeSources);
        }

        public void WriteBlock(AstBlock a)
        {
            WriteGlobal(a.DocComment);
            WriteAttributes(a.Attributes);
            WriteModifiers(a.Modifiers, a.OptionalCondition);
            WriteIdentifier(a.Name);
            WriteExpressions(a.UsingBlocks);
            WriteBlockMembers(a.Members);
        }

        public void WriteClass(AstClass a)
        {
            WriteGlobal(a.DocComment);
            WriteAttributes(a.Attributes);
            WriteModifiers(a.Modifiers, a.OptionalCondition);
            Write((byte) a.Type);
            WriteIdentifier(a.Name);
            WriteExpressions(a.Bases);
            WriteGenericSignature(a.OptionalGeneric);
            WriteBlockMembers(a.Members);
            WriteExpressions(a.Swizzlers);
        }

        public void WriteConstraints(IReadOnlyList<AstConstraint> s)
        {
            Write7BitEncodedInt(s.Count);
            foreach (var a in s)
            {
                WriteIdentifier(a.Parameter);
                Write((byte) a.Type);
                WriteExpressions(a.BaseTypes);
                Write(a.OptionalConstructor);
            }
        }

        public void WriteDelegate(AstDelegate a)
        {
            WriteGlobal(a.DocComment);
            WriteAttributes(a.Attributes);
            WriteModifiers(a.Modifiers, a.OptionalCondition);
            Write(a.ReturnType);
            WriteIdentifier(a.Name);
            WriteParameters(a.Parameters);
            WriteGenericSignature(a.OptionalGenericSignature);
        }

        public void WriteEnum(AstEnum a)
        {
            WriteGlobal(a.DocComment);
            WriteAttributes(a.Attributes);
            WriteModifiers(a.Modifiers, a.OptionalCondition);
            WriteIdentifier(a.Name);
            Write(a.OptionalBaseType);

            Write7BitEncodedInt(a.Literals.Count);
            foreach (var e in a.Literals)
                WriteLiteral(e);
        }

        public void WriteGenericSignature(AstGenericSignature s)
        {
            if (s == null)
            {
                WriteIdentifiers(null);
                return;
            }

            WriteIdentifiers(s.Parameters);
            WriteConstraints(s.Constraints);
        }

        public void WriteLiteral(AstLiteral a)
        {
            WriteGlobal(a.DocComment);
            WriteAttributes(a.Attributes);
            WriteIdentifier(a.Name);
            Write(a.OptionalValue);
        }

        public void WriteDocument(AstDocument a)
        {
            Write(a.Name.Source);
            WriteNamespace_inner(a);
        }

        public void WriteNamespace(AstNamespace a)
        {
            WriteIdentifier(a.Name);
            WriteNamespace_inner(a);
        }

        void WriteNamespace_inner(AstNamespace a)
        {
            Write7BitEncodedInt(a.Usings.Count);
            foreach (var n in a.Usings)
                WriteUsingDirective(n);

            Write7BitEncodedInt(a.Namespaces.Count);
            foreach (var n in a.Namespaces)
                WriteNamespace(n);

            Write7BitEncodedInt(a.Blocks.Count);
            foreach (var n in a.Blocks)
                WriteBlockMember(n);
        }

        public void WriteUsingDirective(AstUsingDirective a)
        {
            Write(a.Expression);
            Write(a.OptionalAlias);
            Write(a.IsStatic);
        }

        public void WriteNode(AstNode a)
        {
            Write((byte) a.NodeType);
            WriteIdentifier(a.Name);
            WriteBlockMembers(a.Members);
        }

        public void WriteAccessor(AstAccessor a)
        {
            if (a == null)
            {
                Write((Source) null);
                return;
            }

            Write(a.Source ?? Source.Unknown);
            WriteModifiers(a.Modifiers, a.OptionalCondition);
            Write(a.OptionalBody);
        }

        public void WriteApply(AstApply a)
        {
            Write((byte) a.Modifier);
            Write(a.Block);
        }

        public void WriteAttribute(AstAttribute a)
        {
            if (!string.IsNullOrEmpty(a.OptionalCondition))
            {
                Write((byte) (a.Modifier | AstAttributeModifier.HasCondition));
                WriteGlobal(a.OptionalCondition);
            }
            else
                Write((byte) (a.Modifier & ~AstAttributeModifier.HasCondition));

            Write(a.Attribute);
            WriteArguments(a.Arguments);
        }

        public void WriteAttributes(IReadOnlyList<AstAttribute> list)
        {
            Write7BitEncodedInt(list.Count);
            foreach (var e in list)
                WriteAttribute(e);
        }

        public void WriteBlockMember(AstBlockMember a)
        {
            Write((byte) a.MemberType);

            switch (a.MemberType)
            {
                case AstMemberType.Field:
                    WriteField(a as AstField);
                    break;
                case AstMemberType.Constructor:
                    WriteConstructor(a as AstConstructor);
                    break;
                case AstMemberType.Finalizer:
                    WriteFinalizer(a as AstFinalizer);
                    break;
                case AstMemberType.Method:
                    WriteMethod(a as AstMethod);
                    break;
                case AstMemberType.Property:
                    WriteProperty(a as AstProperty);
                    break;
                case AstMemberType.Class:
                    WriteClass(a as AstClass);
                    break;
                case AstMemberType.Indexer:
                    WriteIndexer(a as AstIndexer);
                    break;
                case AstMemberType.Operator:
                    WriteOperator(a as AstOperator);
                    break;
                case AstMemberType.Converter:
                    WriteConverter(a as AstConverter);
                    break;
                case AstMemberType.Enum:
                    WriteEnum(a as AstEnum);
                    break;
                case AstMemberType.Delegate:
                    WriteDelegate(a as AstDelegate);
                    break;
                case AstMemberType.Event:
                    WriteEvent(a as AstEvent);
                    break;
                case AstMemberType.MetaProperty:
                    WriteMetaProperty(a as AstMetaProperty);
                    break;
                case AstMemberType.ApplyStatement:
                    WriteApply(a as AstApply);
                    break;
                case AstMemberType.Block:
                    WriteBlock(a as AstBlock);
                    break;
                case AstMemberType.NodeBlock:
                    WriteNode(a as AstNode);
                    break;
                default:
                    throw new InvalidOperationException("Invalid member: " + a.MemberType);
            }
        }

        public void WriteBlockMembers(IReadOnlyList<AstBlockMember> args)
        {
            if (args == null)
            {
                Write7BitEncodedInt(0);
                return;
            }

            Write7BitEncodedInt(args.Count + 1);
            foreach (var a in args)
                WriteBlockMember(a);
        }

        public void WriteConstructor(AstConstructor a)
        {
            Write(a.Source);
            WriteGlobal(a.DocComment);
            WriteAttributes(a.Attributes);
            WriteModifiers(a.Modifiers, a.OptionalCondition);
            WriteParameters(a.Parameters);
            Write((byte) a.CallType);
            WriteArguments(a.CallArguments);
            Write(a.OptionalBody);
        }

        public void WriteConverter(AstConverter a)
        {
            WriteGlobal(a.DocComment);
            WriteAttributes(a.Attributes);
            WriteModifiers(a.Modifiers, a.OptionalCondition);
            Write(a.TargetType);
            WriteParameters(a.Parameters);
            Write(a.OptionalBody);
        }

        public void WriteEvent(AstEvent a)
        {
            WriteGlobal(a.DocComment);
            WriteAttributes(a.Attributes);
            WriteModifiers(a.Modifiers, a.OptionalCondition);
            Write(a.DelegateType);
            Write(a.OptionalInterfaceType);
            WriteIdentifier(a.Name);
            WriteAccessor(a.Add);
            WriteAccessor(a.Remove);
        }

        public void WriteField(AstField a)
        {
            WriteGlobal(a.DocComment);
            WriteAttributes(a.Attributes);
            WriteModifiers(a.Modifiers, a.OptionalCondition);
            Write((byte) a.FieldModifiers);
            Write(a.ReturnType);
            WriteIdentifier(a.Name);
            Write(a.InitValue);
        }

        public void WriteFinalizer(AstFinalizer a)
        {
            Write(a.Source);
            WriteGlobal(a.DocComment);
            WriteAttributes(a.Attributes);
            WriteModifiers(a.Modifiers, a.OptionalCondition);
            WriteParameters(a.Parameters);
            Write(a.OptionalBody);
        }

        public void WriteIndexer(AstIndexer a)
        {
            Write(a.Source);
            WriteGlobal(a.DocComment);
            WriteAttributes(a.Attributes);
            WriteModifiers(a.Modifiers, a.OptionalCondition);
            Write(a.ReturnType);
            Write(a.OptionalInterfaceType);
            WriteParameters(a.Parameters);
            WriteAccessor(a.Get);
            WriteAccessor(a.Set);
        }

        public void WriteMetaProperty(AstMetaProperty a)
        {
            Write((byte) a.Visibility);
            Write(a.OptionalType);
            WriteIdentifier(a.Name);
            WriteMetaPropertyDefinitions(a.Definitions);
        }

        public void WriteMetaPropertyDefinitions(IReadOnlyList<AstMetaPropertyDefinition> l)
        {
            Write7BitEncodedInt(l.Count);
            foreach (var a in l)
            {
                Write(a.Value);
                WriteGlobals(a.Tags);
                WriteReqStatements(a.Requirements);
            }
        }

        public void WriteMethod(AstMethod a)
        {
            WriteGlobal(a.DocComment);
            WriteAttributes(a.Attributes);
            WriteModifiers(a.Modifiers, a.OptionalCondition);
            Write(a.ReturnType);
            Write(a.OptionalInterfaceType);
            WriteIdentifier(a.Name);
            WriteParameters(a.Parameters);
            WriteGenericSignature(a.OptionalGenericSignature);
            Write(a.OptionalBody);
        }

        public void WriteOperator(AstOperator a)
        {
            Write(a.Source);
            WriteGlobal(a.DocComment);
            WriteAttributes(a.Attributes);
            WriteModifiers(a.Modifiers, a.OptionalCondition);
            Write(a.ReturnType);
            Write((byte) a.Operator);
            WriteParameters(a.Parameters);
            Write(a.OptionalBody);
        }

        public void Write(AstParameter a)
        {
            WriteAttributes(a.Attributes);
            Write((byte) a.Modifier);
            Write(a.OptionalType);
            WriteIdentifier(a.Name);
            Write(a.OptionalValue);
        }

        public void WriteParameters(IReadOnlyList<AstParameter> list)
        {
            if (list == null)
            {
                Write7BitEncodedInt(0);
                return;
            }

            Write7BitEncodedInt(list.Count + 1);
            foreach (var e in list)
                Write(e);
        }

        public void WriteProperty(AstProperty a)
        {
            WriteGlobal(a.DocComment);
            WriteAttributes(a.Attributes);
            WriteModifiers(a.Modifiers, a.OptionalCondition);
            Write(a.ReturnType);
            Write(a.OptionalInterfaceType);
            WriteIdentifier(a.Name);
            WriteAccessor(a.Get);
            WriteAccessor(a.Set);
        }

        public void WriteReqStatements(IReadOnlyList<AstReqStatement> l)
        {
            Write7BitEncodedInt(l.Count);
            foreach (var a in l)
            {
                Write(a.Source);
                Write7BitEncodedInt((int) a.Offset);
                WriteIdentifier(a.Name);
                Write(a.Type);
                WriteGlobal(a.Tag);                
            }
        }

        public void Write(AstStatement obj)
        {
            if (obj == null)
            {
                Write((byte) 0);
                return;
            }

            var type = obj.StatementType;
            Write((byte) type);

            switch (type)
            {
                case AstStatementType.Undef:
                    break;
                case AstStatementType.AutoRelease:
                case AstStatementType.Checked:
                case AstStatementType.Unchecked:
                case AstStatementType.Unsafe:
                    WriteModifiedStatement((AstModifiedStatement) obj);
                    break;
                case AstStatementType.While:
                case AstStatementType.DoWhile:
                    WriteLoop((AstLoop) obj);
                    break;
                case AstStatementType.Break:
                case AstStatementType.BuildError:
                case AstStatementType.BuildWarning:
                case AstStatementType.Continue:
                case AstStatementType.DrawDispose:
                case AstStatementType.Return:
                case AstStatementType.Throw:
                case AstStatementType.YieldBreak:
                    WriteEmptyStatement((AstEmptyStatement) obj);
                    break;
                case AstStatementType.Assert:
                case AstStatementType.BuildErrorMessage:
                case AstStatementType.BuildWarningMessage:
                case AstStatementType.DebugLog:
                case AstStatementType.ReturnValue:
                case AstStatementType.ThrowValue:
                case AstStatementType.YieldReturnValue:
                    WriteValueStatement((AstValueStatement) obj);
                    break;
                case AstStatementType.VariableDeclaration:
                    WriteVariableDeclaration((AstVariableDeclaration) obj);
                    break;
                case AstStatementType.FixedArrayDeclaration:
                {
                    var s = (AstFixedArrayDeclaration) obj;
                    Write(s.Type);
                    WriteIdentifier(s.Name);
                    Write(s.OptionalValue);
                    break;
                }
                case AstStatementType.Scope:
                    WriteScope((AstScope) obj);
                    break;
                case AstStatementType.ExternScope:
                    WriteExternScope((AstExternScope) obj);
                    break;
                case AstStatementType.IfElse:
                    WriteIfElse((AstIfElse) obj);
                    break;
                case AstStatementType.For:
                    WriteFor((AstFor) obj);
                    break;
                case AstStatementType.Foreach:
                    WriteForeach((AstForeach) obj);
                    break;
                case AstStatementType.Switch:
                    WriteSwitch((AstSwitch) obj);
                    break;
                case AstStatementType.TryCatchFinally:
                    WriteTryCatchFinally((AstTryCatchFinally) obj);
                    break;
                case AstStatementType.Lock:
                    WriteLock((AstLock) obj);
                    break;
                case AstStatementType.Using:
                    WriteUsing((AstUsing) obj);
                    break;
                case AstStatementType.Draw:
                    WriteDraw((AstDraw) obj);
                    break;
                case (AstStatementType) AstExpressionType.Global:
                case (AstStatementType) AstExpressionType.Void:
                case (AstStatementType) AstExpressionType.Null:
                case (AstStatementType) AstExpressionType.Var:
                case (AstStatementType) AstExpressionType.This:
                case (AstStatementType) AstExpressionType.Base:
                case (AstStatementType) AstExpressionType.True:
                case (AstStatementType) AstExpressionType.False:
                case (AstStatementType) AstExpressionType.Zero:
                    WriteSymbol((AstSymbol) obj);
                    break;
                case (AstStatementType) AstExpressionType.Array:
                case (AstStatementType) AstExpressionType.Nullable:
                case (AstStatementType) AstExpressionType.Checked:
                case (AstStatementType) AstExpressionType.Unchecked:
                case (AstStatementType) AstExpressionType.Unsafe:
                case (AstStatementType) AstExpressionType.Default:
                case (AstStatementType) AstExpressionType.NameOf:
                case (AstStatementType) AstExpressionType.SizeOf:
                case (AstStatementType) AstExpressionType.TypeOf:
                case (AstStatementType) AstExpressionType.Vertex:
                case (AstStatementType) AstExpressionType.Pixel:
                case (AstStatementType) AstExpressionType.ReadOnly:
                case (AstStatementType) AstExpressionType.Volatile:
                case (AstStatementType) AstExpressionType.DecreasePrefix:
                case (AstStatementType) AstExpressionType.DecreasePostfix:
                case (AstStatementType) AstExpressionType.IncreasePrefix:
                case (AstStatementType) AstExpressionType.IncreasePostfix:
                case (AstStatementType) AstExpressionType.Negate:
                case (AstStatementType) AstExpressionType.LogNot:
                case (AstStatementType) AstExpressionType.BitwiseNot:
                    WriteUnary((AstUnary) obj);
                    break;
                case (AstStatementType) AstExpressionType.Is:
                case (AstStatementType) AstExpressionType.As:
                case (AstStatementType) AstExpressionType.Add:
                case (AstStatementType) AstExpressionType.Sub:
                case (AstStatementType) AstExpressionType.Mul:
                case (AstStatementType) AstExpressionType.Div:
                case (AstStatementType) AstExpressionType.Mod:
                case (AstStatementType) AstExpressionType.NullOp:
                case (AstStatementType) AstExpressionType.LogAnd:
                case (AstStatementType) AstExpressionType.LogOr:
                case (AstStatementType) AstExpressionType.Equal:
                case (AstStatementType) AstExpressionType.NotEqual:
                case (AstStatementType) AstExpressionType.LessThan:
                case (AstStatementType) AstExpressionType.LessThanOrEqual:
                case (AstStatementType) AstExpressionType.GreaterThan:
                case (AstStatementType) AstExpressionType.GreaterThanOrEqual:
                case (AstStatementType) AstExpressionType.BitwiseAnd:
                case (AstStatementType) AstExpressionType.BitwiseOr:
                case (AstStatementType) AstExpressionType.BitwiseXor:
                case (AstStatementType) AstExpressionType.ShiftLeft:
                case (AstStatementType) AstExpressionType.ShiftRight:
                case (AstStatementType) AstExpressionType.Assign:
                case (AstStatementType) AstExpressionType.AddAssign:
                case (AstStatementType) AstExpressionType.SubAssign:
                case (AstStatementType) AstExpressionType.MulAssign:
                case (AstStatementType) AstExpressionType.DivAssign:
                case (AstStatementType) AstExpressionType.ModAssign:
                case (AstStatementType) AstExpressionType.BitwiseAndAssign:
                case (AstStatementType) AstExpressionType.BitwiseOrAssign:
                case (AstStatementType) AstExpressionType.BitwiseXorAssign:
                case (AstStatementType) AstExpressionType.ShiftLeftAssign:
                case (AstStatementType) AstExpressionType.ShiftRightAssign:
                case (AstStatementType) AstExpressionType.LogAndAssign:
                case (AstStatementType) AstExpressionType.LogOrAssign:
                case (AstStatementType) AstExpressionType.Sequence:
                    WriteBinary((AstBinary) obj);
                    break;
                case (AstStatementType) AstExpressionType.Call:
                case (AstStatementType) AstExpressionType.LookUp:
                    WriteCall((AstCall) obj);
                    break;
                case (AstStatementType) AstExpressionType.String:
                    Write(obj.Source);
                    WriteGlobal(((AstString) obj).Value);
                    break;
                case (AstStatementType) AstExpressionType.Float:
                    Write(obj.Source);
                    Write(((AstFloat) obj).Value);
                    break;
                case (AstStatementType) AstExpressionType.Double:
                    Write(obj.Source);
                    Write(((AstDouble) obj).Value);
                    break;
                case (AstStatementType) AstExpressionType.Int:
                    var value = ((AstInt) obj).Value;
                    if (value < 0)
                    {
                        Write(obj.Source, true);
                        Write7BitEncodedInt(-value);
                    }
                    else
                    {
                        Write(obj.Source);
                        Write7BitEncodedInt(value);
                    }
                    break;
                case (AstStatementType) AstExpressionType.UInt:
                    Write(obj.Source);
                    Write(((AstUInt) obj).Value);
                    break;
                case (AstStatementType) AstExpressionType.Long:
                    Write(obj.Source);
                    Write(((AstLong) obj).Value);
                    break;
                case (AstStatementType) AstExpressionType.ULong:
                    Write(obj.Source);
                    Write(((AstULong) obj).Value);
                    break;
                case (AstStatementType) AstExpressionType.Char:
                    Write(obj.Source);
                    Write7BitEncodedInt(((AstChar) obj).Value);
                    break;
                case (AstStatementType) AstExpressionType.Defined:
                    WriteDefined((AstDefined) obj);
                    break;
                case (AstStatementType) AstExpressionType.Identifier:
                    WriteIdentifier((AstIdentifier) obj);
                    break;
                case (AstStatementType) AstExpressionType.Parameterizer:
                    WriteParameterizer((AstParameterizer) obj);
                    break;
                case (AstStatementType) AstExpressionType.ArrayInitializer:
                    WriteArrayInitializer((AstArrayInitializer) obj);
                    break;
                case (AstStatementType) AstExpressionType.FixedArrayInitializer:
                    WriteFixedArrayInitializer((AstFixedArrayInitializer) obj);
                    break;
                case (AstStatementType) AstExpressionType.Initializer:
                    WriteExpressions(((AstInitializer) obj).Expressions);
                    break;
                case (AstStatementType) AstExpressionType.Macro:
                    WriteMacro((AstMacro) obj);
                    break;
                case (AstStatementType) AstExpressionType.Local:
                    WriteLocal((AstLocal) obj);
                    break;
                case (AstStatementType) AstExpressionType.FixedArray:
                    WriteFixedArrayType((AstFixedArray) obj);
                    break;
                case (AstStatementType) AstExpressionType.Generic:
                    WriteGeneric((AstGeneric) obj);
                    break;
                case (AstStatementType) AstExpressionType.Ternary:
                    WriteTernary((AstTernary) obj);
                    break;
                case (AstStatementType) AstExpressionType.Cast:
                    WriteCast((AstCast) obj);
                    break;
                case (AstStatementType) AstExpressionType.Member:
                    WriteMember((AstMember) obj);
                    break;
                case (AstStatementType) AstExpressionType.New:
                    WriteNew((AstNew) obj);
                    break;
                case (AstStatementType) AstExpressionType.Prev:
                    WritePrev((AstPrev) obj);
                    break;
                case (AstStatementType) AstExpressionType.VertexAttribImplicit:
                    WriteVertexAttribImplicit((AstVertexAttribImplicit) obj);
                    break;
                case (AstStatementType) AstExpressionType.VertexAttribExplicit:
                    WriteVertexAttribExplicit((AstVertexAttribExplicit) obj);
                    break;
                case (AstStatementType) AstExpressionType.PixelSampler:
                    WritePixelSampler((AstPixelSampler) obj);
                    break;
                case (AstStatementType) AstExpressionType.Import:
                    WriteImport((AstImport) obj);
                    break;
                case (AstStatementType) AstExpressionType.BuiltinType:
                    WriteBuiltinType((AstBuiltinType) obj);
                    break;
                case (AstStatementType) AstExpressionType.Extern:
                    WriteExtern((AstExtern) obj);
                    break;
                case (AstStatementType) AstExpressionType.Lambda:
                    WriteLambda((AstLambda) obj);
                    break;
                case (AstStatementType) AstExpressionType.ParameterList:
                    WriteParameterList((AstParameterList) obj);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported object: " + type);
            }
        }

        public void WriteArgument(AstArgument a)
        {
            Write(a.OptionalName);
            Write((byte) a.Modifier);
            Write(a.Value);
        }

        public void WriteArguments(IReadOnlyList<AstArgument> l)
        {
            if (l == null)
            {
                Write7BitEncodedInt(0);
                return;
            }

            Write7BitEncodedInt(l.Count + 1);
            foreach (var e in l)
                WriteArgument(e);
        }

        public void WriteArrayInitializer(AstArrayInitializer a)
        {
            Write(a.Source);
            WriteExpressions(a.Values);
        }

        public void WriteBinary(AstBinary a)
        {
            Write(a.Left);
            Write(a.Source);
            Write(a.Right);
        }

        public void WriteCall(AstCall a)
        {
            Write(a.Base);
            WriteArguments(a.Arguments);
        }

        public void WriteCast(AstCast a)
        {
            Write(a.TargetType);
            Write(a.Argument);
        }

        public void WriteDefined(AstDefined a)
        {
            Write(a.Source);
            WriteGlobal(a.Condition);
        }

        public void WriteExpressions(IReadOnlyList<AstExpression> l)
        {
            if (l == null)
            {
                Write7BitEncodedInt(0);
                return;
            }

            Write7BitEncodedInt(l.Count + 1);
            foreach (var e in l)
                Write(e);
        }

        public void WriteExtern(AstExtern a)
        {
            Write(a.Source);
            WriteAttributes(a.Attributes);
            Write(a.OptionalType);
            WriteArguments(a.OptionalArguments);
            WriteGlobal(a.Value);
        }

        public void WriteFixedArrayType(AstFixedArray a)
        {
            Write(a.Source);
            Write(a.ElementType);
            Write(a.OptionalSize);
        }

        public void WriteFixedArrayInitializer(AstFixedArrayInitializer a)
        {
            Write(a.Source);
            Write(a.OptionalElementType);
            Write(a.OptionalSize);
            WriteExpressions(a.OptionalValues);
        }

        public void WriteGeneric(AstGeneric a)
        {
            Write(a.Base);
            Write7BitEncodedInt(a.ArgumentCount);
        }

        public void WriteIdentifier(AstIdentifier a)
        {
            Write(a.Source);
            WriteGlobal(a.Symbol);
        }

        public void WriteIdentifiers(IReadOnlyList<AstIdentifier> l)
        {
            if (l == null)
            {
                Write7BitEncodedInt(0);
                return;
            }

            Write7BitEncodedInt(l.Count + 1);
            foreach (var i in l)
                WriteIdentifier(i);
        }

        public void WriteImport(AstImport a)
        {
            Write(a.Source);
            Write(a.Importer);
            WriteArguments(a.Arguments);
        }

        public void WriteLambda(AstLambda a)
        {
            Write(a.Source);
            WriteParameterList(a.ParameterList);
            Write(a.Body);
        }

        public void WriteLocal(AstLocal a)
        {
            WriteIdentifier(a.Name);
        }

        public void WriteMacro(AstMacro a)
        {
            Write(a.Source);
            WriteGlobal(a.Value);
        }

        public void WriteMember(AstMember a)
        {
            Write(a.Base);
            WriteIdentifier(a.Name);
        }

        public void WriteNew(AstNew a)
        {
            Write(a.Source);
            Write(a.OptionalType);
            Write(a.OptionalArraySize);
            WriteArguments(a.OptionalArguments);
            WriteExpressions(a.OptionalCollectionInitializer);
        }

        public void WriteParameterizer(AstParameterizer a)
        {
            Write(a.Base);
            WriteExpressions(a.Arguments);
        }

        public void WriteParameterList(AstParameterList a)
        {
            Write(a.Source);
            WriteParameters(a.Parameters);
        }

        public void WritePixelSampler(AstPixelSampler a)
        {
            Write(a.Source);
            Write(a.Texture);
            Write(a.OptionalState);
        }

        public void WritePrev(AstPrev a)
        {
            Write(a.Source);
            Write7BitEncodedInt((int) a.Offset);
            Write(a.OptionalName);
        }

        public void WriteSymbol(AstSymbol a)
        {
            Write(a.Source);
        }

        public void WriteTernary(AstTernary a)
        {
            Write(a.Condition);
            Write(a.Source);
            Write(a.True);
            Write(a.False);
        }

        public void WriteBuiltinType(AstBuiltinType a)
        {
            Write(a.Source);
            Write((byte) a.BuiltinType);
        }

        public void WriteUnary(AstUnary a)
        {
            Write(a.Source);
            Write(a.Operand);
        }

        public void WriteVertexAttribExplicit(AstVertexAttribExplicit a)
        {
            Write(a.Source);
            Write(a.Type);
            WriteArguments(a.Arguments);
        }

        public void WriteVertexAttribImplicit(AstVertexAttribImplicit a)
        {
            Write(a.Source, a.Normalize);
            Write(a.VertexBuffer);
            Write(a.OptionalIndexBuffer);
        }

        public void WriteDraw(AstDraw a)
        {
            Write(a.Source);
            WriteBlockMembers(a.Block.Members);
        }

        public void WriteEmptyStatement(AstEmptyStatement a)
        {
            Write(a.Source);
        }

        public void WriteExternScope(AstExternScope a)
        {
            Write(a.Source);
            WriteAttributes(a.Attributes);
            WriteArguments(a.OptionalArguments);
            WriteGlobal(a.Body);
        }

        public void WriteFor(AstFor a)
        {
            Write(a.Source);
            Write(a.OptionalInitializer);
            Write(a.OptionalCondition);
            Write(a.OptionalIncrement);
            Write(a.OptionalBody);
        }

        public void WriteForeach(AstForeach a)
        {
            Write(a.Source);
            Write(a.ElementType);
            WriteIdentifier(a.ElementName);
            Write(a.Collection);
            Write(a.OptionalBody);
        }

        public void WriteIfElse(AstIfElse a)
        {
            Write(a.Source);
            Write(a.Condition);
            Write(a.OptionalIfBody);
            Write(a.OptionalElseBody);
        }

        public void WriteLock(AstLock a)
        {
            Write(a.Source);
            Write(a.Object);
            Write(a.OptionalBody);
        }

        public void WriteLoop(AstLoop a)
        {
            Write(a.Source);
            Write(a.Condition);
            Write(a.OptionalBody);
        }

        public void WriteModifiedStatement(AstModifiedStatement a)
        {
            Write(a.Source);
            Write(a.Statement);
        }

        public void WriteScope(AstScope a)
        {
            Write(a.Source);
            WriteStatements(a.Statements);
        }

        public void WriteStatements(IReadOnlyList<AstStatement> list)
        {
            Write7BitEncodedInt(list.Count);

            foreach (var e in list)
                Write(e);
        }

        public void WriteSwitch(AstSwitch a)
        {
            Write(a.Source);
            Write(a.Condition);
            Write7BitEncodedInt(a.Cases.Count);

            foreach (var c in a.Cases)
            {
                Write7BitEncodedInt(c.Values.Count);
                foreach (var e in c.Values)
                    Write(e);
                WriteScope(c.Scope);
            }
        }

        public void WriteTryCatchFinally(AstTryCatchFinally a)
        {
            Write(a.Source);
            WriteScope(a.TryScope);
            Write(a.OptionalFinallyScope);
            Write7BitEncodedInt(a.CatchBlocks.Count);

            foreach (var c in a.CatchBlocks)
            {
                Write(c.OptionalType);
                WriteIdentifier(c.Name);
                WriteScope(c.Body);
            }
        }

        public void WriteUsing(AstUsing a)
        {
            Write(a.Source);
            Write(a.Initializer);
            Write(a.OptionalBody);
        }

        public void WriteValueStatement(AstValueStatement a)
        {
            Write(a.Source);
            Write(a.Value);
        }

        public void WriteVariableDeclaration(AstVariableDeclaration a)
        {
            var flags = a.Variables.Count <<
                (int) AstVariableModifier.Shift |
                (int) (a.Modifier & AstVariableModifier.Mask);
            Write7BitEncodedInt(flags);
            Write(a.Type);

            foreach (var v in a.Variables)
            {
                WriteIdentifier(v.Name);
                Write(v.OptionalValue);
            }
        }

        public void WriteModifiers(Modifiers l, string cond)
        {
            Write((ushort) l);
            if (l.HasFlag(Modifiers.Extern))
                WriteGlobal(cond);
        }
    }
}
