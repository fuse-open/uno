using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;
using Uno.Compiler.API.Domain.AST.Statements;

namespace Uno.Compiler.Frontend.Analysis
{
    public enum TokenType
    {
        EndOfFile,
        Invalid,
        Whitespace,
        DocComment,  // /** ... */
        Identifier,

        // Literals
        DecimalLiteral,
        BinaryLiteral,
        OctalLiteral,
        HexadecimalLiteral,
        FloatLiteral,
        DoubleLiteral,

        // Built-in types
        [Token("object", BuiltinType.Object)] Object,
        [Token("bool", BuiltinType.Bool)] Bool,
        [Token("float", BuiltinType.Float)] Float,
        [Token("decimal")] Decimal, // Reserved: C# keyword
        [Token("double", BuiltinType.Double)] Double,
        [Token("float2", BuiltinType.Float2)] Float2,  // Uno specific
        [Token("float2x2", BuiltinType.Float2x2)] Float2x2, // Uno specific
        [Token("float3", BuiltinType.Float3)] Float3, // Uno specific
        [Token("float4", BuiltinType.Float4)] Float4, // Uno specific
        [Token("float3x3", BuiltinType.Float3x3)] Float3x3, // Uno specific
        [Token("float4x4", BuiltinType.Float4x4)] Float4x4, // Uno specific
        [Token("int2", BuiltinType.Int2)] Int2, // Uno specific
        [Token("int3", BuiltinType.Int3)] Int3, // Uno specific
        [Token("int4", BuiltinType.Int4)] Int4, // Uno specific
        [Token("byte2", BuiltinType.Byte2)] Byte2, // Uno specific
        [Token("byte4", BuiltinType.Byte4)] Byte4, // Uno specific
        [Token("sbyte2", BuiltinType.SByte2)] SByte2, // Uno specific
        [Token("sbyte4", BuiltinType.SByte4)] SByte4, // Uno specific
        [Token("short2", BuiltinType.Short2)] Short2, // Uno specific
        [Token("short4", BuiltinType.Short4)] Short4, // Uno specific
        [Token("ushort2", BuiltinType.UShort2)] UShort2, // Uno specific
        [Token("ushort4", BuiltinType.UShort4)] UShort4, // Uno specific
        [Token("char", BuiltinType.Char)] Char,
        [Token("string", BuiltinType.String)] String,
        [Token("byte", BuiltinType.Byte)] Byte,
        [Token("ushort", BuiltinType.UShort)] UShort,
        [Token("uint", BuiltinType.UInt)] UInt,
        [Token("ulong", BuiltinType.ULong)] ULong,
        [Token("sbyte", BuiltinType.SByte)] SByte,
        [Token("short", BuiltinType.Short)] Short,
        [Token("int", BuiltinType.Int)] Int,
        [Token("long", BuiltinType.Long)] Long,
        [Token("texture2D", BuiltinType.Texture2D)] Texture2D, // Uno specific
        [Token("textureCube", BuiltinType.TextureCube)] TextureCube, // Uno specific
        [Token("sampler2D", BuiltinType.Sampler2D)] Sampler2D, // Uno specific
        [Token("samplerCube", BuiltinType.SamplerCube)] SamplerCube, // Uno specific
        [Token("framebuffer", BuiltinType.Framebuffer)] Framebuffer, // Uno specific

