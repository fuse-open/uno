using System.Collections.Generic;
using System.IO;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.Frontend.Analysis;
using Uno.Logging;

namespace Uno.Compiler.Frontend
{
    public class AstCache : CacheBase<AstDocument>
    {
        public AstCache(Log log, HashSet<string> processedFiles)
            : base(log, AstSerialization.Magic, processedFiles)
        {
        }

        public override bool Parse(SourceBundle bundle, string filename, List<AstDocument> result)
        {
            var p = new Parser(Log, bundle, filename, File.ReadAllText(filename));
            p.Parse(result);
            return !p.HasErrors;
        }

        public override void Deserialize(SourceBundle bundle, string filename, List<AstDocument> resultAsync)
        {
            AstSerialization.Deserialize(bundle, filename, resultAsync);
        }

        public override void Serialize(SourceBundle bundle, string filename, IEnumerable<AstDocument> value)
        {
            value.Serialize(bundle, filename, 0);
        }
    }
}
