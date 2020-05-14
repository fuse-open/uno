using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;
using Uno.Compiler.API.Domain.AST.Statements;
using Uno.Logging;

namespace Uno.Compiler.Frontend.Analysis
{
    public class Parser : LogObject
    {
        public static AstStatement ParseStatement(Log log, Source src, string code)
        {
            var p = new Parser(log, src, code);
            var r = p.Statement();
            p.Done();
            return r;
        }

        public static AstExpression ParseExpression(Log log, Source src, string code, ParseContext context = ParseContext.Default)
        {
            var p = new Parser(log, src, code);
            var r = p.Expression(context);
            p.Done();
            return r;
        }

        public static AstExpression ParseType(Log log, Source src, string code)
        {
            var p = new Parser(log, src, code);
            var r = p.Type();
            p.Done();
            return r;
        }

        const int End = 1;
        internal bool HasErrors;
        readonly SourceFile _file;
        readonly Token[] _stack;
        readonly Token[] _comments;
        readonly string _text;
        int _index = End;
        int _commentIndex;
        int _commentOffset;
        int _errorIndex;

        public Parser(Log log, Source src, string text)
            : this(log, src.Package, src.FullPath, text, src.Line, src.Column)
        {
        }

        public Parser(Log log, SourcePackage upk, string filename, string text, int line = 1, int column = 1)
            : base(log)
        {
            _text = text;
            _file = new SourceFile(upk, filename, text, line, column);
            var tokens = Lexer.Tokenize(_file, text);
            var comments = new List<Token>();

            // Make room for a few dummy tokens on each side so we can drop bounds checking
            _stack = new Token[tokens.Count + 2 * (End + 1) - 1];

            var eof = tokens.Last();
            for (var i = 0; i <= End; i++)
                _stack[i] = eof;

            // Push tokens on stack in reverse order
            for (var i = tokens.Count - 2; i >= 0; i--)
            {
                var token = tokens[i];
                switch (token.Type)
                {
                    case TokenType.PreprocessorDirective:
                    {
                        // FIXME: This is a simplification of reality
                        if (token.StartsWith("#pragma reset"))
                        {
                            _file.AddPart(token.Line);
                            _stack[++_index] = new Token(_file, _text, TokenType.EndOfFile, token.Line, token.Column, token.Offset, token.Offset);
                        }
                        else if (!token.StartsWith("#region") && 
                                 !token.StartsWith("#endregion"))
                            Log.Warning(token, null, "Unrecognized preprocessor directive " + token.Value.Printable());

                        // Ignore preprocessor directives (for now)
                        continue;
                    }
                    case TokenType.DocComment:
                    {
                        comments.Add(token);
                        continue;
                    }
                }

                _stack[++_index] = token;
            }

            for (var i = _index + 1; i < _stack.Length; i++)
                _stack[i] = eof;

            _comments = comments.ToArray();
            _commentIndex = _comments.Length - 1;
        }

        public void Parse(List<AstDocument> result)
        {
            for (; _index > End; _index--)
            {
                var root = new AstDocument(_stack[_index]);
                NamespaceMembers(root);
                result.Add(root);

                if (_stack[_index].Type != TokenType.EndOfFile)
                    Done();
            }
        }

        void Done()
        {
            if (_index > End && !HasErrors)
                Error(_stack[_index], ErrorCode.E1108, "Unexpected token " + _stack[_index].Value.Quote());
        }

        Token Pop(TokenType tokenType, string expected = null)
        {
            if (_stack[_index].Type == tokenType)
                return _stack[_index--];

            Expected(expected ?? Tokens.String[(int) tokenType]);
            return _stack[_index];
        }

        void Scan(TokenType tokenType, string expected = null)
        {
            if (_stack[_index].Type == tokenType)
                --_index;
            else
                Expected(expected ?? Tokens.String[(int) tokenType]);
        }

        void Scan(TokenType tokenType1, TokenType tokenType2)
        {
            if (_stack[_index].Type == tokenType1)
                --_index;
            else
                Expected(Tokens.String[(int) tokenType1]);

            if (_stack[_index].Type == tokenType2)
                --_index;
            else
                Expected(Tokens.String[(int) tokenType2]);
        }

        bool Optional(TokenType tokenType)
        {
            if (_stack[_index].Type == tokenType)
            {
                --_index;
                return true;
            }

            return false;
        }

        void NamespaceMembers(AstNamespace parent)
        {
            for (;;)
            {
            NEXT_MEMBER:
                // Set DocComment mark
                _commentOffset = _stack[_index + 1].Offset;
                var token = _stack[_index];

                switch (token.Type)
                {
                    case TokenType.EndOfFile:
                        // Error recovery
                        for (; HasErrors && _index == End && _errorIndex > End; --_errorIndex)
                        {
                            switch (_stack[_errorIndex].Type)
                            {
                                case TokenType.Public:
                                case TokenType.Protected:
                                case TokenType.Internal:
                                case TokenType.Private:
                                case TokenType.Static:
                                case TokenType.Class:
                                case TokenType.Struct:
                                case TokenType.Interface:
                                case TokenType.Delegate:
                                case TokenType.Namespace:
                                case TokenType.Using:
                                    Recover();
                                    goto NEXT_MEMBER;
                            }
                        }
                        return;
                    case TokenType.RightCurlyBrace:
                        return;
                    case TokenType.LeftCurlyBrace:
                    {
                        --_index;
                        NamespaceMembers(parent);
                        Scan(TokenType.RightCurlyBrace);
                        break;
                    }
                    case TokenType.Namespace:
                    {
                        --_index;
                        for (var ns = parent;;)
                        {
                            var next = new AstNamespace(Identifier());
                            ns.Namespaces.Add(next);
                            ns = next;

                            if (!Optional(TokenType.Period))
                            {
                                Scan(TokenType.LeftCurlyBrace);
                                NamespaceMembers(ns);
                                Scan(TokenType.RightCurlyBrace);
                                break;
                            }
                        }
                        break;
                    }
                    case TokenType.Using:
                    {
                        if (!HasErrors && (
                                parent.Namespaces.Count > 0 || 
                                parent.Blocks.Count > 0))
                            Error(token, ErrorCode.E1106, "Using directives must occur before any namespace members");

                        --_index;
                        var isStatic = Optional(TokenType.Static);
                        var result = Expression();
                        AstIdentifier alias = null;

                        if (Optional(TokenType.Assign))
                        {
                            alias = result as AstIdentifier;
                            if (alias == null)
                                Expected((Token) result.Source, "identifier");
                            else
                                result = Expression();
                        }

                        Scan(TokenType.Semicolon);
                        parent.Usings.Add(new AstUsingDirective(result, alias, isStatic));
                        break;
                    }
                    default:
                    {
                        var attrs = OptionalAttributeList();
                        var comment = DocComment();
                        string cond;
                        var modifiers = ModifierList(out cond);
                        var block = OptionalBlock(comment, attrs, modifiers, cond);

                        if (block != null)
                        {
                            Optional(TokenType.Semicolon);
                            parent.Blocks.Add(block);
                        }
                        else
                            Expected(attrs.Count > 0 || modifiers != 0
                                ? "type or block"
                                : "namespace, type or block");
                        break;
                    }
                }
            }
        }

        AstIdentifier OptionalIdentifier(string defaultValue)
        {
            var token = _stack[_index];
            if (token.Type == TokenType.Identifier)
            {
                --_index;
                return Identifier(token);
            }

            return new AstIdentifier(_stack[_index + 1], defaultValue);
        }

        AstIdentifier OptionalIdentifier()
        {
            var token = _stack[_index];
            if (token.Type == TokenType.Identifier)
            {
                --_index;
                return Identifier(token);
            }

            return null;
        }

        AstIdentifier Identifier(string expected = "identifier")
        {
            var token = _stack[_index];
            if (token.Type == TokenType.Identifier)
            {
                --_index;
                return Identifier(token);
            }

            return Expected(expected);
        }

        AstIdentifier Identifier(string expected, TokenType following)
        {
            var retval = Identifier(expected);
            Scan(following);
            return retval;
        }

        AstIdentifier Identifier(Token token)
        {
            var offset = token.Offset;
            var length = token.Length;

            if (_text[offset] == '@')
            {
                ++offset;
                --length;
            }

            return new AstIdentifier(token, _text.Substring(offset, length));
        }

        IReadOnlyList<AstExpression> BaseTypeList()
        {
            if (!Optional(TokenType.Colon))
                return new AstExpression[0];

            for (var baseTypes = new List<AstExpression>();;)
            {
                baseTypes.Add(Type());
                if (Optional(TokenType.Comma))
                    continue;

                var token = _stack[_index];
                switch (token.Type)
                {
                    case TokenType.LeftCurlyBrace:
                        return baseTypes;
                    case TokenType.Identifier:
                        if (token.IsValue(Tokens.Where))
                            return baseTypes;
                        break;
                }

                Expected("',', '{' or 'where'");
                return baseTypes;
            }
        }

        IReadOnlyList<AstIdentifier> OptionalTypeParameterList()
        {
            if (!Optional(TokenType.LessThan))
                return null;

            for (var result = new List<AstIdentifier>();;)
            {
                result.Add(Identifier());
                if (Optional(TokenType.Comma))
                    continue;
                Scan(TokenType.GreaterThan, "',' or '>'");
                return result;
            }
        }

        AstGenericSignature OptionalGenericSignature(IReadOnlyList<AstIdentifier> typeParamList)
        {
            if (typeParamList == null)
                return null;

            for (var constraints = new List<AstConstraint>();;)
            {
                if (!OptionalContextual(Tokens.Where))
                    return new AstGenericSignature(typeParamList, constraints);

                var parameter = Identifier();
                Scan(TokenType.Colon);

                var clsType = _stack[_index].ConstraintClassType;
                if (clsType != 0)
                    --_index;

                Token newToken = null;
                var baseTypes = new List<AstExpression>();
                if (clsType == 0 || Optional(TokenType.Comma))
                {
                    for (;;)
                    {
                        switch (_stack[_index].Type)
                        {
                            case TokenType.New:
                                newToken = _stack[_index--];
                                Scan(TokenType.LeftParen, TokenType.RightParen);
                                break;
                            default:
                                var type = OptionalType();
                                if (type != null)
                                {
                                    baseTypes.Add(type);
                                    if (Optional(TokenType.Comma))
                                        continue;
                                }
                                break;
                        }

                        break;
                    }
                }

                constraints.Add(new AstConstraint(parameter, clsType, baseTypes, newToken));
            }
        }

