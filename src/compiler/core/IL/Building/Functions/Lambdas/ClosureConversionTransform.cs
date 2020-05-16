using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Core.IL.Building.Functions.Lambdas
{
    /// <summary>
    /// Transform lambdas to classes and methods. Basically:
    ///
    ///     void func()
    ///     {
    ///         t1 local1;
    ///         t2 local2;
    ///
    ///         local1 = foo;
    ///         local2 = bar;
    ///         
    ///         Action lam1 = () => ... local1 ...
    ///         Action lam2 = () => ... local2 ...
    ///     }
    ///
    /// Is transformed into
    ///
    ///     class func_closure
    ///     {
    ///         t1 local1;
    ///         t2 local2;
    ///
    ///         void lam1() { ... local1 ... }
    ///         void lam2() { ... local2 ... }
    ///     }
    /// 
    ///     void func()
    ///     {
    ///         func_closure closure = new func_closure();
    ///         func_closure.local1 = foo;
    ///         func_closure.local2 = bar;
    ///
    ///         Action lam1 = closure.lam1;
    ///         Action lam2 = closure.lam2;
    ///     }
    ///
    /// The implementation works in two passes:
    /// 1. The first pass, implemented by ClosureConversionTransform, gathers
    ///    information about local variables and variable occurrences.
    /// 2. The second pass, implemented by ClosureConvertFunction, does the actual
    ///    rewriting of the function.
    ///    This only removes the top-level lambdas from the function so we run 1 again
    ///    on the lifted functions to lift any nested lambdas.
    /// </summary>
    class ClosureConversionTransform : CompilerPass
    {
        readonly List<LambdaToLift> _lambdasToLift = new List<LambdaToLift>();

        // After visiting the function body this will contain a tree of exactly the
        // same shape as the scope structure of the body, containing information about
        // what local variables are declared in each scope.
        readonly Stack<Tree<HashSet<Variable>>> _locals = new Stack<Tree<HashSet<Variable>>>();

        // All variable occurrences/uses. Is a stack so we can distinguish uses inside
        // and outside lambdas.
        readonly Stack<Variables> _occurrences = new Stack<Variables>();

        // The lambdas, if any, that we are currently inside of
        readonly Stack<Lambda> _lambdas = new Stack<Lambda>();

        // The try-catches, if any, that we are currently inside of
        readonly Stack<TryCatchFinally> _tryCatches = new Stack<TryCatchFinally>();

        public ClosureConversionTransform(CompilerPass parent)
            : base(parent)
        {
        }

        public override bool Begin(Function f)
        {
            if (f.HasBody && HasLambda(f.Body))
            {
                _occurrences.Clear();
                _occurrences.Push(new Variables());
                _lambdasToLift.Clear();
                _locals.Clear();
                _locals.Push(Tree.Create(new HashSet<Variable>()));
                return true;
            }

            return false; // Ignore function bodies without lambdas to save some work
        }

        public bool HasLambda(Scope body)
        {
            var ld = new LambdaDetector(this);
            body.Visit(ld);
            return ld.Result;
        }

        public override void End(Function f)
        {
            if (_lambdasToLift.Count > 0)
            {
                var locals = _locals.Pop();
                var closureVars = Variables.Union(_lambdasToLift.Select(ll => ll.FreeVars));
                var capturedLocals = locals.Select(vars =>
                {
                    vars.IntersectWith(closureVars.Locals);
                    return vars;
                });
                f.Visit(new ClosureConvertFunction(this, closureVars, capturedLocals));
            }
        }

        public override void BeginScope(Scope scope)
        {
            var child = Tree.Create(new HashSet<Variable>(), _locals.Peek());
            _locals.Peek().Children.Add(child);
            _locals.Push(child);

            if (_tryCatches.Count > 0)
            {
                var catchBlock = _tryCatches.Peek().CatchBlocks.FirstOrDefault(cb => cb.Body == scope);
                if (catchBlock != null)
                {
                    // This scope is a catch block
                    _locals.Peek().Data.Add(catchBlock.Exception);
                }
            }
        }

        public override void EndScope(Scope scope)
        {
            foreach (var local in _locals.Peek().Data)
            {
                if (_occurrences.Peek().Locals.Contains(local))
                    _occurrences.Peek().Locals.Remove(local);
            }

            _locals.Pop();
        }

        public override void Begin(ref Statement e)
        {
            switch (e.StatementType)
            {
                case StatementType.VariableDeclaration:
                {
                    for (var var = ((VariableDeclaration) e).Variable; var != null; var = var.Next)
                        _locals.Peek().Data.Add(var);
                    break;
                }
                case StatementType.TryCatchFinally:
                {
                    _tryCatches.Push((TryCatchFinally) e);
                    break;
                }
            }
        }

        public override void End(ref Statement e)
        {
            switch (e.StatementType)
            {
                case StatementType.TryCatchFinally:
                {
                    _tryCatches.Pop();
                    break;
                }
            }
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.Lambda:
                {
                    _lambdas.Push((Lambda) e);
                    _occurrences.Push(new Variables());
                    break;
                }
            }

            if (_lambdas.Count > 0)
            {
                switch (e.ExpressionType)
                {
                    case ExpressionType.LoadLocal:
                    {
                        _occurrences.Peek().Add(((LoadLocal) e).Variable);
                        break;
                    }
                    case ExpressionType.StoreLocal:
                    {
                        _occurrences.Peek().Add(((StoreLocal) e).Variable);
                        break;
                    }
                    case ExpressionType.StoreArgument:
                    {
                        var store = (StoreArgument) e;
                        if (store.Function.Match(f => true, l => false))
                            _occurrences.Peek().Add(store.Parameter);
                        break;
                    }
                    case ExpressionType.LoadArgument:
                    {
                        var load = (LoadArgument) e;
                        if (load.Function.Match(f => true, l => false))
                            _occurrences.Peek().Add(load.Parameter);
                        break;
                    }
                    case ExpressionType.This:
                    {
                        _occurrences.Peek().AddThis();
                        break;
                    }
                    case ExpressionType.StoreThis:
                    {
                        _occurrences.Peek().AddThis();
                        break;
                    }
                }
            }
        }

        public override void End(ref Expression e, ExpressionUsage u)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.Lambda:
                {
                    var lambda = (Lambda)e;
                    _lambdas.Pop();
                    var freeVars = _occurrences.Pop();
                    _occurrences.Peek().UnionWith(freeVars);

                    // We only lift top-most lambdas in the first pass
                    if (_lambdas.Count == 0)
                    {
                        _lambdasToLift.Add(new LambdaToLift(lambda, freeVars));
                    }

                    break;
                }
            }
        }
    }
}