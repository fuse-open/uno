using System;
using System.Collections.Generic;
using Uno.UX.Markup.UXIL.Expressions.Lexer;

namespace Uno.UX.Markup.UXIL.Expressions
{
    class Parser
    {
        readonly List<Token> _tokens;
        int _pos;

        TokenType Peek(int offset = 0)
        {
            if (_tokens.Count <= _pos + offset) return TokenType.EndOfFile;

            return PeekToken(offset).Type;
        }
        Token PeekToken(int offset = 0)
        {
            if (_tokens.Count <= _pos + offset) return null;
            return _tokens[_pos + offset];
        }
        Token Consume(TokenType t = TokenType.Invalid)
        {
            if (t != TokenType.Invalid && Peek() != t)
            {
                Error("Expected " + t);
            }

            _pos++;
            return _tokens[_pos - 1];
        }
        bool TryConsume(TokenType t)
        {
            if (Peek() != t) return false;
            _pos++;
            return true;
        }
        void Error(string message)
        {
            var lastFewTokens = "";
            int c = 0;
            for (var i = _pos; i --> 0;)
            {
                if (c++ > 4) break;
                lastFewTokens = _tokens[i].Value + lastFewTokens;
            }

            throw new Exception(message + ", at end of '" + lastFewTokens + "'");
        }
        bool ExistsLater(TokenType t)
        {
            for (var i = _pos; i < _tokens.Count; i++)
            {
                if (_tokens[i].Type == t) return true;
            }
            return false;
        }
        Expression Expect(Expression e)
        {
            if (e == null)
                Error("Expected expression after " + Peek(-1));
            return e;
        }

        Parser(Uno.Compiler.SourceFile file, string code)
        {
            _tokens = Lexer.Lexer.Tokenize(file, code);
        }

        public static Expression Parse(Uno.Compiler.SourceFile file, string code, bool stringMode)
        {
            if (stringMode)
            {
                if (string.IsNullOrEmpty(code)) return new StringLiteral("");
                return ParseStringExpression(file, code);
            }
            else
            {
                if (string.IsNullOrEmpty(code)) throw new Exception("Expression cannot be empty");
                return new Parser(file, code).ParseCompleteSourceAsExpression();
            }
        }

        Expression ParseCompleteSourceAsExpression()
        {
            var e = ParseExpression(Precedence.Invalid, true);

            if (Peek() != TokenType.EndOfFile)
            {
                Error("Expected end of expression, found '" + PeekToken().Value + "'");
            }

            return e;
        }

        static Expression ParseStringExpression(Uno.Compiler.SourceFile file, string code)
        {
            if (string.IsNullOrEmpty(code)) return null;

            var parts = new List<Expression>();
            int p = 0;

            for (int i = 0; i < code.Length; i++)
            {
                if (code[i] == '{' && (i == 0 || code[i - 1] != '\\'))
                {
                    if (i != p) parts.Add(new StringLiteral(code.Substring(p, i - p)));

                    // Skip to end of binding expression
                    int start = i;
                    i++;
                    for (int scopes = 1; scopes > 0 && i < code.Length; i++)
                    {
                        if (code[i] == '}' && code[i - 1] != '\\') scopes--;
                        if (code[i] == '{' && code[i - 1] != '\\') scopes++;
                    }

                    var binding = code.Substring(start, i - start);
                    var be = Parse(new Uno.Compiler.SourceFile(file.FullPath, binding), binding, false);
                    if (be == null) throw new Exception("Expected binding expression after '{'");
                    parts.Add(be);
                    p = i;
                }
            }
            if (p < code.Length)
            {
                parts.Add(new StringLiteral(code.Substring(p, code.Length - p)));
            }

            return parts.Count > 1
                ? AddExpression.Create(parts.ToArray())
                : parts[0];
        }

