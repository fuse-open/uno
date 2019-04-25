using System.Collections.Generic;

namespace Uno.Configuration.Format
{
    public class StuffItem
    {
        public readonly StuffFile File;
        public readonly int LineNumber;
        public readonly string Key;
        public StuffItemType Type;
        public string Value;
        public StuffItem Next;

        public StuffItem(StuffFile file, StuffItemType type, int line, string key, string value)
        {
            File = file;
            Type = type;
            LineNumber = line;
            Key = key;
            Value = value;
        }

        public List<string> ToList()
        {
            var values = new List<string>();
            for (var item = this; item != null; item = item.Next)
                values.Add(item.Value);
            return values;
        }

        public override string ToString()
        {
            return Next == null
                ? Value
                : string.Join("\n", ToList());
        }
    }
}