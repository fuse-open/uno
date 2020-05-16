using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Core.IL.Building.Functions.Lambdas
{
    class Variables
    {
        public readonly HashSet<Variable> Locals = new HashSet<Variable>();
        public readonly HashSet<Parameter> Params = new HashSet<Parameter>();
        public bool This;

        public void Add(Variable local)
        {
            Locals.Add(local);
        }

        public void Add(Parameter param)
        {
            Params.Add(param);
        }

        public bool Contains(Variable local)
        {
            return Locals.Contains(local);
        }

        public bool Contains(Parameter param)
        {
            return Params.Contains(param);
        }

        public void AddThis()
        {
            This = true;
        }

        public void UnionWith(Variables other)
        {
            Locals.UnionWith(other.Locals);
            Params.UnionWith(other.Params);
            This |= other.This;
        }

        public static Variables Union(params Variables[] vs)
        {
            return Union((IEnumerable<Variables>) vs);
        }

        public static Variables Union(IEnumerable<Variables> vs)
        {
            var result = new Variables();
            foreach (var v in vs)
                result.UnionWith(v);

            return result;
        }

        public static Variables Intersection(Variables v1, params Variables[] vs)
        {
            return Intersection(v1, (IEnumerable<Variables>) vs);
        }

        public static Variables Intersection(Variables v1, IEnumerable<Variables> vs)
        {
            var result = Union(v1);
            foreach (var v2 in vs)
            {
                result.Locals.IntersectWith(v2.Locals);
                result.Params.IntersectWith(v2.Params);
                result.This &= v2.This;
            }
            return result;
        }
    }

    public static class DictionaryExtensions
    {
        public static void IntersectWith<TKey, TValue, TValue2>(
            this Dictionary<TKey, TValue> self,
            Dictionary<TKey, TValue2> other)
        {
            var toRemove = new List<TKey>();
            foreach (var key in self.Keys)
                if (!other.ContainsKey(key))
                    toRemove.Add(key);

            foreach (var key in toRemove)
                self.Remove(key);
        }

        public static void UnionWith<TKey, TValue>(
            this Dictionary<TKey, TValue> self,
            Dictionary<TKey, TValue> other)
        {
            foreach (var kv in other)
                self[kv.Key] = kv.Value;
        }
    }
}