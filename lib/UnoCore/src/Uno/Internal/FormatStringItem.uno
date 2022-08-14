namespace Uno.Internal
{
   public class FormatStringItem : FormatStringToken
    {
        public FormatStringItem(string lexeme) : base(lexeme)
        {
        }

        public int Number
        {
            get
            {
                var index = Lexeme.IndexOf(',');
                if (index == -1)
                    index = Lexeme.IndexOf(':');
                if (index == -1)
                    index = Lexeme.IndexOf('}');
                return int.Parse(Lexeme.Substring(1,index-1));
            }
        }

        public string FormatString
        {
            get
            {
                var colon = Lexeme.IndexOf(':');
                if (colon == -1)
                    return null;
                var end = Lexeme.IndexOf('}');
                if (end == -1)
                    throw new FormatException("Format specifier was invalid");
                return Lexeme.Substring(colon+1, end - colon -1);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var token = obj as FormatStringItem;
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
            if ((Number < 0) || (Number > objs.Length - 1))
                throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
            var o = objs[Number];
            var formatString = FormatString;
            if (string.IsNullOrEmpty(formatString))
            {
                return o.ToString();
            }
            else
            {
                if (o is bool) return NumericFormatter.Format(formatString, (bool)o);
                if (o is char) return NumericFormatter.Format(formatString, (char)o);
                if (o is sbyte) return NumericFormatter.Format(formatString, (sbyte)o);
                if (o is byte) return NumericFormatter.Format(formatString, (byte)o);
                if (o is short) return NumericFormatter.Format(formatString, (short)o);
                if (o is ushort) return NumericFormatter.Format(formatString, (ushort)o);
                if (o is int) return NumericFormatter.Format(formatString, (int)o);
                if (o is uint) return NumericFormatter.Format(formatString, (uint)o);
                if (o is long) return NumericFormatter.Format(formatString, (long)o);
                if (o is ulong) return NumericFormatter.Format(formatString, (ulong)o);
                if (o is float) return NumericFormatter.Format(formatString, (float)o);
                if (o is double) return NumericFormatter.Format(formatString, (double)o);
            }
            return o.ToString();
        }
    }
}
