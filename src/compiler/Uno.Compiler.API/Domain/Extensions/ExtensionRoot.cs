using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Domain.Extensions
{
    public class ExtensionRoot : ExtensionEntity
    {
        public readonly Dictionary<string, Element> Properties = new Dictionary<string, Element>();
        public readonly Dictionary<string, ExtensionEntity> Templates = new Dictionary<string, ExtensionEntity>();
        public readonly Dictionary<DataType, TypeExtension> TypeExtensions = new Dictionary<DataType, TypeExtension>();
        public readonly HashSet<string> ElementDefinitions = new HashSet<string> { "Entity", "Template" };
        public readonly HashSet<string> TypeElementDefinitions = new HashSet<string>();
        public readonly HashSet<string> TypePropertyDefinitions = new HashSet<string>();
        public readonly HashSet<string> MethodPropertyDefinitions = new HashSet<string>();
        public readonly HashSet<string> Defines = new HashSet<string>();
        public readonly List<BundleFile> BundleFiles = new List<BundleFile>();

        public ExtensionRoot()
            : base(Source.Unknown, "<root>", 0)
        {
        }
    }
}