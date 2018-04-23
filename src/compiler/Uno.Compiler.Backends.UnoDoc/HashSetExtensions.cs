using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.Backends.UnoDoc.ViewModels;

namespace Uno.Compiler.Backends.UnoDoc
{
    public static class HashSetExtensions
    {
        public static void AddIfNotExists(this HashSet<DocumentViewModel> hashSet, DocumentViewModel document)
        {
            if (hashSet.Contains(document))
            {
                var existing = hashSet.Single(e => e.Id == document.Id);
                throw new ArgumentException("Attempted to add document " + document.Id + " to target collection - document was already added");
            }
            hashSet.Add(document);
        }
    }
}