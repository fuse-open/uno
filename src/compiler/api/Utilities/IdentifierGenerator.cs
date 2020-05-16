using System.Collections.Generic;
using System.Linq;

namespace Uno.Compiler.API.Utilities
{
    public class IdentifierGenerator
    {
        readonly List<string> _identifiers = new List<string>();
        readonly IKeywords _keywords;
        int _index;

        public IdentifierGenerator(IKeywords keywords)
        {
            _keywords = keywords;
        }

        public string Get(int i)
        {
            while (i >= _identifiers.Count)
            {
                string id = GetRaw(_index++);

                // reject invalid, reserved or global names
                if (char.IsDigit(id.First()) ||
                    id.Contains('_') ||
                    _keywords.IsReserved(id))
                    continue;

                _identifiers.Add(id);
            }

            return _identifiers[i];
        }

        public static string GetRaw(int i)
        {
            var chars = new List<char>();

            do
            {
                chars.Add(Characters[i % Characters.Length]);
                i /= Characters.Length;
            }
            while (i > 0);

            chars.Reverse();
            return new string(chars.ToArray());
        }

        public const string Characters = "_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    }
}
