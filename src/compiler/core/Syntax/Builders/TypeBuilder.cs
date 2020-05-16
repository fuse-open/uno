using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.IL.Utilities;
using Uno.Compiler.Core.Syntax.Binding;
using Uno.Compiler.Core.Syntax.Compilers;
using Uno.Logging;

namespace Uno.Compiler.Core.Syntax.Builders
{
    public partial class TypeBuilder : LogObject
    {
        readonly BuildEnvironment _env;
        readonly ILFactory _ilf;
        readonly NameResolver _resolver;
        readonly Compiler _compiler;
        readonly Dictionary<AstClass, DataType> _cachedClasses = new Dictionary<AstClass, DataType>();
        readonly Dictionary<AstClass, AstClass> _flattenedClasses = new Dictionary<AstClass, AstClass>();
        readonly List<FunctionCompiler> _enqueuedCompilers = new List<FunctionCompiler>();
        readonly List<Entity> _enqueuedAttributes = new List<Entity>();
        readonly List<DataType> _enqueuedTypes = new List<DataType>();
        readonly List<Action> _enqueuedActions = new List<Action>();
        int _assignBaseTypeIndex;
        int _populateMembersIndex;

        public TypeBuilder(
            BuildEnvironment env,
            ILFactory ilf,
            NameResolver resolver,
            Compiler compiler)
            : base(compiler)
        {
            _env = env;
            _ilf = ilf;
            _resolver = resolver;
            _compiler = compiler;
        }

        void EnqueueCompiler(FunctionCompiler fc)
        {
            if (fc.Body != null)
            {
                _enqueuedCompilers.Add(fc);
                fc.Function.Tag = fc;
            }
            else if (_compiler.Backend.CanLink(fc.Function))
            {
                fc.Function.Stats |= EntityStats.CanLink;
            }
        }

        void EnqueueType(DataType dt, Action<DataType> assignBaseType, Action<DataType> populate)
        {
            dt.AssigningBaseType = assignBaseType;
            dt.PopulatingMembers = x =>
            {
                populate(x);
                x.Stats &= ~EntityStats.PopulatingMembers;
            };
            dt.Stats |= EntityStats.PopulatingMembers;
            _enqueuedTypes.Add(dt);
        }

        void EnqueueAttributes(Entity e, Action<Entity> assign)
        {
            e.AssigningAttributes = assign;
            _enqueuedAttributes.Add(e);
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
                {
                    if (_compiler.Backend.CanLink(_enqueuedTypes[_populateMembersIndex]))
                        _enqueuedTypes[_populateMembersIndex].Stats |= EntityStats.CanLink;

                    _enqueuedTypes[_populateMembersIndex++].PopulateMembers();
                }

                for (int i = 0; i < _enqueuedActions.Count; i++)
                    _enqueuedActions[i]();

                _enqueuedActions.Clear();
            }

            for (; _populateMembersIndex < _enqueuedTypes.Count; _populateMembersIndex++)
                Log.Warning(_enqueuedTypes[_populateMembersIndex].Source, ErrorCode.I0000, "Unable to parameterize " + _enqueuedTypes[_populateMembersIndex].Quote());

            _assignBaseTypeIndex = _populateMembersIndex;
        }

        public void Build()
        {
            BuildTypes();

            if (Log.HasErrors)
                return;

            for (int i = 0; i < _enqueuedCompilers.Count; i++)
            {
                BuildTypes();
                _enqueuedCompilers[i].Compile();
            }

            BuildTypes();

            _enqueuedCompilers.Clear();
            _enqueuedTypes.Clear();

            _assignBaseTypeIndex = 0;
            _populateMembersIndex = 0;
        }

        public RefArrayType GetArray(DataType elementType)
        {
            return elementType.RefArray ?? (
                   elementType.RefArray =
                        elementType.Equals(_ilf.Essentials.Array)
                            ? RefArrayType.CreateMaster(_ilf.Essentials.Int, _ilf.Essentials.Array)
                            : RefArrayType.Create(GetArray(_ilf.Essentials.Array), elementType)
                );
        }

        Method CreateMethod(DataType owner, string name, DataType returnType, params Parameter[] parameterList)
        {
            return new Method(owner.Source, owner, null,
                            Modifiers.Public | Modifiers.Intrinsic | Modifiers.Generated, 
                            name, returnType, parameterList);
        }

