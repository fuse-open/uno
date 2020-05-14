using System;
using System.Collections.Generic;

namespace Uno.Compiler.Core.IL.Building.Functions.Lambdas
{
    class Tree<T>
    {
        public readonly T Data;
        public readonly List<Tree<T>> Children;
        public readonly Tree<T> Parent;
        public bool IsRoot => Parent == null;

        public Tree(T data, Tree<T> parent = null, List<Tree<T>> children = null)
        {
            Data = data;
            Children = children ?? new List<Tree<T>>();
            Parent = parent;
        }
    }

    static class Tree
    {
        public static Tree<T> Create<T>(T data, Tree<T> parent = null, List<Tree<T>> children = null)
        {
            return new Tree<T>(data, parent, children);
        }

        public static Tree<T2> Select<T1, T2>(this Tree<T1> self, Func<T1, T2> selector, Tree<T2> parent = null)
        {
            var result = new Tree<T2>(selector(self.Data), parent);
            foreach (var kv in self.Children)
                result.Children.Add(kv.Select(selector, result));
            return result;
        }
    }
}