        Expression ParseExpression(Precedence p, bool vectorMode)
        {
            var e = ParseUnaryExpression(vectorMode);

            while (e != null)
            {
                var op = Peek();

                if (op == TokenType.Period)
                {
                    Consume();
                    var id = Consume();
                    if (!IsIdentifier(id))
                        Error("Expected identifier after '.'");

                    e = new MemberExpression(e, id.Value);
                    continue;
                }

                if (op == TokenType.LeftSquareBrace)
                {
                    Consume();
                    var index = ParseExpression(Precedence.Invalid, true);
                    if (Peek() != TokenType.RightSquareBrace)
                        Error("Expected ]");

                    Consume();

                    e = new LookUpExpression(e, index);
                    continue;
                }

                if (p < Precedence.Assignment)
                {
                    if (TryConsume(TokenType.Colon))
                    {
                        e = new NameValuePairExpression(e, Expect(ParseExpression(Precedence.Assignment, false)));
                        continue;
                    }
                }

                if (p < Precedence.Conditional)
                {
                    if (TryConsume(TokenType.QuestionMark))
                    {
                        var case1 = Expect(ParseExpression(Precedence.Conditional, false));

                        if (!TryConsume(TokenType.Colon))
                            Error("Expected ':'");

                        var case2 = Expect(ParseExpression(Precedence.Conditional, false));

                        e = new ConditionalExpression(e, case1, case2);
                        continue;
                    }
                }

                if (p < Precedence.Multiplicative)
                {
                    if (TryConsume(TokenType.Mul))
                    {
                        e = new MultiplyExpression(e, Expect(ParseExpression(Precedence.Multiplicative, false)));
                        continue;
                    }
                    if (TryConsume(TokenType.Div))
                    {
                        e = new DivideExpression(e, Expect(ParseExpression(Precedence.Multiplicative, false)));
                        continue;
                    }
                }

                if (p < Precedence.Additive)
                {
                    if (TryConsume(TokenType.Plus))
                    {
                        e = new AddExpression(e, Expect(ParseExpression(Precedence.Additive, false)));
                        continue;
                    }
                    if (TryConsume(TokenType.Minus))
                    {
                        e = new SubtractExpression(e, Expect(ParseExpression(Precedence.Additive, false)));
                        continue;
                    }
                }

                if (p < Precedence.LogicalAnd)
                {
                    if (TryConsume(TokenType.LogAnd))
                    {
                        e = new LogicalAndExpression(e, Expect(ParseExpression(Precedence.LogicalAnd, false)));
                        continue;
                    }
                }

                if (p < Precedence.LogicalOr)
                {
                    if (TryConsume(TokenType.LogOr))
                    {
                        e = new LogicalOrExpression(e, Expect(ParseExpression(Precedence.LogicalOr, false)));
                        continue;
                    }
                }

                if (p < Precedence.Relational)
                {
                    if (TryConsume(TokenType.LessThan))
                    {
                        e = new LessThanExpression(e, Expect(ParseExpression(Precedence.Relational, false)));
                        continue;
                    }
                    if (TryConsume(TokenType.LessOrEqual))
                    {
                        e = new LessOrEqualExpression(e, Expect(ParseExpression(Precedence.Relational, false)));
                        continue;
                    }
                    if (TryConsume(TokenType.GreaterThan))
                    {
                        e = new GreaterThanExpression(e, Expect(ParseExpression(Precedence.Relational, false)));
                        continue;
                    }
                    if (TryConsume(TokenType.GreaterOrEqual))
                    {
                        e = new GreaterOrEqualExpression(e, Expect(ParseExpression(Precedence.Relational, false)));
                        continue;
                    }
                    if (TryConsume(TokenType.NotEqual))
                    {
                        e = new NotEqualExpression(e, Expect(ParseExpression(Precedence.Relational, false)));
                        continue;
                    }
                    if (TryConsume(TokenType.Equal))
                    {
                        e = new EqualExpression(e, Expect(ParseExpression(Precedence.Relational, false)));
                        continue;
                    }
                }

                if (p < Precedence.NullCoalescing)
                {
                    if (TryConsume(TokenType.DoubleQuestionMark))
                    {
                        e = new NullCoalesceExpression(e, Expect(ParseExpression(Precedence.NullCoalescing, false)));
                    }
                }

                // Vector op
                if (vectorMode && p == Precedence.Invalid && Peek() == TokenType.Comma)
                {
                    var comps = new List<Expression>();
                    comps.Add(e);
                    while (TryConsume(TokenType.Comma))
                    {
                        e = Expect(ParseExpression(Precedence.Invalid, false));
                        comps.Add(e);
                    }
                    e = new VectorExpression(comps.ToArray()).TryFold();
                }

                break;
            }

            return e;
        }


