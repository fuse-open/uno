namespace Uno.Testing
{
    class IgnoreException : Uno.Exception
    {
        public IgnoreException(string message)
            : base(message)
        {
        }
    }
}
