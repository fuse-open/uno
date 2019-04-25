namespace Uno.Configuration.Format
{
    public struct Token
    {
        public int LineNumber;
        public int LinePosition;
        public TokenType Type;
        public string String;
        public char Punctuation;

        public Token(int line, int pos)
        {
            LineNumber = line;
            LinePosition = pos;
            Type = 0;
            String = null;
            Punctuation = '\0';
        }

        public override string ToString()
        {
            return String ?? Punctuation.ToString();
        }
    }
}