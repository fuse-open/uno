namespace Uno.Runtime.Implementation.Internal
{
    public abstract class FormatStringToken
    {
        public string Lexeme {get; private set;}

        protected FormatStringToken(string lexeme)
        {
            Lexeme = lexeme;
        }
        public abstract string ToString(object[] items);
    }
}
