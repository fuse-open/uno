using System;
using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;
using Uno.Compiler.API.Domain.AST.Statements;
using Uno.Compiler.API.Domain.Serialization;

namespace Uno.Compiler.API.Domain.AST
{
    public sealed class AstReader : CacheReader
    {
        public static AstReader Open(SourcePackage upk, string filename)
        {
            var r = new AstReader(upk, filename);
            r.VerifyMagic(AstSerialization.Magic);
            return r;
        }

        public AstReader(SourcePackage upk, string filename)
            : base(upk, filename)
        {
        }

        public AstBlock ReadBlock()
        {
            string cond;
            return new AstBlock(
                ReadGlobalString(), 
                ReadAttributes(), 
                ReadModifiers(out cond), cond, 
                ReadIdentifier(), 
                ReadExpressions(), 
                ReadBlockMembers());
        }

        public AstClass ReadClass()
        {
            string cond;
            return new AstClass(
                ReadGlobalString(), 
                ReadAttributes(), 
                ReadModifiers(out cond), cond, 
                (AstClassType) ReadByte(), 
                ReadIdentifier(), 
                ReadExpressions(), 
                ReadGenericSignature(), 
                ReadBlockMembers(), 
                ReadExpressions());
        }

        public AstConstraint[] ReadConstraints()
        {
            var len = Read7BitEncodedInt();
            var result = new AstConstraint[len];

            for (int i = 0; i < len; i++)
                result[i] = new AstConstraint(
                    ReadIdentifier(), 
                    (AstConstraintType) ReadByte(), 
                    ReadExpressions(), 
                    ReadSource());

            return result;
        }

        public AstDelegate ReadDelegate()
        {
            string cond;
            return new AstDelegate(
                ReadGlobalString(), 
                ReadAttributes(), 
                ReadModifiers(out cond), cond, 
                ReadExpression(),
                ReadIdentifier(),
                ReadParameters(), 
                ReadGenericSignature());
        }

        public AstEnum ReadEnum()
        {
            string cond;
            return new AstEnum(
                ReadGlobalString(), 
                ReadAttributes(), 
                ReadModifiers(out cond), cond, 
                ReadIdentifier(), 
                ReadExpression(), 
                ReadLiterals());
        }

        AstLiteral[] ReadLiterals()
        {
            var len = Read7BitEncodedInt();
            var literals = new AstLiteral[len];
            for (var i = 0; i < len; i++)
                literals[i] = new AstLiteral(
                    ReadGlobalString(), 
                    ReadAttributes(), 
                    ReadIdentifier(), 
                    ReadExpression());
            return literals;
        }

        public AstGenericSignature ReadGenericSignature()
        {
            var parameters = ReadIdentifiers();
            return parameters == null
                ? null
                : new AstGenericSignature(parameters, 
                    ReadConstraints());
        }

        public AstDocument ReadDocument()
        {
            var src = ReadSource();
            src.File.AddPart(src.Line);
            var result = new AstDocument(src);
            ReadNamespace(result);
            return result;
        }

        public AstNamespace ReadNamespace()
        {
            var result = new AstNamespace(
                ReadIdentifier());
            ReadNamespace(result);
            return result;
        }

        void ReadNamespace(AstNamespace result)
        {
            var uc = Read7BitEncodedInt();
            while (uc-- > 0)
                result.Usings.Add(ReadUsingDirective());

            var nc = Read7BitEncodedInt();
            while (nc-- > 0)
                result.Namespaces.Add(ReadNamespace());

            var bc = Read7BitEncodedInt();
            while (bc-- > 0)
                result.Blocks.Add((AstBlockBase) ReadBlockMember());
        }

        public AstUsingDirective ReadUsingDirective()
        {
            return new AstUsingDirective(
                ReadExpression(), 
                (AstIdentifier) ReadExpression(), 
                ReadBoolean());
        }