        IReadOnlyList<AstBlockMember> Members(AstIdentifier parent = null, List<AstExpression> swizzlers = null)
        {
            Scan(TokenType.LeftCurlyBrace);

            for (var members = new List<AstBlockMember>();;)
            {
            NEXT_MEMBER:
                // Set DocComment mark
                _commentOffset = _stack[_index + 1].Offset;
                IReadOnlyList<AstAttribute> attrs = null;
                var token = _stack[_index];

                switch (token.Type)
                {
                    case TokenType.EndOfFile:
                        // Error recovery
                        for (; HasErrors && _index == End && _errorIndex > End; --_errorIndex)
                        {
                            switch (_stack[_errorIndex].Type)
                            {
                                case TokenType.Public:
                                case TokenType.Protected:
                                case TokenType.Internal:
                                case TokenType.Private:
                                case TokenType.Static:
                                case TokenType.ReadOnly:
                                case TokenType.Volatile:
                                case TokenType.Const:
                                case TokenType.Void:
                                case TokenType.Class:
                                case TokenType.Struct:
                                case TokenType.Interface:
                                case TokenType.Delegate:
                                    Recover();
                                    goto NEXT_MEMBER;
                                case TokenType.Namespace:
                                case TokenType.Using:
                                    return members;
                            }
                        }

                        Expected("member or '}'");
                        return members;
                    case TokenType.RightCurlyBrace:
                        --_index;
                        return members;
                    case TokenType.Identifier:
                        switch (_text[token.Offset])
                        {
                            case 'a':
                                if (OptionalContextual(Tokens.Apply))
                                {
                                    for (ApplyModifier mod = 0;;)
                                    {
                                        if (Optional(TokenType.Virtual))
                                            mod = ApplyModifier.Virtual;

                                        if (Optional(TokenType.Sealed))
                                        {
                                            if (mod != 0 || Optional(TokenType.Virtual))
                                                Error(_stack[_index + 1], ErrorCode.E0000, "Specified both 'virtual' and 'sealed'");
                                            mod = ApplyModifier.Sealed;
                                        }

                                        members.Add(new AstApply(mod, Expression()));
                                        if (Optional(TokenType.Comma))
                                            continue;

                                        Scan(TokenType.Semicolon, "',' or ';'");
                                        goto NEXT_MEMBER;
                                    }
                                }
                                break;
                            case 'd':
                                if (OptionalContextual(Tokens.Drawable))
                                {
                                    Contextual(Tokens.Block);
                                    members.Add(new AstNode(AstNodeType.Drawable,
                                        OptionalIdentifier(".drawable"),
                                        Members()));
                                    goto NEXT_MEMBER;
                                }
                                break;
                            case 'm':
                                if (OptionalContextual(Tokens.Meta))
                                {
                                    Contextual(Tokens.Block);
                                    members.Add(new AstNode(AstNodeType.Node,
                                        OptionalIdentifier(".node"),
                                        Members()));
                                    goto NEXT_MEMBER;
                                }
                                break;
                            case 's':
                                if (OptionalContextual(Tokens.Swizzler))
                                {
                                    if (swizzlers == null)
                                    {
                                        Error(_stack[_index + 1], ErrorCode.E1143, "'swizzler' was not expected in this context. Only structs may specify 'swizzler' types.");
                                        goto NEXT_MEMBER;
                                    }

                                    for (;;)
                                    {
                                        swizzlers.Add(Type());
                                        if (Optional(TokenType.Comma))
                                            continue;
                                        Scan(TokenType.Semicolon, "',' or ';'");
                                        goto NEXT_MEMBER;
                                    }
                                }
                                break;
                        }
                        break;
                    case TokenType.LeftSquareBrace:
                        --_index;
                        attrs = InnerAttributeList();
                        break;
                }

                string cond;
                var comment = DocComment();
                var modifierToken = _stack[_index];
                var modifiers = ModifierList(out cond);
                var block = OptionalBlock(comment, attrs, modifiers, cond);

                if (block != null)
                {
                    members.Add(block);
                    goto NEXT_MEMBER;
                }

                if (OptionalContextual(Tokens.Intrinsic))
                    modifiers |= Modifiers.Intrinsic;

                token = _stack[_index];
                switch (token.Type)
                {
                    case TokenType.Identifier:
                        if (parent != null && token.IsValue(parent.Symbol) &&
                            _stack[_index - 1].Type == TokenType.LeftParen)
                        {
                            --_index;
                            var parameters = ParameterList();
                            if (Optional(TokenType.Semicolon))
                            {
                                members.Add(new AstConstructor(token, comment, attrs, modifiers, cond, parameters));
                                goto NEXT_MEMBER;
                            }

                            AstConstructorCallType callType = 0;
                            IReadOnlyList<AstArgument> callArgs = null;
                            if (Optional(TokenType.Colon))
                            {
                                callType = _stack[_index].ConstructorCallType;
                                if (callType == 0)
                                    Expected("'base' or 'this'");
                                --_index;
                                callArgs = OptionalArgumentList(ParseContext.Default);
                            }

                            members.Add(new AstConstructor(token, comment, attrs, modifiers, cond, parameters, callType, callArgs, FunctionBody()));
                            goto NEXT_MEMBER;
                        }
                        break;

                    case TokenType.Tilde:
                        if (_stack[_index - 1].IsValue(parent.Symbol))
                        {
                            _index -= 2;
                            members.Add(new AstFinalizer(token, comment, attrs, modifiers, cond, ParameterList(), FunctionBody()));
                            goto NEXT_MEMBER;
                        }
                        break;

                    case TokenType.Event:
                        --_index;
                        var delegateType = Type();
                        var eventInterfaceType = OptionalExplicitInterface();
                        var eventName = Identifier();

                        if (Optional(TokenType.Semicolon))
                        {
                            members.Add(new AstEvent(comment, attrs, modifiers, cond, delegateType, eventInterfaceType, eventName, null, null));
                            goto NEXT_MEMBER;
                        }

                        AstAccessor add, remove;
                        AccessorScope(Tokens.Add, out add, Tokens.Remove, out remove);
                        members.Add(new AstEvent(comment, attrs, modifiers, cond, delegateType, eventInterfaceType, eventName, add, remove));
                        goto NEXT_MEMBER;

                    case TokenType.Implicit:
                    case TokenType.Explicit:
                        --_index;
                        modifiers |= token.Modifier;
                        Scan(TokenType.Operator);
                        members.Add(new AstConverter(comment, attrs, modifiers, cond,
                            Type(),
                            ParameterList(),
                            FunctionBody()));
                        goto NEXT_MEMBER;
                }

                token = _stack[_index];
                var isFixed = Optional(TokenType.Fixed);
                var fieldModifierToken = _stack[_index];
                FieldModifiers fieldModifiers = 0;
                if (Optional(TokenType.Const))
                    fieldModifiers |= FieldModifiers.Const;
                if (Optional(TokenType.ReadOnly))
                    fieldModifiers |= FieldModifiers.ReadOnly;

                var type = OptionalType();
                var interfaceToken = _stack[_index];
                var interfaceType = OptionalExplicitInterface();

                switch (_stack[_index].Type)
                {
                    case TokenType.This:
                    {
                        --_index;
                        VerifyFixedArrayNotSet(token, isFixed);
                        VerifyFieldModifiersNotSet(fieldModifierToken, fieldModifiers);

                        if (type == null)
                            type = Error(token, ErrorCode.E0000, "Indexers must have return type");

                        var paramList = ParameterList(ParseContext.Default, TokenType.LeftSquareBrace, TokenType.RightSquareBrace);
                        AstAccessor get, set;
                        AccessorScope(Tokens.Get, out get, Tokens.Set, out set);
                        members.Add(new AstIndexer(interfaceToken, comment, attrs, modifiers, cond, type, interfaceType, paramList, get, set));
                        goto NEXT_MEMBER;
                    }
                    case TokenType.Operator:
                    {
                        --_index;
                        VerifyFixedArrayNotSet(token, isFixed);
                        VerifyFieldModifiersNotSet(fieldModifierToken, fieldModifiers);
                        VerifyExplicitInterfaceNotSet(interfaceType);

                        if (type == null)
                            type = Error(token, ErrorCode.E0000, "Operators must have return type");

                        token = _stack[_index--];
                        if (!modifiers.HasFlag(Modifiers.Static))
                            Error(token, ErrorCode.E1166, "Operators must be declared 'static'");

                        var paramList = ParameterList();
                        members.Add(new AstOperator(interfaceToken, comment, attrs, modifiers, cond, type,
                            GetOperatorType(token, paramList.Count), paramList, FunctionBody()));
                        goto NEXT_MEMBER;
                    }
                }

                var name = OptionalIdentifier();
                var typeParamToken = _stack[_index];
                var typeParamList = OptionalTypeParameterList();

                // Method, field, property or meta property
                var paramList2 = OptionalParameterList();
                if (paramList2 == null)
                {
                    if (typeParamList != null)
                        Error(typeParamToken, ErrorCode.E0000, "Unexpected '<'");

                    if (isFixed)
                    {
                        if (name == null)
                            Expected(typeParamToken, "identifier");
                        type = new AstFixedArray(token, type, ArraySize());
                    }

                    if (Optional(TokenType.Colon))
                    {
                        VerifyExplicitInterfaceNotSet(interfaceType);

                        MetaVisibility metaVisibility = 0;
                        switch (modifiers)
                        {
                            case 0:
                                break;
                            case Modifiers.Private:
                                metaVisibility = MetaVisibility.Private;
                                break;
                            case Modifiers.Public:
                                metaVisibility = MetaVisibility.Public;
                                break;
                            default:
                                Error(modifierToken, ErrorCode.E0000, "Invalid modifier(s) on meta property: " + (modifiers ^ Modifiers.Private ^ Modifiers.Public).ToLiteral().Quote());
                                break;
                        }

                        if (name == null)
                        {
                            if (type == null || type.ExpressionType != AstExpressionType.Identifier)
                            {
                                if (type != null)
                                    Expected((Token) type.Source, "identifier");
                                type = new AstInvalid(_stack[_index]);
                            }

                            name = type as AstIdentifier;
                            type = null;
                        }

                        members.Add(new AstMetaProperty(metaVisibility, type, name, MetaPropertyDefinitionList()));
                        goto NEXT_MEMBER;
                    }

                    if (name == null)
                        name = Expected("identifier");

                    if (_stack[_index].Type == TokenType.LeftCurlyBrace)
                    {
                        AstAccessor get, set;
                        AccessorScope(Tokens.Get, out get, Tokens.Set, out set);
                        members.Add(new AstProperty(comment, attrs, modifiers, cond, type, interfaceType, name, get, set));
                        VerifyFieldModifiersNotSet(fieldModifierToken, fieldModifiers);
                        goto NEXT_MEMBER;
                    }

                    VerifyExplicitInterfaceNotSet(interfaceType);

                    // Field list
                    if (type == null)
                        Expected("type");

                    for (;;)
                    {
                        AstExpression value = null;
                        if (Optional(TokenType.Assign))
                            value = RValue();

                        if (Optional(TokenType.Comma))
                        {
                            members.Add(new AstField(comment, attrs, modifiers, cond, fieldModifiers, type, name, value));
                            name = Identifier();

                            if (isFixed)
                                type = new AstFixedArray(token, ((AstFixedArray) type).ElementType, 
                                    ArraySize());
                            continue;
                        }

                        members.Add(new AstField(comment, attrs, modifiers, cond, fieldModifiers, type, name, value));
                        Scan(TokenType.Semicolon, "',' or ';'");
                        goto NEXT_MEMBER;
                    }
                }

                // Method
                VerifyFixedArrayNotSet(token, isFixed);
                VerifyFieldModifiersNotSet(fieldModifierToken, fieldModifiers);

                if (name == null)
                    name = Error(type.Source, ErrorCode.E0000, "Method must have a return type (mistyped constructor?)");

                members.Add(new AstMethod(comment, attrs, modifiers, cond, type, interfaceType, name, paramList2,
                    OptionalGenericSignature(typeParamList),
                    FunctionBody()));
            }
        }

        AstBlockBase OptionalBlock(string comment, IReadOnlyList<AstAttribute> attrs, Modifiers modifiers, string cond)
        {
            switch (_stack[_index - 1].Type)
            {
                case TokenType.Class:
                case TokenType.Struct:
                case TokenType.Interface:
                    if (OptionalContextual(Tokens.Partial))
                        modifiers |= Modifiers.Partial;
                    if (OptionalContextual(Tokens.Intrinsic))
                        modifiers |= Modifiers.Intrinsic;
                    break;
            }

            var token = _stack[_index];
            switch (token.Type)
            {
                case TokenType.Delegate:
                {
                    --_index;
                    var type = Type();
                    var name = Identifier();
                    var typeParamList = OptionalTypeParameterList();
                    var paramList = ParameterList();
                    var optionalGenericSig = OptionalGenericSignature(typeParamList);
                    Scan(TokenType.Semicolon);
                    return new AstDelegate(comment, attrs, modifiers, cond, type, name, paramList, optionalGenericSig);
                }
                case TokenType.Enum:
                {
                    --_index;
                    var name = Identifier();
                    var baseType = Optional(TokenType.Colon)
                        ? Type()
                        : null;
                    Scan(TokenType.LeftCurlyBrace);

                    for (var literals = new List<AstLiteral>();;)
                    {
                        if (!Optional(TokenType.RightCurlyBrace))
                        {
                            var lattrs = OptionalAttributeList();
                            var lcomment = DocComment();
                            literals.Add(new AstLiteral(
                                lcomment,
                                lattrs,
                                Identifier(), 
                                Optional(TokenType.Assign)
                                    ? Expression()
                                    : null));

                            if (Optional(TokenType.Comma))
                                continue;

                            Scan(TokenType.RightCurlyBrace, "',' or '}'");
                        }

                        return new AstEnum(comment, attrs, modifiers, cond, name, baseType, literals);
                    }
                }
                case TokenType.Class:
                case TokenType.Interface:
                case TokenType.Struct:
                {
                    --_index;
                    var name = Identifier();
                    var typeParamList = OptionalTypeParameterList();
                    var swizzlers = new List<AstExpression>();
                    return new AstClass(comment, attrs, modifiers, cond, token.ClassType, name,
                        BaseTypeList(),
                        OptionalGenericSignature(typeParamList),
                        Members(name, swizzlers), 
                        swizzlers);
                }
                case TokenType.Identifier:
                    return OptionalContextual(Tokens.Block)
                        ? new AstBlock(comment, attrs, modifiers, cond,
                            Identifier(),
                            BaseTypeList(),
                            Members())
                        : null;
                default:
                    return null;
            }
        }

        IReadOnlyList<AstParameter> OptionalParameterList(ParseContext context = ParseContext.Default, TokenType begin = TokenType.LeftParen, TokenType end = TokenType.RightParen)
        {
            return _stack[_index].Type == begin
                ? ParameterList(context, begin, end)
                : null;
        }

