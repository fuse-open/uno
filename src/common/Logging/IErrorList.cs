namespace Uno.Logging
{
    public interface IErrorList
    {
        void AddFatalError(Source src, string code, string msg);
        void AddError(Source src, string code, string msg);
        void AddWarning(Source src, string code, string msg);
        void AddMessage(Source src, string code, string msg);
    }
}