        Operator CreateOperator(DataType owner, OperatorType type, DataType returnType, params Parameter[] parameterList)
        {
            return new Operator(owner.Source, owner, type, null,
                        Modifiers.Public | Modifiers.Static | Modifiers.Intrinsic | Modifiers.Generated,
                        returnType, parameterList);
        }

        Parameter CreateParameter(SourceObject owner, DataType type, string name)
        {
            return new Parameter(owner.Source, AttributeList.Empty, 0, type, name, null);
        }

        void CreateGenericSignature(DataType gt, AstGenericSignature sig, bool isOverriddenMethodOrExplicitInterfaceImplementation)
        {
            var result = new GenericParameterType[sig.Parameters.Count];

            if (isOverriddenMethodOrExplicitInterfaceImplementation)
            {
                if (sig.Constraints.Count > 0)
                    Log.Error(sig.Constraints[0].Parameter.Source, ErrorCode.E0000, "Constraints for override and explicit interface implementation methods are inherited from the base method, so they cannot be specified directly");
            }
            else
            {
                for (int i = 0; i < sig.Constraints.Count; i++)
                {
                    var c1 = sig.Constraints[i];

                    var found = false;

                    for (int j = 0; j < sig.Parameters.Count; j++)
                    {
                        if (sig.Parameters[j].Symbol == c1.Parameter.Symbol)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        Log.Error(c1.Parameter.Source, ErrorCode.E0000, "Generic parameter " + c1.Parameter.Symbol.Quote() + " does not exist");
                        continue;
                    }

                    for (int j = 0; j < i; j++)
                    {
                        var c2 = sig.Constraints[j];

                        if (c1.Parameter.Symbol == c2.Parameter.Symbol)
                            Log.Error(c1.Parameter.Source, ErrorCode.E0000, "Generic constraint for parameter " + c1.Parameter.Symbol.Quote() + " is already defined at " + c2.Parameter.Source);
                    }
                }
            }

            for (int i = 0, l = sig.Parameters.Count; i < l; i++)
            {
                var p = sig.Parameters[i];
                result[i] = new GenericParameterType(p.Source, gt, p.Symbol);

                for (int j = 0; j < sig.Constraints.Count; j++)
                {
                    var c = sig.Constraints[j];

                    if (c.Parameter.Symbol == p.Symbol)
                    {
                        if (c.BaseTypes != null)
                            EnqueueType(result[i], x => CompileBaseTypes(x, c.BaseTypes), x => { });

                        if (c.Type != 0)
                            result[i].SetConstraintType((GenericConstraintType) c.Type);

                        if (c.OptionalConstructor != null)
                            result[i].Constructors.Add(new Constructor(c.OptionalConstructor, result[i], null, Modifiers.Public | Modifiers.Generated | Modifiers.Extern, ParameterList.Empty));

                        break;
                    }
                }

                if (result[i].AssigningBaseType == null)
                    EnqueueType(result[i], x => x.SetBase(_ilf.Essentials.Object), x => { });
            }

            gt.MakeGenericDefinition(result);
        }

        internal static Modifiers GetTypeModifiers(Namescope parent, Modifiers modifiers)
        {
            if ((modifiers & Modifiers.ProtectionModifiers) == 0)
                modifiers |= parent is DataType ? Modifiers.Private : Modifiers.Internal;

            return modifiers;
        }

        Modifiers GetMemberModifiers(Source src, DataType parent, Modifiers modifiers)
        {
            if (parent is InterfaceType)
            {
                if ((modifiers & ~Modifiers.Extern) != 0)
                    Log.Error(src, ErrorCode.E0000, (modifiers & ~Modifiers.Extern).ToLiteral().Quote() + " is not valid for interface member");

                // All interface members are implicit 'public abstract'
                modifiers |= Modifiers.Public | Modifiers.Abstract;
            }
            else if ((modifiers & Modifiers.ProtectionModifiers) == 0)
                modifiers |= Modifiers.Private;

            return modifiers;
        }

        Modifiers GetAccessorModifiers(Source src, Member parent, Modifiers modifiers)
        {
            if (modifiers != 0)
            {
                if ((modifiers & ~Modifiers.ProtectionModifiers) != 0)
                    Log.Error(src, ErrorCode.E3043, (modifiers & ~Modifiers.ProtectionModifiers).ToLiteral().Quote() + " is not valid for this item");

                return (parent.Modifiers & ~Modifiers.ProtectionModifiers) | modifiers;
            }

            return parent.Modifiers;
        }
    }
}
