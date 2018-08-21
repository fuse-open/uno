using Uno;
using Fuse;
using Fuse.Drawing;
using Fuse.Designer;

public partial class Cell
{
    public Cell()
    {
        InitializeUX();
        Name = "";
        Text = "";
        Color = float4(1);
    }

    public Cell(string name) : this()
    {
        Name = name;
    }

    public string Name { get; set; }

    public string Text
    {
        get { return _text.Text; }
        set { _text.Text = value; }
    }

    [Color]
    public float4 Color
    {
        get { return ((SolidColor)_background.Fill).Color; }
        set
        {
            _background.Fill = new SolidColor(value);
        }
    }
}