        public AstNode ReadNode()
        {
            return new AstNode(
                (AstNodeType) ReadByte(), 
                ReadIdentifier(), 
                ReadBlockMembers());
        }

        public Modifiers ReadModifiers(out string cond)
        {
            var retval = (Modifiers) ReadUInt16();
            cond = retval.HasFlag(Modifiers.Extern)
                ? ReadGlobalString()
                : null;
            return retval;
        }

        public AstAccessor ReadAccessor()
        {
            string cond;
            var src = ReadSource();
            return src == null
                ? null
                : new AstAccessor(src, 
                    ReadModifiers(out cond), cond, 
                    (AstScope) ReadStatement());
        }

        public AstApply ReadApply()
        {
            return new AstApply(
                (ApplyModifier) ReadByte(), 
                ReadExpression());
        }

        public AstAttribute ReadAttribute()
        {
            string cond = null;
            var modifier = (AstAttributeModifier) ReadByte();
            if (modifier.HasFlag(AstAttributeModifier.HasCondition))
            {
                modifier &= ~AstAttributeModifier.HasCondition;
                cond = ReadGlobalString();
            }

            return new AstAttribute(
                cond, modifier, 
                ReadExpression(), 
                ReadArguments());
        }

        public IReadOnlyList<AstAttribute> ReadAttributes()
        {
            var len = Read7BitEncodedInt();
            var attrs = new AstAttribute[len];
            for (int i = 0; i < len; i++)
                attrs[i] = ReadAttribute();
            return attrs;
        }

        public AstBlockMember ReadBlockMember()
        {
            var type = (AstMemberType) ReadByte();
            switch (type)
            {
                case AstMemberType.Field:
                    return ReadField();
                case AstMemberType.Constructor:
                    return ReadConstructor();
                case AstMemberType.Finalizer:
                    return ReadFinalizer();
                case AstMemberType.Method:
                    return ReadMethod();
                case AstMemberType.Property:
                    return ReadProperty();
                case AstMemberType.Class:
                    return ReadClass();
                case AstMemberType.Indexer:
                    return ReadIndexer();
                case AstMemberType.Operator:
                    return ReadOperator();
                case AstMemberType.Converter:
                    return ReadConverter();
                case AstMemberType.Enum:
                    return ReadEnum();
                case AstMemberType.Delegate:
                    return ReadDelegate();
                case AstMemberType.Event:
                    return ReadEvent();
                case AstMemberType.MetaProperty:
                    return ReadMetaProperty();
                case AstMemberType.ApplyStatement:
                    return ReadApply();
                case AstMemberType.Block:
                    return ReadBlock();
                case AstMemberType.NodeBlock:
                    return ReadNode();
                default:
                    throw new InvalidOperationException("Invalid block member: " + type);
            }
        }

        public AstBlockMember[] ReadBlockMembers()
        {
            var len = Read7BitEncodedInt() - 1;
            if (len == -1)
                return null;

            var result = new AstBlockMember[len];
            for (int i = 0; i < len; i++)
                result[i] = ReadBlockMember();
            return result;
        }

        public AstConstructor ReadConstructor()
        {
            string cond;
            return new AstConstructor(
                ReadSource(), 
                ReadGlobalString(), 
                ReadAttributes(), 
                ReadModifiers(out cond), cond, 
                ReadParameters(), 
                (AstConstructorCallType) ReadByte(), 
                ReadArguments(), 
                (AstScope) ReadStatement());
        }

        public AstConverter ReadConverter()
        {
            string cond;
            return new AstConverter(
                ReadGlobalString(), 
                ReadAttributes(), 
                ReadModifiers(out cond), cond, 
                ReadExpression(), 
                ReadParameters(), 
                (AstScope) ReadStatement());
        }

        public AstEvent ReadEvent()
        {
            string cond;
            return new AstEvent(
                ReadGlobalString(), 
                ReadAttributes(), 
                ReadModifiers(out cond), cond, 
                ReadExpression(), 
                ReadExpression(), 
                ReadIdentifier(), 
                ReadAccessor(), 
                ReadAccessor());
        }

