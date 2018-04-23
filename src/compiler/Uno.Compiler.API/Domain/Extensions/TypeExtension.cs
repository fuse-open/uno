using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Domain.Extensions
{
    public class TypeExtension : ExtensionEntity
    {
        public readonly Dictionary<string, Element> Properties = new Dictionary<string, Element>();
        public readonly Dictionary<Function, FunctionExtension> MethodExtensions = new Dictionary<Function, FunctionExtension>();

        public TypeExtension(Source src, DataType dt, Disambiguation disamg = 0)
            : base(src, dt, disamg)
        {
        }
    }
}