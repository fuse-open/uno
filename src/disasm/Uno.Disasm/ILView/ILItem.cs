using System;
using System.ComponentModel;
using Uno.Compiler;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Disasm.ILView.Commands;
using Uno.Disasm.ILView.Members;
using Uno.Disasm.ILView.Namespaces;
using Uno.Disasm.ILView.Packages;

namespace Uno.Disasm.ILView
{
    public abstract class ILItem : IComparable<ILItem>, INotifyPropertyChanged
    {
        public static ILCommand[] Commands =
        {
            new OpenWith(),
            null,
            new CopyFullPath(),
            new ShowInExplorer(),
        };

        bool _isExpanded;
        bool _isSelected;
        bool _isVisible = true;
        readonly SortedCollection<ILItem> _children = new SortedCollection<ILItem>();
        public SortedCollection<ILItem> Children => _children;
        public event PropertyChangedEventHandler PropertyChanged;

        public abstract string DisplayName { get; }
        public abstract ILIcon Icon { get; }
        public virtual Syntax Syntax => Syntax.Uno;
        public virtual object Object => null;
        public string Suffix { get; set; }
        public bool HasSuffix => Suffix != null;
        public object Tag { get; set; }

        public void AddChild(ILItem child)
        {
            Children.Add(child);
        }

        public string GetText(Disassembler disasm, VisibilityFlags flags)
        {
            try
            {
                disasm.Clear();
                Disassemble(disasm, flags.HasFlag(VisibilityFlags.PublicOnly));
            }
            catch (Exception e)
            {
                disasm.AppendLine();
                disasm.Append(e);
                disasm.AppendLine();
            }

            return disasm.GetText();
        }

        protected virtual void Disassemble(Disassembler disasm, bool publicOnly)
        {
            Disassemble(disasm);
        }

        protected virtual void Disassemble(Disassembler disasm)
        {
            disasm.AppendHeader(ToString());

            foreach (var c in Children)
                disasm.AppendLine(c.ToString());
        }

        public override string ToString()
        {
            return (Object ?? DisplayName).ToString();
        }

        public void UpdateVisibility(VisibilityFlags flags)
        {
            IsVisible = CanShow(flags);

            if (!IsVisible)
                return;            
            foreach (var c in Children)
                c.UpdateVisibility(flags);
            
            int childCount = GetVisibleChildCount();

            if (this is NamespaceItem ||
                this is ParameterizationCollection ||
                this is ReferenceCollection ||
                this is ResourceCollection ||
                this is FolderBase)
            {
                Suffix = "(" + childCount + (
                        this is FolderBase || this is ResourceCollection 
                            ? " file" : 
                        this is ReferenceCollection 
                            ? " package" 
                            : " item"
                    ) + (
                        childCount != 1 
                            ? "s" 
                            : null
                    ) + ")";
                OnPropertyChanged("HasSuffix");
                OnPropertyChanged("Suffix");
            }
        }

        bool CanShow(VisibilityFlags flags)
        {
            var obj = Object;
            var dt = obj as DataType;
            var member = obj as Member;
            var block = obj as Block;
            var mp = obj as MetaProperty;
            return !(flags.HasFlag(VisibilityFlags.PublicOnly) && (
                        dt != null && !dt.IsPublic ||
                        member != null && !member.IsPublic ||
                        mp != null && !mp.IsPublic ||
                        block != null && !block.IsPublic ||
                        block == null && this is BlockItem ||
                        this is DefinitionCollection) ||
                    flags.HasFlag(VisibilityFlags.ProjectOnly) &&
                        this is PackageItem && !((PackageItem) this).Package.Flags.HasFlag(SourcePackageFlags.Startup) ||
                    Children.Count == 0 && (
                        this is ReferenceCollection ||
                        this is ResourceCollection ||
                        this is ParameterizationCollection ||
                        this is DefinitionCollection ||
                        this is OverloadCollection ||
                        this is FolderBase
                    ));
        }

        static int GetOrderIndex(ILItem item)
        {
            if (item is PackageItem) return -4;
            if (item is BuildItem) return -3;
            if (item is ReferenceCollection) return -2;
            if (item is ResourceCollection) return -2;
            if (item is FolderBase) return -1;
            if (item is NamespaceItem) return 0;
            if (item is DefinitionCollection) return 1;
            if (item is OverloadCollection) return 1;
            if (item is ParameterizationCollection) return 1;
            if (item is TypeItem) return 2;
            if (item is BlockItem) return 2;
            if (item is LiteralItem) return 3;
            if (item is FunctionItem) return (item as FunctionItem).Function.IsConstructor ? 4 : 5;
            if (item is PropertyItem) return (item as PropertyItem).Property.Parameters.Length > 0 ? 6 : 7;
            if (item is EventItem) return 8;
            if (item is FieldItem) return 9;
            return 100;
        }

        public ILItem GetVisibleChild()
        {
            foreach (var c in Children)
                if (c.IsVisible)
                    return c;

            return null;
        }

        public int GetVisibleChildCount()
        {
            int count = 0;
            foreach (var c in Children)
            {
                if (c.IsVisible)
                {
                    if (this is FolderBase && c is FolderBase)
                        count += c.GetVisibleChildCount();
                    else if (c is TypeItem)
                        count += (c as TypeItem).Overloads.GetVisibleChildCount() + 1;
                    else
                        count++;
                }
            }

            return count;
        }

        public int CompareTo(ILItem item)
        {
            if (item == null) return 100;
            var diff = GetOrderIndex(this) - GetOrderIndex(item);
            if (diff == 0) return DisplayName.CompareTo(item.DisplayName);
            return diff;
        }

        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Select()
        {
            IsSelected = true;
        }

        public void Expand()
        {
            IsExpanded = true;

            var item = this;
            while (item.GetVisibleChildCount() == 1)
            {
                item = item.GetVisibleChild();
                item.IsExpanded = true;
            }

            IsSelected = true;
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    OnPropertyChanged("IsExpanded");

                    if (this is FolderBase)
                        OnPropertyChanged("Icon");
                }
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
                else if (_isSelected)
                    OnPropertyChanged("IsSelected");
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (value != _isVisible)
                {
                    _isVisible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }
    }
}
