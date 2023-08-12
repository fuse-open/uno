using System;
using System.Collections.Generic;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.API.Domain.AST.Members;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.Core.IL.Utilities;
using Uno.Compiler.Core.Syntax.Binding;
using Uno.Compiler.Core.Syntax.Compilers;
using Uno.Compiler.Core.Syntax.Generators;
using Uno.Compiler.Frontend.Analysis;
using Uno.Logging;

namespace Uno.Compiler.Core.Syntax.Builders
{
    public partial class BlockBuilder : LogObject
    {
        readonly Backend _backend;
        readonly Namespace _il;
        readonly ILFactory _ilf;
        readonly NameResolver _resolver;
        readonly Compiler _compiler;
        readonly BuildQueue _queue;
        readonly Dictionary<AstBlockBase, Block> _processedBlocks = new Dictionary<AstBlockBase, Block>();

        Block _terminals;
        readonly HashSet<string> _terminalNames = new HashSet<string>();

        internal BlockBuilder(
            Backend backend,
            Namespace il,
            ILFactory ilf,
            NameResolver resolver,
            Compiler compiler,
            BuildQueue queue)
            : base(compiler)
        {
            _backend = backend;
            _il = il;
            _ilf = ilf;
            _resolver = resolver;
            _compiler = compiler;
            _queue = queue;
        }

        public Block TerminalProperties
        {
            get
            {
                if (_terminals == null)
                {
                    _terminals = new Block(Source.Unknown, _il, null, Modifiers.Public | Modifiers.Generated, ".terminals");
                    _il.Blocks.Add(_terminals);

                    foreach (var t in _backend.ShaderBackend.InputProperties)
                        CompileTerminalProperty(t);
                    foreach (var t in _backend.ShaderBackend.OutputProperties)
                        CompileTerminalProperty(t);
                }

                return _terminals;
            }
        }

        void CompileTerminalProperty(TerminalProperty t)
        {
            var dt = _ilf.GetType(t.TypeString);
            var mp = new MetaProperty(Source.Unknown, _terminals, dt, t.Name, MetaVisibility.Public);
            var fc = new FunctionCompiler(_compiler, mp);

            _terminals.Members.Add(mp);
            _terminalNames.Add(mp.Name);

            if (t.DefaultString != null)
            {
                var e = Parser.ParseExpression(Log, Source.Unknown, t.DefaultString, ParseContext.MetaProperty);
                var v = fc.CompileExpression(e);
                mp.SetDefinitions(new MetaDefinition(v, new string[0]));
            }
            else
                mp.SetDefinitions();
        }
    }
}