        IReadOnlyList<AstParameter> ParameterList(ParseContext context = ParseContext.Default, TokenType begin = TokenType.LeftParen, TokenType end = TokenType.RightParen)
        {
            Scan(begin);
            if (Optional(end))
                return new AstParameter[0];

            for (var list = new List<AstParameter>();;)
            {
                var attributes = OptionalAttributeList();
                var modifier = _stack[_index].ParameterModifier;
                if (modifier != 0)
                    --_index;

                var token = _stack[_index];
                var isFixed = Optional(TokenType.Fixed);
                var type = Type();
                var name = Identifier();
                AstExpression value = null;

                if (isFixed)
                    type = new AstFixedArray(token, type, ArraySize());
                else if (Optional(TokenType.Assign))
                    value = Expression(context);

                list.Add(new AstParameter(attributes, modifier, type, name, value));

                if (Optional(TokenType.Comma))
                    continue;
                if (!Optional(end))
                    Expected("',' or " + Tokens.String[(int) end]);

                return list;
            }
        }

        IReadOnlyList<AstAttribute> OptionalAttributeList()
        {
            return Optional(TokenType.LeftSquareBrace)
                ? InnerAttributeList()
                : AstAttribute.Empty;
        }

        IReadOnlyList<AstAttribute> InnerAttributeList()
        {
            string cond;
            var modifierToken = _stack[_index];
            var modifiers = ModifierList(out cond);
            if ((modifiers & ~Modifiers.Extern) != 0 || modifiers != 0 && cond == null)
                Error(modifierToken, ErrorCode.E0000, "Only 'extern(CONDITION)' can be used as attribute modifier");

            for (var attribs = new List<AstAttribute>();;)
            {
                AstAttributeModifier modifier = 0;
                if (Optional(TokenType.Return))
                    modifier = AstAttributeModifier.Return;
                else if (OptionalContextual("assembly"))
                    modifier = AstAttributeModifier.Assembly;
                else if (OptionalContextual("module"))
                    modifier = AstAttributeModifier.Module;

                if (modifier != 0)
                    Scan(TokenType.Colon);

                var type = Type();
                var args = OptionalArgumentList(ParseContext.Default) ?? AstArgument.Empty;
                attribs.Add(new AstAttribute(cond, modifier, type, args));

                if (Optional(TokenType.RightSquareBrace))
                {
                    if (Optional(TokenType.LeftSquareBrace))
                    {
                        modifierToken = _stack[_index];
                        modifiers = ModifierList(out cond);

                        if ((modifiers & ~Modifiers.Extern) != 0 || modifiers != 0 && cond == null)
                            Error(modifierToken, ErrorCode.E0000, "Only 'extern(CONDITION)' can be used as attribute modifier");

                        continue;
                    }

                    return attribs;
                }

                if (Optional(TokenType.Comma))
                    continue;

                Expected("',' or ']'");
                return attribs;
            }
        }

        void VerifyFixedArrayNotSet(Source src, bool isFixed)
        {
            if (isFixed)
                Error(src, ErrorCode.E0000, "Unexpected 'fixed'");
        }

        void VerifyFieldModifiersNotSet(Source src, FieldModifiers f)
        {
            if (f != 0)
                Error(src, ErrorCode.E0000, "Unexpected " + f.ToLiteral().Quote());
        }

        void VerifyExplicitInterfaceNotSet(AstExpression e)
        {
            if (e != null)
                Error(e.Source, ErrorCode.E0000, "Unexpected type");
        }

        OperatorType GetOperatorType(Token token, int parameterCount)
        {
            OperatorType retval;
            switch (parameterCount)
            {
                case 1:
                    retval = token.UnaryOperator;
                    if (retval == 0)
                        Expected(token, "overloadable unary operator");
                    return retval;
                case 2:
                    retval = token.BinaryOperator;
                    if (retval == 0)
                        Expected(token, "overloadable binary operator");
                    return retval;
                default:
                    Expected(token, "unary or binary operator");
                    return 0;
            }
        }

        AstScope FunctionBody()
        {
            var token = _stack[_index];
            return Optional(TokenType.Semicolon)
                    ? null :
                Optional(TokenType.AlphaBlock)
                    ? new AstScope(token, ExternScope(token))
                    : Scope(ParseContext.Default, "';', '{' or '@{'");
        }

        AstExternScope ExternScope(Token token, Source src = null, IReadOnlyList<AstAttribute> attrs = null, IReadOnlyList<AstArgument> args = null)
        {
            return new AstExternScope(src ?? token, attrs ?? AstAttribute.Empty, args,
                new SourceValue(token, SmartTrim(token.Offset + 2, token.Length - 4)));
        }

        string SmartTrim(int offset, int length)
        {
            var code = _text.Substring(offset, length);
            var lines = code.Split('\n');
            var indentBaseLine = lines.FirstOrDefault(line => line.Any(c => !char.IsWhiteSpace(c))) ?? "";
            var indentString = new string(indentBaseLine.TakeWhile(char.IsWhiteSpace).ToArray());
            var unindentedLines = lines.Select(
                line => line.Substring(CommonPrefixLength(line, indentString)));
            return string.Join("\n", unindentedLines).Trim();
        }

        static int CommonPrefixLength(string s1, string s2)
        {
            int i = 0;
            for (; i < s1.Length && i < s2.Length; ++i)
                if (s1[i] != s2[i])
                    return i;
            return i;
        }

        void AccessorScope(string name0, out AstAccessor accessor0, string name1, out AstAccessor accessor1)
        {
            Scan(TokenType.LeftCurlyBrace);
            accessor0 = OptionalAccessor(name0);
            accessor1 = OptionalAccessor(name1);

            if (accessor0 == null)
                accessor0 = OptionalAccessor(name0);
            if (accessor0 == null && accessor1 == null)
                Expected("accessor inside " + name0 + "/" + name1 + " scope");

            Scan(TokenType.RightCurlyBrace);
        }

        AstAccessor OptionalAccessor(string name)
        {
            var old = _index;
            var src = _stack[_index];
            string cond;
            var modifiers = ModifierList(out cond);

            if (!OptionalContextual(name))
            {
                Backtrack(old);
                return null;
            }

            return new AstAccessor(src, modifiers, cond, FunctionBody());
        }

        IReadOnlyList<string> TagList()
        {
            var result = new List<string>();

            while (OptionalContextual(Tokens.Tag))
            {
                for (Scan(TokenType.LeftParen);;)
                {
                    result.Add(StringValue().String);
                    if (Optional(TokenType.Comma))
                        continue;

                    Scan(TokenType.RightParen, "')' or ','");
                    break;
                }
            }

            return result;
        }

        IReadOnlyList<AstReqStatement> ReqStatementList()
        {
            for (var result = new List<AstReqStatement>();;)
            {
                if (!OptionalContextual(Tokens.Req))
                    return result;

                for (Scan(TokenType.LeftParen);;)
                {
                    result.Add(new AstReqStatement(
                        _stack[_index], 
                        OptionalContextual(Tokens.Prev)
                            ? PrevOffset()
                            : 0,
                        Identifier(), 
                        Optional(TokenType.As)
                            ? Type()
                            : null, 
                        OptionalContextual(Tokens.Tag)
                            ? StringValue().String
                            : null));

                    if (Optional(TokenType.Comma))
                        continue;

                    Scan(TokenType.RightParen, "')' or ','");
                    break;
                }
            }
        }

        IReadOnlyList<AstMetaPropertyDefinition> MetaPropertyDefinitionList()
        {
            for (var defs = new List<AstMetaPropertyDefinition>();;)
            {
                var tags = TagList();
                var reqs = ReqStatementList();
                var token = _stack[_index];

                switch (token.Type)
                {
                    case TokenType.Fixed:
                    {
                        defs.Add(new AstMetaPropertyDefinition(
                            FixedArrayInitializer(ParseContext.MetaProperty),
                            tags, reqs));
                        break;
                    }
                    case TokenType.LeftCurlyBrace:
                    {
                        --_index;
                        defs.Add(new AstMetaPropertyDefinition(
                            InnerScope(token, ParseContext.MetaProperty), 
                            tags, reqs));
                        break;
                    }
                    default:
                    {
                        var s = Expression(ParseContext.MetaProperty, 0, "expression or scope");
                        if (!(s is AstIdentifier && (s as AstIdentifier).Symbol == Tokens.Undefined))
                            defs.Add(new AstMetaPropertyDefinition(s, tags, reqs));
                        break;
                    }
                }

                if (Optional(TokenType.Comma))
                    continue;

                Scan(TokenType.Semicolon, "';' or ','");
                return defs;
            }
        }

        Modifiers ModifierList(out string cond)
        {
            cond = null;
            Modifiers result = 0;
            for (;;)
            {
                var token = _stack[_index];
                var modifier = token.Modifier;
                switch (modifier)
                {
                    case 0:
                    case Modifiers.Explicit:
                    case Modifiers.Implicit:
                        return result;
                }

                AddModifier(ref result, token, modifier);
                if (modifier == Modifiers.Extern && _stack[_index].Type == TokenType.LeftParen)
                    cond = ConditionString();
            }
        }

        void AddModifier(ref Modifiers result, Token token, Modifiers modifier)
        {
            if (result.HasFlag(modifier))
                Error(token, ErrorCode.E1198, modifier.ToLiteral().Quote() + " is already specified for this item");

            result |= modifier;
            --_index;
        }

        string ConditionString()
        {
            var sb = new StringBuilder();
            Scan(TokenType.LeftParen);
            AppendCondition(sb, Expression());
            Scan(TokenType.RightParen);
            return sb.ToString();
        }

        void AppendCondition(StringBuilder sb, AstExpression e, bool p = false)
        {
            switch (e.ExpressionType)
            {
                case AstExpressionType.Identifier:
                {
                    sb.Append(((AstIdentifier) e).Symbol);
                    break;
                }
                case AstExpressionType.LogNot:
                {
                    var s = (AstUnary) e;
                    sb.Append("!");
                    AppendCondition(sb, s.Operand, true);
                    break;
                }
                case AstExpressionType.LogAnd:
                {
                    var s = (AstBinary) e;
                    sb.AppendWhen(p, "(");
                    AppendCondition(sb, s.Left, true);
                    sb.Append(" && ");
                    AppendCondition(sb, s.Right, true);
                    sb.AppendWhen(p, ")");
                    break;
                }
                case AstExpressionType.LogOr:
                {
                    var s = (AstBinary) e;
                    sb.AppendWhen(p, "(");
                    AppendCondition(sb, s.Left, true);
                    sb.Append(" || ");
                    AppendCondition(sb, s.Right, true);
                    sb.AppendWhen(p, ")");
                    break;
                }
                default:
                {
                    Error(e.Source, ErrorCode.E0000, "Invalid condition. Did you mean '!', '||' or '&&'?");
                    break;
                }
            }
        }

        AstScope Scope(ParseContext context = ParseContext.Default, string expected = "'{'")
        {
            return InnerScope(
                Pop(TokenType.LeftCurlyBrace, expected),
                context);
        }

        AstStatement EmbeddedStatement(ParseContext context = ParseContext.Default)
        {
            return Optional(TokenType.Semicolon)
                ? null
                : Statement(context, "';' or embedded statement");
        }

        SourceValue StringValue()
        {
            var token = _stack[_index];
            var value = OptionalExpression(ParseContext.Default, Precedence.Unary);

            if (value is AstString)
                return new SourceValue(token, ((AstString) value).Value);

            Expected(token, "string");
            return new SourceValue(token, "");
        }

        SourceValue StringValue(TokenType following)
        {
            var retval = StringValue();
            Scan(following);
            return retval;
        }

        AstStatement ExternStatement(ParseContext context, Token token, 
            IReadOnlyList<AstAttribute> attributes = null, AstExpression type = null, IReadOnlyList<AstArgument> args = null)
        {
            switch (_stack[_index].Type)
            {
                case TokenType.AlphaBlock:
                    return type != null
                        ? (AstStatement) Expected("extern expression")
                        :                ExternScope(_stack[_index--], token, attributes ?? AstAttribute.Empty, args);
                case TokenType.StringLiteral:
                case TokenType.AlphaString:
                    return new AstExtern(token, 
                        attributes ?? AstAttribute.Empty,
                        type, args,
                        StringValue(TokenType.Semicolon));
                default:
                    return attributes != null || type != null || args != null
                        ? Expected("extern statement")
                        : InnerVariable(context, 
                            AstVariableModifier.Extern,
                            OptionalContextual("var")
                                ? new AstSymbol(_stack[_index + 1], AstSymbolType.Var)
                                : Type(),
                            TokenType.Semicolon);
            }
        }

