using Uno;
using Uno.UX;
using Uno.Collections;
using Fuse;
using Fuse.Controls;
using Fuse.Elements;
using Fuse.Designer;
using HttpStatusCodeTester;

public partial class DataGrid
{
    public DataGrid()
    {
        InitializeUX();
    }

    GridData _content;

    [Primary]
    public GridData Content
    {
        get
        {
            return _content;
        }
        set
        {
            _content = value;
            Populate();
        }
    }

    void Populate()
    {
        header.ColumnCount = _content.ColumnNames.Count;
        foreach(var columnName in _content.ColumnNames)
            header.Children.Add(CreateColumn(columnName));

        foreach(var item in _content.Items)
            panel.Children.Add(item);
    }

    TextBlock CreateColumn(string name)
    {
        var txt = new TextBlock();
        txt.Text = name;
        return txt;
    }
}

