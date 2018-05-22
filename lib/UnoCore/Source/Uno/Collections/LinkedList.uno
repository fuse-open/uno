using Uno.Compiler.ExportTargetInterop;

namespace Uno.Collections
{
    [extern(DOTNET) DotNetType("System.Collections.Generic.LinkedListNode`1")]
    public sealed class LinkedListNode<T>
    {
        public LinkedList<T> List { get; internal set; }
        public LinkedListNode<T> Next { get; internal set; }
        public LinkedListNode<T> Previous { get; internal set; }
        public T Value { get; private set; }

        public LinkedListNode(T value)
        {
            Value = value;
        }
    }

    [extern(DOTNET) DotNetType("System.Collections.Generic.LinkedList`1")]
    public sealed class LinkedList<T> : ICollection<T>
    {
        public int Count { get; private set; }
        public LinkedListNode<T> First { get; private set; }
        public LinkedListNode<T> Last { get; private set; }

        public struct Enumerator : IEnumerator<T>
        {
            LinkedList<T> _list;
            LinkedListNode<T> _current, _next;

            internal Enumerator(LinkedList<T> list)
            {
                _list = list;
                _next = _list.First;
            }

            public T Current { get { return _current.Value; } }

            public void Dispose()
            {
                // TODO
            }

            public void Reset()
            {
                _next = _list.First;
            }

            public bool MoveNext()
            {
                if (_next == null)
                    return false;

                _current = _next;
                _next = _next.Next;
                return true;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value)
        {
            var newNode = new LinkedListNode<T>(value);
            AddAfter(node, newNode);
            return newNode;
        }

        public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.List != this)
                throw new InvalidOperationException("The LinkedList node does not belong to current LinkedList.");

            if (newNode == null)
                throw new ArgumentNullException(nameof(newNode));

            if (newNode.List != null)
                throw new InvalidOperationException("The LinkedList node already belongs to a LinkedList.");

            UncheckedAddAfter(node, newNode);
        }

        void UncheckedAddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            // Preconditions: What's checked in AddAfter

            if (node == Last)
                Last = newNode;

            newNode.List = this;
            newNode.Next = node.Next;
            newNode.Previous = node;
            if (node.Next != null)
                node.Next.Previous = newNode;
            node.Next = newNode;

            Count = Count + 1;
        }

        public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)
        {
            var newNode = new LinkedListNode<T>(value);
            AddBefore(node, newNode);
            return newNode;
        }

        public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.List != this)
                throw new InvalidOperationException("The LinkedList node does not belong to current LinkedList.");

            if (newNode == null)
                throw new ArgumentNullException(nameof(newNode));

            if (newNode.List != null)
                throw new InvalidOperationException("The LinkedList node already belongs to a LinkedList.");

            UncheckedAddBefore(node, newNode);
        }

        void UncheckedAddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            // Preconditions: What's checked in AddBefore

            if (node == First)
                First = newNode;

            newNode.List = this;
            newNode.Next = node;
            newNode.Previous = node.Previous;
            if (node.Previous != null)
                node.Previous.Next = newNode;
            node.Previous = newNode;

            Count = Count + 1;
        }

        public void AddFirst(LinkedListNode<T> node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.List != null)
                throw new InvalidOperationException("The LinkedList node already belongs to a LinkedList.");

            if (Last == null)
            {
                // empty list
                node.Previous = node.Next = null;
                node.List = this;
                First = Last = node;
                Count = 1;
            }
            else
            {
                UncheckedAddBefore(First, node);
            }
        }

        public LinkedListNode<T> AddFirst(T value)
        {
            var node = new LinkedListNode<T>(value);
            AddFirst(node);
            return node;
        }

        public void AddLast(LinkedListNode<T> node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.List != null)
                throw new InvalidOperationException("The LinkedList node already belongs to a LinkedList.");

            if (Last == null)
            {
                // empty list
                node.Previous = node.Next = null;
                node.List = this;
                First = Last = node;
                Count = 1;
            }
            else
            {
                UncheckedAddAfter(Last, node);
            }
        }

        public LinkedListNode<T> AddLast(T value)
        {
            var node = new LinkedListNode<T>(value);
            AddLast(node);
            return node;
        }

        public LinkedListNode<T> Find(T value)
        {
            var curr = First;
            while (curr != null)
            {
                if (curr.Value.Equals(value))
                    return curr;
                curr = curr.Next;
            }
            return null;
        }

        public void Clear()
        {
            var curr = First;
            while (curr != null)
            {
                var next = curr.Next;

                curr.Previous = null;
                curr.Next = null;
                curr.List = null;

                curr = next;
            }

            First = Last = null;
            Count = 0;
        }

        public void ICollection<T>.Add(T value)
        {
            AddLast(value);
        }

        public bool Contains(T value)
        {
            return Find(value) != null;
        }

        public void Remove(LinkedListNode<T> node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.List != this)
                throw new InvalidOperationException("The LinkedList node does not belong to current LinkedList.");

            // relink nodes
            if (node.Previous != null)
                node.Previous.Next = node.Next;
            if (node.Next != null)
                node.Next.Previous = node.Previous;

            // update first and last (if needed)
            if (node == First)
                First = node.Next;
            if (node == Last)
                Last = node.Previous;

            node.List = null;
            Count = Count - 1;
        }

        public bool Remove(T value)
        {
            var node = Find(value);
            if (node != null)
            {
                Remove(node);
                return true;
            }
            return false;
        }
    }
}
