using System.Collections.Generic;
using Uno.Collections;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Domain.Extensions
{
    public class ExtensionRoot : ExtensionEntity
    {
        public readonly LowerCamelDictionary<Element> Properties = new();
        public readonly LowerCamelDictionary<ExtensionEntity> Templates = new();
        public readonly Dictionary<DataType, TypeExtension> TypeExtensions = new();
        public readonly LowerCamelSet ElementDefinitions = new LowerCamelSet { "entity", "template" };
        public readonly LowerCamelSet TypeElementDefinitions = new();
        public readonly LowerCamelSet TypePropertyDefinitions = new();
        public readonly LowerCamelSet MethodPropertyDefinitions = new();
        public readonly HashSet<string> Defines = new();
        public readonly List<BundleFile> BundleFiles = new();

        public ExtensionRoot()
            : base(Source.Unknown, "<root>", 0)
        {
        }
    }
}