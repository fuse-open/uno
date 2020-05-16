namespace Uno.Support.WinForms
{
    public interface IUnoWindow
    {
        void Close();
        void SetClientSize(int width, int height);
        string Title { get; set; }
        bool IsFullscreen { get; set; }
    }
}