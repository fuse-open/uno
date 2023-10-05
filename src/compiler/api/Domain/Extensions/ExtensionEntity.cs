using System.Collections.Generic;
using Uno.Collections;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Domain.Extensions
{
    public class ExtensionEntity : IDisambiguable
    {
        public EntityStats Status;
        public readonly Source Source;
        public readonly object Object;
        public readonly Disambiguation Disambiguation;
        public readonly List<CopyFile> CopyFiles = new();
        public readonly List<ImageFile> ImageFiles = new();
        public readonly LowerCamelListDictionary<Element> Requirements = new();
        public readonly HashSet<IEntity> RequiredEntities = new();
        public readonly HashSet<ExtensionEntity> RequiredTemplates = new();

        Source IDisambiguable.Source => Source;
        Disambiguation IDisambiguable.Disambiguation => Disambiguation;

        public ExtensionEntity(Source src, object obj, Disambiguation disamg)
        {
            Source = src;
            Object = obj;
            Disambiguation = disamg;
        }

        public override string ToString()
        {
            return (Object ?? "<null>").ToString();
        }
    }
}
