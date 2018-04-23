using Uno;
using Uno.Collections;

namespace Uno.Data.Json
{
    abstract class Value
    {
    }

    sealed class Null : Value
    {
        Null() { }

        public static readonly Null Singleton = new Null();
    }

    abstract class AtomicValue<T> : Value
    {
        T _val;
        public T Value { get { return _val; } }

        protected AtomicValue(T val)
        {
            _val = val;
        }

        public override string ToString()
        {
            return _val.ToString();
        }
    }

    sealed class Boolean : AtomicValue<bool>
    {
        Boolean(bool b) : base(b) { }

        public static readonly Boolean True = new Boolean(true);
        public static readonly Boolean False = new Boolean(false);
    }

    sealed class Number : AtomicValue<double>
    {
        public Number(double d) : base(d) { }
    }

    sealed class String : AtomicValue<string>
    {
        public String(string d) : base(d) { }
    }

    sealed class Object : Value
    {
        Dictionary<string, Value> _values = new Dictionary<string, Value>();

        public string[] Keys
        {
            get
            {
                return _values.Keys.ToArray();
            }
        }

        public int Count
        {
            get { return _values.Count; }
        }

        public Value this[string key]
        {
            get
            {
                return _values[key];
            }
        }

        public void Add(string key, Value val)
        {
            if (_values.ContainsKey(key))
                _values[key] = val;
            else
                _values.Add(key, val);
        }

        public bool ContainsKey(string key)
        {
            return _values.ContainsKey(key);
        }
    }

    sealed class Array : Value
    {
        List<Value> _values = new List<Value>();

        internal void Add(Value v)
        {
            _values.Add(v);
        }

        public Value this[int index]
        {
            get { return _values[index]; }
        }

        public int Count { get { return _values.Count; } }
    }
}
