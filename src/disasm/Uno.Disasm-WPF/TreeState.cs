using System.Collections.Generic;
using System.Windows.Controls;
using Uno.Disasm.ILView;

namespace Uno.Disasm
{
    class TreeState
    {
        ILItem _selectedItem;
        public readonly HashSet<string> Expanded = new HashSet<string>();
        public readonly HashSet<string> Collapsed = new HashSet<string>();
        public string Selection;

        public TreeState()
        {
        }

        public TreeState(TreeView view)
        {
            _selectedItem = (ILItem)view.SelectedItem;

            foreach (var _item in view.Items)
                Save((ILItem) _item);
        }

        public bool Restore(TreeView view)
        {
            var hasSelection = false;
            foreach (var item in view.Items)
                Restore((ILItem)item, ref hasSelection);
            return hasSelection;
        }

        void Restore(ILItem item, ref bool hasSelection, string prefix = "")
        {
            prefix += item.DisplayName;

            if (prefix == Selection)
            {
                item.Select();
                hasSelection = true;
            }

            if (item.Children.Count > 0)
            {
                if (Expanded.Contains(prefix))
                    item.IsExpanded = true;
                else if (Collapsed.Contains(prefix))
                {
                    item.IsExpanded = false;
                    return;
                }

                prefix += ".";

                foreach (var child in item.Children)
                    Restore(child, ref hasSelection, prefix);
            }
        }

        void Save(ILItem item, string prefix = "")
        {
            prefix += item.DisplayName;

            if (item == _selectedItem)
                Selection = prefix;

            if (item.Children.Count > 0)
            {
                if (item.IsExpanded)
                    Expanded.Add(prefix);
                else
                {
                    Collapsed.Add(prefix);
                    return;
                }

                prefix += ".";

                foreach (var child in item.Children)
                    Save(child, prefix);
            }
        }
    }
}