        AstStatement Statement(ParseContext context = ParseContext.Default, string expected = "statement")
        {
            var token = _stack[_index];
            switch (token.Type)
            {
                case TokenType.EndOfFile:
                    return Expected(expected);
                case TokenType.AlphaBlock:
                {
                    --_index;
                    return ExternScope(token);
                }
                case TokenType.Extern:
                {
                    --_index;
                    return ExternStatement(context, token, null,
                        Optional(TokenType.LessThan)
                            ? Type(TokenType.GreaterThan)
                            : null,
                        OptionalArgumentList(context));
                }
                case TokenType.LeftSquareBrace:
                {
                    --_index;
                    var attrs = InnerAttributeList();
                    AstExpression type = null;
                    IReadOnlyList<AstArgument> args = null;

                    token = _stack[_index];
                    if (token.Type == TokenType.Extern)
                    {
                        _index--;
                        if (Optional(TokenType.LessThan))
                            type = Type(TokenType.GreaterThan);
                        args = OptionalArgumentList(context);
                    }

                    return ExternStatement(context, token, attrs, type, args);
                }
                case TokenType.LeftCurlyBrace:
                {
                    --_index;
                    return InnerScope(token, context);
                }
                case TokenType.Checked:
                case TokenType.Unchecked:
                case TokenType.Unsafe:
                {
                    --_index;
                    switch (_stack[_index].Type)
                    {
                        case TokenType.LeftCurlyBrace:
                            --_index;
                            return new AstModifiedStatement(token, token.StatementModifier, InnerScope(token, context));
                        case TokenType.LeftParen:
                            --_index;
                            return new AstUnary(token, token.UnaryPrefix, Expression(context, TokenType.RightParen, TokenType.Semicolon));
                        default:
                            return Expected("'{' or '('");
                    }
                }
                case TokenType.If:
                {
                    --_index;
                    AstExpression cond;
                    if (Optional(TokenType.Defined))
                        cond = new AstDefined(_stack[_index], ConditionString());
                    else
                    {
                        cond = Conditional();
                        // Compiler will conditionally compile blocks of code when 'if defined(...)'.
                        // See if cond can be rewritten to a single AstDefined node end emit a warning if so.
                        var def = TryFlattenDefined(cond);
                        if (def != null)
                        {
                            Log.Warning(token, ErrorCode.W0000, "Test was rewritten as 'if defined(" + def.Condition + ")' to enable conditional compilation");
                            cond = def;
                        }
                    }

                    return new AstIfElse(
                        token,
                        cond,
                        EmbeddedStatement(context),
                        Optional(TokenType.Else)
                            ? EmbeddedStatement(context)
                            : null);
                }
                case TokenType.While:
                {
                    --_index;
                    return new AstLoop(token, AstLoopType.While, Conditional(context), EmbeddedStatement(context));
                }
                case TokenType.Do:
                {
                    --_index;
                    var body = EmbeddedStatement(context);
                    Scan(TokenType.While);
                    return new AstLoop(token, AstLoopType.DoWhile, Conditional(context, TokenType.Semicolon), body);
                }
                case TokenType.BuildError:
                case TokenType.BuildWarning:
                case TokenType.Return:
                case TokenType.Throw:
                {
                    --_index;
                    return Optional(TokenType.Semicolon)
                        ? (AstStatement) new AstEmptyStatement(token, token.EmptyStatementType)
                        :                new AstValueStatement(token, token.ValueStatementType,
                                                Expression(context, "';' or exception", TokenType.Semicolon));
                }
                case TokenType.Break:
                case TokenType.Continue:
                {
                    --_index;
                    Scan(TokenType.Semicolon);
                    return new AstEmptyStatement(token, token.EmptyStatementType);
                }
                case TokenType.Foreach:
                {
                    --_index;
                    Scan(TokenType.LeftParen);
                    return new AstForeach(token,
                        Type(0, "iterator type"),
                        Identifier("iterator name", TokenType.In),
                        Expression(context, "collection expression", TokenType.RightParen),
                        EmbeddedStatement(context));
                }
                case TokenType.For:
                {
                    --_index;
                    Scan(TokenType.LeftParen);
                    return new AstFor(token, !Optional(TokenType.Semicolon)
                            ? InitializerStatement(context, TokenType.Semicolon)
                            : null,
                        OptionalExpression(context, TokenType.Semicolon, "';' or conditional expression"),
                        OptionalSequence(context, TokenType.RightParen, "')' or increment expression"),
                        EmbeddedStatement(context));
                }
                case TokenType.Try:
                {
                    --_index;
                    var tryScope = Scope(context);
                    AstScope finallyScope = null;
                    var catchBlocks = new List<AstCatch>();

                    switch (_stack[_index].Type)
                    {
                        case TokenType.Catch:
                        case TokenType.Finally:
                        {
                            for (;;)
                            {
                                var catchToken = _stack[_index];
                                if (!Optional(TokenType.Catch))
                                    break;

                                AstExpression exceptionType = null;
                                AstIdentifier exceptionName = null;
                                if (Optional(TokenType.LeftParen))
                                {
                                    exceptionType = OptionalExpression(context);
                                    if (exceptionType != null)
                                        exceptionName = OptionalIdentifier(".exception");
                                    Scan(TokenType.RightParen);
                                }

                                if (exceptionType == null)
                                    exceptionName = new AstIdentifier(catchToken, ".exception");

                                catchBlocks.Add(new AstCatch(exceptionType, exceptionName, Scope(context)));
                            }

                            if (Optional(TokenType.Finally))
                                finallyScope = Scope(context);
                            break;
                        }
                        default:
                        {
                            Expected("'catch' or 'finally'");
                            break;
                        }
                    }

                    return new AstTryCatchFinally(token, tryScope, finallyScope, catchBlocks);
                }
                case TokenType.Lock:
                {
                    --_index;
                    return new AstLock(token, Conditional(context, "lock expression"), EmbeddedStatement(context));
                }
                case TokenType.Using:
                {
                    --_index;
                    Scan(TokenType.LeftParen);
                    return new AstUsing(token, 
                        InitializerStatement(context, TokenType.RightParen), 
                        EmbeddedStatement(context));
                }
                case TokenType.Switch:
                {
                    --_index;
                    var cond = Conditional(context, "switch expression");
                    Scan(TokenType.LeftCurlyBrace);
                    var cases = new List<AstSwitchCase>();
                    var values = new List<AstExpression>();
                    var includeDefault = false;
                    var defaultFound = false;

                    for (;;)
                    {
                        if (Optional(TokenType.Case))
                        {
                            values.Add(Expression(context));
                            var s = CaseScope(context, Pop(TokenType.Colon));
                            if (s.Statements.Count > 0)
                            {
                                cases.Add(new AstSwitchCase(values, s));
                                values = new List<AstExpression>();
                                includeDefault = false;
                            }
                        }
                        else if (Optional(TokenType.Default))
                        {
                            if (defaultFound)
                                Error(_stack[_index], ErrorCode.E1052, "Multiple 'default' blocks in switch");

                            var s = CaseScope(context, Pop(TokenType.Colon));
                            defaultFound = true;
                            values.Add(null);

                            if (s.Statements.Count > 0)
                            {
                                cases.Add(new AstSwitchCase(values, s));
                                values = new List<AstExpression>();
                                includeDefault = false;
                            }
                            else
                                includeDefault = true;
                        }
                        else if (Optional(TokenType.RightCurlyBrace))
                        {
                            if (values.Count > 0 || includeDefault)
                                Expected(_stack[_index + 1], "'case' preceding '}'");
                            break;
                        }
                        else
                        {
                            Expected("'case', 'default' or '}'");
                            break;
                        }
                    }

                    return new AstSwitch(token, cond, cases);
                }
                case TokenType.Assert:
                {
                    --_index;
                    return new AstValueStatement(token, token.ValueStatementType, Expression(context, "value", TokenType.Semicolon));
                }
                case TokenType.DebugLog:
                {
                    --_index;
                    var p = Optional(TokenType.LeftParen);
                    var e = Expression(context, 0, "value");
                    
                    // Expand multiple arguments to 'string.Join(" ", ...ARGS)'
                    if (Optional(TokenType.Comma))
                    {
                        var args = new List<AstArgument> {new AstString(Source.Unknown, " "), e};

                        do
                            args.Add(Expression(context, 0, "argument"));
                        while (Optional(TokenType.Comma));

                        e = new AstCall(AstCallType.Function,
                                new AstMember(
                                    new AstBuiltinType(Source.Unknown, BuiltinType.String),
                                    new AstIdentifier(Source.Unknown, "Join")),
                                args.ToArray());
                    }

                    if (p)
                        Scan(TokenType.RightParen);

                    Scan(TokenType.Semicolon);
                    return new AstValueStatement(token, token.ValueStatementType, e);
                }
                case TokenType.Const:
                {
                    --_index;
                    return InnerVariable(context, 
                        AstVariableModifier.Const, 
                        Type(), 
                        TokenType.Semicolon);
                }
                case TokenType.Fixed:
                {
                    --_index;
                    var type = Type();
                    var id = Identifier("fixed array name");
                    var fat = new AstFixedArray(token, type, ArraySize());
                    var result = new AstFixedArrayDeclaration(fat,
                        id, Optional(TokenType.Assign)
                                ? (_stack[_index].Type == TokenType.Fixed
                                        ? FixedArrayInitializer(context) :
                                    Optional(TokenType.LeftCurlyBrace)
                                        ? new AstFixedArrayInitializer(
                                                _stack[_index + 1],
                                                fat.ElementType,
                                                fat.OptionalSize,
                                                InnerArrayInitializer(context))
                                        : Expression(context))
                                : null);
                
                    Scan(TokenType.Semicolon, "';'");
                    return result;
                }
                case TokenType.Identifier:
                {
                    switch (_stack[_index + 1].Type)
                    {
                        case TokenType.LeftParen:
                        case TokenType.LeftSquareBrace:
                        case TokenType.DoubleColon:
                        case TokenType.Period:
                        case TokenType.Assign:
                            break;
                        default:
                        {
                            switch (_text[token.Offset])
                            {
                                case 'd':
                                {
                                    if (OptionalContextual(Tokens.Draw))
                                    {
                                        if (Optional(TokenType.Semicolon))
                                            return new AstDraw(token, new AstBlockMember[0]);

                                        var members = new List<AstBlockMember>();

                                        if (_stack[_index].Type == TokenType.LeftCurlyBrace)
                                        {
                                            var block = Members();
                                            if (Optional(TokenType.Semicolon))
                                                return new AstDraw(token, block);

                                            Scan(TokenType.Comma, "',' or ';' in draw statement");
                                            members.Add(new AstBlock(new AstIdentifier(token, ".anonymous"), block));
                                        }

                                        for (;;)
                                        {
                                            var mod = Optional(TokenType.Virtual)
                                                ? ApplyModifier.Virtual
                                                : 0;

                                            if (Optional(TokenType.Sealed))
                                            {
                                                if (mod != 0 || Optional(TokenType.Virtual))
                                                    Error(_stack[_index + 1], ErrorCode.E0000, "Specified both 'virtual' and 'sealed'");
                                                mod = ApplyModifier.Sealed;
                                            }

                                            var blockToken = _stack[_index];
                                            var block = OptionalExpression(context);

                                            if (block == null)
                                            {
                                                if (_stack[_index].Type == TokenType.LeftCurlyBrace)
                                                    members.Add(new AstBlock(
                                                        new AstIdentifier(blockToken, ".lambda"),
                                                        Members()));
                                                else if (OptionalContextual(Tokens.Block))
                                                    members.Add(new AstBlock(
                                                        new AstIdentifier(blockToken, ".lambda"),
                                                        BaseTypeList(),
                                                        Members()));
                                                else
                                                    Expected("block expression");
                                            }
                                            else
                                                members.Add(new AstApply(mod, block));

                                            if (Optional(TokenType.Comma))
                                                continue;

                                            if (_stack[_index].Type == TokenType.LeftCurlyBrace)
                                                members.AddRange(Members());

                                            Scan(TokenType.Semicolon, "',' or ';'");
                                            return new AstDraw(token, members);
                                        }
                                    }

                                    if (OptionalContextual(Tokens.DrawDispose))
                                    {
                                        Scan(TokenType.Semicolon);
                                        return new AstEmptyStatement(token, AstEmptyStatementType.DrawDispose);
                                    }

                                    break;
                                }
                                case 'v':
                                {
                                    if (_stack[_index - 1].Type == TokenType.Identifier &&
                                            OptionalContextual(Tokens.Var))
                                        return InnerVariable(context, 0, 
                                            new AstSymbol(token, AstSymbolType.Var), 
                                            TokenType.Semicolon);
                                    break;
                                }
                                case 'y':
                                {
                                    if (OptionalContextual(Tokens.Yield))
                                    {
                                        switch (_stack[_index].Type)
                                        {
                                            case TokenType.Break:
                                                --_index;
                                                Scan(TokenType.Semicolon);
                                                return new AstEmptyStatement(token, AstEmptyStatementType.YieldBreak);
                                            case TokenType.Return:
                                                --_index;
                                                return new AstValueStatement(token, AstValueStatementType.YieldReturn,
                                                    Expression(context, "';' or return value", TokenType.Semicolon));
                                        }
                                    }

                                    break;
                                }
                            }

                            break;
                        }
                    }

                    break;
                }
            }

            var retval = Expression(context, 0, expected);
            return Optional(TokenType.Semicolon)
                 ? retval
                 : InnerVariable(context, 0, retval, 
                    TokenType.Semicolon);
        }