        Expression ParseUnaryExpression(bool vectorMode)
        {
            if (IsIdentifier(PeekToken()) && Peek(1) == TokenType.LeftParen && ExistsLater(TokenType.RightParen))
            {
                var funcName = Consume().Value;
                Consume(TokenType.LeftParen);

                var args = new List<Expression>();

                while (Peek() != TokenType.RightParen)
                {
                    var e = Expect(ParseExpression(Precedence.Invalid, false));
                    args.Add(e);

                    if (TryConsume(TokenType.Comma)) continue;
                    if (Peek() != TokenType.RightParen)
                    {
                        Error("Expected ',' or ')'");
                        return null;
                    }
                }

                Consume(TokenType.RightParen);

                return new FunctionCallExpression(funcName, args.ToArray());
            }
            if (Peek() == TokenType.ExclamationMark)
            {
                Consume();
                var e = Expect(ParsePrimaryExpression());
                return new LogicalNotExpression(e);
            }
            if (Peek() == TokenType.Minus)
            {
                Consume();
                var e = Expect(ParsePrimaryExpression());

                var vl = e as Literal;
                if (vl != null)
                {
                    return new Literal("-" + vl.Value);
                }

                return new NegateExpression(e);
            }
            else
            {
                var e = ParsePrimaryExpression();

                if (Peek() == TokenType.Mod)
                {
                    Consume();
                    return new MultiplyExpression(new Literal("1%"), e);
                }
                if (Peek() == TokenType.Identifier && PeekToken().Value == "px")
                {
                    Consume();
                    return new MultiplyExpression(new Literal("1px"), e);
                }
                if (Peek() == TokenType.Identifier && PeekToken().Value == "pt")
                {
                    Consume();
                    return new MultiplyExpression(new Literal("1pt"), e);
                }

                return e;
            }
        }

        Expression ParsePrimaryExpression()
        {
            if (TryConsume(TokenType.LeftParen))
            {
                var e = Expect(ParseExpression(Precedence.Invalid, true));
                Consume(TokenType.RightParen);
                return e;
            }

            if (TryConsume(TokenType.This))
                return new ThisExpression();

            var literal = ParseLiteral();
            if (literal != null)
                return literal;

            if (IsIdentifier(PeekToken()))
            {
                var id = Consume();

                if (IsUnaryArgument(PeekToken()))
                {
                    // ReadClear[x]/WriteClear[x]/... expr
                    var modeExpressionPrefixes = new Dictionary<string, Modifier>
                    {
                        // Note that item order is significant; earlier entries take precedence over later entries.
                        //  This is important for ambiguous entries like Read/ReadClear, Write/WriteClear, ...
                        { "ReadClear", Modifier.Read | Modifier.Clear },
                        { "WriteClear", Modifier.Write | Modifier.Clear },
                        { "Clear", Modifier.Clear | Modifier.Read | Modifier.Write },
                        { "Read", Modifier.Read },
                        { "Write", Modifier.Write }
                    };

                    foreach (var kvp in modeExpressionPrefixes)
                    {
                        var prefix = kvp.Key;
                        if (!id.Value.StartsWith(prefix))
                            continue;

                        var uu = ParseUserDefinedUnary(id.Value.Substring(prefix.Length));
                        if (uu != null)
                            return new ModeExpression(uu, kvp.Value);
                    }

                    // Snapshot[X] expr
                    const string snapshotPrefix = "Snapshot";
                    if (id.Value.StartsWith(snapshotPrefix))
                    {
                        var uu = ParseUserDefinedUnary(id.Value.Substring(snapshotPrefix.Length));
                        if (uu != null)
                            return new UserDefinedUnaryOperator(snapshotPrefix, uu);
                    }

                    // Property expr
                    var u = TryParseUserDefinedUnary(id.Value);
                    if (u != null)
                        return u;
                }

                return new Identifier(id.Value);
            }

            return ParseCurlyExpression();
        }