        public AstField ReadField()
        {
            string cond;
            return new AstField(
                ReadGlobalString(), 
                ReadAttributes(), 
                ReadModifiers(out cond), cond, 
                (FieldModifiers) ReadByte(), 
                ReadExpression(), 
                ReadIdentifier(), 
                ReadExpression());
        }

        public AstFinalizer ReadFinalizer()
        {
            string cond;
            return new AstFinalizer(
                ReadSource(), 
                ReadGlobalString(), 
                ReadAttributes(), 
                ReadModifiers(out cond), cond, 
                ReadParameters(), 
                (AstScope) ReadStatement());
        }

        public AstIndexer ReadIndexer()
        {
            string cond;
            return new AstIndexer(
                ReadSource(), 
                ReadGlobalString(), 
                ReadAttributes(), 
                ReadModifiers(out cond), cond, 
                ReadExpression(), 
                ReadExpression(), 
                ReadParameters(), 
                ReadAccessor(), 
                ReadAccessor());
        }

        public AstMetaProperty ReadMetaProperty()
        {
            return new AstMetaProperty(
                (MetaVisibility) ReadByte(), 
                ReadExpression(), 
                ReadIdentifier(), 
                ReadMetaPropertyDefinitions());
        }

        public AstMetaPropertyDefinition[] ReadMetaPropertyDefinitions()
        {
            var len = Read7BitEncodedInt();
            var result = new AstMetaPropertyDefinition[len];
            for (var i = 0; i < len; i++)
                result[i] = new AstMetaPropertyDefinition(
                    ReadStatement(), 
                    ReadGlobalStrings(), 
                    ReadReqStatements());
            return result;
        }

        public AstMethod ReadMethod()
        {
            string cond;
            return new AstMethod(
                ReadGlobalString(), 
                ReadAttributes(), 
                ReadModifiers(out cond), cond, 
                ReadExpression(), 
                ReadExpression(), 
                ReadIdentifier(), 
                ReadParameters(), 
                ReadGenericSignature(), 
                (AstScope) ReadStatement());
        }

        public AstOperator ReadOperator()
        {
            string cond;
            return new AstOperator(
                ReadSource(), 
                ReadGlobalString(), 
                ReadAttributes(), 
                ReadModifiers(out cond), cond, 
                ReadExpression(), 
                (OperatorType) ReadByte(), 
                ReadParameters(), 
                (AstScope) ReadStatement());
        }

        public AstParameter ReadParameter()
        {
            return new AstParameter(
                ReadAttributes(),
                (ParameterModifier) ReadByte(), 
                ReadExpression(), 
                ReadIdentifier(), 
                ReadExpression());
        }

        public AstParameter[] ReadParameters()
        {
            int len = Read7BitEncodedInt() - 1;
            if (len == -1)
                return null;
            var result = new AstParameter[len];
            for (int i = 0; i < len; i++)
                result[i] = ReadParameter();
            return result;
        }

        public AstProperty ReadProperty()
        {
            string cond;
            return new AstProperty(
                ReadGlobalString(), 
                ReadAttributes(), 
                ReadModifiers(out cond), cond, 
                ReadExpression(), 
                ReadExpression(), 
                ReadIdentifier(), 
                ReadAccessor(), 
                ReadAccessor());
        }

        public AstReqStatement[] ReadReqStatements()
        {
            var len = Read7BitEncodedInt();
            var result = new AstReqStatement[len];
            for (int i = 0; i < len; i++)
                result[i] = new AstReqStatement(
                    ReadSource(), 
                    (uint) Read7BitEncodedInt(), 
                    ReadIdentifier(), 
                    ReadExpression(), 
                    ReadGlobalString());
            return result;
        }

        public AstExpression ReadExpression()
        {
            return (AstExpression) ReadStatement();
        }

