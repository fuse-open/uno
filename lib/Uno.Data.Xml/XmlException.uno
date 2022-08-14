using Uno.Compiler.ExportTargetInterop;

namespace Uno.Data.Xml
{
    [DotNetType("System.Xml.XmlException")]
    public class XmlException : Exception
    {
        public XmlException(string message)
            : base(message)
        {
        }

        public XmlException(string message, string paramName)
            : base(paramName + ": " + message)
        {
        }
    }
}