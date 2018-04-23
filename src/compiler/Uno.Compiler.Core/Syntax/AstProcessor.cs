using System.Collections.Generic;
using Uno.Collections;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.API.Domain.AST.Members;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.Core.Syntax.Binding;
using Uno.Compiler.Core.Syntax.Builders;
using Uno.Logging;

namespace Uno.Compiler.Core.Syntax
{
    public class AstProcessor : LogObject
    {
        readonly Namespace _il;
        readonly BlockBuilder _blockBuilder;
        readonly TypeBuilder _typeBuilder;
        readonly NameResolver _resolver;
        readonly BuildEnvironment _env;
        readonly List<AstILNode> _nodes = new List<AstILNode>();
        readonly ListDictionary<Namespace, AstNamespace> _astMap = new ListDictionary<Namespace, AstNamespace>();

        public AstProcessor(Namespace il, BlockBuilder blockBuilder, TypeBuilder typeBuilder, NameResolver resolver, BuildEnvironment env)
            : base(env)
        {
            _il = il;
            _blockBuilder = blockBuilder;
            _typeBuilder = typeBuilder;
            _resolver = resolver;
            _env = env;
        }

        public void AddRange(IEnumerable<AstNamespace> ast)
        {
            foreach (var ns in ast)
                _nodes.Add(CreateNode(ns, _il));
        }

        public void Add(AstNamespace ns)
        {
            _nodes.Add(CreateNode(ns, _il));
        }

        AstILNode CreateNode(AstNamespace ns, Namespace parent)
        {
            if (parent.UnoName != ns.Name.Symbol)
                throw new FatalException(ns.Name.Source, ErrorCode.I3331, "Invalid namespace name");

            parent.Packages.Add(ns.Name.Source.Package);
            _astMap.Add(parent, ns);

            var result = new AstILNode(parent, ns);

            foreach (var e in ns.Namespaces)
                result.Nodes.Add(CreateNode(e, _resolver.GetNamespace(parent, e.Name.Symbol)));

            return result;
        }

        public void Process()
        {
            CreateBlocks(_il);
            _astMap.Clear();
            _resolver.AddUsings(_nodes, new Namespace[0], new DataType[0]);
            _nodes.Clear();
        }

        void CreateBlocks(Namespace ns)
        {
            List<AstNamespace> ast;
            if (_astMap.TryGetValue(ns, out ast))
            {
                var items = new List<AstBlockBase>();

                foreach (var e in ast)
                    if (_env.Test(e.Name.Source.Package.Source, e.Name.Source.Package.BuildCondition))
                        items.AddRange(e.Blocks);

                foreach (var e in items)
                    CreateBlock(e, ns, items);
            }

            foreach (var e in ns.Namespaces)
                CreateBlocks(e);
        }

        public void CreateBlock(AstBlockBase block, Namescope parent, IEnumerable<AstBlockMember> parentItems)
        {
            if (!_env.Test(block.Name.Source, block.OptionalCondition))
                return;

            switch (block.MemberType)
            {
                case AstMemberType.Class:
                    _typeBuilder.CreateClass((AstClass)block, parent, parentItems);
                    break;
                case AstMemberType.Delegate:
                    _typeBuilder.CreateDelegate((AstDelegate)block, parent);
                    break;
                case AstMemberType.Enum:
                    _typeBuilder.CreateEnum((AstEnum)block, parent);
                    break;
                case AstMemberType.Block:
                    _blockBuilder.CreateBlock((AstBlock)block, parent);
                    break;
            }
        }
    }
}