        public AstStatement ReadStatement()
        {
            var type = (AstStatementType) ReadByte();
            switch (type)
            {
                case AstStatementType.Undef:
                    return null;
                case AstStatementType.AutoRelease:
                case AstStatementType.Checked:
                case AstStatementType.Unchecked:
                case AstStatementType.Unsafe:
                    return ReadModifiedStatement(type);
                case AstStatementType.While:
                case AstStatementType.DoWhile:
                    return ReadLoop(type);    
                case AstStatementType.Break:
                case AstStatementType.BuildError:
                case AstStatementType.BuildWarning:
                case AstStatementType.Continue:
                case AstStatementType.DrawDispose:
                case AstStatementType.Return:
                case AstStatementType.Throw:
                case AstStatementType.YieldBreak:
                    return ReadEmptyStatement(type);
                case AstStatementType.Assert:
                case AstStatementType.BuildErrorMessage:
                case AstStatementType.BuildWarningMessage:
                case AstStatementType.DebugLog:
                case AstStatementType.ReturnValue:
                case AstStatementType.ThrowValue:
                case AstStatementType.YieldReturnValue:
                    return ReadValueStatement(type);
                case AstStatementType.VariableDeclaration:
                    return ReadVariableDeclaration();
                case AstStatementType.FixedArrayDeclaration:
                    return new AstFixedArrayDeclaration(
                        (AstFixedArray) ReadExpression(), 
                        ReadIdentifier(), 
                        ReadExpression());
                case AstStatementType.Scope:
                    return ReadScope();
                case AstStatementType.ExternScope:
                    return ReadExternScope();
                case AstStatementType.IfElse:
                    return ReadIfElse();
                case AstStatementType.For:
                    return ReadFor();
                case AstStatementType.Foreach:
                    return ReadForeach();
                case AstStatementType.Switch:
                    return ReadSwitch();
                case AstStatementType.TryCatchFinally:
                    return ReadTryCatchFinally();
                case AstStatementType.Lock:
                    return ReadLock();
                case AstStatementType.Using:
                    return ReadUsing();
                case AstStatementType.Draw:
                    return ReadDraw();
                case (AstStatementType) AstExpressionType.Var:
                case (AstStatementType) AstExpressionType.Null:
                case (AstStatementType) AstExpressionType.Global:
                case (AstStatementType) AstExpressionType.Void:
                case (AstStatementType) AstExpressionType.This:
                case (AstStatementType) AstExpressionType.Base:
                case (AstStatementType) AstExpressionType.True:
                case (AstStatementType) AstExpressionType.False:
                case (AstStatementType) AstExpressionType.Zero:
                    return ReadSymbol((AstSymbolType) type);
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
                    return ReadUnary((AstUnaryType) type);
                case (AstStatementType) AstExpressionType.As:
                case (AstStatementType) AstExpressionType.Is:
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
                    return ReadBinary((AstBinaryType) type);
                case (AstStatementType) AstExpressionType.Call:
                case (AstStatementType) AstExpressionType.LookUp:
                    return ReadCall((AstCallType) type);
                case (AstStatementType) AstExpressionType.String:
                    return new AstString(ReadSource(), ReadGlobalString());
                case (AstStatementType) AstExpressionType.Float:
                    return new AstFloat(ReadSource(), ReadSingle());
                case (AstStatementType) AstExpressionType.Double:
                    return new AstDouble(ReadSource(), ReadDouble());
                case (AstStatementType) AstExpressionType.Int:
                    bool flag;
                    return new AstInt(ReadSource(out flag),
                            flag
                                ? -Read7BitEncodedInt()
                                :  Read7BitEncodedInt());
                case (AstStatementType) AstExpressionType.UInt:
                    return new AstUInt(ReadSource(), ReadUInt32());
                case (AstStatementType) AstExpressionType.Long:
                    return new AstLong(ReadSource(), ReadInt64());
                case (AstStatementType) AstExpressionType.ULong:
                    return new AstULong(ReadSource(), ReadUInt64());
                case (AstStatementType) AstExpressionType.Char:
                    return new AstChar(ReadSource(), (char) Read7BitEncodedInt());
                case (AstStatementType) AstExpressionType.Defined:
                    return ReadDefined();
                case (AstStatementType) AstExpressionType.Identifier:
                    return ReadIdentifier();
                case (AstStatementType) AstExpressionType.Parameterizer:
                    return ReadParameterizer();
                case (AstStatementType) AstExpressionType.ArrayInitializer:
                    return ReadArrayInitializer();
                case (AstStatementType) AstExpressionType.FixedArrayInitializer:
                    return ReadFixedArrayInitializer();
                case (AstStatementType) AstExpressionType.Initializer:
                    return new AstInitializer(ReadExpressions());
                case (AstStatementType) AstExpressionType.Macro:
                    return ReadMacro();
                case (AstStatementType) AstExpressionType.Local:
                    return ReadLocal();
                case (AstStatementType) AstExpressionType.FixedArray:
                    return ReadFixedArray();
                case (AstStatementType) AstExpressionType.Generic:
                    return ReadGeneric();
                case (AstStatementType) AstExpressionType.Ternary:
                    return ReadTernary();
                case (AstStatementType) AstExpressionType.Cast:
                    return ReadCast();
                case (AstStatementType) AstExpressionType.Member:
                    return ReadMember();
                case (AstStatementType) AstExpressionType.New:
                    return ReadNew();
                case (AstStatementType) AstExpressionType.Prev:
                    return ReadPrev();
                case (AstStatementType) AstExpressionType.VertexAttribImplicit:
                    return ReadVertexAttribImplicit();
                case (AstStatementType) AstExpressionType.VertexAttribExplicit:
                    return ReadVertexAttribExplicit();
                case (AstStatementType) AstExpressionType.PixelSampler:
                    return ReadPixelSampler();
                case (AstStatementType) AstExpressionType.Import:
                    return ReadImport();
                case (AstStatementType) AstExpressionType.BuiltinType:
                    return ReadBuiltinType();
                case (AstStatementType) AstExpressionType.Extern:
                    return ReadExtern();
                case (AstStatementType) AstExpressionType.Lambda:
                    return ReadLambda();
                case (AstStatementType) AstExpressionType.ParameterList:
                    return ReadParameterList();
                default:
                    throw new InvalidOperationException("Invalid statement: " + type);
            }
        }

