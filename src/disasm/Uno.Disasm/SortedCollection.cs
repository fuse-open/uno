using System;
using System.Collections.ObjectModel;

namespace Uno.Disasm
{
    public class SortedCollection<T> : ObservableCollection<T> where T : IComparable<T>
    {
        int FindIndex(T item)
        {
            int left = 0,
                right = Count - 1;

            while (left <= right)
            {
                int mid = (left + right) / 2;
                int diff = item.CompareTo(this[mid]);

                if (diff > 0)
                    left = mid + 1;
                else if (diff < 0)
                    right = mid - 1;
                else
                    return mid + 1;
            }

            return left;
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(FindIndex(item), item);
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            // not allowed on sorted collection
            throw new InvalidOperationException();
        }

        protected override void SetItem(int index, T item)
        {
            throw new NotImplementedException();
        }
    }
}
