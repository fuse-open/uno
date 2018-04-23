using System.Collections.Generic;
using Uno.Compiler.Backends.UnoDoc.ViewModels;

namespace Uno.Compiler.Backends.UnoDoc
{
    public class DocumentViewModelEqualityComparer : IEqualityComparer<DocumentViewModel>
    {
        public bool Equals(DocumentViewModel x, DocumentViewModel y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(DocumentViewModel obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}