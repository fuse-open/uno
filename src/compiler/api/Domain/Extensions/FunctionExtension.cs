using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Domain.Extensions
{
    public class FunctionExtension : ExtensionEntity
    {
        public readonly Dictionary<string, Element> Properties = new Dictionary<string, Element>();
        public readonly Namescope[] Scopes;

        public Source ImplementationSource { get; private set; }
        public ImplementationType ImplementationType { get; private set; }
        public string ImplementationString { get; private set; }
        public bool IsDefaultImplementation { get; private set; }
        public bool HasImplementation => ImplementationType != ImplementationType.None;

        public FunctionExtension(Source src, Function func, Disambiguation disamg = 0, params Namescope[] scopes)
            : base(src, func, disamg)
        {
            Scopes = scopes;
        }

        public void SetImplementation(Source src, ImplementationType type, string str, bool isDefault)
        {
            ImplementationSource = src;
            ImplementationType = type;
            ImplementationString = str;
            IsDefaultImplementation = isDefault;
        }
    }
}
