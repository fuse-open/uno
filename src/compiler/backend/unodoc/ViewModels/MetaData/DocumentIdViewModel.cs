using System.Collections.Generic;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class DocumentIdViewModel
    {
        public string Id { get; private set; }
        public string ParentId { get; private set; }
        public string Type { get; private set; }
        public List<string> Modifiers { get; private set; } 

        public DocumentIdViewModel(string id, string parentId, string type, List<string> modifiers)
        {
            Id = id;
            ParentId = parentId;
            Type = type;
            Modifiers = modifiers;
        }
    }
}