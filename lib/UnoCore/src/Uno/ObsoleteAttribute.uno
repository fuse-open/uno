using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [DotNetType("System.ObsoleteAttribute")]
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum |
        AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field |
        AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate)]
    public sealed class ObsoleteAttribute : Attribute
    {
        ObsoleteAttribute() { }

        ObsoleteAttribute(string message)
        {
            Message = message;
        }

        ObsoleteAttribute(string message, bool isError)
        {
            Message = message;
            IsError = isError;
        }

        public string Message { get; private set; }
        public bool IsError { get; private set; }
    }
}
