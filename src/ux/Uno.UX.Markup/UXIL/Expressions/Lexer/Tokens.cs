using System.Collections.Generic;
using System.Reflection;

namespace Uno.UX.Markup.UXIL.Expressions.Lexer
{
    public static class Tokens
    {
        public static bool IsReserved(string str)
        {
            return Reserved.ContainsKey(str);
        }

        internal static readonly Dictionary<string, TokenType> Reserved = GetReserved();

        // This is called on type initialization, so we use this to initialize our arrays.
        static Dictionary<string, TokenType> GetReserved()
        {
            var reserved = new Dictionary<string, TokenType>();

            foreach (var m in typeof(TokenType).GetMembers())
            {
                foreach (var e in m.GetCustomAttributes(typeof(TokenAttribute), false))
                {
                    var a = (TokenAttribute)e;
                    var t = (TokenType)((FieldInfo)m).GetValue(null);
                    reserved[a.Value] = t;
                }
            }

            return reserved;
        }
    }
}
