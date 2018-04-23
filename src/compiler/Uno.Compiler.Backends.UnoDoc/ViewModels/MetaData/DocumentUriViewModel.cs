using System;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class DocumentUriViewModel
    {
        public string IdUri { get; private set; }
        public string Href { get; private set; }
        public bool IsVirtual { get; private set; }

        public DocumentUriViewModel(string idUri, string href, bool isVirtual)
        {
            if (idUri == null)
            {
                Console.Write("");
            }
            IdUri = idUri;
            Href = href;
            IsVirtual = isVirtual;
        }
    }
}