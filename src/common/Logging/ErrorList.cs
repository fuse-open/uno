namespace Uno.Logging
{
    class ErrorList : IErrorList
    {
        public static readonly ErrorList Null = new ErrorList();

        ErrorList() { }
        public void Reset() { }
        public void AddFatalError(Source src, string code, string msg) { }
        public void AddError(Source src, string code, string msg) { }
        public void AddWarning(Source src, string code, string msg) { }
        public void AddMessage(Source src, string code, string msg) { }
    }
}