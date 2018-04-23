using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.TypeInitializationException")]
    public class TypeInitializationException : Exception
    {
        public TypeInitializationException(string fullTypeName, Exception innerException)
            : base(string.Format("The type initializer for '{0}' threw an exception.", fullTypeName), innerException)
        {
            _typeName = fullTypeName;
        }

        readonly string _typeName;
        public string TypeName { get { return _typeName; } }
    }
}
