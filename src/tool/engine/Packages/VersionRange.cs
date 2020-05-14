using System;

namespace Uno.Build.Packages
{
    public class VersionRange
    {
        public static VersionRange Parse(string str)
        {
            if (string.IsNullOrEmpty(str))
                return new VersionRange(Operator.Null);

            var op = Operator.Equals;
            string lessThanPart = null;
            var mainPart = str;

            if (str.StartsWith(">="))
            {
                op = Operator.GreaterThanOrEqual;
                mainPart = str.Substring(2);
            }
            else if (str.StartsWith("<="))
            {
                op = Operator.LessThanOrEqual;
                mainPart = str.Substring(2);
            }
            else if (str.StartsWith("=="))
            {
                op = Operator.Equals;
                mainPart = str.Substring(2);
            }
            else if (str.StartsWith("~>"))
            {
                op = Operator.GreaterThanOrEqual;
                mainPart = str.Substring(2);

                var lastDot = mainPart.LastIndexOf('.');
                if (lastDot != -1)
                {
                    var firstPart = mainPart.Substring(0, lastDot);
                    if (firstPart.Length > 0)
                    {
                        lessThanPart =
                            firstPart.Substring(0, firstPart.Length - 1) +
                            (char)(firstPart[firstPart.Length - 1] + 1);
                    }
                }

                if (lessThanPart == null)
                    throw new FormatException("'~>' requires atleast two version parts in " + str.Quote());
            }
            else if (str.StartsWith(">"))
            {
                op = Operator.GreaterThan;
                mainPart = str.Substring(1);
            }
            else if (str.StartsWith("<"))
            {
                op = Operator.LessThan;
                mainPart = str.Substring(1);
            }
            else if (str.StartsWith("="))
            {
                op = Operator.Equals;
                mainPart = str.Substring(1);
            }

            foreach (var c in mainPart)
            {
                if (char.IsLetterOrDigit(c) || c == '.' || c == '-')
                    continue;
                throw new FormatException("Illegal char " + c.Quote() + " found in " + str.Quote());
            }

            return new VersionRange(op, mainPart.Split('.'), lessThanPart?.Split('.'));
        }

        readonly Operator _op;
        readonly string[] _mainPart;
        readonly string[] _lessThanPart;

        VersionRange(Operator op, string[] mainPart = null, string[] lessThanPart = null)
        {
            _op = op;
            _mainPart = mainPart;
            _lessThanPart = lessThanPart;
        }

        public bool IsCompatible(string version)
        {
            if (_op == Operator.Null)
                return true;

            var parts = version.Split('.');
            var diff = Compare(parts, _mainPart);
            var result = false;

            switch (_op)
            {
                case Operator.Equals:
                    result = diff == 0;
                    break;
                case Operator.LessThan:
                    result = diff < 0;
                    break;
                case Operator.GreaterThan:
                    result = diff > 0;
                    break;
                case Operator.LessThanOrEqual:
                    result = diff <= 0;
                    break;
                case Operator.GreaterThanOrEqual:
                    result = diff >= 0;
                    break;
            }

            if (result && _lessThanPart != null)
                result = Compare(parts, _lessThanPart) < 0;

            return result;
        }

        public static int Compare(string a, string b)
        {
            return Compare(a.Split('.'), b.Split('.'));
        }

        static int Compare(string[] a, string[] b)
        {
            var min = Math.Min(a.Length, b.Length);

            for (var i = 0; i < min; i++)
            {
                var diff = ComparePart(a[i], b[i]);
                if (diff == 0)
                    continue;
                return diff;
            }

            for (var i = min; i < a.Length; i++)
                if (a[i].TrimStart('0').Length > 0)
                    return char.IsLetter(a[i][0]) 
                        ? -1 
                        : 1;
            for (var i = min; i < b.Length; i++)
                if (b[i].TrimStart('0').Length > 0)
                    return char.IsLetter(b[i][0]) 
                        ? 1 
                        : -1;
            return 0;
        }

        static int ComparePart(string a, string b)
        {
            int ai, bi;
            if (!int.TryParse(a, out ai))
                ai = -1;
            if (!int.TryParse(b, out bi))
                bi = -1;
            return ai == bi
                ? ai == -1
                    ? string.Compare(a, b, StringComparison.InvariantCultureIgnoreCase)
                    : 0
                : ai - bi;
        }

        enum Operator
        {
            Null,
            Equals,
            LessThan,
            GreaterThan,
            LessThanOrEqual,
            GreaterThanOrEqual
        }
    }
}
