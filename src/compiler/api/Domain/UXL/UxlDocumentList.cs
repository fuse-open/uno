using System.Collections.Generic;

namespace Uno.Compiler.API.Domain.UXL
{
    public static class UxlDocumentList
    {
        public static void Serialize(this IEnumerable<UxlDocument> list, SourceBundle bundle, string filename)
        {
            using (var w = UxlDocument.CreateWriter(bundle, filename))
                foreach (var e in list)
                    e.Write(w);
        }

        public static void Deserialize(SourceBundle bundle, string filename, List<UxlDocument> resultAsync)
        {
            using (var r = UxlDocument.CreateReader(bundle, filename))
            {
                while (r.BaseStream.Position < r.BaseStream.Length)
                {
                    var doc = UxlDocument.Read(r, bundle);

                    lock (resultAsync)
                        resultAsync.Add(doc);
                }
            }
        }
    }
}