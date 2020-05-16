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
        readonly Dictionary<AstBlockBase, Block> _processedBlocks = new Dictionary<AstBlockBase, Block>();
        readonly List<BlockBase> _enqueuedBlocks = new List<BlockBase>();
        readonly List<KeyValuePair<AstMetaProperty, MetaProperty>> _enqueuedProperties = new List<KeyValuePair<AstMetaProperty, MetaProperty>>();
        readonly HashSet<DataType> _enqueuedDrawClasses = new HashSet<DataType>();

        Block _terminals;
        readonly HashSet<string> _terminalNames = new HashSet<string>();

        public BlockBuilder(
            Backend backend,
            Namespace il,
            ILFactory ilf,
            NameResolver resolver,
            Compiler compiler)
            : base(compiler)
        {
            _backend = backend;
            _il = il;
            _ilf = ilf;
            _resolver = resolver;
            _compiler = compiler;
        }

        void EnqueueBlock(BlockBase b, Action<BlockBase> populate)
        {
            b.Populating = populate;
            _enqueuedBlocks.Add(b);
        }

        public void Build()
        {
            FlattenTypes();

            for (int i = 0; i < _enqueuedBlocks.Count; i++)
                if (!_enqueuedBlocks[i].Source.Package.CanLink)
                    _enqueuedBlocks[i].Populate();

            _enqueuedBlocks.Clear();

            for (int i = 0; i < _enqueuedProperties.Count; i++)
                CompileMetaPropertyDefinitions(_enqueuedProperties[i].Key, _enqueuedProperties[i].Value);

            _enqueuedProperties.Clear();

            foreach (var dt in _enqueuedDrawClasses)
                DrawCallGenerator.GenerateDrawCalls(_compiler, dt);

            _enqueuedDrawClasses.Clear();
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