        // Keywords
        [Token("abstract", Modifiers.Abstract)] Abstract,
        [Token("as", Precedence.Relational, AstBinaryType.As)] As,
        [Token("assert", 0, AstValueStatementType.Assert)] Assert, // Uno specific
        [Token("base", AstSymbolType.Base, AstConstructorCallType.Base)] Base,
        [Token("break", AstEmptyStatementType.Break)] Break,
        [Token("case")] Case,
        [Token("catch")] Catch,
        [Token("checked", AstUnaryType.Checked, AstStatementModifier.Checked)] Checked,
        [Token("class", AstClassType.Class, AstConstraintType.Class)] Class,
        [Token("const", ParameterModifier.Const)] Const,
        [Token("continue", AstEmptyStatementType.Continue)] Continue,
        [Token("debug_log", 0, AstValueStatementType.DebugLog)] DebugLog, // Uno specific
        [Token("default", AstUnaryType.Default)] Default,
        [Token("defined")] Defined, // Uno specific
        [Token("delegate")] Delegate,
        [Token("do")] Do,
        [Token("build_error", AstEmptyStatementType.BuildError, AstValueStatementType.BuildError)] BuildError, // Uno specific
        [Token("build_warning", AstEmptyStatementType.BuildWarning, AstValueStatementType.BuildWarning)] BuildWarning, // Uno specific
        [Token("else")] Else,
        [Token("enum")] Enum,
        [Token("event")] Event,
        [Token("explicit", Modifiers.Explicit)] Explicit,
        [Token("extern", Modifiers.Extern)] Extern,
        [Token("false", AstSymbolType.False)] False,
        [Token("finally")] Finally,
        [Token("fixed")] Fixed,
        [Token("for")] For,
        [Token("foreach")] Foreach,
        [Token("goto")] Goto, // Reserved: C# keyword
        [Token("if")] If,
        [Token("implicit", Modifiers.Implicit)] Implicit,
        [Token("import")] Import, // Uno specific
        [Token("in")] In,
        [Token("interface", AstClassType.Interface)] Interface,
        [Token("internal", Modifiers.Internal)] Internal,
        [Token("is", Precedence.Relational, AstBinaryType.Is)] Is,
        [Token("lock")] Lock,
        [Token("namespace")] Namespace,
        [Token("nameof", AstUnaryType.NameOf)] NameOf,
        [Token("new", Modifiers.New)] New,
        [Token("null", AstSymbolType.Null)] Null,
        [Token("operator")] Operator,
        [Token("out", ParameterModifier.Out)] Out,
        [Token("override", Modifiers.Override)] Override,
        [Token("params", ParameterModifier.Params)] Params,
        [Token("private", Modifiers.Private)] Private,
        [Token("protected", Modifiers.Protected)] Protected,
        [Token("public", Modifiers.Public)] Public,
        [Token("readonly")] ReadOnly,
        [Token("ref", ParameterModifier.Ref)] Ref,
        [Token("return", AstEmptyStatementType.Return, AstValueStatementType.Return)] Return,
        [Token("sealed", Modifiers.Sealed)] Sealed,
        [Token("sizeof", AstUnaryType.SizeOf)] SizeOf,
        [Token("stackalloc")] StackAlloc, // Reserved: C# keyword
        [Token("static", Modifiers.Static)] Static,
        [Token("struct", AstClassType.Struct, AstConstraintType.Struct)] Struct,
        [Token("switch")] Switch,
        [Token("this", AstSymbolType.This, AstConstructorCallType.This, ParameterModifier.This)] This,
        [Token("throw", AstEmptyStatementType.Throw, AstValueStatementType.Throw)] Throw,
        [Token("true", AstSymbolType.True)] True,
        [Token("try")] Try,
        [Token("typeof", AstUnaryType.TypeOf)] TypeOf,
        [Token("unchecked", AstUnaryType.Unchecked, AstStatementModifier.Unchecked)] Unchecked,
        [Token("unsafe", AstUnaryType.Unsafe, AstStatementModifier.Unsafe)] Unsafe,
        [Token("using")] Using,
        [Token("virtual", Modifiers.Virtual)] Virtual,
        [Token("void", AstSymbolType.Void)] Void,
        [Token("volatile")] Volatile,
        [Token("while")] While,

        // Punctuation
        [Token(";")] Semicolon,
        [Token(".", Precedence.Primary, (AstBinaryType) 0xFF)] Period,
        [Token(",")] Comma,
        [Token(":")] Colon,
        [Token("?", Precedence.Conditional)] QuestionMark,
        [Token("::")] DoubleColon,
        [Token("??", Precedence.NullCoalescing, AstBinaryType.Null)] DoubleQuestionMark,

        // Braces
        [Token("{")] LeftCurlyBrace,
        [Token("}")] RightCurlyBrace,
        [Token("(")] LeftParen,
        [Token(")")] RightParen,
        [Token("[")] LeftSquareBrace,
        [Token("]")] RightSquareBrace,

        // Unary operators
        [Token("!", OperatorType.LogicalNot, AstUnaryType.LogNot)] ExclamationMark,
        [Token("~", OperatorType.OnesComplement, AstUnaryType.BitwiseNot)] Tilde,
        [Token("++", OperatorType.Increase, AstUnaryType.IncreasePrefix, AstUnaryType.IncreasePostfix)] Increase,
        [Token("--", OperatorType.Decrease, AstUnaryType.DecreasePrefix, AstUnaryType.DecreasePostfix)] Decrease,

