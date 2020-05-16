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

        public override bool Parse(SourcePackage upk, string filename, List<AstDocument> result)
        {
            var p = new Parser(Log, upk, filename, File.ReadAllText(filename));
            p.Parse(result);
            return !p.HasErrors;
        }

        public override void Deserialize(SourcePackage upk, string filename, List<AstDocument> resultAsync)
        {
            AstSerialization.Deserialize(upk, filename, resultAsync);
        }

        public override void Serialize(SourcePackage upk, string filename, IEnumerable<AstDocument> value)
        {
            value.Serialize(upk, filename, 0);
        }
    }
}
