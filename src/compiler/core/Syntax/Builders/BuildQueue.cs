using System;
using System.Collections.Generic;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.AST.Members;
using Uno.Compiler.API.Domain.AST.Statements;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.Core.Syntax.Compilers;
using Uno.Compiler.Core.Syntax.Generators;
using Uno.Logging;

namespace Uno.Compiler.Core.Syntax.Builders
{
    public class BuildQueue : LogObject
    {
        readonly BuildEnvironment _env;
        readonly Backend _backend;
        readonly Compiler _compiler;
        readonly List<BlockBase> _enqueuedBlocks = new List<BlockBase>();
        readonly List<KeyValuePair<AstMetaProperty, MetaProperty>> _enqueuedMetaProperties = new List<KeyValuePair<AstMetaProperty, MetaProperty>>();
        readonly HashSet<DataType> _enqueuedDrawClasses = new HashSet<DataType>();
        readonly List<FunctionCompiler> _enqueuedFunctions = new List<FunctionCompiler>();
        readonly List<Entity> _enqueuedAttributes = new List<Entity>();
        readonly List<DataType> _enqueuedTypes = new List<DataType>();
        readonly List<Action> _enqueuedActions = new List<Action>();
        int _assignBaseTypeIndex;
        int _populateMembersIndex;

        public BuildQueue(Log log, BuildEnvironment env, Backend backend, Compiler compiler)
            : base(log)
        {
            _env = env;
            _backend = backend;
            _compiler = compiler;
        }

        internal FunctionCompiler EnqueueFunction(Function func, DataType parameterizedParent, AstScope body)
        {
            var fc = new FunctionCompiler(_compiler, func, parameterizedParent, body);
            func.Tag = fc;

            if (_env.IsGeneratingCode)
                func.Stats |= EntityStats.CanLink;
            else if (body != null)
                _enqueuedFunctions.Add(fc);
            else if (_compiler.Backend.CanLink(func))
                func.Stats |= EntityStats.CanLink;

            return fc;
        }

        internal void EnqueueDrawClass(DataType declaringType)
        {
            _enqueuedDrawClasses.Add(declaringType);
        }

        internal void EnqueueType(DataType dt, Action<DataType> assignBaseType, Action<DataType> populate)
        {
            if (_env.IsGeneratingCode)
            {
                assignBaseType(dt);
                populate(dt);
                return;
            }

            dt.AssigningBaseType = assignBaseType;
            dt.PopulatingMembers = x =>
            {
                populate(x);
                x.Stats &= ~EntityStats.PopulatingMembers;
            };
            dt.Stats |= EntityStats.PopulatingMembers;
            _enqueuedTypes.Add(dt);
        }

        internal void EnqueueAttributes(Entity e, Action<Entity> assign)
        {
            e.AssigningAttributes = assign;
            _enqueuedAttributes.Add(e);
        }

        internal void EnqueueAttributes(Member e, DataType parameterizedParent, IReadOnlyList<AstAttribute> attributes)
        {
            if (attributes.Count > 0)
                EnqueueAttributes(e, x => e.SetAttributes(_compiler.CompileAttributes(parameterizedParent, attributes)));
        }

        public void BuildTypes()
        {
            for (int count = 0, j = 0; count != _enqueuedTypes.Count && j < 10; j++)
            {
                count = _enqueuedTypes.Count;

                while (_assignBaseTypeIndex < count)
                    _enqueuedTypes[_assignBaseTypeIndex++].AssignBaseType();

                for (int i = 0; i < _enqueuedAttributes.Count; i++)
                    _enqueuedAttributes[i].AssignAttributes();

                _enqueuedAttributes.Clear();

                while (_populateMembersIndex < count)
                    if (_backend.CanLink(_enqueuedTypes[_populateMembersIndex]))
                        _enqueuedTypes[_populateMembersIndex++].Stats |= EntityStats.CanLink;
                    else
                        _enqueuedTypes[_populateMembersIndex++].PopulateMembers();

                for (int i = 0; i < _enqueuedActions.Count; i++)
                    _enqueuedActions[i]();

                _enqueuedActions.Clear();
            }

            for (; _populateMembersIndex < _enqueuedTypes.Count; _populateMembersIndex++)
                Log.Warning(_enqueuedTypes[_populateMembersIndex].Source, ErrorCode.I0000, "Unable to parameterize " + _enqueuedTypes[_populateMembersIndex].Quote());

            _assignBaseTypeIndex = _populateMembersIndex;
        }

        public void BuildTypesAndFunctions()
        {
            BuildTypes();

            for (var i = 0; i < _enqueuedFunctions.Count; i++)
            {
                _enqueuedFunctions[i].Compile();
                BuildTypes();
            }

            _enqueuedFunctions.Clear();
            _enqueuedTypes.Clear();

            _assignBaseTypeIndex = 0;
            _populateMembersIndex = 0;
        }

        public void BuildEverything()
        {
            _compiler.BlockBuilder.FlattenTypes();

            do
            {
                BuildTypesAndFunctions();
                BuildBlocks();
                BuildMetaProperties();

                foreach (var dt in _enqueuedDrawClasses)
                    DrawCallGenerator.GenerateDrawCalls(_compiler, dt);

                _enqueuedDrawClasses.Clear();

            } while (_enqueuedFunctions.Count > 0);
        }

        public void BuildBlocks()
        {
            for (var i = 0; i < _enqueuedBlocks.Count; i++)
                if (!_enqueuedBlocks[i].Source.Bundle.CanLink)
                    _enqueuedBlocks[i].Populate();

            _enqueuedBlocks.Clear();
        }

        internal void EnqueueBlock(BlockBase b, Action<BlockBase> populate)
        {
            b.Populating = populate;
            _enqueuedBlocks.Add(b);
        }

        public void BuildMetaProperties()
        {
            for (var i = 0; i < _enqueuedMetaProperties.Count; i++)
                _compiler.BlockBuilder.CompileMetaPropertyDefinitions(_enqueuedMetaProperties[i].Key, _enqueuedMetaProperties[i].Value);

            _enqueuedMetaProperties.Clear();
        }

        public void EnqueueMetaProperty(AstMetaProperty ast, MetaProperty result)
        {
            _enqueuedMetaProperties.Add(new KeyValuePair<AstMetaProperty, MetaProperty>(ast, result));
        }

        internal void Enqueue(Action action)
        {
            _enqueuedActions.Add(action);
        }
    }
}