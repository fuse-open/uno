using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Statements;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.Core.IL.Building.Functions;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        public Statement CompileStatement(AstStatement e)
        {
            switch (e.StatementType)
            {
                default:
                    if (e is AstExpression)
                        return CompileExpression(e as AstExpression);
                    break;
                case AstStatementType.VariableDeclaration:
                    return CompileVariableDeclaration(e as AstVariableDeclaration);
                case AstStatementType.FixedArrayDeclaration:
                    return CompileFixedArrayDeclaration(e as AstFixedArrayDeclaration);
                case AstStatementType.Scope:
                    return CompileScope(e as AstScope);

                case AstStatementType.Draw:
                    return Compiler.BlockBuilder.CompileDraw(Function, VariableScopeStack, e as AstDraw);
                case AstStatementType.DrawDispose:
                    return new DrawDispose(e.Source);

                case AstStatementType.Break:
                    return new Break(e.Source);
                case AstStatementType.Continue:
                    return new Continue(e.Source);

                case AstStatementType.Unchecked:
                {
                    CheckCastStack.Add(false);

                    try
                    {
                        return CompileStatement(((AstModifiedStatement) e).Statement);
                    }
                    finally
                    {
                        CheckCastStack.RemoveLast();
                    }
                }
                case AstStatementType.IfElse:
                {
                    var s = e as AstIfElse;

                    if (s.Condition is AstDefined)
                    {
                        var def = s.Condition as AstDefined;

                        return Environment.Test(def.Source, def.Condition)
                            ? s.OptionalIfBody != null
                                ? CompileStatement(s.OptionalIfBody)
                                : new NoOp(s.Source)
                            : s.OptionalElseBody != null
                                ? CompileStatement(s.OptionalElseBody)
                                : new NoOp(s.Source);
                    }

                    var r = new IfElse(s.Source, CompileCondition(s.Condition));

                    if (s.OptionalIfBody != null)
                        r.OptionalIfBody = CompileStatement(s.OptionalIfBody);
                    if (s.OptionalElseBody != null)
                        r.OptionalElseBody = CompileStatement(s.OptionalElseBody);

                    return r;
                }
                case AstStatementType.ExternScope:
                {
                    var s = (AstExternScope) e;
                    return new ExternScope(s.Source, 
                        Compiler.CompileAttributes(Namescope, s.Attributes),
                        s.Body.String, 
                        ExtensionTransform.CreateObject(s.Source, Function, TypeBuilder.Parameterize(Function.DeclaringType)), 
                        s.OptionalArguments != null
                            ? CompileArgumentList(s.OptionalArguments)
                            : ExtensionTransform.CreateArgumentList(s.Source, Function), 
                        GetUsings(s.Source));
                }
                case AstStatementType.While:
                {
                    var s = e as AstLoop;
                    var r = new While(s.Source, false, CompileCondition(s.Condition));

                    var vscope = new VariableScope();
                    VariableScopeStack.Add(vscope);

                    if (s.OptionalBody != null)
                        r.OptionalBody = CompileStatement(s.OptionalBody);

                    if (s.OptionalBody == null || (!s.OptionalBody.IsInvalid && ((s.OptionalBody is AstScope && (s.OptionalBody as AstScope).IsClosed) || !(s.OptionalBody is AstScope))))
                    {
                        VariableScopeStack.Remove(vscope);
                        CurrentVariableScope.Scopes.Add(vscope);
                    }

                    return r;
                }
                case AstStatementType.DoWhile:
                {
                    var s = e as AstLoop;
                    var r = new While(s.Source, true, CompileCondition(s.Condition));
                    var vscope = new VariableScope();
                    VariableScopeStack.Add(vscope);

                    if (s.OptionalBody != null)
                        r.OptionalBody = CompileStatement(s.OptionalBody);

                    if (s.OptionalBody == null || (!s.OptionalBody.IsInvalid && ((s.OptionalBody is AstScope && (s.OptionalBody as AstScope).IsClosed) || !(s.OptionalBody is AstScope))))
                    {
                        VariableScopeStack.Remove(vscope);
                        CurrentVariableScope.Scopes.Add(vscope);
                    }

                    return r;
                }
                case AstStatementType.For:
                {
                    var s = e as AstFor;
                    var r = new For(s.Source);
                    var vscope = new VariableScope();
                    VariableScopeStack.Add(vscope);

                    if (s.OptionalInitializer != null)
                        r.OptionalInitializer = CompileStatement(s.OptionalInitializer);
                    if (s.OptionalCondition != null)
                        r.OptionalCondition = CompileCondition(s.OptionalCondition);
                    if (s.OptionalBody != null)
                        r.OptionalBody = CompileStatement(s.OptionalBody);
                    if (s.OptionalIncrement != null)
                        r.OptionalIncrement = CompileExpression(s.OptionalIncrement);

                    if (s.OptionalBody == null ||
                        (!s.OptionalBody.IsInvalid && ((s.OptionalBody is AstScope && (s.OptionalBody as AstScope).IsClosed) ||
                        !(s.OptionalBody is AstScope))))
                    {
                        VariableScopeStack.Remove(vscope);
                        CurrentVariableScope.Scopes.Add(vscope);
                    }

                    return r;
                }
                case AstStatementType.Foreach:
                {
                    var s = e as AstForeach;
                    var scope = new Scope(s.Source);
                    var collection = CompileExpression(s.Collection);
                    Statement result;

                    var vscope = new VariableScope();
                    VariableScopeStack.Add(vscope);
                    VerifyVariableName(s.ElementName.Source, s.ElementName.Symbol);

                    if (collection.ReturnType.IsArray ||
                        collection.ReturnType == Essentials.String)
                    {
                        var loop = new For(s.Source) {OptionalBody = scope};
                        var collectionVar = new Variable(s.Collection.Source, Function, Namescope.GetUniqueIdentifier("array"), collection.ReturnType);

                        if (collection is LoadLocal)
                            collectionVar = (collection as LoadLocal).Variable;
                        else
                            vscope.Variables.Add(collectionVar.Name, collectionVar);

                        var indexInitializer = new Constant(s.Collection.Source, Essentials.Int, 0);
                        var lengthInitializer = CompileImplicitCast(s.Collection.Source, Essentials.Int,
                                CompileExpression(
                                    new AstMember(
                                        new AstIdentifier(s.Collection.Source, collectionVar.Name),
                                        new AstIdentifier(s.Collection.Source, "Length"))));

                        var indexVar = new Variable(s.Collection.Source, Function, Namescope.GetUniqueIdentifier("index"), Essentials.Int);
                        var lengthVar = new Variable(s.Collection.Source, Function, Namescope.GetUniqueIdentifier("length"), Essentials.Int);

                        if (collection is LoadLocal)
                        {
                            indexVar.OptionalValue = indexInitializer;
                            lengthVar.OptionalValue = lengthInitializer;
                            loop.OptionalInitializer = new VariableDeclaration(indexVar, lengthVar);
                        }
                        else
                            loop.OptionalInitializer = new SequenceOp(
                                new StoreLocal(s.Collection.Source, collectionVar, collection),
                                new StoreLocal(s.Collection.Source, indexVar, indexInitializer),
                                new StoreLocal(s.Collection.Source, lengthVar, lengthInitializer));

                        vscope.Variables.Add(indexVar.Name, indexVar);
                        vscope.Variables.Add(lengthVar.Name, lengthVar);

                        loop.OptionalCondition = CompileCondition(
                            new AstBinary(AstBinaryType.LessThan,
                                new AstIdentifier(s.Collection.Source, indexVar.Name),
                                s.Collection.Source,
                                new AstIdentifier(s.Collection.Source, lengthVar.Name)));

                        loop.OptionalIncrement = new FixOp(s.Collection.Source, FixOpType.IncreaseBefore,
                            new LoadLocal(s.Collection.Source, indexVar));

                        var elementType = s.ElementType.ExpressionType == AstExpressionType.Var
                            ? collection.ReturnType.IsArray 
                                    ? collection.ReturnType.ElementType 
                                    : Essentials.Char
                            : NameResolver.GetType(Namescope, s.ElementType);

                        var elementVar = new Variable(s.ElementName.Source, Function, s.ElementName.Symbol, elementType, VariableType.Iterator,
                            CompileImplicitCast(s.ElementName.Source, elementType,
                                CompileExpression(
                                    new AstCall(AstCallType.LookUp,
                                        new AstIdentifier(s.ElementName.Source, collectionVar.Name),
                                        new AstIdentifier(s.ElementName.Source, indexVar.Name)))));

                        scope.Statements.Add(new VariableDeclaration(elementVar));
                        vscope.Variables.Add(elementVar.Name, elementVar);
                        result = loop;
                    }
                    else
                    {
                        // TODO: Verify that collection implements IEnumerable<T>

                        var loop = new While(s.Source) {OptionalBody = scope};
                        var enumeratorInitializer = ILFactory.CallMethod(s.Collection.Source, collection.Address, "GetEnumerator");
                        var enumeratorVar = new Variable(s.Collection.Source, Function, Namescope.GetUniqueIdentifier("enum"), enumeratorInitializer.ReturnType, VariableType.Default, enumeratorInitializer);

                        vscope.Variables.Add(enumeratorVar.Name, enumeratorVar);

                        loop.Condition = CompileImplicitCast(s.Collection.Source, Essentials.Bool,
                            ILFactory.CallMethod(s.Collection.Source,
                                new LoadLocal(s.Collection.Source, enumeratorVar).Address,
                                "MoveNext"));

                        var elementInitializer = CompileExpression(new AstMember(
                            new AstIdentifier(s.ElementName.Source, enumeratorVar.Name),
                            new AstIdentifier(s.ElementName.Source, "Current")));

                        var elementType = s.ElementType.ExpressionType == AstExpressionType.Var 
                                ? elementInitializer.ReturnType 
                                : NameResolver.GetType(Namescope, s.ElementType);
                        var elementVar = new Variable(s.Source, Function, s.ElementName.Symbol, elementType, VariableType.Iterator, 
                            CompileImplicitCast(s.ElementName.Source, elementType, elementInitializer));

                        var hasDispose = false;
                        foreach (var m in enumeratorVar.ValueType.Methods)
                        {
                            if (m.Name == "Dispose" && m.Parameters.Length == 0)
                            {
                                hasDispose = true;
                                break;
                            }
                        }

                        // Optimization: avoid casting to IDisposable when Dispose() method is found
                        var dispose = hasDispose
                            ? ILFactory.CallMethod(s.Collection.Source,
                                new LoadLocal(s.Collection.Source, enumeratorVar).Address,
                                "Dispose")
                            : ILFactory.CallMethod(s.Collection.Source,
                                new CastOp(s.Collection.Source, Essentials.IDisposable,
                                    new LoadLocal(s.Collection.Source, enumeratorVar)).Address,
                                "Dispose");

                        scope.Statements.Add(new VariableDeclaration(elementVar));
                        vscope.Variables.Add(elementVar.Name, elementVar);
                        result = new Scope(s.Source,
                            new VariableDeclaration(enumeratorVar),
                            new TryCatchFinally(s.Source,
                                new Scope(s.Source, loop),
                                new Scope(s.Source, dispose)));
                    }

                    if (s.OptionalBody != null)
                    {
                        var body = CompileStatement(s.OptionalBody);

                        if (body is Scope)
                            scope.Statements.AddRange((body as Scope).Statements);
                        else
                            scope.Statements.Add(body);
                    }

                    if (s.OptionalBody == null ||
                        !s.OptionalBody.IsInvalid && ((s.OptionalBody is AstScope && (s.OptionalBody as AstScope).IsClosed) ||
                        !(s.OptionalBody is AstScope)))
                    {
                        VariableScopeStack.Remove(vscope);
                        CurrentVariableScope.Scopes.Add(vscope);
                    }

                    return result;
                }
                case AstStatementType.Return:
                    return new Return(e.Source);
                case AstStatementType.ReturnValue:
                {
                    var s = e as AstValueStatement;

                    var returnValue = CompileExpression(s.Value);

                    var returnType = Lambdas.Count == 0
                        ? Function.ReturnType
                        : Lambdas.Peek().DelegateType.ReturnType;

                    return new Return(s.Source, CompileImplicitCast(s.Source, returnType, returnValue));
                }
                case AstStatementType.TryCatchFinally:
                {
                    var s = e as AstTryCatchFinally;
                    var tryScope = CompileScope(s.TryScope);
                    var catchBlocks = new List<CatchBlock>();
                    Scope finallyScope = null;

                    foreach (var c in s.CatchBlocks)
                    {
                        var exceptionType =
                            c.OptionalType != null ?
                                NameResolver.GetType(Namescope, c.OptionalType) :
                                Essentials.Exception;

                        var vscope = new VariableScope();
                        VariableScopeStack.Add(vscope);

                        VerifyVariableName(c.Name.Source, c.Name.Symbol);
                        var exceptionVar = new Variable(c.Name.Source, Function, c.Name.Symbol, exceptionType, VariableType.Exception);
                        vscope.Variables.Add(exceptionVar.Name, exceptionVar);

                        var catchBody = CompileScope(c.Body);

                        if (c.Body == null || !c.Body.IsInvalid && c.Body.IsClosed)
                        {
                            VariableScopeStack.Remove(vscope);
                            CurrentVariableScope.Scopes.Add(vscope);
                        }

                        catchBlocks.Add(new CatchBlock(c.Name.Source, exceptionVar, catchBody));
                    }

                    if (s.OptionalFinallyScope != null)
                        finallyScope = CompileScope(s.OptionalFinallyScope);

                    return new TryCatchFinally(s.Source, tryScope, finallyScope, catchBlocks.ToArray());
                }
                case AstStatementType.Lock:
                {
                    var s = e as AstLock;
                    var scope = new Scope(s.Source);
                    var tryScope = new Scope(s.Source);
                    var finallyScope = new Scope(s.Source);
                    var obj = CompileExpression(s.Object);

                    switch (obj.ExpressionType)
                    {
                        case ExpressionType.LoadLocal:
                        case ExpressionType.LoadArgument:
                        case ExpressionType.LoadField:
                        case ExpressionType.This:
                        case ExpressionType.Base:
                            if (!obj.ReturnType.IsReferenceType)
                                Log.Error(obj.Source, ErrorCode.E0000, "Only reference types can be used in 'lock'");
                            break;

                        default:
                            Log.Error(obj.Source, ErrorCode.E0000, "Only variables can occur inside 'lock' initializer");
                            break;
                    }

                    if (s.OptionalBody != null)
                        tryScope.Statements.Add(CompileStatement(s.OptionalBody));

                    scope.Statements.Add(ILFactory.CallMethod(obj.Source, Essentials.Monitor, "Enter", obj));
                    finallyScope.Statements.Add(ILFactory.CallMethod(obj.Source, Essentials.Monitor, "Exit", obj));
                    scope.Statements.Add(new TryCatchFinally(s.Source, tryScope, finallyScope));
                    return scope;
                }
                case AstStatementType.Using:
                {
                    var s = e as AstUsing;
                    var scope = new Scope(s.Source);
                    var objects = new List<Expression>();
                    var vscope = new VariableScope();
                    VariableScopeStack.Add(vscope);
                    var init = CompileStatement(s.Initializer);

                    for (int i = 0; i < 1; i++)
                    {
                        switch (init.StatementType)
                        {
                            case StatementType.VariableDeclaration:
                            {
                                scope.Statements.Add(init);
                                for (var var = ((VariableDeclaration) init).Variable;
                                     var != null;
                                     var = var.Next)
                                    objects.Add(new LoadLocal(var.Source, var));
                                continue;
                            }
                            case StatementType.Expression:
                            {
                                if (AddObjects(objects, (Expression) init))
                                    continue;
                                break;
                            }
                        }

                        // TODO: Actually some expressions are valid as well in C#. Read spec and implement later
                        Log.Error(init.Source, ErrorCode.E0000, "Only variable declarations, fields and/or local variables can occur inside 'using' initializer");
                    }

                    var tryScope = new Scope(s.Source);
                    var finallyScope = new Scope(s.Source);

                    if (s.OptionalBody != null)
                        tryScope.Statements.Add(CompileStatement(s.OptionalBody));

                    foreach (var dispose in Enumerable.Reverse(objects))
                    {
                        var idisposable = CompileImplicitCast(dispose.Source, Essentials.IDisposable, dispose);
                        finallyScope.Statements.Add(ILFactory.CallMethod(dispose.Source, idisposable, "Dispose"));
                    }

                    scope.Statements.Add(new TryCatchFinally(s.Source, tryScope, finallyScope));

                    if (s.OptionalBody == null ||
                        (!s.OptionalBody.IsInvalid && ((s.OptionalBody is AstScope && (s.OptionalBody as AstScope).IsClosed) ||
                        !(s.OptionalBody is AstScope))))
                    {
                        VariableScopeStack.Remove(vscope);
                        CurrentVariableScope.Scopes.Add(vscope);
                    }

                    return scope;
                }
                case AstStatementType.ThrowValue:
                {
                    var s = e as AstValueStatement;
                    return new Throw(s.Source, CompileExpression(s.Value));
                }
                case AstStatementType.Throw:
                {
                    for (int i = VariableScopeStack.Count - 1; i >= 0; i--)
                        foreach (var v in VariableScopeStack[i].Variables.Values)
                            if (v.IsException)
                                return new Throw(e.Source, new LoadLocal(e.Source, v), true);

                    return Error(e.Source, ErrorCode.E0000, "Cannot rethrow outside of catch block");
                }
                case AstStatementType.Switch:
                {
                    var s = e as AstSwitch;
                    var c = CompileExpression(s.Condition);

                    if (!c.ReturnType.IsIntegralType)
                        c = TryCompileImplicitCast(s.Source, Essentials.Int, c) ??
                                Error(s.Condition.Source, ErrorCode.E3415, "A switch expression must be of enum or integral type");

                    var cases = new List<SwitchCase>();

                    foreach (var a in s.Cases)
                    {
                        var handler = CompileScope(a.Scope);
                        var values = new List<Constant>();
                        var includesDefault = false;

                        foreach (var v in a.Values)
                        {
                            if (v == null)
                            {
                                includesDefault = true;
                                continue;
                            }

                            var sym = CompileImplicitCast(v.Source, c.ReturnType, CompileExpression(v));
                            var constSym = Compiler.ConstantFolder.TryMakeConstant(sym);

                            if (constSym != null)
                                values.Add(constSym);
                            else if (sym.IsInvalid)
                                values.Add(new Constant(sym.Source, DataType.Invalid, -1));
                            else
                                Log.Error(v.Source, ErrorCode.E3410, "Case-expression must be constant");
                        }

                        cases.Add(new SwitchCase(values.ToArray(), includesDefault, handler));
                    }

                    return new Switch(s.Source, c, cases.ToArray());
                }

                case AstStatementType.Assert:
                {
                    if (!Environment.Debug)
                        return new NoOp(e.Source, "Stripped assert");

                    var s = e as AstValueStatement;
                    var value = CompileExpression(s.Value);
                    var args = new List<Expression>
                    {
                        value,
                        new Constant(s.Source, Essentials.String, value.ToString()),
                        new Constant(s.Source, Essentials.String, s.Source.File.ToString().Replace('\\', '/')),
                        new Constant(s.Source, Essentials.Int, s.Source.Line),
                    };
                    var locals = new List<StoreLocal>();

                    switch (value.ExpressionType)
                    {
                        case ExpressionType.CallUnOp:
                        {
                            var o = value as CallUnOp;
                            args.Add(CreateAssertIndirection(ref o.Operand, locals, Namescope));
                            break;
                        }
                        case ExpressionType.CallBinOp:
                        {
                            var o = value as CallBinOp;
                            args.Add(CreateAssertIndirection(ref o.Left, locals, Namescope));
                            args.Add(CreateAssertIndirection(ref o.Right, locals, Namescope));
                            break;
                        }
                        case ExpressionType.BranchOp:
                        {
                            var o = value as BranchOp;
                            args.Add(CreateAssertIndirection(ref o.Left, locals, Namescope));
                            args.Add(CreateAssertIndirection(ref o.Right, locals, Namescope));
                            break;
                        }
                        case ExpressionType.CallMethod:
                        {
                            var o = value as CallMethod;
                            for (int i = 0; i < o.Arguments.Length; i++)
                                args.Add(CreateAssertIndirection(ref o.Arguments[i], locals, Namescope));
                            break;
                        }
                    }

                    var result = ILFactory.CallMethod(s.Source, "Uno.Diagnostics.Debug", "Assert", args.ToArray());

                    while (locals.Count > 0)
                        result = new SequenceOp(locals.RemoveLast(), result);

                    return result;
                }
                case AstStatementType.DebugLog:
                {
                    if (!Environment.Debug)
                        return new NoOp(e.Source, "Stripped debug_log");
                    var s = (AstValueStatement) e;
                    var message = CompileExpression(s.Value);
                    var messageType = ILFactory.GetExpression(s.Source, "Uno.Diagnostics.DebugMessageType.Debug");
                    var file = new Constant(s.Source, Essentials.String, s.Source.File.ToString().Replace('\\', '/'));
                    var line = new Constant(s.Source, Essentials.Int, s.Source.Line);
                    return ILFactory.CallMethod(s.Source, "Uno.Diagnostics.Debug", "Log", message, messageType, file, line);
                }
                case AstStatementType.BuildError:
                    return CreateBuildError(e.Source, true);
                case AstStatementType.BuildWarning:
                    return CreateBuildError(e.Source, false);
                case AstStatementType.BuildErrorMessage:
                {
                    var s = (AstValueStatement)e;
                    return CreateBuildError(e.Source, true, s.Value);
                }
                case AstStatementType.BuildWarningMessage:
                {
                    var s = (AstValueStatement) e;
                    return CreateBuildError(e.Source, false, s.Value);
                }
            }

            Log.Error(e.Source, ErrorCode.I3411, "Unknown statement type <" + e.StatementType + ">");
            return Expression.Invalid;
        }

        bool AddObjects(List<Expression> objects, Expression obj)
        {
            switch (obj.ExpressionType)
            {
                case ExpressionType.LoadLocal:
                case ExpressionType.LoadArgument:
                case ExpressionType.LoadField:
                case ExpressionType.This:
                case ExpressionType.Base:
                    objects.Add(obj);
                    return true;
                case ExpressionType.SequenceOp:
                    var s = (SequenceOp) obj;
                    return AddObjects(objects, s.Left) && AddObjects(objects, s.Right);
                default:
                    return false;
            }
        }

        Statement CreateBuildError(Source src, bool isError, AstExpression value = null)
        {
            var type = "build_" + (isError
                                    ? "error"
                                    : "warning");

            if (!IsFunctionScope)
                return Error(src, ErrorCode.E0000, type.Quote() + " can only be used in methods");

            var message = value != null
                    ? Compiler.CompileConstant(value, Namescope, Essentials.String).Value as string ?? 
                        "<invalid>"
                    : Function.Quote() + " does not support this build target";

            if (isError)
            {
                /*if (!Compiler.Backend.IsDefault)
                    Log.Error(s.Source, ErrorCode.E0000, message);*/
                return new Throw(src,
                    ILFactory.NewObject(src, Essentials.Exception,
                        new Constant(src, Essentials.String, type + ": " + message)));
            }

            if (!Compiler.Backend.IsDefault)
                Log.Warning(src, ErrorCode.W0000, type + ": " + message);
            return new NoOp(src, message);
        }

        static Expression CreateAssertIndirection(ref Expression e, List<StoreLocal> locals, Namescope scope)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.AddressOf:
                    return CreateAssertIndirection(ref (e as AddressOf).Operand, locals, scope);

                case ExpressionType.LoadLocal:
                case ExpressionType.LoadArgument:
                case ExpressionType.LoadField:
                case ExpressionType.LoadElement:
                case ExpressionType.Invalid:
                case ExpressionType.Constant:
                    return e;
            }

            var v = new Variable(e.Source, null, scope.GetUniqueIdentifier("assert"), e.ReturnType);
            var s = new StoreLocal(e.Source, v, e);
            var l = new LoadLocal(e.Source, v);

            locals.Add(s);
            return e = l;
        }
    }
}