        // Binary operators
        [Token("+=", Precedence.Assignment, AstBinaryType.AddAssign, Associativity.RightToLeft)] AddAssign,
        [Token("-=", Precedence.Assignment, AstBinaryType.SubAssign, Associativity.RightToLeft)] MinusAssign,
        [Token("*=", Precedence.Assignment, AstBinaryType.MulAssign, Associativity.RightToLeft)] MulAssign,
        [Token("/=", Precedence.Assignment, AstBinaryType.DivAssign, Associativity.RightToLeft)] DivAssign,
        [Token("%=", Precedence.Assignment, AstBinaryType.ModAssign, Associativity.RightToLeft)] ModAssign,
        [Token("&=", Precedence.Assignment, AstBinaryType.BitwiseAndAssign, Associativity.RightToLeft)] BitwiseAndAssign,
        [Token("|=", Precedence.Assignment, AstBinaryType.BitwiseOrAssign, Associativity.RightToLeft)] BitwiseOrAssign,
        [Token("^=", Precedence.Assignment, AstBinaryType.BitwiseXorAssign, Associativity.RightToLeft)] BitwiseXorAssign,
        [Token("<<=", Precedence.Assignment, AstBinaryType.ShiftLeftAssign, Associativity.RightToLeft)] ShlAssign,
        [Token(">>=", Precedence.Assignment, AstBinaryType.ShiftRightAssign, Associativity.RightToLeft)] ShrAssign,
        [Token("&&=", Precedence.Assignment, AstBinaryType.LogAndAssign, Associativity.RightToLeft)] LogAndAssign,
        [Token("||=", Precedence.Assignment, AstBinaryType.LogOrAssign, Associativity.RightToLeft)] LogOrAssign,
        [Token("&&", Precedence.ConditionalAnd, AstBinaryType.LogAnd)] LogAnd,
        [Token("||", Precedence.ConditionalOr, AstBinaryType.LogOr)] LogOr,
        [Token("==", Precedence.Equality, OperatorType.Equality, AstBinaryType.Equal)] Equal,
        [Token("!=", Precedence.Equality, OperatorType.Inequality, AstBinaryType.NotEqual)] NotEqual,
        [Token(">=", Precedence.Relational, OperatorType.GreaterThanOrEqual, AstBinaryType.GreaterThanOrEqual)] GreaterOrEqual,
        [Token("<=", Precedence.Relational, OperatorType.LessThanOrEqual, AstBinaryType.LessThanOrEqual)] LessOrEqual,
        [Token("=>", Precedence.Assignment, 0, Associativity.RightToLeft)] Lambda,
        [Token("<<", Precedence.Shift, OperatorType.LeftShift, AstBinaryType.ShiftLeft)] Shl,
        [Token(">>", Precedence.Shift, OperatorType.RightShift, AstBinaryType.ShiftRight)] Shr,
        [Token("=", Precedence.Assignment, AstBinaryType.Assign, Associativity.RightToLeft)] Assign,
        [Token("+", Precedence.Additive, OperatorType.UnaryPlus, OperatorType.Addition, AstUnaryType.Promote, AstBinaryType.Add)] Plus,
        [Token("-", Precedence.Additive, OperatorType.UnaryNegation, OperatorType.Subtraction, AstUnaryType.Negate, AstBinaryType.Sub)] Minus,
        [Token("*", Precedence.Multiplicative, OperatorType.Multiply, AstBinaryType.Mul)] Mul,
        [Token("/", Precedence.Multiplicative, OperatorType.Division, AstBinaryType.Div)] Div,
        [Token("%", Precedence.Multiplicative, OperatorType.Modulus, AstBinaryType.Mod)] Mod,
        [Token("<", Precedence.Relational, OperatorType.LessThan, AstBinaryType.LessThan)] LessThan,
        [Token(">", Precedence.Relational, OperatorType.GreaterThan, AstBinaryType.GreaterThan)] GreaterThan,
        [Token("^", Precedence.LogicalXor, OperatorType.ExclusiveOr, AstBinaryType.BitwiseXor)] BitwiseXor,
        [Token("|", Precedence.LogicalOr, OperatorType.BitwiseOr, AstBinaryType.BitwiseOr)] BitwiseOr,
        [Token("&", Precedence.LogicalAnd, OperatorType.BitwiseAnd, AstBinaryType.BitwiseAnd)] BitwiseAnd,

        // Others
        [Token("#")] PreprocessorDirective,
        [Token("'")] CharLiteral,
        [Token("\"")] StringLiteral,
        [Token("$\"")] DollarString,
        [Token("@\"")] AlphaString,
        [Token("@{")] AlphaBlock,         // @{...@}
        [Token("@(")] AlphaExpression,    // @(...)
        [Token("//")] SingleLineComment,
        [Token("/*")] MultiLineComment,

        Max
    }
}
