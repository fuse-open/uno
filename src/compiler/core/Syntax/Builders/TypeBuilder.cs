using System;
using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.IL.Utilities;
using Uno.Compiler.Core.Syntax.Binding;
using Uno.Logging;

namespace Uno.Compiler.Core.Syntax.Builders
{
    public partial class TypeBuilder : LogObject
    {
        readonly BuildEnvironment _env;
        readonly ILFactory _ilf;
        readonly NameResolver _resolver;
        readonly Compiler _compiler;
        readonly BuildQueue _queue;
        readonly Dictionary<AstClass, DataType> _cachedClasses = new Dictionary<AstClass, DataType>();
        readonly Dictionary<AstClass, AstClass> _flattenedClasses = new Dictionary<AstClass, AstClass>();

        internal TypeBuilder(
            BuildEnvironment env,
            ILFactory ilf,
            NameResolver resolver,
            Compiler compiler,
            BuildQueue queue)
            : base(compiler)
        {
            _env = env;
            _ilf = ilf;
            _resolver = resolver;
            _compiler = compiler;
            _queue = queue;
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
                            _queue.EnqueueType(result[i], x => CompileBaseTypes(x, c.BaseTypes), x => { });

                        if (c.Type != 0)
                            result[i].SetConstraintType((GenericConstraintType) c.Type);

                        if (c.OptionalConstructor != null)
                            result[i].Constructors.Add(new Constructor(c.OptionalConstructor, result[i], null, Modifiers.Public | Modifiers.Generated | Modifiers.Extern, ParameterList.Empty));

                        break;
                    }
                }

                if (result[i].AssigningBaseType == null)
                    _queue.EnqueueType(result[i], x => x.SetBase(_ilf.Essentials.Object), x => { });
            }

            gt.MakeGenericDefinition(result);
        }

        internal static Modifiers GetTypeModifiers(Namescope parent, Modifiers modifiers)
        {
            return (modifiers & Modifiers.ProtectionModifiers) == 0
                ? modifiers | (
                    parent is DataType
                        ? Modifiers.Private
                        : Modifiers.Internal)
                : modifiers;
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
