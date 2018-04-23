using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Backends.UnoDoc.Builders
{
    public class AttachedUxProperty
    {
        public string Name { get; private set; }
        public Method UnderlyingMethod { get; private set; }
        public DataType ReturnType { get; private set; }

        public AttachedUxProperty(string name, Method underlyingMethod, DataType returnType)
        {
            Name = name;
            UnderlyingMethod = underlyingMethod;
            ReturnType = returnType;
        }
    }
}