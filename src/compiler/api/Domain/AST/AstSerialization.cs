using System.Collections.Generic;

namespace Uno.Compiler.API.Domain.AST
{
    public static class AstSerialization
    {
        public const byte Version = 55;
        public const uint Magic = 0x4F4E55 | Version << 24;

        public static void Serialize(this IEnumerable<AstDocument> list, SourceBundle bundle, string filename, AstSerializationFlags flags)
        {
            using (var w = AstWriter.Create(bundle, filename, flags))
                foreach (var e in list)
                    w.WriteDocument(e);
        }

        public static void Deserialize(SourceBundle bundle, string filename, List<AstDocument> resultAsync)
        {
            using (var r = AstReader.Open(bundle, filename))
            {
                while (r.BaseStream.Position < r.BaseStream.Length)
                {
                    var ast = r.ReadDocument();
                    lock (resultAsync)
                        resultAsync.Add(ast);
                }
            }
        }

        public static void Serialize(this AstBlock block, string filename, AstSerializationFlags flags = 0)
        {
            using (var w = AstWriter.Create(block.Name.Source.Bundle, filename, flags))
                w.WriteBlock(block);
        }

        public static AstBlock DeserializeBlock(SourceBundle bundle, string filename)
        {
            using (var r = AstReader.Open(bundle, filename))
                return r.ReadBlock();
        }
    }
}
