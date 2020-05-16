using System;
using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.Syntax.Compilers;

namespace Uno.Compiler.Core.Syntax.Builders
{
    public partial class TypeBuilder
    {
        void PopulateClass(AstClass astClass, DataType result)
        {
            var parameterizedType = Parameterize(result);
            var fieldInitializers = new List<AstExpression>();
            var deferredActions = new List<Action>();
            parameterizedType.AssignBaseType();
            parameterizedType.Base?.PopulateMembers();

            foreach (var it in result.Interfaces)
                it.PopulateMembers();

            // Create dummies for members
            foreach (var item in astClass.Members)
            {
                if (item is AstClassMember)
                {
                    var m = (AstClassMember) item;
                    if (!_compiler.Environment.Test(((item as AstNamedMember)?.Name ?? astClass.Name).Source, m.OptionalCondition))
                        continue;
                }

                switch (item.MemberType)
                {
                    case AstMemberType.Method:
                    {
                        var md = item as AstMethod;

                        ClassType gt = null;
                        if (md.OptionalGenericSignature != null)
                        {
                            gt = new ClassType(md.Name.Source, parameterizedType, md.DocComment, Modifiers.Private | Modifiers.Static | Modifiers.Generated, md.Name.Symbol);
                            CreateGenericSignature(gt, md.OptionalGenericSignature, md.Modifiers.HasFlag(Modifiers.Override) || md.OptionalInterfaceType != null);
                        }

                        var pl = _compiler.CompileParameterList(gt ?? parameterizedType, md.Parameters, deferredActions);
                        var rt = _resolver.GetType(gt ?? parameterizedType, md.ReturnType);
                        var me = new Method(md.Name.Source, result, md.DocComment, GetMemberModifiers(md.Name.Source, parameterizedType, md.Modifiers), md.Name.Symbol, gt, rt, pl);
                        result.Methods.Add(me);

                        if (md.Attributes.Count > 0)
                            EnqueueAttributes(me, x => me.SetAttributes(_compiler.CompileAttributes(parameterizedType, md.Attributes)));

                        gt?.Methods.Add(me);

                        EnqueueCompiler(new FunctionCompiler(_compiler, me, parameterizedType, md.OptionalBody));

                        if (md.OptionalInterfaceType != null)
                        {
                            var it = _resolver.GetType(parameterizedType, md.OptionalInterfaceType);

                            if (!it.IsInvalid)
                            {
                                var im = TryImplementMethod(it, me);

                                if (im == null)
                                    Log.Error(md.OptionalInterfaceType.Source, ErrorCode.E0000, "Method " + (me.UnoName + me.Parameters.BuildString()).Quote() + " does not exist in interface " + it.Quote());
                                else
                                    me.SetImplementedMethod(im);
                            }
                        }
                        break;
                    }
                    case AstMemberType.Constructor:
                    {
                        var md = item as AstConstructor;
                        var pl = _compiler.CompileParameterList(parameterizedType, md.Parameters, deferredActions);
                        var ctor = new Constructor(md.Source, result, md.DocComment, md.Modifiers, pl);

                        if (md.Attributes.Count > 0)
                            EnqueueAttributes(ctor, x => ctor.SetAttributes(_compiler.CompileAttributes(parameterizedType, md.Attributes)));

                        var fc = new FunctionCompiler(_compiler, ctor, parameterizedType, md.OptionalBody);
                        EnqueueCompiler(fc);

                        if (ctor.IsStatic)
                        {
                            if (result.Initializer != null)
                                Log.Error(ctor.Source, ErrorCode.E3002, parameterizedType.Quote() + " already has a static constructor");

                            result.Initializer = ctor;
                        }
                        else
                        {
                            ctor.Modifiers = GetMemberModifiers(md.Source, parameterizedType, md.Modifiers);
                            result.Constructors.Add(ctor);
                        }

                        if (md.OptionalBody != null)
                            ctor.SetBody(new Scope(ctor.Source));

                        if (md.CallType == AstConstructorCallType.This && parameterizedType.IsStruct && md.CallArguments.Count == 0)
                        {
                            ctor.Body.Statements.Add(new StoreThis(ctor.Source, new Default(ctor.Source, parameterizedType)));
                        }
                        else if (md.CallType != AstConstructorCallType.None)
                        {
                            if (ctor.Body == null)
                                Log.Error(md.Source, ErrorCode.E0000, ctor.Quote() + " cannot call constructor without method body");
                            else if (md.CallType != AstConstructorCallType.Base || parameterizedType.Base != DataType.Invalid)
                                _enqueuedActions.Add(() =>
                                    {
                                        var bt = md.CallType == AstConstructorCallType.Base ? parameterizedType.Base : parameterizedType;
                                        bt.PopulateMembers();

                                        Constructor callCtor;
                                        Expression[] callArgs;
                                        if (!fc.TryResolveConstructorOverload(md.Source, bt.Constructors, md.CallArguments, out callCtor, out callArgs))
                                            Log.Error(md.Source, ErrorCode.E3003, bt.Quote() + " does not have a constructor that matches this argument list");
                                        else
                                            ctor.Body.Statements.Add(new CallConstructor(md.Source, callCtor, callArgs));
                                    });
                        }
                        else if (!ctor.IsStatic &&
                            parameterizedType.Base != null &&
                            parameterizedType.TypeType != TypeType.Struct &&
                            ctor.Body != null)
                        {
                            _enqueuedActions.Add(() =>
                                {
                                    Constructor callCtor;
                                    Expression[] callArgs;
                                    if (!fc.TryResolveConstructorOverload(md.Source, parameterizedType.Base.Constructors, AstArgument.Empty, out callCtor, out callArgs))
                                        Log.Error(md.Source, ErrorCode.E3004, "Base class " + parameterizedType.Base.Quote() + " does not have a default constructor");
                                    else
                                        ctor.Body.Statements.Add(new CallConstructor(md.Source, callCtor, callArgs));
                                });
                        }
                        break;
                    }
                    case AstMemberType.Finalizer:
                    {
                        var fd = item as AstFinalizer;
                        var pl = _compiler.CompileParameterList(parameterizedType, fd.Parameters, deferredActions);
                        var finalizer = new Finalizer(fd.Source, result, fd.DocComment, fd.Modifiers, pl);

                        if (fd.Attributes.Count > 0)
                            EnqueueAttributes(finalizer, x => finalizer.SetAttributes(_compiler.CompileAttributes(parameterizedType, fd.Attributes)));

                        if (result.Finalizer != null)
                            Log.Error(fd.Source, ErrorCode.E3002, parameterizedType.Quote() + " already has a finalizer");

                        result.Finalizer = finalizer;
                        EnqueueCompiler(new FunctionCompiler(_compiler, finalizer, parameterizedType, fd.OptionalBody));
                        break;
                    }
                    case AstMemberType.Property:
                    {
                        var pd = item as AstProperty;
                        var dt = _resolver.GetType(parameterizedType, pd.ReturnType);
                        var prop = new Property(pd.Name.Source, pd.DocComment, GetMemberModifiers(pd.Name.Source, parameterizedType, pd.Modifiers), pd.Name.Symbol, result, dt);
                        result.Properties.Add(prop);

                        bool hasImplicitImplementation = false;

                        // Implicit implementation
                        if (!prop.IsAbstract)
                        {
                            if (pd.Get != null && pd.Get.OptionalBody == null ||
                                pd.Set != null && pd.Set.OptionalBody == null)
                            {
                                if (pd.Set != null && pd.Set.OptionalBody == null && (pd.Get == null || pd.Get?.OptionalBody != null))
                                {
                                    if (!prop.IsExtern)
                                        Log.Error(pd.Set.Source, ErrorCode.E3042, prop.Quote(".set") + " must declare a body because it is not marked abstract or extern. Automatically implemented properties must define both get and set accessors");
                                }
                                else if (pd.Get != null && pd.Get.OptionalBody == null && (pd.Set == null || pd.Set?.OptionalBody != null))
                                {
                                    if (!prop.IsExtern)
                                        Log.Error(pd.Get.Source, ErrorCode.E3041, prop.Quote(".get") + " must declare a body because it is not marked abstract or extern. Automatically implemented properties must define both get and set accessors");
                                }
                                else
                                {
                                    prop.CreateImplicitField(prop.Source);

                                    var getScope = new Scope(pd.Get.Source);
                                    prop.CreateGetMethod(pd.Get.Source, GetAccessorModifiers(pd.Get.Source, prop, pd.Get.Modifiers) | Modifiers.Generated, getScope);

                                    var setScope = new Scope(pd.Set.Source);
                                    var setMethod = prop.CreateSetMethod(pd.Set.Source, GetAccessorModifiers(pd.Set.Source, prop, pd.Set.Modifiers) | Modifiers.Generated, setScope);
                                    var propIndex = result.Properties.Count - 1;

                                    _enqueuedActions.Add(() =>
                                        {
                                            parameterizedType.PopulateMembers();

                                            var obj = prop.Modifiers.HasFlag(Modifiers.Static) ? null : new This(prop.Source, parameterizedType).Address;
                                            var field = parameterizedType.Properties[propIndex].ImplicitField;

                                            getScope.Statements.Add(
                                                new Return(pd.Get.Source, new LoadField(pd.Get.Source, obj, field)));
                                            setScope.Statements.Add(
                                                new StoreField(pd.Set.Source, obj, field, new LoadArgument(pd.Set.Source, setMethod, 0)));
                                        });

                                    hasImplicitImplementation = true;
                                }
                            }
                        }

                        if (pd.Attributes.Count > 0)
                            EnqueueAttributes(prop,
                                x =>
                                {
                                    prop.SetAttributes(_compiler.CompileAttributes(parameterizedType, pd.Attributes));
                                    prop.GetMethod?.SetAttributes(prop.Attributes);
                                    prop.SetMethod?.SetAttributes(prop.Attributes);
                                    prop.ImplicitField?.SetAttributes(prop.Attributes);
                                });

                        if (!hasImplicitImplementation)
                        {
                            if (pd.Get != null)
                                EnqueueCompiler(new FunctionCompiler(_compiler, prop.CreateGetMethod(pd.Get.Source, GetAccessorModifiers(pd.Get.Source, prop, pd.Get.Modifiers)), parameterizedType, pd.Get.OptionalBody));
                            if (pd.Set != null)
                                EnqueueCompiler(new FunctionCompiler(_compiler, prop.CreateSetMethod(pd.Set.Source, GetAccessorModifiers(pd.Set.Source, prop, pd.Set.Modifiers)), parameterizedType, pd.Set.OptionalBody));
                        }

                        if (pd.OptionalInterfaceType != null)
                        {
                            var it = _compiler.NameResolver.GetType(parameterizedType, pd.OptionalInterfaceType);

                            if (!it.IsInvalid)
                            {
                                var im = TryImplementProperty(it, prop);

                                if (im == null)
                                    Log.Error(pd.OptionalInterfaceType.Source, ErrorCode.E0000, "Property " + prop.UnoName.Quote() + " does not exist in interface " + it.Quote());
                                else
                                    prop.SetImplementedProperty(im);
                            }
                        }
                        break;
                    }
                    case AstMemberType.Field:
                    {
                        var f = item as AstField;
                        var dt = _compiler.NameResolver.GetType(parameterizedType, f.ReturnType);

                        if (f.FieldModifiers.HasFlag(FieldModifiers.Const))
                        {
                            var m = new Literal(f.Name.Source, result, f.Name.Symbol, f.DocComment, GetMemberModifiers(f.Name.Source, parameterizedType, f.Modifiers), dt, f.InitValue);
                            result.Literals.Add(m);

                            if (f.Attributes.Count > 0)
                                EnqueueAttributes(m, x => m.SetAttributes(_compiler.CompileAttributes(parameterizedType, f.Attributes)));

                            if (f.InitValue == null)
                                Log.Error(f.Name.Source, ErrorCode.E3007, "'const' fields must provide a constant value");
                            else
                                deferredActions.Add(() => m.Value = _compiler.CompileConstant(f.InitValue, parameterizedType, dt).Value);
                        }
                        else
                        {
                            var m = new Field(f.Name.Source, result, f.Name.Symbol, f.DocComment, GetMemberModifiers(f.Name.Source, parameterizedType, f.Modifiers), f.FieldModifiers, dt);
                            fieldInitializers.Add(f.InitValue);
                            result.Fields.Add(m);

                            if (f.Attributes.Count > 0)
                                EnqueueAttributes(m, x => m.SetAttributes(_compiler.CompileAttributes(parameterizedType, f.Attributes)));
                        }
                        break;
                    }
                    case AstMemberType.Operator:
                    {
                        var od = item as AstOperator;
                        var rt = _resolver.GetType(parameterizedType, od.ReturnType);
                        var pl = _compiler.CompileParameterList(parameterizedType, od.Parameters, deferredActions);
                        var op = new Operator(od.Source, result, od.Operator, od.DocComment, GetMemberModifiers(od.Source, parameterizedType, od.Modifiers), rt, pl);
                        result.Operators.Add(op);

                        if (od.Attributes.Count > 0)
                            EnqueueAttributes(op, x => op.SetAttributes(_compiler.CompileAttributes(parameterizedType, od.Attributes)));

                        EnqueueCompiler(new FunctionCompiler(_compiler, op, parameterizedType, od.OptionalBody));
                        break;
                    }
                    case AstMemberType.Converter:
                    {
                        var cd = item as AstConverter;
                        var rt = _resolver.GetType(parameterizedType, cd.TargetType);
                        var pl = _compiler.CompileParameterList(parameterizedType, cd.Parameters, deferredActions);
                        var type = cd.Modifiers.HasFlag(Modifiers.Implicit) ? CastModifier.Implicit : CastModifier.Explicit;
                        var cast = new Cast(cd.TargetType.Source, result, type, cd.DocComment, GetMemberModifiers(cd.TargetType.Source, parameterizedType, cd.Modifiers), rt, pl);
                        result.Casts.Add(cast);

                        if (cd.Attributes.Count > 0)
                            EnqueueAttributes(cast, x => cast.SetAttributes(_compiler.CompileAttributes(parameterizedType, cd.Attributes)));

                        EnqueueCompiler(new FunctionCompiler(_compiler, cast, parameterizedType, cd.OptionalBody));
                        break;
                    }
                    case AstMemberType.Indexer:
                    {
                        var id = item as AstIndexer;
                        var rt = _resolver.GetType(parameterizedType, id.ReturnType);
                        var pl = _compiler.CompileParameterList(parameterizedType, id.Parameters, deferredActions);
                        var indexer = new Property(id.Source, id.DocComment, GetMemberModifiers(id.Source, parameterizedType, id.Modifiers), "Item", result, rt, pl);
                        result.Properties.Add(indexer);

                        if (id.Attributes.Count > 0)
                            EnqueueAttributes(indexer,
                                x =>
                                {
                                    indexer.SetAttributes(_compiler.CompileAttributes(parameterizedType, id.Attributes));
                                    indexer.GetMethod?.SetAttributes(indexer.Attributes);
                                    indexer.SetMethod?.SetAttributes(indexer.Attributes);
                                });

                        if (id.Get != null)
                            EnqueueCompiler(new FunctionCompiler(_compiler, indexer.CreateGetMethod(id.Get.Source, GetAccessorModifiers(id.Get.Source, indexer, id.Get.Modifiers)), parameterizedType, id.Get.OptionalBody));
                        if (id.Set != null)
                            EnqueueCompiler(new FunctionCompiler(_compiler, indexer.CreateSetMethod(id.Set.Source, GetAccessorModifiers(id.Set.Source, indexer, id.Set.Modifiers)), parameterizedType, id.Set.OptionalBody));

                        if (id.OptionalInterfaceType != null)
                        {
                            var it = _resolver.GetType(parameterizedType, id.OptionalInterfaceType);

                            if (!it.IsInvalid)
                            {
                                var im = TryImplementProperty(it, indexer);

                                if (im == null)
                                    Log.Error(id.OptionalInterfaceType.Source, ErrorCode.E0000, "Indexer 'this" + indexer.Parameters.BuildString("[", "]") + "' does not exist in interface " + it.Quote());
                                else
                                    indexer.SetImplementedProperty(im);
                            }
                        }
                        break;
                    }
                    case AstMemberType.Event:
                    {
                        var ed = item as AstEvent;
                        var dt = _resolver.GetType(parameterizedType, ed.DelegateType);

                        if (dt.IsInvalid)
                            break;

                        if (!dt.IsDelegate)
                            Log.Error(ed.Name.Source, ErrorCode.E3012, "Events must have delegate type. " + dt + " is a " + dt.TypeType);

                        var ev = new Event(ed.Name.Source, ed.DocComment, GetMemberModifiers(ed.Name.Source, parameterizedType, ed.Modifiers), result, dt, ed.Name.Symbol);
                        result.Events.Add(ev);

                        if (ed.Attributes.Count > 0)
                            EnqueueAttributes(ev,
                                x =>
                                {
                                    ev.SetAttributes(_compiler.CompileAttributes(parameterizedType, ed.Attributes));
                                    ev.AddMethod?.SetAttributes(ev.Attributes);
                                    ev.RemoveMethod?.SetAttributes(ev.Attributes);
                                });

                        // Implicit implementation
                        if (ed.Add == null && ed.Remove == null)
                        {
                            if (!ev.IsAbstract)
                            {
                                ev.CreateImplicitField(ed.Name.Source);

                                var addScope = new Scope(ed.Name.Source);
                                var addMethod = ev.CreateAddMethod(ev.Source, ev.Modifiers | Modifiers.Generated, addScope);
                                var removeScope = new Scope(ed.Name.Source);
                                var removeMethod = ev.CreateRemoveMethod(ev.Source, ev.Modifiers | Modifiers.Generated, removeScope);
                                var eventIndex = result.Events.Count - 1;

                                _enqueuedActions.Add(() =>
                                    {
                                        parameterizedType.PopulateMembers();

                                        var obj = ev.IsStatic ? null : new This(ev.Source, parameterizedType).Address;
                                        var field = parameterizedType.Events[eventIndex].ImplicitField;

                                        addScope.Statements.Add(
                                            new StoreField(ev.Source, obj, field,
                                                new CastOp(ev.Source, ev.ReturnType,
                                                    _ilf.CallMethod(ev.Source, _ilf.Essentials.Delegate, "Combine",
                                                        new CastOp(ev.Source, _ilf.Essentials.Delegate, new LoadField(ev.Source, obj, field)),
                                                        new LoadArgument(ev.Source, addMethod, 0)))));
                                        removeScope.Statements.Add(
                                            new StoreField(ev.Source, obj, field,
                                                new CastOp(ev.Source, ev.ReturnType,
                                                    _ilf.CallMethod(ev.Source, _ilf.Essentials.Delegate, "Remove",
                                                        new CastOp(ev.Source, _ilf.Essentials.Delegate, new LoadField(ev.Source, obj, field)),
                                                        new LoadArgument(ev.Source, removeMethod, 0)))));
                                    });
                            }
                            else
                            {
                                ev.CreateAddMethod(ev.Source, ev.Modifiers);
                                ev.CreateRemoveMethod(ev.Source, ev.Modifiers);
                            }

                            break;
                        }

                        if (ed.Add != null && ed.Add.Modifiers != 0 || ed.Remove != null && ed.Remove.Modifiers != 0)
                            Log.Error(ed.Name.Source, ErrorCode.E3040, "Modifiers can not be placed on event accessor declarations");

                        if (ed.Add != null)
                            EnqueueCompiler(new FunctionCompiler(_compiler, ev.CreateAddMethod(ed.Add.Source, ev.Modifiers), parameterizedType, ed.Add.OptionalBody));
                        if (ed.Remove != null)
                            EnqueueCompiler(new FunctionCompiler(_compiler, ev.CreateRemoveMethod(ed.Remove.Source, ev.Modifiers), parameterizedType, ed.Remove.OptionalBody));

                        if (ed.OptionalInterfaceType != null)
                        {
                            var it = _resolver.GetType(parameterizedType, ed.OptionalInterfaceType);

                            if (!it.IsInvalid)
                            {
                                var im = TryImplementEvent(it, ev);

                                if (im == null)
                                    Log.Error(ed.OptionalInterfaceType.Source, ErrorCode.E0000, "Event " + ev.UnoName.Quote() + " does not exist in interface " + it.Quote());
                                else
                                    ev.SetImplementedEvent(im);
                            }
                        }
                        break;
                    }
                }

                switch (item.MemberType)
                {
                    case AstMemberType.Method:
                    case AstMemberType.Property:
                    case AstMemberType.Indexer:
                    case AstMemberType.Event:
                        switch (parameterizedType.TypeType)
                        {
                            case TypeType.Class:
                            case TypeType.Interface:
                            case TypeType.Struct:
                                continue;
                        }
                        break;
                    case AstMemberType.Field:
                    case AstMemberType.Constructor:
                    case AstMemberType.Operator:
                    case AstMemberType.Converter:
                    case AstMemberType.Class:
                    case AstMemberType.Enum:
                    case AstMemberType.Delegate:
                        switch (parameterizedType.TypeType)
                        {
                            case TypeType.Class:
                            case TypeType.Struct:
                                continue;
                        }
                        break;
                    case AstMemberType.Finalizer:
                    case AstMemberType.MetaMethod:
                    case AstMemberType.MetaProperty:
                    case AstMemberType.NodeBlock:
                    case AstMemberType.Block:
                    case AstMemberType.ApplyStatement:
                        switch (parameterizedType.TypeType)
                        {
                            case TypeType.Class:
                                continue;
                        }
                        break;
                }

                Log.Error(((item as AstNamedMember)?.Name ?? astClass.Name).Source, ErrorCode.E3013, "<" + item.MemberType + "> is not allowed in this scope");
            }

            // Compile swizzle types
            if (astClass.Swizzlers != null)
                foreach (var e in astClass.Swizzlers)
                    result.Swizzlers.Add(_resolver.GetType(parameterizedType, e));

            // Compile field initializers
            if (fieldInitializers.Count > 0)
                _enqueuedActions.Add(() => CompileFieldInitializers(result, parameterizedType, fieldInitializers));

            // Create default constructor in non-static classes
            if (result.IsClass && !result.IsStatic && result.Constructors.Count == 0)
            {
                var ctor = new Constructor(parameterizedType.Source, parameterizedType, null, 
                    (parameterizedType.IsAbstract ? Modifiers.Protected : Modifiers.Public) | Modifiers.Generated, 
                    ParameterList.Empty, new Scope(parameterizedType.Source));
                result.Constructors.Add(ctor);
                _enqueuedActions.Add(() =>
                    {
                        parameterizedType.AssignBaseType();
                        if (parameterizedType.Base == null)
                            return;

                        parameterizedType.Base.PopulateMembers();

                        Constructor callCtor = null;
                        Expression[] callArgs = null;
                        if (parameterizedType.Base != DataType.Invalid && !new FunctionCompiler(_compiler, parameterizedType).TryResolveConstructorOverload(parameterizedType.Source, parameterizedType.Base.Constructors, AstArgument.Empty, out callCtor, out callArgs))
                            Log.Error(parameterizedType.Source, ErrorCode.E3001, parameterizedType.Base.Quote() + " does not have a default constructor");
                        else
                            ctor.Body.Statements.Add(new CallConstructor(ctor.Source, callCtor, callArgs));
                    });
            }

            // Resolve overridden members
            foreach (var m in result.Methods)
            {
                m.SetOverriddenMethod(TryOverrideMethod(m));

                if (m.IsGenericDefinition)
                {
                    if (m.ImplementedMethod != null || m.OverriddenMethod != null)
                    {
                        // Inherit base generic constraints
                        var baseParams = (m.OverriddenMethod ?? m.ImplementedMethod).GenericDefinition.GenericParameters;

                        for (int i = 0; i < m.GenericParameters.Length; i++)
                        {
                            var baseParam = baseParams[i];
                            var thisParam = m.GenericParameters[i];

                            baseParam.AssignBaseType();
                            thisParam.AssignBaseType();
                            thisParam.SetBase(baseParam.Base);
                            thisParam.SetConstraintType(baseParam.ConstraintType);
                            thisParam.SetInterfaces(baseParam.Interfaces);

                            if (baseParam.Constructors.Count > 0)
                                thisParam.Constructors.Add(new Constructor(baseParam.Constructors[0].Source, thisParam, null,
                                    Modifiers.Public | Modifiers.Generated | Modifiers.Extern, ParameterList.Empty));
                        }
                    }

                    Parameterize(m.Source, m, m.GenericParameters);
                }
            }

            foreach (var m in result.Properties)
                m.SetOverriddenProperty(TryOverrideProperty(m));
            foreach (var m in result.Events)
                m.SetOverriddenEvent(TryOverrideEvent(m));

            if (!result.IsInterface && result.Interfaces.Length > 0)
                ImplementInterfaces(result);

            _compiler.BlockBuilder.PopulateBlock(astClass, result.Block);

            foreach (var action in deferredActions)
                action();
        }
    }
}
