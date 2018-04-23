using System.Collections.Generic;
using Uno.Compiler.API.Domain.UXL;
using Uno.Logging;

namespace Uno.Compiler.Frontend
{
    public class UxlCache : CacheBase<UxlDocument>
    {
        public UxlCache(Log log, HashSet<string> processedFiles)
            : base(log, UxlDocument.Magic, processedFiles)
        {
        }

        public override bool Parse(SourcePackage upk, string filename, List<UxlDocument> result)
        {
            return UxlParser.Parse(Log, upk, filename, result);
        }

        public override void Deserialize(SourcePackage upk, string filename, List<UxlDocument> resultAsync)
        {
            UxlDocumentList.Deserialize(upk, filename, resultAsync);
        }

        public override void Serialize(SourcePackage upk, string filename, IEnumerable<UxlDocument> value)
        {
            value.Serialize(upk, filename);
        }
    }
}
