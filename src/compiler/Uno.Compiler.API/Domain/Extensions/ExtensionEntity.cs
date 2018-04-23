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
        public readonly List<CopyFile> CopyFiles = new List<CopyFile>();
        public readonly List<ImageFile> ImageFiles = new List<ImageFile>();
        public readonly ListDictionary<string, Element> Requirements = new ListDictionary<string, Element>();
        public readonly HashSet<IEntity> RequiredEntities = new HashSet<IEntity>();
        public readonly HashSet<ExtensionEntity> RequiredTemplates = new HashSet<ExtensionEntity>();

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