        public AstCatch ReadCatch()
        {
            return new AstCatch(
                ReadExpression(), 
                ReadIdentifier(), 
                ReadScope());
        }

        public AstDraw ReadDraw()
        {
            return new AstDraw(
                ReadSource(), 
                ReadBlockMembers());
        }

        public AstEmptyStatement ReadEmptyStatement(AstStatementType type)
        {
            return new AstEmptyStatement(
                ReadSource(), (AstEmptyStatementType) type);
        }

        public AstExternScope ReadExternScope()
        {
            return new AstExternScope(
                ReadSource(),
                ReadAttributes(),
                ReadArguments(),
                ReadGlobalValue());
        }

        public AstFor ReadFor()
        {
            return new AstFor(
                ReadSource(), 
                ReadStatement(),
                ReadExpression(), 
                ReadExpression(), 
                ReadStatement());
        }

        public AstForeach ReadForeach()
        {
            return new AstForeach(
                ReadSource(), 
                ReadExpression(), 
                ReadIdentifier(), 
                ReadExpression(), 
                ReadStatement());
        }

        public AstIfElse ReadIfElse()
        {
            return new AstIfElse(
                ReadSource(), 
                ReadExpression(), 
                ReadStatement(), 
                ReadStatement());
        }

        public AstLock ReadLock()
        {
            return new AstLock(
                ReadSource(), 
                ReadExpression(), 
                ReadStatement());
        }

        public AstLoop ReadLoop(AstStatementType type)
        {
            return new AstLoop(
                ReadSource(), 
                (AstLoopType) type, 
                ReadExpression(), 
                ReadStatement());
        }

