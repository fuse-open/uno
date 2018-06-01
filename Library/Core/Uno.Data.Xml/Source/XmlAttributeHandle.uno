using Uno.Compiler.ExportTargetInterop;
using Uno.Collections;
using Uno.IO;
using Uno;

using System.Xml.Linq;

namespace Uno.Data.Xml
{
    [TargetSpecificImplementation]
    internal class XmlAttributeHandle
    {
        public string Name { get; private set; }
        public string Value { get; set; }

        public extern(!CIL) XmlAttributeHandle(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public extern(CIL) XmlAttributeHandle(XAttribute attrib)
        {
            Name = attrib.Name.LocalName;
            Value = attrib.Value;
        }
    }
}
