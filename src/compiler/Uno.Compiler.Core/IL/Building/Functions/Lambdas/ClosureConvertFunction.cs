using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.IL.Building.Functions.Lambdas
{
    /// <summary>
    /// Create a closure object for each scope that contains
    /// (or whose child scopes contain) a lambda, linking the closure object
    /// to the parent scope's closure object to get sharing of local variables
    /// across lambdas in different scopes.
    ///
    /// The top-most scope gets 'this' and any parameters that are captured.
    /// Usages of captured local variables or parameters are rewritten to
    /// variables in the closure objects.
    ///
    /// Lambdas are added to the closure object and rewritten to delegate calls.
    /// </summary>
    class ClosureConvertFunction : CompilerPass
    {
        private readonly ClosureConversionTransform _closureConversionTransform;

        readonly Variables _closureVars;

        readonly Stack<Closure> _closureStack = new Stack<Closure>();

        readonly Stack<Tree<HashSet<Variable>>> _capturedLocals = new Stack<Tree<HashSet<Variable>>>();

        readonly Stack<int> _capturedLocalsCursor = new Stack<int>();

        // The try-catches, if any, that we are currently inside of
        readonly Stack<TryCatchFinally> _tryCatches = new Stack<TryCatchFinally>();

        Lambda _lambda; // Set if inside a lambda that we're currently converting, otherwise null
        Method _lambdaMethod; // Set to the generated method if inside a lambda that we're currently converting, otherwise null

        // Types to recurse into after this pass is done
        readonly List<DataType> _generatedTypes = new List<DataType>();

        public ClosureConvertFunction(
            ClosureConversionTransform parent,
            Variables closureVars,
            Tree<HashSet<Variable>> capturedLocals)
            : base(parent)
        {
            _closureConversionTransform = parent;
            _closureVars = closureVars;
            _capturedLocals.Push(capturedLocals);
            _capturedLocalsCursor.Push(0);
        }

        Closure CreateClosure(Closure parent, HashSet<Variable> locals)
        {
            if (parent != null && locals.Count == 0)
                return parent;

            var type = new ClassType(
                Source.Unknown,
                Function.DeclaringType,
                Function.Name + " lambda closure",
                Modifiers.Private | Modifiers.Generated,
                Function.DeclaringType.GetUniqueIdentifier(Function.Name + "_$closure"));

            type.SetBase(Essentials.Object);

            var constrBody = new Scope(
                Source.Unknown,
                new CallConstructor(Source.Unknown, Essentials.Object.TryGetConstructor()));

            type.Constructors.Add(
                new Constructor(
                    Source.Unknown,
                    type,
                    "",
                    Modifiers.Public | Modifiers.Generated,
                    ParameterList.Empty,
                    constrBody));

            Function.DeclaringType.NestedTypes.Add(type);
            _generatedTypes.Add(type);

            var decl = new VariableDeclaration(
                Source.Unknown,
                Function,
                Function.DeclaringType.GetUniqueIdentifier("generated_closure"),
                type,
                optionalValue: ILFactory.NewObject(type));

            var statements = new List<Statement>();

            statements.Add(decl);

            var result = new Closure(type, statements, new LoadLocal(Source.Unknown, decl.Variable), parent);

            // The root closure gets 'this' and the params
            if (parent == null)
            {
                if (_closureVars.This)
                {
                    var field = new Field(
                        Source.Unknown,
                        type,
                        type.GetUniqueIdentifier("self"),
                        "",
                        Modifiers.Public | Modifiers.Generated,
                        0,
                        Function.DeclaringType);

                    type.Fields.Add(field);

                    statements.Add(result.StoreThis(new This(Function.DeclaringType)));
                }

                foreach (var p in _closureVars.Params)
                {
                    var field = new Field(
                        p.Source,
                        type,
                        type.GetUniqueIdentifier(p.Name),
                        "",
                        Modifiers.Public | Modifiers.Generated,
                        0,
                        p.Type);

                    type.Fields.Add(field);
                    result.ParameterFields[p] = field;
                    statements.Add(result.Store(p, new LoadArgument(Source.Unknown, Function, ParamIndex(p))));
                }
            }
            // Non-root closures get a parent field for the parent closure
            else
            {
                type.Fields.Add(
                    new Field(
                        Source.Unknown,
                        type,
                        type.GetUniqueIdentifier("parent"),
                        "",
                        Modifiers.Public | Modifiers.Generated,
                        0,
                        parent.Type));
                statements.Add(result.StoreParent(parent.Expression));
            }

            foreach (var v in locals)
            {
                var field = new Field(
                    v.Source,
                    type,
                    type.GetUniqueIdentifier(v.Name),
                    "",
                    Modifiers.Public | Modifiers.Generated,
                    0,
                    v.ValueType);

                type.Fields.Add(field);
                result.VariableFields[v] = field;
            }

            return result;
        }

        int ParamIndex(Parameter p)
        {
            for (var i = 0; i < Function.Parameters.Length; ++i)
                if (Function.Parameters[i] == p)
                    return i;

            throw new InvalidOperationException("ClosureConvertFunction: Can't find parameter " + p);
        }

        public override void End(Function f)
        {
            foreach (var dt in _generatedTypes)
                dt.Visit(_closureConversionTransform);
        }

        public override void BeginScope(Scope scope)
        {
            if (_lambda != null)
                return;

            _capturedLocals.Push(_capturedLocals.Peek().Children[_capturedLocalsCursor.Peek()]);
            _capturedLocalsCursor.Push(_capturedLocalsCursor.Pop() + 1);
            _capturedLocalsCursor.Push(0);
            _closureStack.Push(
                CreateClosure(
                    _closureStack.Count == 0 ? null : _closureStack.Peek(),
                    _capturedLocals.Peek().Data));
        }

        public override void EndScope(Scope scope)
        {
            if (_lambda != null)
                return;

            var i = 0;

            var closure = _closureStack.Pop();

            // The parent scope can have the same closure object if its captured variables
            // are the same, so we only emit init statements for the top-most occurerence
            // of each closure
            if (_closureStack.Count == 0 || _closureStack.Peek() != closure)
                foreach (var statement in closure.InitStatements)
                    scope.Statements.Insert(i++, statement);

            if (_tryCatches.Count > 0)
            {
                var catchBlock = _tryCatches.Peek().CatchBlocks.FirstOrDefault(cb => cb.Body == scope);
                if (catchBlock != null)
                {
                    // This scope is a catch block
                    var v = catchBlock.Exception;
                    if (_capturedLocals.Peek().Data.Contains(v))
                    {
                        // Assign the exception variable to the closure variable
                        scope.Statements.Insert(
                            i++,
                            closure.Store(v, new LoadLocal(Source.Unknown, v)));
                    }
                }
            }

            _capturedLocals.Pop();
            _capturedLocalsCursor.Pop();
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.Lambda:
                {
                    if (_lambda != null)
                        break;

                    _lambda = (Lambda)e;

                    var closureType = _closureStack.Peek().Type;

                    _lambdaMethod = new Method(
                        e.Source,
                        _closureStack.Peek().Type,
                        "Lambda method",
                        Modifiers.Public | Modifiers.Generated,
                        closureType.GetUniqueIdentifier("generated_lambda"),
                        _lambda.DelegateType.ReturnType,
                        _lambda.Parameters,
                        _lambda.Body as Scope ?? new Scope(_lambda.Source, _lambda.Body));

                    closureType.Methods.Add(_lambdaMethod);

                    break;
                }
            }
        }

        public override void Begin(ref Statement e)
        {
            switch (e.StatementType)
            {
                case StatementType.TryCatchFinally:
                {
                    _tryCatches.Push((TryCatchFinally)e);
                    break;
                }
            }
        }

        public override void End(ref Statement s)
        {
            switch (s.StatementType)
            {
                case StatementType.VariableDeclaration:
                {
                    var varDecl = (VariableDeclaration) s;

                    // Single variable case
                    if (varDecl.Variable.Next == null)
                    {
                        var v = varDecl.Variable;
                        if (_capturedLocals.Peek().Data.Contains(v))
                        {
                            s = v.OptionalValue == null
                                ? new NoOp(s.Source)
                                : _closureStack.Peek().Store(v, v.OptionalValue);
                        }
                    }
                    else // Multi-variable case
                    {
                        for (var v = varDecl.Variable; v != null; v = v.Next)
                        {
                            // TODO
                            // This is a bit of a hack due to the AST being difficult to work with,
                            // but basically, we don't remove the declaration of captured locals if
                            // they occur in multi-variable declarations even though their
                            // occurrences will be replaced with closure variables.
                            // If the declaration has a value, e.g. like x in
                            //
                            // int x = val, y = ...;
                            //
                            // it's transformed into
                            //
                            // int x = closure.x = val, y = ...;
                            if (_capturedLocals.Peek().Data.Contains(v))
                            {
                                if (v.OptionalValue != null)
                                {
                                    v.OptionalValue = _closureStack.Peek().Store(v, v.OptionalValue);
                                }
                            }
                        }
                    }
                    break;
                }
                case StatementType.TryCatchFinally:
                {
                    _tryCatches.Pop();
                    break;
                }
            }
        }

        public override void End(ref Expression e, ExpressionUsage u)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.Lambda:
                {
                    if (e == _lambda)
                    {
                        var lambda = (Lambda) e;
                        e = new NewDelegate(
                            e.Source,
                            (DelegateType) lambda.ReturnType,
                            _closureStack.Peek().Expression,
                            _lambdaMethod);

                        _lambda = null;
                        _lambdaMethod = null;
                    }
                    break;
                }
                case ExpressionType.LoadLocal:
                {
                    var load = (LoadLocal)e;

                    if (_closureVars.Contains(load.Variable))
                    {
                        e = _lambda == null
                            ? _closureStack.Peek().Load(load.Variable)
                            : _closureStack.Peek().LoadInside(load.Variable);
                    }
                    break;
                }
                case ExpressionType.LoadArgument:
                {
                    var load = (LoadArgument)e;
                    if (_closureVars.Contains(load.Parameter))
                    {
                        e = _lambda == null
                            ? _closureStack.Peek().Load(load.Parameter)
                            : _closureStack.Peek().LoadInside(load.Parameter);
                    }
                    else if (load.Function.Match(_ => false, lam => lam == _lambda))
                    {
                        e = new LoadArgument(load.Source, _lambdaMethod, load.Index);
                    }
                    break;
                }
                case ExpressionType.This:
                {
                    if (_closureVars.This && _lambda != null)
                    {
                        e = _closureStack.Peek().ThisInside();
                    }
                    break;
                }
                case ExpressionType.StoreLocal:
                {
                    var store = (StoreLocal)e;
                    if (_closureVars.Contains(store.Variable))
                    {
                        e = _lambda == null
                            ? _closureStack.Peek().Store(store.Variable, store.Value)
                            : _closureStack.Peek().StoreInside(store.Variable, store.Value);
                    }
                    break;
                }
                case ExpressionType.StoreArgument:
                {
                    var store = (StoreArgument)e;
                    if (_closureVars.Contains(store.Parameter))
                    {
                        e = _lambda == null
                            ? _closureStack.Peek().Store(store.Parameter, store.Value)
                            : _closureStack.Peek().StoreInside(store.Parameter, store.Value);
                    }
                    else if (store.Function.Match(_ => false, lam => lam == _lambda))
                    {
                        e = new StoreArgument(store.Source, _lambdaMethod, store.Index, store.Value);
                    }
                    break;
                }
                case ExpressionType.StoreThis:
                {
                    var store = (StoreThis)e;
                    if (_closureVars.This)
                    {
                        e = _lambda == null
                            ? _closureStack.Peek().StoreThis(store.Value)
                            : _closureStack.Peek().StoreThisInside(store.Value);
                    }
                    break;
                }
            }
        }
    }
}