        bool IsUnaryArgument(Token t)
        {
            return IsIdentifier(t) || (t.Type == TokenType.This) || (t.Type == TokenType.LeftCurlyBrace);
        }

        Expression ParseUserDefinedUnary(string unary)
        {
            return TryParseUserDefinedUnary(unary) ?? ParseExpression(Precedence.Invalid, true);
        }

        Expression TryParseUserDefinedUnary(string unary)
        {
            return !string.IsNullOrEmpty(unary)
                ? new UserDefinedUnaryOperator(unary, Expect(ParseExpression(Precedence.Invalid, true)))
                : null;
        }
        
        Expression ParseLiteral()
        {
            var num = ParseNumericLiteral();
            if (num != null) return num;

            switch (Peek())
            {
                case TokenType.ColorCodeLiteral:
                    break;

                case TokenType.StringLiteral:
                    return new StringLiteral(Consume().Value.Trim('\"'));

                case TokenType.CharLiteral:
                    return new StringLiteral(Consume().Value.Trim('\''));

                case TokenType.True:
                case TokenType.False:
                case TokenType.This:
                    return new Literal(Consume().Value);

                default: return null;
            }

            return new Literal(Consume().Value);
        }

        Literal ParseNumericLiteral()
        {
            switch (Peek())
            {
                case TokenType.DoubleLiteral:
                case TokenType.FloatLiteral:
                case TokenType.DecimalLiteral:
                case TokenType.PointLiteral:
                case TokenType.PixelLiteral:
                    break;
                default: return null;
            }

            return new Literal(
                Peek(1) == TokenType.Mod
                    ? Consume().Value + Consume().Value
                    : Consume().Value);
        }

        bool IsIdentifier(Token token)
        {
            return
                token.Type == TokenType.Identifier ||
                token.Value.Length > 0 && (char.IsLetter(token.Value[0]) || token.Value[0] == '_');
        }

        Expression ParseCurlyExpression()
        {
            if (Peek() != TokenType.LeftCurlyBrace) return null;

            if (!ExistsLater(TokenType.RightCurlyBrace))
            {
                Error("Missing matching '}' after '{'");
                return null;
            }

            Consume();

            if (Peek() == TokenType.RightCurlyBrace)
            {
                Consume();

                // This handles the special case of <Text Value="{}" />
                return new Binding(new Identifier(""));
            }

            if (Peek() == TokenType.Assign)
            {
                Consume();
                var e = Expect(ParseExpression(Precedence.Invalid, true));
                Consume(TokenType.RightCurlyBrace);
                return new RawExpression(e);
            }
            else 
            {
                var e = Expect(ParseExpression(Precedence.Invalid, true));

                if (Peek() != TokenType.RightCurlyBrace)
                {
                    if (e is Identifier)
                    {
                        Error("'" + ((Identifier)e).Name + "' is not a valid UX operator (not found)");
                        return null;
                    }
                }

                Consume(TokenType.RightCurlyBrace);
                return e is Binding
                    ? e
                    : new Binding(e);
            }
        }
    }
}