        AstStatement InnerVariable(ParseContext context, AstVariableModifier modifier, AstExpression type, TokenType following)
        {
            for (var result = new List<AstVariable>();;)
            {
                result.Add(new AstVariable(
                    Identifier("variable name"),
                    Optional(TokenType.Assign)
                        ? RValue(context)
                        : null));

                if (Optional(TokenType.Comma))
                    continue;

                if (!Optional(following))
                    Expected("',' or " + Tokens.String[(int) following]);

                return new AstVariableDeclaration(modifier, type, result);
            }
        }

        AstExpression RValue(ParseContext context = ParseContext.Default)
        {
            var token = _stack[_index];
            return Optional(TokenType.LeftCurlyBrace)
                ? new AstArrayInitializer(token, InnerArrayInitializer(context))
                : Expression(context);
        }

        AstStatement InitializerStatement(ParseContext context, TokenType following)
        {
            var token = _stack[_index];
            switch (_text[token.Offset])
            {
                case 'v':
                    if (_stack[_index - 1].Type == TokenType.Identifier &&
                            OptionalContextual(Tokens.Var))
                        return InnerVariable(context, 0, 
                            new AstSymbol(token, AstSymbolType.Var), 
                            following);
                    break;
            }

            var type = Expression(context, 0, "initalizer or variable declaration");
            switch (_stack[_index].Type)
            {
                case TokenType.Comma:
                {
                    for (var list = new List<AstExpression> {type};;)
                    {
                        --_index;
                        list.Add(Expression());
                        switch (_stack[_index].Type)
                        {
                            case TokenType.Comma:
                                continue;
                            default:
                                Scan(following);
                                return new AstInitializer(list);
                        }
                    }
                }
                case TokenType.Identifier:
                    return InnerVariable(context, 0, type, 
                        following);
                default:
                    Scan(following);
                    return type;
            }
        }

        AstExpression OptionalSequence(ParseContext context, TokenType following, string expected)
        {
            if (Optional(following))
                return null;

            for (var result = Expression(context, 0, expected);;)
            {
                var token = _stack[_index];
                switch (token.Type)
                {
                    case TokenType.Comma:
                        --_index;
                        result = new AstBinary(AstBinaryType.Sequence, result, token,
                            Expression(context));
                        continue;
                    default:
                        Scan(following);
                        return result;
                }
            }
        }

        AstScope CaseScope(ParseContext context, Token token)
        {
            for (var statements = new List<AstStatement>();;)
            {
            NEXT_CASE:
                switch (_stack[_index].Type)
                {
                    case TokenType.Case:
                    case TokenType.Default:
                    case TokenType.RightCurlyBrace:
                        return new AstScope(token, statements);
                    case TokenType.EndOfFile:
                        // Error recovery
                        for (; HasErrors && _index == End && _errorIndex > End; --_errorIndex)
                        {
                            switch (_stack[_errorIndex].Type)
                            {
                                case TokenType.Case:
                                    Recover();
                                    goto NEXT_CASE;
                                case TokenType.Default:
                                    if (_stack[_errorIndex - 1].Type == TokenType.Colon)
                                    {
                                        Recover();
                                        goto NEXT_CASE;
                                    }
                                    break;
                                case TokenType.Semicolon:
                                    Recover();
                                    --_index;
                                    goto NEXT_STATEMENT;
                                case TokenType.Public:
                                case TokenType.Protected:
                                case TokenType.Internal:
                                case TokenType.Private:
                                case TokenType.Static:
                                case TokenType.Void:
                                case TokenType.Class:
                                case TokenType.Struct:
                                case TokenType.Interface:
                                case TokenType.Enum:
                                case TokenType.Namespace:
                                    return new AstScope(token, statements);
                            }
                        }
                        return new AstScope(token, statements);
                }
            NEXT_STATEMENT:
                statements.Add(Statement(context, "'case', 'default', '}' or statement"));
            }
        }

        AstScope InnerScope(Token token, ParseContext context)
        {
            for (var statements = new List<AstStatement>();;)
            {
                switch (_stack[_index].Type)
                {
                    case TokenType.EndOfFile:
                        // Error recovery
                        for (; HasErrors && _index == End && _errorIndex > End; --_errorIndex)
                        {
                            switch (_stack[_errorIndex].Type)
                            {
                                case TokenType.Semicolon:
                                    Recover();
                                    --_index;
                                    goto NEXT_STATEMENT;
                                case TokenType.Public:
                                case TokenType.Protected:
                                case TokenType.Internal:
                                case TokenType.Private:
                                case TokenType.Static:
                                case TokenType.Void:
                                case TokenType.Class:
                                case TokenType.Struct:
                                case TokenType.Interface:
                                case TokenType.Enum:
                                case TokenType.Namespace:
                                    return new AstScope(token, statements);
                            }
                        }

                        Expected("statement or '}'");
                        return new AstScope(token, statements, false);
                    case TokenType.RightCurlyBrace:
                        --_index;
                        return new AstScope(token, statements);
                    case TokenType.Semicolon:
                        --_index;
                        continue;
                }

            NEXT_STATEMENT:
                statements.Add(Statement(context, "statement or '}'"));
            }
        }

        AstFixedArrayInitializer FixedArrayInitializer(ParseContext context)
        {
            var token = _stack[_index--];
            if (_stack[_index].Type == TokenType.LeftSquareBrace)
            {
                Scan(TokenType.LeftSquareBrace, TokenType.RightSquareBrace);
                Scan(TokenType.LeftCurlyBrace);
                return new AstFixedArrayInitializer(token, null, null, InnerArrayInitializer(context));
            }

            var at = InnerFixedArray(token);
            return Optional(TokenType.LeftCurlyBrace)
                ? new AstFixedArrayInitializer(token, at.ElementType, at.OptionalSize, InnerArrayInitializer(context))
                : new AstFixedArrayInitializer(token, at.ElementType, at.OptionalSize);
        }

        AstExpression ArraySize(ParseContext context, string expected)
        {
            Scan(TokenType.LeftSquareBrace);
            var retval = Expression(context, 0, expected);
            Scan(TokenType.RightSquareBrace);
            return retval;
        }

        AstExpression ArraySize()
        {
            Scan(TokenType.LeftSquareBrace);
            var retval = OptionalExpression();
            Scan(TokenType.RightSquareBrace);
            return retval;
        }

        string ParenthesizeCondition(string cond)
        {
            foreach (var e in cond)
                if (!char.IsLetterOrDigit(e))
                    return "(" + cond + ")";

            return cond;
        }

        AstDefined TryFlattenDefined(AstExpression e)
        {
            switch (e.ExpressionType)
            {
                case AstExpressionType.LogAnd:
                case AstExpressionType.LogOr:
                {
                    var op = (AstBinary) e;
                    var left = TryFlattenDefined(op.Left);
                    var right = TryFlattenDefined(op.Right);

                    if (left != null && right != null)
                    {
                        switch (op.Type)
                        {
                            case AstBinaryType.LogAnd:
                                return new AstDefined(op.Source,
                                    ParenthesizeCondition(left.Condition) + " && " +
                                    ParenthesizeCondition(right.Condition));
                            case AstBinaryType.LogOr:
                                return new AstDefined(op.Source,
                                    ParenthesizeCondition(left.Condition) + " || " +
                                    ParenthesizeCondition(right.Condition));
                        }
                    }

                    return null;
                }
                case AstExpressionType.LogNot:
                {
                    var op = (AstUnary) e;
                    var arg = TryFlattenDefined(op.Operand);
                    return arg != null
                        ? new AstDefined(op.Source,
                                "!" + ParenthesizeCondition(arg.Condition))
                        : null;
                }
                case AstExpressionType.Defined:
                {
                    return (AstDefined) e;
                }
            }

            return null;
        }

        AstExpression Conditional(ParseContext context = ParseContext.Default, string expected = "conditional expression")
        {
            Scan(TokenType.LeftParen);
            var retval = Expression(context, 0, expected);
            Scan(TokenType.RightParen);
            return retval;
        }

        AstExpression Conditional(ParseContext context, TokenType following)
        {
            Scan(TokenType.LeftParen);
            var retval = Expression(context, 0, "conditional expression");
            Scan(TokenType.RightParen, following);
            return retval;
        }

        int EscapeSequence(Token token, int end, ref int i)
        {
            if (++i < end)
            {
                var x = -1;
                var c = _text[i];
                switch (c)
                {
                    case '\'':
                    case '\"':
                    case '\\':
                        return c;
                    case '0':
                        return '\0';
                    case 'a':
                        return '\a';
                    case 'b':
                        return '\b';
                    case 'f':
                        return '\f';
                    case 'n':
                        return '\n';
                    case 'r':
                        return '\r';
                    case 't':
                        return '\t';
                    case 'v':
                        return '\v';
                    case 'u':
                        x = Hexadecimal(end, ref i, 4, 4);
                        break;
                    case 'U':
                        x = Hexadecimal(end, ref i, 8, 8);
                        if (x != -1)
                        {
                            if (x <= 0x10FFFF)
                                return x;

                            Error(token, ErrorCode.E1223, "Unicode characters above U+10FFFF are not supported");
                        }
                        break;
                    case 'x':
                        x = Hexadecimal(end, ref i, 1, 4);
                        break;
                }

                if (x != -1)
                    return (char) x;
            }

            Error(token, ErrorCode.E1007, "Invalid escape sequence");
            return 0;
        }

        int Hexadecimal(int end, ref int i, int minDigits, int maxDigits)
        {
            var x = 0;
            var start = ++i;
            var numDigits = 0;
            for (; i - start < maxDigits && i < end; i++)
            {
                var c = _text[i];
                if (c >= '0' && c <= '9')
                {
                    x *= 16;
                    x += c - '0';
                    numDigits++;
                }
                else if (c >= 'a' && c <= 'f')
                {
                    x *= 16;
                    x += c - 'a' + 10;
                    numDigits++;
                }
                else if (c >= 'A' && c <= 'F')
                {
                    x *= 16;
                    x += c - 'A' + 10;
                    numDigits++;
                }
                else
                    break;
            }

            i--;
            return numDigits >= minDigits
                ? x
                : -1;
        }

        IReadOnlyList<AstExpression> InnerArrayInitializer(ParseContext context)
        {
            for (var result = new List<AstExpression>();;)
            {
                if (Optional(TokenType.RightCurlyBrace))
                    return result;

                result.Add(OptionalExpression(context) ??
                           Expected("expression or '}'"));

                if (Optional(TokenType.Comma))
                    continue;
                if (!Optional(TokenType.RightCurlyBrace))
                    Expected("',' or '}'");

                return result;
            }
        }

        IReadOnlyList<AstArgument> ArgumentList(ParseContext context)
        {
            Scan(TokenType.LeftParen);
            return InnerArgumentList(context);
        }

        IReadOnlyList<AstArgument> InnerArgumentList(ParseContext context, TokenType end = TokenType.RightParen)
        {
            for (var result = new List<AstArgument>();;)
            {
                if (Optional(end))
                    return result;

                var token = _stack[_index];
                AstIdentifier name = null;

                if (token.Type == TokenType.Identifier &&
                    _stack[_index - 1].Type == TokenType.Colon)
                {
                    _index -= 2;
                    name = Identifier(token);
                }

                var pm = _stack[_index].ParameterModifier;
                switch (pm)
                {
                    default:
                        --_index;
                        break;
                    case ParameterModifier.Params:
                    case ParameterModifier.This:
                        pm = 0;
                        break;
                    case 0:
                        break;
                }

                result.Add(new AstArgument(name, pm, Expression(context)));
                if (Optional(TokenType.Comma))
                    continue;

                Scan(end, "',' or " + Tokens.String[(int) end]);
                return result;
            }
        }

