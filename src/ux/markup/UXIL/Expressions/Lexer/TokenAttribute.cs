using System;

namespace Uno.UX.Markup.UXIL.Expressions.Lexer
{
    public class TokenAttribute : Attribute
    {
        public readonly string Value;

        public TokenAttribute(string value)
        {
            Value = value;
        }
    }
}
