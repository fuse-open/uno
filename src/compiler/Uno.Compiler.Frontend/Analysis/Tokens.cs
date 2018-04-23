using System.Collections.Generic;
using System.Reflection;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;
using Uno.Compiler.API.Domain.AST.Statements;
using ParameterModifier = Uno.Compiler.API.Domain.ParameterModifier;

namespace Uno.Compiler.Frontend.Analysis
{
    public static class Tokens
    {
        // Contextual keywords
        public const string Add = "add";
        public const string Apply = "apply";
        public const string Block = "block";
        public const string Draw = "draw";
        public const string DrawDispose = "draw_dispose";
        public const string Drawable = "drawable";
        public const string Get = "get";
        public const string Global = "global";
        public const string Immutable = "immutable";
        public const string Interpolate = "interpolate";
        public const string Intrinsic = "intrinsic";
        public const string Local = "local";
        public const string Meta = "meta";
        public const string Norm = "norm";
        public const string Partial = "partial";
        public const string Pixel = "pixel";
        public const string PixelSampler = "pixel_sampler";
        public const string Prev = "prev";
        public const string Remove = "remove";
        public const string Req = "req";
        public const string Sample = "sample";
        public const string Set = "set";
        public const string Swizzler = "swizzler";
        public const string Tag = "tag";
        public const string Undefined = "undefined";
        public const string Var = "var";
        public const string Vertex = "vertex";
        public const string VertexAttrib = "vertex_attrib";
        public const string VertexTexture = "vertex_texture";
        public const string Where = "where";
        public const string Yield = "yield";

        public static bool IsReserved(string str)
        {
            return Reserved.ContainsKey(str);
        }

        internal static readonly AstUnaryType[] UnaryPrefix = new AstUnaryType[(int) TokenType.Max];
        internal static readonly AstUnaryType[] UnaryPostfix = new AstUnaryType[(int) TokenType.Max];
        internal static readonly AstBinaryType[] BinaryType = new AstBinaryType[(int) TokenType.Max];
        internal static readonly Precedence[] PrecedencePlusAssociativity = new Precedence[(int) TokenType.Max];
        internal static readonly Precedence[] Precedence = new Precedence[(int) TokenType.Max];
        internal static readonly ParameterModifier[] ParameterModifier = new ParameterModifier[(int) TokenType.Max];
        internal static readonly Modifiers[] Modifier = new Modifiers[(int) TokenType.Max];
        internal static readonly AstValueStatementType[] ValueStatementType = new AstValueStatementType[(int) TokenType.Max];
        internal static readonly AstSymbolType[] SymbolType = new AstSymbolType[(int) TokenType.Max];
        internal static readonly AstEmptyStatementType[] EmptyStatementType = new AstEmptyStatementType[(int) TokenType.Max];
        internal static readonly AstStatementModifier[] StatementModifier = new AstStatementModifier[(int) TokenType.Max];
        internal static readonly AstClassType[] ClassType = new AstClassType[(int) TokenType.Max];
        internal static readonly string[] String = new string[(int) TokenType.Max];
        internal static readonly OperatorType[] UnaryOperator = new OperatorType[(int) TokenType.Max];
        internal static readonly OperatorType[] BinaryOperator = new OperatorType[(int) TokenType.Max];
        internal static readonly AstConstraintType[] ConstraintClassType = new AstConstraintType[(int) TokenType.Max];
        internal static readonly BuiltinType[] BuiltinType = new BuiltinType[(int) TokenType.Max];
        internal static readonly AstConstructorCallType[] ConstructorCallType = new AstConstructorCallType[(int) TokenType.Max];
        internal static readonly Dictionary<string, TokenType> Reserved = GetReserved();

        // This is called on type initialization, so we use this to initialize our arrays.
        static Dictionary<string, TokenType> GetReserved()
        {
            var reserved = new Dictionary<string, TokenType>();

            for (var i = 0; i < String.Length; i++)
                String[i] = "<" + (TokenType) i + ">";

            foreach (var m in typeof(TokenType).GetMembers())
            {
                foreach (var e in m.GetCustomAttributes(typeof(TokenAttribute), false))
                {
                    var a = (TokenAttribute) e;
                    var t = (TokenType) ((FieldInfo) m).GetValue(null);
                    var i = (int) t;
                    reserved[a.Value] = t;
                    UnaryPrefix[i] = a.UnaryType;
                    UnaryPostfix[i] = a.UnaryPostfix;
                    BinaryType[i] = a.BinaryType;
                    PrecedencePlusAssociativity[i] = a.Precedence + (int) a.Associativity;
                    Precedence[i] = a.Precedence;
                    ParameterModifier[i] = a.ParameterModifier;
                    Modifier[i] = a.Modifier;
                    ValueStatementType[i] = a.ValueStatementType;
                    SymbolType[i] = a.SymbolType;
                    EmptyStatementType[i] = a.EmptyStatementType;
                    StatementModifier[i] = a.StatementModifier;
                    ClassType[i] = a.ClassType;
                    String[i] = a.Value.Quote();
                    UnaryOperator[i] = a.UnaryOperator;
                    BinaryOperator[i] = a.BinaryOperator;
                    ConstraintClassType[i] = a.ConstraintClassType;
                    BuiltinType[i] = a.BuiltinType;
                    ConstructorCallType[i] = a.ConstructorCallType;
                }
            }

            return reserved;
        }
    }
}