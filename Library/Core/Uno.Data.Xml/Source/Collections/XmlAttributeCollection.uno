using Uno.Collections;
using Uno;

namespace Uno.Data.Xml
{
    public class XmlAttributeCollection : XmlNodeCollectionBase<XmlAttribute>
    {
        public XmlAttributeCollection(XmlLinkedNode owner) : base(owner)
        { }


        public XmlAttribute this[string name]
        {
            get
            {
                foreach(var child in this)
                {
                    if (child.Name == name)
                        return child;
                }
                return null;
            }
        }
        public override void Add(XmlAttribute item)
        {
            ValidationHelper.ValidateAttributeAdding(_owner, item);

            if (LastChild != null)
            {
                item.PreviousSibling = LastChild;
                LastChild.NextSibling = item;
            }
            item.NextSibling = null;

            base.Add(item);
        }

        public override bool Remove(XmlAttribute item)
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

        public override void Insert(int index, XmlAttribute item)
        {
            if (!HasChildren && index == 0)
            {
                Add(item);
                return;
            }

            ValidationHelper.ValidateAttributeAdding(_owner, item);
            var existingItem =  this[index];
            item.NextSibling = existingItem;
            if (existingItem.PreviousSibling != null)
            {
                item.PreviousSibling = existingItem.PreviousSibling;
                item.PreviousSibling.NextSibling = item;
                existingItem.PreviousSibling = item;
            }

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
