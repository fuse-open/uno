using System.Collections.Generic;

namespace Uno.Compiler.API.Domain.UXL
{
    public static class UxlDocumentList
    {
        public static void Serialize(this IEnumerable<UxlDocument> list, SourcePackage upk, string filename)
        {
            using (var w = UxlDocument.CreateWriter(upk, filename))
                foreach (var e in list)
                    e.Write(w);
        }

        public static void Deserialize(SourcePackage upk, string filename, List<UxlDocument> resultAsync)
        {
            using (var r = UxlDocument.CreateReader(upk, filename))
            {
                while (r.BaseStream.Position < r.BaseStream.Length)
                {
                    var doc = UxlDocument.Read(r, upk);

                    lock (resultAsync)
                        resultAsync.Add(doc);
                }
            }
        }
    }
}