        public AstModifiedStatement ReadModifiedStatement(AstStatementType type)
        {
            return new AstModifiedStatement(
                ReadSource(), (AstStatementModifier) type, 
                ReadStatement());
        }

        public AstScope ReadScope()
        {
            return new AstScope(
                ReadSource(), 
                ReadStatements());
        }

        public AstStatement[] ReadStatements()
        {
            var len = Read7BitEncodedInt();
            var result = new AstStatement[len];

            for (var i = 0; i < len; i++)
                result[i] = ReadStatement();

            return result;
        }

        public AstSwitch ReadSwitch()
        {
            return new AstSwitch(
                ReadSource(), 
                ReadExpression(), 
                ReadSwitchCases());
        }

        AstSwitchCase[] ReadSwitchCases()
        {
            var caseCount = Read7BitEncodedInt();
            var cases = new AstSwitchCase[caseCount];

            for (var i = 0; i < caseCount; i++)
            {
                var valueCount = Read7BitEncodedInt();
                var values = new AstExpression[valueCount];

                for (var j = 0; j < valueCount; j++)
                    values[j] = ReadExpression();

                cases[i] = new AstSwitchCase(values, ReadScope());
            }

            return cases;
        }

        public AstTryCatchFinally ReadTryCatchFinally()
        {
            return new AstTryCatchFinally(
                ReadSource(), 
                ReadScope(), 
                (AstScope) ReadStatement(), 
                ReadCatchBlocks());
        }

        AstCatch[] ReadCatchBlocks()
        {
            var len = Read7BitEncodedInt();
            var result = new AstCatch[len];
            for (var i = 0; i < len; i++)
                result[i] = ReadCatch();
            return result;
        }

        public AstUsing ReadUsing()
        {
            return new AstUsing(
                ReadSource(), 
                ReadStatement(),
                ReadStatement());
        }

        public AstValueStatement ReadValueStatement(AstStatementType type)
        {
            return new AstValueStatement(
                ReadSource(), 
                (AstValueStatementType) type, 
                ReadExpression());
        }

        public AstVariableDeclaration ReadVariableDeclaration()
        {
            var flags = Read7BitEncodedInt();
            var type = ReadExpression();
            var vars = new AstVariable[flags >> (int) AstVariableModifier.Shift];
            for (var i = 0; i < vars.Length; i++)
                vars[i] = new AstVariable(
                    ReadIdentifier(),
                    ReadExpression());
            return new AstVariableDeclaration(
                (AstVariableModifier) flags & AstVariableModifier.Mask, 
                type, vars);
        }

        public AstArgument[] ReadArguments()
        {
            var len = Read7BitEncodedInt() - 1;
            if (len == -1)
                return null;

            var result = new AstArgument[len];
            for (int i = 0; i < len; i++)
                result[i] = new AstArgument(
                    (AstIdentifier) ReadExpression(), 
                    (ParameterModifier) ReadByte(), 
                    ReadExpression());
            return result;
        }

        public AstArrayInitializer ReadArrayInitializer()
        {
            return new AstArrayInitializer(
                ReadSource(), 
                ReadExpressions());
        }

        public AstBinary ReadBinary(AstBinaryType type)
        {
            return new AstBinary(type,
                ReadExpression(),
                ReadSource(),
                ReadExpression());
        }

        public AstCall ReadCall(AstCallType type)
        {
            return new AstCall(type, 
                ReadExpression(),
                ReadArguments());
        }

        public AstCast ReadCast()
        {
            return new AstCast(
                ReadExpression(), 
                ReadExpression());
        }

        public AstDefined ReadDefined()
        {
            return new AstDefined(
                ReadSource(), 
                ReadGlobalString());
        }

        public AstExpression[] ReadExpressions()
        {
            int len = Read7BitEncodedInt() - 1;
            if (len == -1)
                return null;

            var result = new AstExpression[len];
            for (int i = 0; i < len; i++)
                result[i] = ReadExpression();
            return result;
        }

