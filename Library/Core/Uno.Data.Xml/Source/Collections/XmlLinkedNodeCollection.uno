using Uno.Collections;
using Uno;

namespace Uno.Data.Xml
{
    public class XmlLinkedNodeCollection : XmlNodeCollectionBase<XmlLinkedNode>
    {
        public XmlLinkedNodeCollection(XmlLinkedNode owner) : base(owner)
        { }

        public override void Add(XmlLinkedNode item)
        {
            ValidationHelper.ValidateNodeAdding(_owner, item);

            if (item.NodeType == XmlNodeType.Declaration && HasChildren)
            {
                Insert(0, item);
                return;
            }

            if (item.Parent != null)
            {
                item = item.Clone();
            }

            item.Parent = _owner;
            if (LastChild != null)
            {
                item.PreviousSibling = LastChild;
                LastChild.NextSibling = item;
            }
            item.NextSibling = null;
            base.Add(item);
        }

        public override bool Remove(XmlLinkedNode item)
        {
            if (item.PreviousSibling != null)
            {
                item.PreviousSibling.NextSibling = item.NextSibling;
            }
            if (item.NextSibling != null)
            {
                item.NextSibling.PreviousSibling = item.PreviousSibling;
            }

            return base.Remove(item);
        }

        public override void Insert(int index, XmlLinkedNode item)
        {
            if (!HasChildren && index == 0)
            {
                Add(item);
                return;
            }

            ValidationHelper.ValidateNodeAdding(_owner, item);

            if (item.NodeType == XmlNodeType.Declaration && index != 0)
            {
                Insert(0, item);
                return;
            }

            if (item.Parent != null)
            {
                item = item.Clone();
            }

            var existingItem =  this[index];
            item.NextSibling = existingItem;
            item.Parent = _owner;
            if (existingItem.PreviousSibling != null)
            {
                existingItem.PreviousSibling.NextSibling = item;
            }
            item.PreviousSibling = existingItem.PreviousSibling;
            existingItem.PreviousSibling = item;

            base.Insert(index, item);
        }

        public override void RemoveAt(int index)
        {
            var item = this[index];
            if (item.PreviousSibling != null)
            {
                item.PreviousSibling.NextSibling = item.NextSibling;
            }
            if (item.NextSibling != null)
            {
                item.NextSibling.PreviousSibling = item.PreviousSibling;
            }

            base.RemoveAt(index);
        }
    }
}
