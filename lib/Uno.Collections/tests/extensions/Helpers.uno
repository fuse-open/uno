using Uno;

namespace Collections.Extensions.Test
{
    class MatchExpressions
    {
        public int Integer { get; set; }

        public string String { get; set; }

        public bool MatchInt(int arg)
        {
            return arg == Integer;
        }

        public bool MatchDummyItemName(DummyItem item)
        {
            return item.Name == String;
        }
    }

    class OrderByExpressions
    {
        public int CompareInt(int item)
        {
            return item;
        }

        public int CompareIntEx(int a, int b)
        {
            return a - b;
        }

        public int CompareDummyItem(DummyItem item)
        {
            return item.Value;
        }

        public int CompareDummyItemEx(DummyItem a, DummyItem b)
        {
            return a.Value - b.Value;
        }
    }
}