        public AstExtern ReadExtern()
        {
            return new AstExtern(
                ReadSource(), 
                ReadAttributes(),
                ReadExpression(), 
                ReadArguments(),
                ReadGlobalValue());
        }

        public AstFixedArray ReadFixedArray()
        {
            return new AstFixedArray(
                ReadSource(), 
                ReadExpression(), 
                ReadExpression());
        }

        public AstFixedArrayInitializer ReadFixedArrayInitializer()
        {
            return new AstFixedArrayInitializer(
                ReadSource(), 
                ReadExpression(), 
                ReadExpression(), 
                ReadExpressions());
        }

        public AstGeneric ReadGeneric()
        {
            return new AstGeneric(
                ReadExpression(), 
                Read7BitEncodedInt());
        }

        public AstIdentifier ReadIdentifier()
        {
            return new AstIdentifier(
                ReadSource(), 
                ReadGlobalString());
        }

        public AstIdentifier[] ReadIdentifiers()
        {
            int len = Read7BitEncodedInt() - 1;
            if (len == -1)
                return null;
            var result = new AstIdentifier[len];
            for (int i = 0; i < len; i++)
                result[i] = ReadIdentifier();
            return result;
        }

        public AstImport ReadImport()
        {
            return new AstImport(
                ReadSource(), 
                ReadExpression(), 
                ReadArguments());
        }

        public AstLambda ReadLambda()
        {
            return new AstLambda(
                ReadSource(),
                ReadParameterList(),
                ReadStatement());
        }

        public AstLocal ReadLocal()
        {
            return new AstLocal(
                ReadIdentifier());
        }

        public AstMacro ReadMacro()
        {
            return new AstMacro(
                ReadSource(), 
                ReadGlobalString());
        }

        public AstMember ReadMember()
        {
            return new AstMember(
                ReadExpression(), 
                ReadIdentifier());
        }

        public AstNew ReadNew()
        {
            return new AstNew(
                ReadSource(), 
                ReadExpression(), 
                ReadExpression(), 
                ReadArguments(), 
                ReadExpressions());
        }

        public AstParameterizer ReadParameterizer()
        {
            return new AstParameterizer(
                ReadExpression(), 
                ReadExpressions());
        }

        public AstParameterList ReadParameterList()
        {
            return new AstParameterList(
                ReadSource(), 
                ReadParameters());
        }

        public AstPixelSampler ReadPixelSampler()
        {
            return new AstPixelSampler(
                ReadSource(), 
                ReadExpression(), 
                ReadExpression());
        }

        public AstPrev ReadPrev()
        {
            return new AstPrev(
                ReadSource(), 
                (uint) Read7BitEncodedInt(), 
                (AstIdentifier) ReadExpression());
        }

        public AstSymbol ReadSymbol(AstSymbolType type)
        {
            return new AstSymbol(
                ReadSource(), type);
        }

        public AstTernary ReadTernary()
        {
            return new AstTernary(
                ReadExpression(),
                ReadSource(),
                ReadExpression(), 
                ReadExpression());
        }

        public AstBuiltinType ReadBuiltinType()
        {
            return new AstBuiltinType(
                ReadSource(), 
                (BuiltinType) ReadByte());
        }

        public AstUnary ReadUnary(AstUnaryType type)
        {
            return new AstUnary(
                ReadSource(), type, 
                ReadExpression());
        }

        public AstVertexAttribExplicit ReadVertexAttribExplicit()
        {
            return new AstVertexAttribExplicit(
                ReadSource(),
                ReadExpression(),
                ReadArguments());
        }

        public AstVertexAttribImplicit ReadVertexAttribImplicit()
        {
            bool flag;
            return new AstVertexAttribImplicit(
                ReadSource(out flag),
                ReadExpression(), 
                ReadExpression(), 
                flag);
        }
    }
}
