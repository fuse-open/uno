using Uno.Compiler.ExportTargetInterop;

namespace Uno.Collections
{
    [extern(DOTNET) DotNetType("System.Collections.Generic.IDictionary`2")]
    public interface IDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>
    {
        ICollection<TKey> Keys { get; }
        ICollection<TValue> Values { get; }

        void Add(TKey key, TValue value);

        bool TryGetValue(TKey key, out TValue value);

        bool Remove(TKey key);

        bool ContainsKey(TKey key);

        TValue this[TKey key] { get; set; }
    }
}