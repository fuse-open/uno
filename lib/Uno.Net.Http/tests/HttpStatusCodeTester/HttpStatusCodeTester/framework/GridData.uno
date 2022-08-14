using Uno;
using Uno.Collections;
using Fuse;
using Fuse.Controls;

namespace HttpStatusCodeTester
{
    public abstract class GridData
    {
        public IList<string> ColumnNames
        {
            get
            {
                if(Items == null || Items.Count == 0) return new List<string>();

                var firstRow = Items[0];
                var columnNames = firstRow.Children;

                var list = new List<string>();
                foreach(var columnName in columnNames)
                {
                    var cell = columnName as Cell;
                    if(cell == null) throw new Exception("Err");
                    list.Add(cell.Name);
                }
                return list;
            }
        }

        public IList<Panel> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        IList<Panel> _items;

        protected GridData()
        {
            _items = new List<Panel>();
        }
    }
}
