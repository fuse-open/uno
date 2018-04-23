using Uno;

namespace Collections.Extensions.Test
{
    public class DummyItem
    {
        public DummyItem(string name)
        {
            Name = name;
        }

        public DummyItem(int value)
        {
            Value = value;
        }

        public int Value { get; set; }

        public string Name { get; set; }
    }
}