using Uno.Compiler.ExportTargetInterop;

namespace Uno.Collections
{
    [extern(DOTNET) DotNetType("System.Collections.Generic.KeyValuePair`2")]
    public struct KeyValuePair<TKey, TValue>
    {
        TKey _key;
        TValue _value;

        public KeyValuePair(TKey key, TValue value)
        {
            _key = key;
            _value = value;
        }

        public TKey Key
        {
            get { return _key; }
        }

        public TValue Value
        {
            get { return _value; }
        }
    }
}
