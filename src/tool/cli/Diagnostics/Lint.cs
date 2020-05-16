using System.Collections.Generic;
using System.IO;
using Uno.Compiler;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.Frontend.Analysis;

namespace Uno.CLI.Diagnostics
{
    public class Lint : Command
    {
        public override string Name => "lint";
        public override string Description => "Parses uno source files and output syntax errors";
        public override bool IsExperimental => true;

        public override void Help()
        {
            WriteUsage("[source-path ...]");
        }

        public override void Execute(IEnumerable<string> args)
        {
            foreach (var arg in args)
            {
                var path = Path.GetFullPath(arg);
                var parser = new Parser(Log, SourcePackage.Unknown, path, File.ReadAllText(path));
                var astDocs = new List<AstDocument>();
                parser.Parse(astDocs);
            }
        }
    }
}