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

        public override bool Parse(SourceBundle bundle, string filename, List<UxlDocument> result)
        {
            return UxlParser.Parse(Log, bundle, filename, result);
        }

        public override void Deserialize(SourceBundle bundle, string filename, List<UxlDocument> resultAsync)
        {
            UxlDocumentList.Deserialize(bundle, filename, resultAsync);
        }

        public override void Serialize(SourceBundle bundle, string filename, IEnumerable<UxlDocument> value)
        {
            value.Serialize(bundle, filename);
        }
    }
}
