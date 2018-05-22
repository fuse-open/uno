using Uno.Collections;
using Uno;

namespace Uno.Data.Xml
{
    public abstract class XmlNodeCollectionBase<T> : IList<T> where T : XmlNode
    {
        IList<T> _children;
        IList<T> Children
        {
            get { return _children ?? (_children = new List<T>()); }
        }

        protected readonly XmlLinkedNode _owner;

        protected XmlNodeCollectionBase(XmlLinkedNode owner)
        {
            _owner = owner;
        }

        public int Count
        {
            get
            {
                if (HasChildren)
                {
                    return Children.Count;
                }
                return 0;
            }
        }

        public void Clear()
        {
            if (HasChildren)
            {
                Children.Clear();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        public bool HasChildren
        {
            get { return Children.Count > 0; }
        }

        public bool Contains(T item)
        {
            return HasChildren && Children.Contains(item);
        }

        public T FirstChild
        {
            get
            {
                return HasChildren ? Children[0] : null;
            }
        }

        public T LastChild
        {
            get
            {
                return HasChildren ? Children[Children.Count - 1] : null;
            }
        }

        public T this[int index]
        {
            get
            {
                return Children[index];
            }
        }

        public virtual void Add(T item)
        {
            Children.Add(item);
        }

        public virtual bool Remove(T item)
        {
            return Children.Remove(item);
        }

        public virtual void Insert(int index, T item)
        {
            Children.Insert(index, item);
        }

        public virtual void RemoveAt(int index)
        {
            Children.RemoveAt(index);
        }
    }
}
