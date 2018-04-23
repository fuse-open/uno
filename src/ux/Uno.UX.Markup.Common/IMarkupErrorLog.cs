namespace Uno.UX.Markup.Common
{
    public interface IMarkupErrorLog
    {
        void ReportError(string message);
        void ReportWarning(string message);

        void ReportError(string path, int line, string message);
        void ReportWarning(string path, int line, string message);
    }
}
