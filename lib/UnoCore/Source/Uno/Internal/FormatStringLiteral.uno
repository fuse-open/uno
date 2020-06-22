namespace Uno.Internal
{
    public class FormatStringLiteral : FormatStringToken
    {
        public FormatStringLiteral(string lexeme) : base(lexeme) {}

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var token = obj as FormatStringLiteral;
            if (token == null)
                return false;

            return Lexeme == token.Lexeme;
        }

        public override int GetHashCode()
        {
            return Lexeme.GetHashCode();
        }

        public override string ToString(object[] objs)
        {
            return Lexeme;
        }
    }
}