        IReadOnlyList<AstArgument> OptionalArgumentList(ParseContext context)
        {
            return Optional(TokenType.LeftParen)
                ? InnerArgumentList(context)
                : null;
        }

        AstStatement LambdaBody(ParseContext context)
        {
            var token = _stack[_index];
            switch (token.Type)
            {
                case TokenType.LeftCurlyBrace:
                    --_index;
                    return InnerScope(token, context);
                case TokenType.AlphaBlock:
                    --_index;
                    return ExternScope(token);
                default:
                    return Expression(context, Precedence.Assignment, "'{', '@{' or expression");
            }
        }

        uint PrevOffset()
        {
            if (Optional(TokenType.LeftParen))
            {
                var token = _stack[_index];
                var value = OptionalExpression() as AstInt;

                if (value != null && value.Value >= 0)
                {
                    Scan(TokenType.RightParen);
                    return (uint) value.Value;
                }

                Expected(token, "non-negative integer");
            }

            for (uint offset = 1;; offset++)
                if (!OptionalContextual(Tokens.Prev))
                    return offset;
        }

        void Contextual(string keyword)
        {
            if (_stack[_index].IsValue(keyword))
                --_index;
            else
                Expected(keyword.Quote());
        }

        bool OptionalContextual(string keyword)
        {
            if (_stack[_index].IsValue(keyword))
            {
                --_index;
                return true;
            }

            return false;
        }

        AstExpression OptionalMetaPropertyContextual()
        {
            var token = _stack[_index];
            switch (_text[token.Offset])
            {
                case 'i':
                    if (OptionalContextual(Tokens.Immutable))
                    {
                        Log.Warning(token, null, "'immutable' is deprecated -- replace with 'readonly'");
                        return new AstUnary(token, AstUnaryType.ReadOnly,
                            Expression(ParseContext.MetaProperty, Precedence.Primary));                        
                    }
                    if (OptionalContextual(Tokens.Interpolate))
                        Error(token, ErrorCode.E0000, "Unexpected 'interpolate'");
                    break;
                case 'm':
                    if (OptionalContextual(Tokens.Meta))
                        return new AstPrev(token, 0,
                            Identifier());
                    break;
                case 'p':
                    if (OptionalContextual(Tokens.Prev))
                        return new AstPrev(token, PrevOffset(),
                            OptionalIdentifier());
                    if (OptionalContextual(Tokens.Pixel))
                        return new AstUnary(token, AstUnaryType.Pixel,
                            Expression(ParseContext.MetaProperty, Precedence.Primary));
                    if (OptionalContextual(Tokens.PixelSampler))
                    {
                        var args = ArgumentList(ParseContext.MetaProperty);
                        if (args.Count != 1 &&
                            args.Count != 2)
                            Error(token, ErrorCode.E1030, "'pixel_sampler' operator requires 1 or 2 arguments");
                        foreach (var a in args)
                            if (a.Modifier != 0)
                                Error(token, ErrorCode.E1031, "Arguments to 'pixel_sampler' operator cannot specify modifiers");

                        return new AstPixelSampler(token, args[0].Value, args.Count > 1 ? args[1].Value : null);
                    }
                    break;
                case 'r':
                    if (Optional(TokenType.ReadOnly))
                        return new AstUnary(token, AstUnaryType.ReadOnly,
                            Expression(ParseContext.MetaProperty, Precedence.Primary));                        
                    break;
                case 'v':
                    if (Optional(TokenType.Volatile))
                        return new AstUnary(token, AstUnaryType.Volatile,
                            Expression(ParseContext.MetaProperty, Precedence.Primary));
                    if (OptionalContextual(Tokens.Vertex))
                        return new AstUnary(token, AstUnaryType.Vertex,
                            Expression(ParseContext.MetaProperty, Precedence.Primary));
                    if (OptionalContextual(Tokens.VertexAttrib))
                    {
                        if (Optional(TokenType.LessThan))
                            return new AstVertexAttribExplicit(token,
                                Type(TokenType.GreaterThan),
                                ArgumentList(ParseContext.MetaProperty));

                        Scan(TokenType.LeftParen);
                        var norm = OptionalContextual(Tokens.Norm);
                        var vertexBuffer = Expression(ParseContext.MetaProperty);
                        var indexBuffer = Optional(TokenType.Comma)
                            ? Expression(ParseContext.MetaProperty)
                            : null;
                        Scan(TokenType.RightParen);
                        return new AstVertexAttribImplicit(vertexBuffer.Source, vertexBuffer, indexBuffer, norm);
                    }
                    break;
                case 's':
                    if (OptionalContextual(Tokens.Sample))
                    {
                        var args = ArgumentList(ParseContext.MetaProperty);

                        if (args.Count != 2 &&
                            args.Count != 3)
                            Expected("'sample' operator requires 2 or 3 arguments");

                        foreach (var a in args)
                            if (a.Modifier != 0)
                                Error(token, ErrorCode.E0000, "Arguments to 'sample' operator cannot specify modifiers");

                        return new AstCall(AstCallType.Function,
                                new AstMember(
                                    new AstPixelSampler(token, args[0].Value,
                                        args.Count > 2
                                            ? args[2].Value
                                            : null),
                                    new AstIdentifier(token, "Sample")),
                                args[1].Value);
                    }
                    break;
            }

            return null;
        }

        AstExpression OptionalExpression(ParseContext context = ParseContext.Default, Precedence prec = 0)
        {
            return Expression(context, prec, null);
        }

        AstExpression OptionalExpression(ParseContext context, TokenType following, string expected = null)
        {
            var retval = Expression(context, 0, null);
            Scan(following, expected);
            return retval;
        }

        AstExpression Expression(ParseContext context, string expected, TokenType following)
        {
            var retval = Expression(context, 0, expected);
            Scan(following);
            return retval;
        }

        AstExpression Expression(ParseContext context, TokenType following1, TokenType following2)
        {
            var retval = Expression(context);
            Scan(following1);
            Scan(following2);
            return retval;
        }

