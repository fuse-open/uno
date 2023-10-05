using System.Collections.Generic;
using Uno.Collections;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Domain.Extensions
{
    public class TypeExtension : ExtensionEntity
    {
        public readonly LowerCamelDictionary<Element> Properties = new();
        public readonly Dictionary<Function, FunctionExtension> MethodExtensions = new();

        public TypeExtension(Source src, DataType dt, Disambiguation disamg = 0)
            : base(src, dt, disamg)
        {
        }
    }
}