        AstExpression Expression(ParseContext context = ParseContext.Default, Precedence prec = 0, string expected = "expression")
        {
            var old = _index;
            var token = _stack[old];
            AstExpression root;

            switch (token.Type)
            {
                case TokenType.EndOfFile:
                    break;
                case TokenType.Minus:
                {
                    switch (_stack[--_index].Type)
                    {
                        case TokenType.HexadecimalLiteral:
                        case TokenType.DecimalLiteral:
                        {
                            var value = Expression(ParseContext.Default, Precedence.Unary);
                            switch (value.ExpressionType)
                            {
                                case AstExpressionType.Zero:
                                    root = value;
                                    goto NEXT_EXPR;
                                case AstExpressionType.Int:
                                    root = new AstInt(token, -((AstInt) value).Value);
                                    goto NEXT_EXPR;
                                case AstExpressionType.UInt:
                                    root = ((AstUInt) value).Value == 0x80000000
                                        ? (AstExpression) new AstInt(token, -0x80000000)
                                        :                 new AstLong(token, -((AstUInt) value).Value);
                                    goto NEXT_EXPR;
                                case AstExpressionType.Long:
                                    root = new AstLong(token, -((AstLong) value).Value);
                                    goto NEXT_EXPR;
                                case AstExpressionType.ULong:
                                    if (((AstULong) value).Value == 0x8000000000000000)
                                    {
                                        root = new AstLong(token, -0x8000000000000000);
                                        goto NEXT_EXPR;
                                    }
                                    break;
                            }

                            root = new AstUnary(token, AstUnaryType.Negate, value);
                            goto NEXT_EXPR;
                        }
                        case TokenType.FloatLiteral:
                            root = new AstFloat(token, -((AstFloat) Expression(ParseContext.Default, Precedence.Unary)).Value);
                            goto NEXT_EXPR;
                        case TokenType.DoubleLiteral:
                            root = new AstDouble(token, -((AstDouble) Expression(ParseContext.Default, Precedence.Unary)).Value);
                            goto NEXT_EXPR;
                        default:
                            root = new AstUnary(token, AstUnaryType.Negate, Expression(context, Precedence.Primary));
                            goto NEXT_EXPR;
                    }
                }
                case TokenType.Plus:
                {
                    switch (_stack[--_index].Type)
                    {
                        case TokenType.HexadecimalLiteral:
                        case TokenType.DecimalLiteral:
                        case TokenType.FloatLiteral:
                        case TokenType.DoubleLiteral:
                            root = Expression(ParseContext.Default, Precedence.Unary);
                            goto NEXT_EXPR;
                        default:
                            root = new AstUnary(token, AstUnaryType.Promote, Expression(context, Precedence.Primary));
                            goto NEXT_EXPR;
                    }
                }
                case TokenType.Increase:
                case TokenType.Decrease:
                case TokenType.ExclamationMark:
                case TokenType.Tilde:
                {
                    --_index;
                    root = new AstUnary(token, token.UnaryPrefix, Expression(context, Precedence.Primary));
                    goto NEXT_EXPR;
                }
                case TokenType.LeftParen:
                {
                    old = --_index;
                    // First check for contextual syntax
                    if (context == ParseContext.MetaProperty)
                    {
                        root = OptionalMetaPropertyContextual();
                        if (root != null)
                        {
                            if (Optional(TokenType.RightParen))
                                goto NEXT_EXPR;

                            goto BAIL;
                        }
                    }

                    var typeToken = _stack[_index]; // Required for fixing #724 below
                    var type = OptionalType();

                    if (type != null && Optional(TokenType.RightParen))
                    {
                        // --- Solution to ticket #724 ----
                        //
                        // This should not parse as a cast to type 'a':
                        //     float b = (a) - 1.0f;
                        //
                        // To solve this we must look ahead and see if there is an unary operator following the cast.
                        // If there is, then the 'cdt' expression must be exactly one type alias, otherwise
                        // we treat this is not a cast.

                        switch (_stack[_index].Type)
                        {
                            case TokenType.Minus:
                            case TokenType.Plus:
                            case TokenType.ExclamationMark:
                            case TokenType.Tilde:
                                if (typeToken.Type < TokenType.Object ||
                                    typeToken.Type > TokenType.Framebuffer)
                                    goto BAIL;
                                break;
                        }

                        // --- End Solution ----

                        root = OptionalExpression(context, Precedence.Primary);
                        if (root == null)
                            goto BAIL;

                        root = new AstCast(type, root);
                        goto NEXT_EXPR;
                    }

                BAIL:
                    // Required for fixing #724 above
                    Backtrack(old);
                    root = OptionalExpression(context);
                    if (root != null && Optional(TokenType.RightParen))
                        goto NEXT_EXPR;

                    ParameterModifier modifier = 0;
                    for (var parameters = new List<AstParameter>();;)
                    {
                        if (root == null)
                        {
                            if (Optional(TokenType.Comma))
                            {
                                root = OptionalExpression(context);
                                continue;
                            }

                            if (!Optional(TokenType.RightParen))
                            {
                                if (modifier != 0)
                                {
                                    root = Expected(parameters.Count == 0 ? "')'" : "',' or ')'");
                                    goto NEXT_EXPR;
                                }

                                modifier = _stack[_index].ParameterModifier;

                                if (modifier != 0)
                                {
                                    --_index;
                                    root = OptionalExpression(context); // TODO
                                }
                            }

                            root = new AstLambda(Pop(TokenType.Lambda), new AstParameterList(token, parameters), LambdaBody(context));
                            goto NEXT_EXPR;
                        }

                        var name = OptionalIdentifier();

                        if (name == null)
                        {
                            name = root as AstIdentifier ?? Expected("identifier");
                            root = null;
                        }

                        parameters.Add(new AstParameter(AstAttribute.Empty, modifier, root, name));
                        root = OptionalExpression(context);
                        modifier = 0;
                    }
                }
                case TokenType.DecimalLiteral:
                {
                    --_index;
                    if (token.Length == 1)
                    {
                        root = _text[token.Offset] == '0'
                            ? (AstExpression) new AstSymbol(token, AstSymbolType.Zero)
                            :                 new AstInt(token, _text[token.Offset] - '0');
                        goto NEXT_EXPR;
                    }

                    var offset = token.Offset;
                    var end = offset + token.Length - 1;
                    var flags = GetIntegralFlags(ref end);

                    if (flags == 0 && token.Length < 10)
                    {
                        var value = 0;
                        for (var i = offset; i <= end; i++)
                        {
                            unchecked
                            {
                                value *= 10;
                                value += _text[i] - '0';
                            }
                        }

                        root = new AstInt(token, value);
                    }
                    else
                    {
                        const ulong Max = ulong.MaxValue / 10;
                        ulong value = 0;

                        for (var i = offset; i <= end; i++)
                        {
                            if (value > Max)
                                Error(token, ErrorCode.E1008, "Decimal too large (" + token.Value + ")");

                            unchecked
                            {
                                value *= 10;
                                value += (ulong) (_text[i] - '0');
                            }
                        }

                        root = Value(token, value, flags);
                    }

                    goto NEXT_EXPR;
                }
                case TokenType.HexadecimalLiteral:
                {
                    --_index;
                    var offset = token.Offset + 2;
                    var end = offset + token.Length - 3;
                    var flags = GetIntegralFlags(ref end);

                    if (flags == 0 && token.Length < 8)
                    {
                        var value = 0;
                        for (var i = offset; i <= end; i++)
                        {
                            unchecked
                            {
                                var c = _text[i];
                                value <<= 4;
                                value += c <= '9'
                                    ? c - '0'
                                    : c <= 'F'
                                        ? c - 'A' + 10
                                        : c - 'a' + 10;
                            }
                        }

                        root = new AstInt(token, value);
                    }
                    else
                    {
                        const ulong Max = ulong.MaxValue >> 4;
                        ulong value = 0;

                        for (var i = offset; i <= end; i++)
                        {
                            if (value > Max)
                                Error(token, ErrorCode.E1008, "Hexadecimal too large (" + token.Value + ")");

                            unchecked
                            {
                                var c = _text[i];
                                value <<= 4;
                                value += c <= '9'
                                    ? (ulong) (c - '0')
                                    : c <= 'F'
                                        ? (ulong) (c - 'A' + 10)
                                        : (ulong) (c - 'a' + 10);
                            }
                        }

                        root = Value(token, value, flags);
                    }
                    
                    goto NEXT_EXPR;
                }
                case TokenType.FloatLiteral:
                {
                    --_index;
                    root = Float(token, 1);
                    goto NEXT_EXPR;
                }
                case TokenType.DoubleLiteral:
                {
                    --_index;
                    var offset = token.Offset;
                    var length = token.Length;
                    switch (_text[offset + length - 1])
                    {
                        case 'd':
                        case 'D':
                            --length;
                            break;
                    }

                    double value;
                    if (!double.TryParse(_text.Substring(offset, length), NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                        Error(token, ErrorCode.E1008, "Invalid double (" + token.Value + ")");

                    root = new AstDouble(token, value);
                    goto NEXT_EXPR;
                }
                case TokenType.StringLiteral:
                {
                    --_index;
                    if (token.Length == 3)
                    {
                        root = new AstString(token, _text[token.Offset + 1].ToString());
                        goto NEXT_EXPR;
                    }

                    var sb = new StringBuilder(token.Length);
                    var end = token.EndOffset - 1;

                    for (var i = token.Offset + 1; i < end; i++)
                    {
                        var c = _text[i];
                        if (c == '\\')
                        {
                            var next = EscapeSequence(token, end, ref i);
                            if (next < 0x10000)
                                sb.Append((char) next);
                            else
                            {
                                next -= 0x10000;
                                sb.Append((char) (next / 0x400 + 0xD800));
                                sb.Append((char) (next % 0x400 + 0xDC00));
                            }
                        }
                        else
                            sb.Append(c);
                    }

                    root = new AstString(token, sb.ToString());
                    goto NEXT_EXPR;
                }
                case TokenType.CharLiteral:
                {
                    --_index;
                    if (token.Length == 3)
                    {
                        root = new AstChar(token, _text[token.Offset + 1]);
                        goto NEXT_EXPR;
                    }

                    var i = token.Offset + 1;
                    var c = _text[i];
                    if (c == '\\' && token.Length <= 12)
                    {
                        var ret = EscapeSequence(token, token.EndOffset - 1, ref i);
                        root = ret < 0x10000
                            ? (AstExpression) new AstChar(token, (char) ret)
                            :                 Error(token, ErrorCode.E1222,
                                                    "Unicode characters in the range U+10000 to U+10FFFF are not permitted in character literals");
                        goto NEXT_EXPR;
                    }

                    root = Error(token, ErrorCode.E1225, "Invalid character (" + token.Value + ")");
                    goto NEXT_EXPR;
                }
                case TokenType.AlphaExpression:
                {
                    --_index;
                    root = new AstMacro(token, token.Value);
                    goto NEXT_EXPR;
                }
                case TokenType.Null:
                case TokenType.Base:
                case TokenType.This:
                case TokenType.True:
                case TokenType.False:
                {
                    --_index;
                    root = new AstSymbol(token, token.SymbolType);
                    goto NEXT_EXPR;
                }
                case TokenType.New:
                {
                    --_index;
                    var type = OptionalNonArrayType();
                    if (type != null && Optional(TokenType.QuestionMark))
                        type = new AstUnary(type.Source, AstUnaryType.Nullable, type);

                    AstExpression arraySize = null;
                    List<AstExpression> collectionInitializer = null;
                    IReadOnlyList<AstArgument> args = null;
                    if (Optional(TokenType.LeftSquareBrace))
                    {
                        if (type != null)
                        {
                            type = new AstUnary(type.Source, AstUnaryType.Array, type);
                            arraySize = OptionalExpression(context);
                        }

                        for (;;)
                        {
                            Scan(TokenType.RightSquareBrace);
                            if (type == null ||
                                    !Optional(TokenType.LeftSquareBrace))
                                break;

                            type = new AstUnary(type.Source, AstUnaryType.Array, type);
                        }
                    }
                    else if (type != null)
                        args = OptionalArgumentList(context);

                    if (Optional(TokenType.LeftCurlyBrace))
                    {
                        for (collectionInitializer = new List<AstExpression>();;)
                        {
                            if (Optional(TokenType.RightCurlyBrace))
                                break;

                            token = _stack[_index];
                            if (Optional(TokenType.LeftCurlyBrace))
                                collectionInitializer.Add(new AstArrayInitializer(token, InnerArrayInitializer(context)));
                            else
                            {
                                AstExpression exp = null;
                                if (_stack[_index - 0].Type == TokenType.Identifier &&
                                    _stack[_index - 1].Type == TokenType.Assign &&
                                    _stack[_index - 2].Type == TokenType.LeftCurlyBrace)
                                {
                                    var id = Identifier();
                                    _index -= 2;
                                    exp = new AstBinary(AstBinaryType.Assign, id, token,
                                        new AstArrayInitializer(token, InnerArrayInitializer(context)));
                                }

                                collectionInitializer.Add(exp ?? Expression(context));
                            }

                            if (Optional(TokenType.Comma))
                            {
                                if (Optional(TokenType.RightCurlyBrace))
                                    break;
                            }
                            else if (Optional(TokenType.RightCurlyBrace))
                                break;
                            else
                            {
                                Expected("',' or '}'");
                                break;
                            }
                        }
                    }

                    root = new AstNew(token, type, arraySize, args, collectionInitializer);
                    goto NEXT_EXPR;
                }
                case TokenType.Import:
                {
                    --_index;
                    switch (_stack[_index].Type)
                    {
                        case TokenType.LeftParen:
                            --_index;
                            root = new AstImport(token, Expression());
                            Scan(TokenType.RightParen);
                            goto NEXT_EXPR;
                        default:
                            root = new AstImport(token, Type(), ArgumentList(context));
                            goto NEXT_EXPR;
                    }
                }
                case TokenType.Defined:
                {
                    --_index;
                    root = new AstDefined(token, ConditionString());
                    goto NEXT_EXPR;
                }
                case TokenType.Extern:
                {
                    --_index;
                    root = new AstExtern(token,
                        AstAttribute.Empty,
                        Optional(TokenType.LessThan)
                            ? Type(TokenType.GreaterThan)
                            : null,
                        OptionalArgumentList(context),
                        StringValue());
                    goto NEXT_EXPR;
                }
                case TokenType.LeftSquareBrace:
                {
                    --_index;
                    var attrs = InnerAttributeList();
                    AstExpression type = null;
                    IReadOnlyList<AstArgument> args = null;

                    token = _stack[_index];
                    if (token.Type == TokenType.Extern)
                    {
                        _index--;
                        if (Optional(TokenType.LessThan))
                            type = Type(TokenType.GreaterThan);
                        args = OptionalArgumentList(context);
                    }

                    root = new AstExtern(token,
                        attrs, type, args,
                        StringValue());
                    goto NEXT_EXPR;
                }
                case TokenType.Checked:
                case TokenType.Unchecked:
                case TokenType.Unsafe:
                case TokenType.Default:
                case TokenType.NameOf:
                case TokenType.SizeOf:
                case TokenType.TypeOf:
                {
                    --_index;
                    root = new AstUnary(token, token.UnaryPrefix, Conditional(context, "operand"));
                    goto NEXT_EXPR;
                }
                case TokenType.Identifier:
                {
                    if (token.IsValue(Tokens.Local) &&
                        _stack[_index - 1].Type == TokenType.DoubleColon)
                    {
                        _index -= 2;
                        root = new AstLocal(Identifier());
                        goto NEXT_EXPR;
                    }

                    root = context == ParseContext.MetaProperty
                        ? OptionalMetaPropertyContextual() ?? OptionalNonArrayType()
                        : OptionalNonArrayType();
                    if (root == null)
                        break;
                    goto NEXT_EXPR;
                }
                case TokenType.ReadOnly:
                case TokenType.Volatile:
                {
                    root = context == ParseContext.MetaProperty
                        ? OptionalMetaPropertyContextual()
                        : null;
                    if (root == null)
                        break;
                    goto NEXT_EXPR;
                }
                default:
                {
                    root = OptionalNonArrayType();
                    if (root == null)
                        break;
                    goto NEXT_EXPR;
                }
            }

            return expected != null
                ? Expected(expected)
                : null;

        NEXT_EXPR:
            token = _stack[_index];
            switch (token.Type)
            {
                case TokenType.Increase:
                case TokenType.Decrease:
                {
                    --_index;
                    root = new AstUnary(token, token.UnaryPostfix, root);
                    goto NEXT_EXPR;
                }
                case TokenType.LeftParen:
                {
                    --_index;
                    root = new AstCall(AstCallType.Function, root, InnerArgumentList(context));
                    goto NEXT_EXPR;
                }
                case TokenType.LeftSquareBrace:
                {
                    --_index;
                    root = Optional(TokenType.RightSquareBrace)
                        ? (AstExpression) new AstUnary(token, AstUnaryType.Array, root)
                        :                 new AstCall(AstCallType.LookUp, root, InnerArgumentList(context, TokenType.RightSquareBrace));
                    goto NEXT_EXPR;
                }
                case TokenType.DoubleColon:
                case TokenType.Period:
                {
                    if (Precedence.Primary < prec)
                        return root;

                    --_index;
                    root = new AstMember(root, Identifier());

                    // This might be a parameterized generic member
                    root = InnerType(root);
                    goto NEXT_EXPR;
                }
                case TokenType.Lambda:
                {
                    if (Precedence.Assignment < prec)
                        return root;

                    --_index;
                    if (root.ExpressionType == AstExpressionType.Identifier)
                    {
                        var i = (AstIdentifier) root;
                        var ps = new AstParameterList(i.Source, new[] {new AstParameter(null, 0, null, i)});
                        root = new AstLambda(token, ps, LambdaBody(context));
                    }
                    else
                    {
                        root = Expected("Identifier or parameter list");
                    }

                    goto NEXT_EXPR;
                }
                case TokenType.Is:
                case TokenType.As:
                {
                    if (Precedence.Relational < prec)
                        return root;

                    --_index;
                    root = new AstBinary(token.BinaryType, root, token,
                        Type(token.PrecedencePlusAssociativity));
                    goto NEXT_EXPR;
                }
                case TokenType.Comma:
                case TokenType.AddAssign:
                case TokenType.MinusAssign:
                case TokenType.MulAssign:
                case TokenType.DivAssign:
                case TokenType.ModAssign:
                case TokenType.BitwiseAndAssign:
                case TokenType.BitwiseOrAssign:
                case TokenType.BitwiseXorAssign:
                case TokenType.ShlAssign:
                case TokenType.ShrAssign:
                case TokenType.LogAndAssign:
                case TokenType.LogOrAssign:
                case TokenType.LogAnd:
                case TokenType.LogOr:
                case TokenType.Equal:
                case TokenType.NotEqual:
                case TokenType.GreaterOrEqual:
                case TokenType.LessOrEqual:
                case TokenType.Shl:
                case TokenType.Shr:
                case TokenType.Assign:
                case TokenType.Plus:
                case TokenType.Minus:
                case TokenType.Mul:
                case TokenType.Div:
                case TokenType.Mod:
                case TokenType.LessThan:
                case TokenType.GreaterThan:
                case TokenType.BitwiseXor:
                case TokenType.BitwiseOr:
                case TokenType.BitwiseAnd:
                case TokenType.DoubleQuestionMark:
                {
                    if (token.Precedence < prec)
                        return root;

                    --_index;
                    root = new AstBinary(token.BinaryType, root, token,
                        Expression(context, token.PrecedencePlusAssociativity));
                    goto NEXT_EXPR;
                }
                case TokenType.QuestionMark:
                {
                    if (Precedence.Conditional < prec)
                        return root;

                    old = --_index;
                    var e = OptionalExpression(context);

                    if (e == null || !Optional(TokenType.Colon))
                    {
                        Backtrack(old);
                        root = new AstUnary(token, AstUnaryType.Nullable, root);
                    }
                    else
                        root = new AstTernary(root, token, e,
                            Expression(context, Precedence.Conditional));

                    goto NEXT_EXPR;
                }
                default:
                    return root;
            }
        }

        public AstExpression Type(Precedence prec = 0, string expected = "type")
        {
            return OptionalType(prec) ?? Expected(expected);
        }

        public AstExpression Type(TokenType following)
        {
            var retval = Type();
            Scan(following);
            return retval;
        }

        public AstExpression OptionalType(Precedence prec = 0)
        {
            var token = _stack[_index];
            switch (token.Type)
            {
                case TokenType.Void:
                    --_index;
                    return new AstSymbol(token, AstSymbolType.Void);
                case TokenType.Fixed:
                    --_index;
                    return InnerFixedArray(token);
                default:
                    return OptionalContextual(Tokens.Var)
                        ? new AstSymbol(token, AstSymbolType.Var)
                        : OptionalTypeInternal(prec);
            }
        }

        AstExpression OptionalTypeInternal(Precedence prec = 0)
        {
            var type = OptionalNonArrayType();

            if (type == null)
                return null;

            var token = _stack[_index];
            if (token.Type == TokenType.QuestionMark)
            {
                if (Precedence.Conditional < prec)
                    return type;

                --_index;
                type = new AstUnary(token, AstUnaryType.Nullable, type);
                token = _stack[_index];
            }

            while (_stack[_index].Type == TokenType.LeftSquareBrace && 
                   _stack[_index - 1].Type == TokenType.RightSquareBrace)
            {
                _index -= 2;
                type = new AstUnary(token, AstUnaryType.Array, type);
            }

            return type;
        }

        AstExpression InnerType(AstExpression root)
        {
        BEGIN:
            var old = _index;
            if (Optional(TokenType.LessThan))
            {
                var argCount = 1;
                while (Optional(TokenType.Comma))
                    argCount++;

                if (Optional(TokenType.GreaterThan))
                    return new AstGeneric(root, argCount);

                if (argCount > 1)
                {
                    Expected("'>'");
                    return root;
                }

                var args = new List<AstExpression>();
            TOP:
                var type = OptionalType();
                if (type == null)
                {
                    Backtrack(old);
                    return root;
                }

                args.Add(type);
                if (Optional(TokenType.Comma))
                    goto TOP;

                // Expand '>>' to '>' '>'
                if (_stack[_index].Type == TokenType.Shr)
                {
                    var shr = _stack[_index];
                    _stack[  _index] = shr.SubToken(TokenType.GreaterThan, 1, 1);
                    _stack[++_index] = shr.SubToken(TokenType.GreaterThan, 0, 1);
                }

                if (!Optional(TokenType.GreaterThan))
                {
                    Backtrack(old);
                    return root;
                }

                root = new AstParameterizer(root, args);
            }

            switch (_stack[_index].Type)
            {
                case TokenType.DoubleColon:
                case TokenType.Period:
                    --_index;
                    var id = OptionalIdentifier();

                    if (id == null)
                    {
                        Backtrack(old);
                        return root;
                    }

                    root = new AstMember(root, id);
                    goto BEGIN;
            }

            return root;
        }

        public AstExpression OptionalNonArrayType()
        {
            var token = _stack[_index];

            switch (token.Type)
            {
                case TokenType.Object:
                case TokenType.Bool:
                case TokenType.Float:
                case TokenType.Double:
                case TokenType.Float2:
                case TokenType.Float2x2:
                case TokenType.Float3:
                case TokenType.Float4:
                case TokenType.Float3x3:
                case TokenType.Float4x4:
                case TokenType.Int2:
                case TokenType.Int3:
                case TokenType.Int4:
                case TokenType.Byte2:
                case TokenType.Byte4:
                case TokenType.SByte2:
                case TokenType.SByte4:
                case TokenType.Short2:
                case TokenType.Short4:
                case TokenType.UShort2:
                case TokenType.UShort4:
                case TokenType.Char:
                case TokenType.String:
                case TokenType.Byte:
                case TokenType.UShort:
                case TokenType.UInt:
                case TokenType.ULong:
                case TokenType.SByte:
                case TokenType.Short:
                case TokenType.Int:
                case TokenType.Long:
                case TokenType.Texture2D:
                case TokenType.TextureCube:
                case TokenType.Sampler2D:
                case TokenType.SamplerCube:
                case TokenType.Framebuffer:
                    --_index;
                    return InnerType(new AstBuiltinType(token, token.BuiltinType));
                case TokenType.Identifier:
                    --_index;

                    if (_stack[_index].Type == TokenType.DoubleColon && 
                        token.IsValue(Tokens.Global))
                    {
                        --_index;
                        return InnerType(new AstMember(new AstSymbol(token, AstSymbolType.Global), 
                            Identifier()));
                    }

                    return InnerType(Identifier(token));
                default:
                    return null;
            }
        }

        AstFixedArray InnerFixedArray(Token token)
        {
            var typeToken = _stack[_index];
            var type = OptionalTypeInternal() ?? Expected("type");
            return _stack[_index].Type == TokenType.LeftSquareBrace
                ? new AstFixedArray(token, type, ArraySize(ParseContext.Default, "fixed array size"))
                : new AstFixedArray(token, (type as AstUnary)?.Operand ?? Expected(typeToken, "array"));
        }

        // Syntax: <non-array-type> '.'
        public AstExpression OptionalExplicitInterface()
        {
            var old = _index;
            switch (_stack[old].Type)
            {
                case TokenType.Colon:
                case TokenType.Semicolon:
                case TokenType.Operator:
                    return null;
            }

            switch (_stack[old - 1].Type)
            {
                case TokenType.LeftCurlyBrace:
                case TokenType.LeftParen:
                case TokenType.LeftSquareBrace:
                case TokenType.Semicolon:
                case TokenType.Assign:
                case TokenType.Comma:
                case TokenType.Colon:
                    return null;
                case TokenType.DoubleColon:
                    if (_stack[old].IsValue(Tokens.Global))
                    {
                        _index -= 2;
                        return OptionalExplicitInterface_inner(old, null, 
                            new AstMember(
                                new AstSymbol(_stack[old], AstSymbolType.Global), 
                                Identifier())) ?? Expected("type");
                    }
                    break;
            }

            var id = OptionalIdentifier();
            return id == null
                ? null
                : OptionalExplicitInterface_inner(old, null, id);
        }

        AstExpression OptionalExplicitInterface_inner(int old, AstExpression oldExpression, AstExpression baseExpression)
        {
            switch (_stack[_index].Type)
            {
                case TokenType.DoubleColon:
                case TokenType.Period:
                    switch (_stack[--_index].Type)
                    {
                        case TokenType.This:
                        case TokenType.Identifier:
                            switch (_stack[_index - 1].Type)
                            {
                                case TokenType.LeftParen:
                                case TokenType.LeftCurlyBrace:
                                case TokenType.LeftSquareBrace:
                                    return baseExpression;
                            }

                            break;
                    }

                    old = _index;
                    var id = OptionalIdentifier();
                    return id == null
                        ? baseExpression
                        : OptionalExplicitInterface_inner(old, baseExpression, new AstMember(baseExpression, id));
            }

            if (Optional(TokenType.LessThan))
            {
                for (var types = new List<AstExpression>();;)
                {
                    var pt = OptionalType();
                    if (pt != null)
                    {
                        types.Add(pt);
                        if (Optional(TokenType.Comma))
                            continue;
                    }

                    Scan(TokenType.GreaterThan);
                    return OptionalExplicitInterface_inner(old, oldExpression, new AstParameterizer(baseExpression, types));
                }
            }

            Backtrack(old);
            return oldExpression;
        }

        AstFloat Float(Token token, int suffixLength)
        {
            float value;
            if (!float.TryParse(_text.Substring(token.Offset, token.Length - suffixLength), 
                    NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                Error(token, ErrorCode.E1008, "Invalid float (" + token.Value + ")");
            return new AstFloat(token, value);
        }

        AstFloat HexFloat1(Token token, int offset)
        {
            unchecked
            {
                var c = _text[token.Offset + offset];
                var d = c <= '9'
                    ? c - '0'
                    : c <= 'F'
                        ? c - 'A' + 10
                        : c - 'a' + 10;
                return new AstFloat(token, d / 15.0f);
            }
        }

        AstFloat HexFloat2(Token token, int offset)
        {
            unchecked
            {
                var c0 = _text[token.Offset + offset];
                var c1 = _text[token.Offset + offset + 1];
                var d0 = c0 <= '9'
                    ? c0 - '0'
                    : c0 <= 'F'
                        ? c0 - 'A' + 10
                        : c0 - 'a' + 10;
                var d1 = c1 <= '9'
                    ? c1 - '0'
                    : c1 <= 'F'
                        ? c1 - 'A' + 10
                        : c1 - 'a' + 10;
                return new AstFloat(token, (d0 << 4 | d1) / 255.0f);
            }
        }

        AstInvalid Expected(string what)
        {
            return Expected(_stack[_index], what);
        }

        AstInvalid Expected(Token token, string what)
        {
            for (int i = _index,
                     l = Math.Min(_stack.Length, _index + 64);
                 i < l;
                 ++i)
            {
                if (_stack[_index] != token)
                    continue;

                what += " following " + _stack[_index + 1].Value.Printable();
                break;
            }

            return Error(token, ErrorCode.E0000, 
                "Expected " + what + " -- found " + (
                    token.Type == TokenType.EndOfFile
                        ? "<EOF>"
                        : token.Value.Printable()
                ) + " (" + (
                    token.Type == TokenType.Invalid && token == _stack[End + 1] ||
                            token.Type == TokenType.EndOfFile
                        ? "unexpected end-of-file"
                        : token.Type.ToString()
                ) + ")");
        }

        AstInvalid Error(Source src, ErrorCode code, string msg)
        {
            if (!HasErrors || _index > End)
            {
                Log.Error(src, code, msg);
                HasErrors = true;
                _errorIndex = _index - 1;
                _index = End;
            }

            return new AstInvalid(src);
        }

        void Backtrack(int old)
        {
            if (_index > End || !HasErrors)
                _index = old;
        }

        void Recover()
        {
            _index = _errorIndex;
            _errorIndex = 0;
        }

        string DocComment()
        {
            while (_commentIndex >= 0 &&
                   _comments[_commentIndex].Offset < _commentOffset)
                --_commentIndex;

            if (_commentIndex < 0)
                return null;

            var c = _comments[_commentIndex];
            var offset = _stack[_index].Offset;

            if (c.Offset > offset)
                return null;

            while (_commentIndex > 0 &&
                   _comments[_commentIndex - 1].Offset < offset)
                --_commentIndex;

            c = _comments[_commentIndex];
            return SmartTrim(c.Offset, c.Length);
        }

        AstExpression Value(Token token, ulong u, IntegralFlags flags)
        {
            return u <= int.MaxValue && (flags & IntegralFlags.LongUnsigned) == 0
                ? new AstInt(token, (int) u)
                : u <= uint.MaxValue && (flags & IntegralFlags.Long) == 0
                    ? new AstUInt(token, (uint) u)
                    : u <= long.MaxValue && (flags & IntegralFlags.Unsigned) == 0
                        ? (AstExpression) new AstLong(token, (long) u)
                        :                 new AstULong(token, u);
        }

        IntegralFlags GetIntegralFlags(ref int end)
        {
            switch (_text[end])
            {
                case 'l':
                case 'L':
                    --end;
                    switch (_text[end])
                    {
                        case 'u':
                        case 'U':
                            --end;
                            return IntegralFlags.LongUnsigned;
                        default:
                            return IntegralFlags.Long;
                    }
                case 'u':
                case 'U':
                    --end;
                    switch (_text[end])
                    {
                        case 'l':
                        case 'L':
                            --end;
                            return IntegralFlags.LongUnsigned;
                        default:
                            return IntegralFlags.Unsigned;
                    }
            }

            return 0;
        }

        [Flags]
        enum IntegralFlags
        {
            Long = 1 << 0,
            Unsigned = 1 << 1,
            LongUnsigned = Long | Unsigned
        }
    }
}
